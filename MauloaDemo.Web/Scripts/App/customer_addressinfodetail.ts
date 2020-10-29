define(['models/address_info', 'models/customer' ],
    function (AddressInfo, Customer) {

    var addressInfoViewModel = <any>kendo.observable({

        address_info: {},
        finalized: false,
        dirty: false,
        saved_callback: null,

        paxTypeList: [{ pax_type: 'G', description: '新郎' }
                    , { pax_type: 'B', description: '新婦' }
                    , { pax_type: 'A', description: '列席' } ],

        openWindow: function (address_info, finalized, saved_callback) {
            this.set("address_info", address_info);
            this.set("finalized", finalized);
            this.set("saved_callback", saved_callback);
            this.set("dirty", false);

            var win = $("#AddressInfoDialog");
            var kendoWindow = win.data("kendoWindow");
            kendoWindow.setOptions({
                title: address_info.info_id == 0 ? "New Address" : "Address",
                width: "70%",
                minWidth: 360,
                minHeight: 500,
            });
            kendoWindow.center().open();
        },

        closeDialog: function (e) {
            if (e) e.preventDefault();
            $(".addressinfo-dialog").parent().data("kendoWindow").close();
        },

        onOkClick: function (e) {
            if (e) e.preventDefault();
            var address_info = addressInfoViewModel.get("address_info");
            addressInfoViewModel.saveAddressInfo(address_info);
        },

        isSaveButtonVisible: function () {
            return App.User.AccessLevel >= 3;
        },

        isDisabled: function () {
            return !!addressInfoViewModel.get("finalized") || App.User.AccessLevel < 3;
        },

        saveAddressInfo: function (address_info) {
            //log("saveAddressInfo", address_info);
            App.Utils.ShowLoading(true);

            address_info.save({
                success: function (result) {
                    if (result && result.info_id > 0 && result.message === "ok") {
                        //log("Address Info saved. info_id=" + result.info_id);
                        addressInfoViewModel.set("address_info.info_id", result.info_id);
                        App.Utils.HideLoading(true);
                        addressInfoViewModel.closeDialog();
                        var callback = addressInfoViewModel.get("saved_callback");
                        if (callback) callback();

                    } else {
                        App.Utils.HideLoading(true);
                        App.Utils.ShowAlert(result.message, true);
                    }
                },
                fail: function (jqXHR, textStatus, errorThrown) {
                    App.Utils.HideLoading(true);
                    App.Utils.ShowAlertAjaxErr(jqXHR, textStatus, errorThrown);
                }
            });
        }
    });

    addressInfoViewModel.bind("change", function (e) {
        //フィールド名が「address_info.」で始まる場合はdirtyフラグを立てる。
        if (e.field.indexOf("address_info.") == 0) {
            addressInfoViewModel.set("dirty", true);
        }
    });

    return addressInfoViewModel;

});
