//TypeScriptコンパイラ用型定義IScheduleNoteTemplateEditView
interface IScheduleNoteTemplateEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(template_cd: string, is_copied: boolean): void;
    openEditWindow(template_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/ScheduleNoteTemplate'],
    function (ScheduleNoteTemplate : ScheduleNoteTemplateFn) {

        var scheduleNoteTemplateEditView : IScheduleNoteTemplateEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (template_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                ScheduleNoteTemplate.fetch(template_cd, function (item) {
                        if (template_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        scheduleNoteTemplateEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        scheduleNoteTemplateEditView.set("error", errorThrown);
                    }).always(function () {
                        scheduleNoteTemplateEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (template_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(template_cd || "New ScheduleNoteTemplate");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, scheduleNoteTemplateEditView);
                scheduleNoteTemplateEditView.init(template_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = scheduleNoteTemplateEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = scheduleNoteTemplateEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        scheduleNoteTemplateEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        scheduleNoteTemplateEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return scheduleNoteTemplateEditView;
    }
);




