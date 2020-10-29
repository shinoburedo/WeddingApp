(function () {
    'use strict';

    var vm = App.ViewModels.MyWeddingOrderDetailDialog = kendo.observable({
        element: ".item-detail-window",

        region_cd: null,
        model: null,
        save_done: false,
        AvailDate: [],
        AvailTime: [],
        SelectDate: null,
        SelectTime: null,
        WedDate: null,
        msgAvailDate: "",
        msgAvailTime: "",
        visibleAddCart: true,

        isEditAvailDate: function () {
            var list = this.get("AvailDate");
            if (!list) return false;
            if (list.length <= 0) return false;
            return true;
        },

        isEditAvailTime: function () {
            var list = this.get("AvailTime");
            if (!list) return false;
            if (list.length <= 0) return false;
            return true;
        },

        getPhotoPath: function () {
            var model = this.get("model");
            if (!model) return "";
            return App.getUrl(model.PhotoPath);
        },

        getQuantity: function () {
            var model = this.get("model");
            if (!model) return "";
            return kendo.toString(model.quantity, ",0");
        },

        getPriceCharge: function () {
            var model = this.get("model");
            if (!model) return "";
            return App.Utils.formatCur(model.price_charge, model.price_type, model.region_cur_symbol);
        },

        getPriceChargeExt: function () {
            var model = this.get("model");
            if (!model) return "";
            return App.Utils.formatCur(model.price_charge_ext, model.price_type, model.region_cur_symbol);
        },

        getCxlCharge: function () {
            var model = this.get("model");
            if (!model) return "";
            return App.Utils.formatCur(model.cxl_charge, model.price_type, model.region_cur_symbol);
        },

        getRefund: function () {
            var model = this.get("model");
            if (!model) return "";
            var refund = model.price_charge_ext - model.cxl_charge;
            return App.Utils.formatCur(refund, model.price_type, model.region_cur_symbol);
        },

        isPkg: function () {
            var model = this.get("model");
            if (!model) return false;
            return model.isPkg;
        },

        open: function (item_cd, lang, wed_date) {
            vm.set("model", null);
            vm.set("save_done", false);

            var div = $(this.get("element"));
            var win = div.data("kendoWindow");
            //win.setOptions({ title: "Option: " + id });
            win.unbind();                               //deactivateでunbindしているが念のため。
            win.bind("activate", function () {
                App.Utils.ShowLoading(true);
                App.Models.CItem.detail(item_cd, lang, wed_date)
                    .done(function (model) {
                        App.Utils.HideLoading(true);
                        var item = new App.Models.CItem(model);
                        item.parseJSON();
                        vm.set("model", item);
                        vm.set("WedDate", wed_date);                        
                        if (model != null) {
                            vm.set("AvailDate", item.church_avail);
                            if (item.church_avail != null || item.church_avail.length > 0) {
                                this.visibleAddCart = false;
                            }
                            $("#drpAvailDate").data("kendoDropDownList").select(3);
                            vm.onAvailDateChange();
                            vm.set("SelectDate", kendo.parseDate(wed_date, 'yyyy/MM/dd'));                            
                        }
                    });
            });
            win.bind("deactivate", function () {
                this.unbind();                          //イベントハンドラーの登録を解除。（これをしないとactivateのハンドラが多重呼び出しされてしまう。）
            });
            win.center().open();
        },

        close: function () {
            $(vm.element).data("kendoWindow").close();
        },

        onAvailDateChange: function (e) {
            if (e) e.preventDefault();

            var index = -1;
            if (!e) {
                index = $("#drpAvailDate").data("kendoDropDownList").selectedIndex;
            } else {
                index = e.sender.selectedIndex;
            }
            if (index < 0) return false;
            var selected = this.AvailDate[index];
            if (selected.status === "×") {
                this.msgAvailDate = "指定された日付は選択できません。"
            } else {
                //指定された日付のそれぞれの時刻の予約状況を取得
                var block_date = kendo.toString(selected.block_date, "yyyy/MM/dd HH:mm:ss");
                App.Models.CItem.ChurchAvail(this.model.item_cd, block_date)
                    .done(function (list) {
                        vm.set("AvailTime", list);
                        vm.onAvailTimeChange();
                    });
            }
        },

        onAvailTimeChange: function (e) {
            if (e) e.preventDefault();

            var index = -1;
            if (!e) {
                index = $("#drpAvailTime").data("kendoDropDownList").selectedIndex;
            } else {
                index = e.sender.selectedIndex;
            }
            if (index < 0) return false;
            var selected = this.AvailTime[index];
            if (selected.status === "×") {
                this.msgAvailDate = "指定された時刻は選択できません。"
            } else {
                this.visibleAddCart = true;
            }
        },

        onAddCartClick: function (e) {
            if (e) e.preventDefault();

            var quantity = 1;
            //var quantity = $("#quantity").val();
            //if (!quantity) return false;

            var wed_date = vm.get("SelectDate");
            var min_date = moment().startOf('day');

            min_date = min_date.add(5, 'days').toDate();
            if (wed_date < min_date) {
                var msg = App.L("この商品は本日より5日後以降から購入できます。",
                                "This item is available after 5 days from today.");
                alert(msg);
                return false;
            }

            var media_info_mot = "";
            //if ("@IsMediaMot" === "True") {
            //    media_info_mot = $("#media_info_mot").data("kendoDropDownList").value();
            //    if (!media_info_mot) {
            //        var msg = App.L("マウントを選択してください。", "Please select Mount.");
            //        alert(msg);
            //        return false;
            //    }
            //}

            var media_info_cov = "";
            //if ("@IsMediaCov" === "True") {
            //    media_info_cov = $("#media_info_cov").data("kendoDropDownList").value();
            //    if (!media_info_cov) {
            //        var msg = App.L("カバーを選択してください。", "Please select Cover.");
            //        alert(msg);
            //        return false;
            //    }
            //}

            var media_info_typ = "";
            //if ("@IsMediaTyp" === "True") {
            //    media_info_typ = $("#media_info_typ").data("kendoDropDownList").value();
            //    if (!media_info_typ) {
            //        var msg = App.L("タイプを選択してください。", "Please select Type.");
            //        alert(msg);
            //        return false;
            //    }
            //}

            App.Utils.ShowLoading();

            var model = vm.get("model");

            var index = $("#drpAvailTime").data("kendoDropDownList").selectedIndex;
            if (index < 0) return false;
            var selected = this.AvailTime[index];
            var is_reserve = selected.status === "△";

            var strPrm = JSON.stringify({
                item_cd: model.item_cd,
                item_type: model.item_type,
                quantity: quantity,
                fixed_qty: model.fixed_qty,
                wed_date: kendo.toString(wed_date, "yyyy/MM/dd"),
                wed_time: $("#wed_time").val(),
                is_reserve: is_reserve,
                alb_mount: media_info_mot,
                alb_cover: media_info_cov,
                alb_type: media_info_typ
            });


            var request = $.ajax({
                url: App.getUrl("cart/addcart"),
                type: "POST",
                data: strPrm,
            processData: false,
            contentType: "application/json; charset=utf-8"
        })
        .done(function (data) {
            if (data.Result === "success" && data.Count > 0) {
                window.location.href = App.getUrl("cart/index");
                var centered = $("#centeredNotification").kendoNotification({
                    stacking: "down",
                    show: onShow,
                    button: false
                }).data("kendoNotification");
                var msg = App.L("カートに追加されました。", "Item has been added to cart.");
                centered.show(msg);
            } else if (data.Result === "error" && data.Message) {
                alert(data.Message);
            } else {
                $("div.head_middle_cart").text("");
                $("div.head_middle_cart").removeClass("head_middle_cart_count");
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert("Error: " + errorThrown + "\n\n");
        })
        .always(function () {
            App.Utils.HideLoading(true);
        });

        },

        onCloseClick: function (e) {
            if (e) e.preventDefault();
            vm.close();
        }

    });

    kendo.bind(vm.element, vm);


    ////DatePickerにユーザーの日付書式を反映。
    //App.Utils.applyUserDateFormat(vm.element);


})();