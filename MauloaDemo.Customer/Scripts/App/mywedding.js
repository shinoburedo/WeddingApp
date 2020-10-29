(function () {

    var vm = App.ViewModels.MyWedding = kendo.observable({
        element: ".mywedding",

        accountId: App.User.Id,
        c_num: App.User.c_num,
        userName: App.User.Name,
        is_loading: true,
        show_cxl: true,

        wtAccount: new App.Models.WtAccount(),
        regions: [],
        bgList: new kendo.data.DataSource({ data: [{ value: "G", text: App.L("新郎側", "Groom") }, { value: "B", text: App.L("新婦側", "Bride") }] }),

        airportList: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getUrl("MyWedding/GetAirportList"),
                    type: "GET",
                    dataType: "json"
                }
            }
        }),

        getRegionData: function (region_cd) {
            return _.find(vm.get("regions"), function (item) { return item.region_cd == region_cd });
        },

        hasRegions: function () {
            return this.get("regions").length > 0;
        },
        hasNoRegions: function () {
            if (this.get("is_loading")) return false;
            return !this.get("c_num") || this.get("regions").length == 0;
        },

        onConfirmationLetterClick: function (e) {
            if (e) e.preventDefault();
            //console.log("onConfirmationLetterClick:", e);
            var region_cd = $(e.target).data("regioncd");
            var url = App.getUrl("MyWedding/PreviewConfirmationLetter") + "?rg=" + region_cd;
            window.open(url, "_blank");
        },

        onEditClick: function (e) {
            //console.log("onEditClick", e);
            if (e) e.preventDefault();

            var region_cd = $(e.target).data("regioncd");
            var regionData = vm.getRegionData(region_cd);

            //タイミングの関係で初期状態では表示されていないのでホテル名を手動でコンボボックスに反映。
            var cboHotel = $(e.target).closest(".regions-item").find("select.cboHotel").data("kendoDropDownList");
            cboHotel.value(regionData.customer.gb_hotel1);

            regionData.set("is_editing", true);
            ////フライト日のMax、Minを挙式日にセット
            //var wed_date = regionData.customer.wed_date;
            //$(e.target).closest(".regions-item").find("input.flight_date1").data("kendoDatePicker").max(wed_date);
            //$(e.target).closest(".regions-item").find("input.arr_time1").data("kendoDatePicker").max(wed_date);
            //$(e.target).closest(".regions-item").find("input.flight_date2").data("kendoDatePicker").min(wed_date);
            //$(e.target).closest(".regions-item").find("input.arr_time2").data("kendoDatePicker").min(wed_date);
        },

        onSaveClick: function (e) {
            if (e) e.preventDefault();

            var region_cd = $(e.target).data("regioncd");
            var regionData = vm.getRegionData(region_cd);
            //フライト情報入力チェック
            if (!vm.validateInput(regionData.customer)) return false;

            var cboHotel = $(e.target).closest(".regions-item").find("select.cboHotel").data("kendoDropDownList");

            var data = regionData.toJSON();
            delete data.hotelList;          //DataSourceをパラメータに含めるとエラーになるので削除。
            delete data.orders;             //DataSourceをパラメータに含めるとエラーになるので削除。
            delete data.winkOptionList;     //DataSourceをパラメータに含めるとエラーになるので削除。
            App.addAntiForgeryToken(data);  //CSRF対策用トークンを追加。

            App.Utils.ShowLoading(true);
            $.ajax({
                url: App.getUrl("MyWedding/UpdateMyWeddingInfoByRegion"),
                data: data,
                dataType: "json",
                type: "POST"
            }).done(function (result) {
                if (result.status == "ok") {
                    regionData.set("customer.gb_hotel1_name", cboHotel.text());
                    regionData.set("is_editing", false);
                    App.Utils.ShowAlert(App.L("保存しました。", "Data updated."), false);
                    getWtAccount();
                } else {
                    App.Utils.ShowAlert(App.L("エラーが発生しました。", "An error occurred. ") + "\n\n" + result.message, true);
                }
            });
        },

        validateInput: function (customer) {

            var weddate = customer.wed_date;
            if (weddate != null) {
                var depdate1 = customer.flight_date1;
                var onemonth_weddate = new Date(weddate);
                onemonth_weddate.setMonth(onemonth_weddate.getMonth() - 1);
                if (depdate1 != null) {
                    if (weddate < depdate1) {
                        alert(App.L("ﾌﾗｲﾄ(現地到着) 出発日、到着日が挙式日の翌日以降です。", "Flight Date (Local Arrival) is after the wedding date."));
                        return false;
                    }
                    if (onemonth_weddate >= depdate1) {
                        if (!confirm(App.L("ﾌﾗｲﾄ(現地到着) 出発日、到着日が挙式日より１か月以上前です。よろしいですか？", "Flight Date (Local Arrival) is more than 1 month before the wedding date. Do you want to continue?"))) {
                            return false;
                        }
                    }
                }
                var arrdate1 = customer.arr_time1;
                if (arrdate1 != null) {
                    if (weddate < arrdate1) {
                        alert(App.L("ﾌﾗｲﾄ(現地到着) 出発日、到着日が挙式日の翌日以降です。", "Flight Date (Local Arrival) is after the wedding date."));
                        return false;
                    }
                    if (onemonth_weddate >= arrdate1) {
                        if (!confirm(App.L("ﾌﾗｲﾄ(現地到着) 出発日、到着日が挙式日より１か月以上前です。よろしいですか？", "Flight Date (Local Arrival) is more than 1 month before the wedding date. Do you want to continue?"))) {
                            return false;
                        }
                    }
                }
                var depdate2 = customer.flight_date2;
                if (depdate2 != null) {
                    if (weddate > depdate2) {
                        alert(App.L("ﾌﾗｲﾄ(現地出国) 出発日、到着日が挙式日前です。", "Flight Date (Local Departure) is before the wedding date."));
                        return false;
                    }
                }
                var arrdate2 = customer.arr_time2;
                if (arrdate2 != null) {
                    if (weddate > arrdate2) {
                        alert(App.L("ﾌﾗｲﾄ(現地出国) 出発日、到着日が挙式日前です。", "Flight Date (Local Departure) is before the wedding date."));
                        return false;
                    }
                }
            }
            return true;
        },

        onCancelClick: function (e) {
            if (e) e.preventDefault();

            var region_cd = $(e.target).data("regioncd");
            var regionData = vm.getRegionData(region_cd);
            regionData.set("is_editing", false);
            getWtAccount();
        },

        onOrderCancelClick: function (e) {
            if (e) e.preventDefault();
            var region_cd = $(e.target).data("regioncd");
            var wt_id = $(e.target).data("wtid");
            //console.log("onOrderCancelClick: region_cd=", region_cd, "wt_id=", wt_id);

            App.ViewModels.MyWeddingCxlDialog.open(region_cd, wt_id);
        },

        cancelDone: function (e) {
            //console.log("cancelDone: region_cd=", e.region_cd, "wt_id=", e.wt_id);
            getMyWeddingInfo();
        },

        postDone: function (e) {
            //console.log("postDone: region_cd=", e.region_cd, "wt_id=", e.wt_id);
            getMyWeddingInfo();
        },

        onPaymentClick: function (e) {
            if (e) e.preventDefault();
            var region_cd = $(e.target).data("regioncd");
            var wt_id = $(e.target).data("wtid");
            //console.log("onPaymentClick: region_cd=", region_cd, "wt_id=", wt_id);

            App.ViewModels.MyWeddingPaymentDialog.open(region_cd, wt_id);
        },

        paymentDone: function (e) {
            //console.log("paymentDone: region_cd=", e.region_cd, "wt_id=", e.wt_id);
            getMyWeddingInfo();
        },

        onShowCxlClick: function (e) {
            //CheckBoxのcheckedの値がViewModelに反映されるのを待って全ての情報を再取得。
            setTimeout(function () {
                getWtAccount();
            }, 1);
        },

        onOrderDetailClick: function (e) {
            if (e) e.preventDefault();
            var btn = $(e.delegateTarget);
            var region_cd = btn.data("regioncd");
            var wt_id = btn.data("wtid");
            //console.log("onOrderDetailClick: region_cd=", region_cd, "wt_id=", wt_id);

            App.ViewModels.MyWeddingOrderDetailDialog.open(region_cd, wt_id);
        },

        onReviewClick: function (e) {
            if (e) e.preventDefault();
            var region_cd = $(e.target).data("regioncd");
            var wt_id = $(e.target).data("wtid");
            var account_id = $(e.target).data("acctid");
            var item_cd = $(e.target).data("itemcd");
            var item_name = $(e.target).data("itemname");

            App.ViewModels.MyWeddingReviewDialog.open(region_cd, wt_id, account_id, item_cd, item_name);
        },

        onReceiptClick: function (e) {
            if (e) e.preventDefault();
            var region_cd = $(e.target).data("regioncd");
            var c_num = vm.get("c_num");
            App.ViewModels.MyWeddingReceiptDialog.open(region_cd, c_num);
        },

        saveDone: function (e) {
            //console.log("saveDone: region_cd=", e.region_cd, "wt_id=", e.wt_id);
            getMyWeddingInfo();
        }

    });

    kendo.bind(vm.element, vm);

    App.ViewModels.MyWeddingCxlDialog.bind("cancel", vm.cancelDone);
    App.ViewModels.MyWeddingPaymentDialog.bind("payment", vm.paymentDone);
    App.ViewModels.MyWeddingOrderDetailDialog.bind("save", vm.saveDone);
    App.ViewModels.MyWeddingReviewDialog.bind("post", vm.postDone);

    //DatePickerにユーザーの日付書式を反映。
    App.Utils.applyUserDateFormat(vm.element);

    //データを取得。
    getWtAccount();

    $("#chkShowCxl").on("click", vm.onShowCxlClick);

    function getWtAccount() {
        vm.set("wtAccount", new App.Models.WtAccount());
        App.Utils.ShowLoading(true);
        App.Models.WtAccount.fetch()
            .done(function (data) {
                //console.log("wtAccount=", data);
                var wtAccount = new App.Models.WtAccount(data);
                vm.set("wtAccount", wtAccount);
                getMyWeddingInfo();
            });
    }

    function getMyWeddingInfo() {
        App.Models.WtAccount.fetchMyWeddingInfo()
            .done(function (data) {
                //console.log("fetchMyWeddingInfo: raw data=", data.length, data);

                var show_cxl = !!(vm.get("show_cxl"));
                var regions = [];
                for (var i = 0; i < data.length; i++) {
                    var rgn = data[i];
                    var new_rgn = {
                        region_cd: rgn.region_cd,
                        region_name_dis: rgn.region_name_dis,
                        is_editing: false,
                        show_cxl: show_cxl,
                        hotelList: App.Models.Hotel.getDataSource(rgn.region_cd, (rgn.customer ? rgn.customer.area_cd : ""))
                    };
                    if (rgn.customer) {
                        new_rgn.customer = new App.Models.Customer(rgn.customer);
                        new_rgn.customer.parseJSON();
                        new_rgn.jpnInfo = new App.Models.JpnInfo(rgn.jpnInfo);
                        new_rgn.jpnInfo.parseJSON();
                        new_rgn.wtWedInfo = new App.Models.WtWedInfo(rgn.wtWedInfo);
                        new_rgn.wtWedInfo.parseJSON();

                        //地域毎のオーダー一覧を取得。
                        new_rgn.orders = App.Models.WtBooking.getDataSource(new_rgn.region_cd, new_rgn.customer.c_num, new_rgn.show_cxl);
                        new_rgn.winkOptionList = App.Models.WtBooking.getWinkOptionList(new_rgn.region_cd, new_rgn.customer.c_num);
                        regions.push(new_rgn);
                    }
                }
                //console.log("fetchMyWeddingInfo: regions=", regions);

                vm.set("regions", regions);
                vm.set("is_loading", false);
                App.Utils.HideLoading(true);

                setTimeout(function () {
                    //ListView内のDatePickerにユーザーの日付書式を反映。
                    App.Utils.applyUserDateFormat(vm.element);
                    App.Utils.AddNoImageHandler();
                }, 800);
            });
    }

})();