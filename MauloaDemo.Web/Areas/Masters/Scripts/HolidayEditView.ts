//TypeScriptコンパイラ用型定義
interface IHolidayEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(holiday: Date, is_copied: boolean): void;
    openEditWindow(holiday: Date, is_new: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
}

define(['Models/Holiday'],
    function (Holiday: HolidayFn) {

        var holidayEditView : IHolidayEditView= <any> kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (holiday, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading(true);
                Holiday.fetch(holiday, function (item) {
                    if (holiday == null || is_copied) {
                            item.set("is_new", true);     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        holidayEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        holidayEditView.set("error", errorThrown);
                    }).always(function () {
                        holidayEditView.set("status", "");
                        App.Utils.HideLoading(true);
                    });
            },

            openEditWindow: function (holiday: Date, is_copied: boolean) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(kendo.toString(holiday, "MM/dd/yyyy") || "New Holiday");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, holidayEditView);
                holidayEditView.init(holiday, is_copied);
            },

            isCodeEditable: function () {
                var item = holidayEditView.get("model");
                if (!item) return true;
                return item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading(true);
                var item = holidayEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        holidayEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        holidayEditView.onClose();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return holidayEditView;
    }
);




