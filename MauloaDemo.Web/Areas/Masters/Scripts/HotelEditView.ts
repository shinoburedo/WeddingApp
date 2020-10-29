//TypeScriptコンパイラ用型定義
interface IHotelEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(hotel_cd: string, is_copied: boolean): void;
    openEditWindow(hotel_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
}

define(['Models/Hotel'],
    function (Hotel) {

        var hotelEditView : IHotelEditView = <any> kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (hotel_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading(true);
                Hotel.fetch(hotel_cd, function (item) {
                        if (hotel_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        hotelEditView.set("model", item);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        hotelEditView.set("error", errorThrown);
                    }).always(function () {
                        hotelEditView.set("status", "");
                        App.Utils.HideLoading(true);
                    });
            },

            openEditWindow: function (hotel_cd: string, is_copied: boolean) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(hotel_cd || "New Hotel");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, hotelEditView);
                hotelEditView.init(hotel_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = hotelEditView.get("model");
                if (!item) return true;
                return item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading(true);
                var item = hotelEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        hotelEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        hotelEditView.onClose();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return hotelEditView;
    }
);




