//TypeScriptコンパイラ用型定義
interface IVendorEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(vendor_cd: string, is_copied: boolean): void;
    openEditWindow(vendor_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/Vendor'],
    function (Vendor : VendorFn) {

        var vendorEditView : IVendorEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (vendor_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                Vendor.fetch(vendor_cd, function (item) {
                        if (vendor_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        vendorEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        vendorEditView.set("error", errorThrown);
                    }).always(function () {
                        vendorEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (vendor_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(vendor_cd || "New Vendor");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, vendorEditView);
                vendorEditView.init(vendor_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = vendorEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = vendorEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        vendorEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        vendorEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return vendorEditView;
    }
);




