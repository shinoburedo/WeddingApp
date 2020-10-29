define(['models/cos_info', 'models/customer' ],
    function (CosInfo, Customer) {

    var cosInfoViewModel = <any>kendo.observable({

        cos_info: {},
        finalized: false,
        dirty: false,
        saved_callback: null,

        paxTypeList: [{ pax_type: 'G', description: App.L('新郎', 'Groom') }
                    , { pax_type: 'B', description: App.L('新婦', 'Bride') }
                    , { pax_type: 'A', description: App.L('列席', 'Attendant') } ],

        openWindow: function (cos_info, finalized, saved_callback) {
            this.set("cos_info", cos_info);
            this.set("finalized", finalized);
            this.set("saved_callback", saved_callback);
            this.set("dirty", false);

            var win = $("#CosInfoDialog");
            var kendoWindow = win.data("kendoWindow");
            kendoWindow.setOptions({
                title: cos_info.id === 0 ? "New Costume" : "Costume",
                width: "70%",
                minWidth: 360,
                minHeight: 250,
            });
            kendoWindow.center().open();
        },

        closeDialog: function (e) {
            if (e) e.preventDefault();
            $(".cosinfo-dialog").parent().data("kendoWindow").close();
        },

        onOkClick: function (e) {
            if (e) e.preventDefault();
            var cos_info = cosInfoViewModel.get("cos_info");
            cosInfoViewModel.saveCosInfo(cos_info);
        },

        isSaveButtonVisible: function () {
            return App.User.AccessLevel >= 3;
        },

        isDisabled: function () {
            return !!cosInfoViewModel.get("finalized") || App.User.AccessLevel < 3;
        },

        isArrangementVisible: function () {
            return !App.User.IsAgent;
        },

        saveCosInfo: function (cos_info) {
            //log("saveCosInfo", cos_info);
            App.Utils.ShowLoading(true);

            cos_info.save({
                success: function (result) {
                    if (result && result.info_id > 0 && result.message === "ok") {
                        //log("Costume Info saved. info_id=" + result.info_id);
                        cosInfoViewModel.set("cos_info.info_id", result.info_id);
                        App.Utils.HideLoading(true);
                        cosInfoViewModel.closeDialog();
                        var callback = cosInfoViewModel.get("saved_callback");
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

    cosInfoViewModel.bind("change", function (e) {
        //フィールド名が「cos_info.」で始まる場合はdirtyフラグを立てる。
        if (e.field.indexOf("cos_info.") == 0) {
            cosInfoViewModel.set("dirty", true);
        }
    });

    return cosInfoViewModel;

});
