//TypeScriptコンパイラ用型定義
interface IPickupPlaceEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(place_id: string, is_copied: boolean): void;
    openEditWindow(place_id: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/PickupPlace'],
    function (PickupPlace : PickupPlaceFn) {

        var pickupPlaceEditView : IPickupPlaceEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (place_id, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                PickupPlace.fetch(place_id, function (item) {
                        if (place_id == "" || is_copied) {
                            item.dirty = true;
                            item.set("is_new", true);
                            item.set("id", 0);
                            item.set("place_id", 0);
                        }
                        pickupPlaceEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        pickupPlaceEditView.set("error", errorThrown);
                    }).always(function () {
                        pickupPlaceEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (place_id, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(place_id || "New PickupPlace");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, pickupPlaceEditView);
                pickupPlaceEditView.init(place_id, is_copied);
            },

            isCodeEditable: function () {
                var item = pickupPlaceEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = pickupPlaceEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        pickupPlaceEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        pickupPlaceEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return pickupPlaceEditView;
    }
);




