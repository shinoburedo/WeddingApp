(function () {
    'use strict';

    var vm = App.ViewModels.MyWeddingCxlDialog = kendo.observable({
        element: ".cancel-confrim-window",

        region_cd: null,
        wt_id: null,
        model: null,
        cancel_done: false,

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

        getCxlChargeExt: function () {
            var model = this.get("model");
            if (!model) return "";
            return App.Utils.formatCur(model.cxl_charge_temp_ext, model.price_type, model.region_cur_symbol);
        },

        getRefund: function () {
            var model = this.get("model");
            if (!model) return "";
            var refund = model.price_charge_ext - model.cxl_charge_temp_ext;
            return App.Utils.formatCur(refund, model.price_type, model.region_cur_symbol);
        },

        isPkg: function () {
            var model = this.get("model");
            if (!model) return false;
            return model.isPkg;
        },

        invisibleCxl: function () {
            var model = this.get("model");
            if (!model) return false;
            return !model.canCxlOnline || this.get("cancel_done");
        },

        disableOnlineCxl: function () {
            var model = this.get("model");
            if (!model) return false;
            return !model.canCxlOnline;
        },

        open: function (region_cd, id) {
            vm.set("region_cd", region_cd);
            vm.set("wt_id", id);
            vm.set("model", null);
            vm.set("cancel_done", false);

            var div = $(this.get("element"));
            var win = div.data("kendoWindow");
            //win.setOptions({ title: "Option: " + id });
            win.unbind();                               //deactivateでunbindしているが念のため。
            win.bind("activate", function () {
                App.Utils.ShowLoading(true);
                App.Models.WtBooking.fetch(region_cd, id)
                    .done(function (model) {
                        App.Utils.HideLoading(true);
                        var item = new App.Models.WtBooking(model);
                        item.parseJSON();
                        vm.set("model", item);
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

        onOkClick: function (e) {
            if (e) e.preventDefault();

            var region_cd = vm.get("region_cd");
            var wt_id = vm.get("wt_id");

            App.Utils.ShowLoading(true);
            App.Models.WtBooking.cancelOrder(region_cd, wt_id)
                .done(function (result) {
                    App.Utils.HideLoading(true);
                    if (result.status == "ok") {
                        vm.set("cancel_done", true);
                        vm.trigger("cancel", { region_cd: region_cd, wt_id: wt_id });
                        App.Utils.ShowAlert(App.L("キャンセルしました。", "This item has been canceled."));
                    } else {
                        App.Utils.ShowAlert(result.message, true);
                    }
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