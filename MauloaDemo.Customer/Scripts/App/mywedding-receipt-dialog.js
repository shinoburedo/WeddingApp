(function () {
    'use strict';

    var vm = App.ViewModels.MyWeddingReceiptDialog = kendo.observable({
        element: ".receipt-window",

        region_cd: null,
        wt_id: null,
//        cancel_done: false,
        receipt_list: [],

        visible: function () {
            var list = this.get("receipt_list");
            if (!list) return false;
            if (list.length === 0) return false;
            if (list.total() === 0) return false;
            return true;
        },

        invisible: function () {
            return !this.visible();
        },

        open: function (region_cd, c_num) {
            vm.set("region_cd", region_cd);
            vm.set("cancel_done", false);

            var div = $(this.get("element"));
            var win = div.data("kendoWindow");
            //win.setOptions({ title: "Option: " + id });
            win.unbind();                               //deactivateでunbindしているが念のため。
            win.bind("activate", function () {
//                App.Utils.ShowLoading(true);
                var list = App.Models.WtBooking.getReceiptList(region_cd, c_num);
                vm.set("receipt_list", list);
                
                //App.Models.WtBooking.getReceiptList(region_cd, c_num)
                //    .done(function (list) {
                //        App.Utils.HideLoading(true);
                //        vm.set("receipt_list", list);
                //    });
            });
            win.bind("deactivate", function () {
                this.unbind();                          //イベントハンドラーの登録を解除。（これをしないとactivateのハンドラが多重呼び出しされてしまう。）
            });
            win.center().open();
        },

        close: function () {
            $(vm.element).data("kendoWindow").close();
        },

        onPrintClick: function (e) {
            if (e) e.preventDefault();
            //console.log("onConfirmationLetterClick:", e);
            var region_cd = $(e.target).data("regioncd");

            var checkboxes = $("#receiptList").find("input:checkbox");
            var selectedItems = [];
            var cnt_pkg = 0;
            var dataSource = this.get("receipt_list");
            var str_wt_id = "";

            $(checkboxes).each(function (i, checkbox) {
                checkbox = $(checkbox);
                if (checkbox.is(":checked")) {
                    var id = checkbox.parent().parent().attr('id');
                    if (str_wt_id) {
                        str_wt_id += "," + id;
                    } else {
                        str_wt_id = id;
                    }
                    //var booking = dataSource.get(id);
                    //selectedItems.push(booking);
                }
            });

            if (!str_wt_id) {
                alert(App.L("領収書を出力する商品を選択してください。", "Please select one or more items you would like to print receipt."));
                return false;
            }
            //if (selectedItems.length === 0) {
            //    alert(App.L("領収書を出力する商品を選択してください。", "Please select one or more items you would like to print receipt."));
            //    return false;
            //}

            var prmData = {
                list: selectedItems
            };

            var url = App.getUrl("MyWedding/PreviewReceipt") + "?list=" + str_wt_id;
            window.open(url, "_blank");

//            var request = $.ajax({
//                url: App.getUrl("MyWedding/PreviewReceipt"),
//                type: "POST",
//                data: JSON.stringify(prmData),
//            processData: false,
//            contentType: "application/json; charset=utf-8"
//            }).done(function (data) {
//                window.open(data, "_blank");
////            App.Utils.HideLoading(true);
//            //if (data.Result === "success") {
//            //    window.location.href = "@(Url.Action("Customer", "Purchase"))";
//            //} else if (data.Result === "confirm") {
//            //    window.location.href = "@(Url.Action("Confirmation", "Cart"))";
//            //} else if (data.Result === "error" && data.Message) {
//            //    alert(data.Message);
//            //} else {
//            //    alert("Error!");
//            //}
//        }).fail(function (jqXHR, textStatus, errorThrown) {
//            App.Utils.HideLoading(true);
//            alert("Error: " + errorThrown + "\n\n");
//        });



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