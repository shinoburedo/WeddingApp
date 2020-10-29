//TypeScriptコンパイラ用型定義
interface ISchedulePatternEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(sch_pattern_id: string, is_copied: boolean): void;
    openEditWindow(sch_pattern_id: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/SchedulePattern'],
    function (SchedulePattern : SchedulePatternFn) {

        var schedulePatternEditView : ISchedulePatternEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",
            lines: [], //グリッド用データソース
            items: [], //グリッド用データソース
            notes: [], //グリッド用データソース

            init: function (sch_pattern_id, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                schedulePatternEditView.set("model", null);
                schedulePatternEditView.set("notes", new kendo.data.DataSource([]));
                schedulePatternEditView.set("items", new kendo.data.DataSource([]));

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                SchedulePattern.fetch(sch_pattern_id, function (item) {
                    if (sch_pattern_id == "" || is_copied) {
                        item.dirty = true;
                        item.set("is_new", true);
                        item.set("id", 0);
                        item.set("sch_pattern_id", 0);
                        //item.set("model.Lines", new kendo.data.DataSource([]));
                    }
                    schedulePatternEditView.set("model", item);
                    if (sch_pattern_id) {
                        var items = SchedulePattern.getItemDataSource({ id: sch_pattern_id });
                        schedulePatternEditView.set("items", items);
                        var notes = SchedulePattern.getNoteDataSource({ id: sch_pattern_id });
                        schedulePatternEditView.set("notes", notes);
                    }

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        schedulePatternEditView.set("error", errorThrown);
                    }).always(function () {
                        schedulePatternEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (sch_pattern_id, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title("Edit SchedulePattern" || "New SchedulePattern");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, schedulePatternEditView);
                schedulePatternEditView.init(sch_pattern_id, is_copied);
            },

            isCodeEditable: function () {
                var item = schedulePatternEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = schedulePatternEditView.get("model");
                var lines = $("#grdLine").data("kendoGrid").dataSource.data();
                item.Lines = lines.toJSON();
                item.Items = schedulePatternEditView.get("items").data().toJSON();
                item.Notes = schedulePatternEditView.get("notes").data().toJSON();

                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        schedulePatternEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        schedulePatternEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return schedulePatternEditView;
    }
);




