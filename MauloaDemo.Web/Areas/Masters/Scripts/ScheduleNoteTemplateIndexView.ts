//TypeScriptコンパイラ用型定義
interface IScheduleNoteTemplateIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/ScheduleNoteTemplate', 'ScheduleNoteTemplateEditView'],
    function (ScheduleNoteTemplate : ScheduleNoteTemplateFn, scheduleNoteTemplateEditView: IScheduleNoteTemplateEditView) {

        var scheduleNoteTemplateIndexView : IScheduleNoteTemplateIndexView = <any> kendo.observable({

            search: {
                template_cd: "",
                title: ""
            },

            list: ScheduleNoteTemplate.getDataSource(function () {
                return scheduleNoteTemplateIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                scheduleNoteTemplateIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var template_cd = $(e.target).closest("a.btn").data("key");
                scheduleNoteTemplateEditView.openEditWindow(template_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                scheduleNoteTemplateEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var template_cd = item.get("template_cd");
                scheduleNoteTemplateEditView.openEditWindow(template_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IScheduleNoteTemplate>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var template_cd = item.get("template_cd");
                if (confirm("Are you sure to delete '" + template_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + template_cd + "' has been deleted.", false);
                            scheduleNoteTemplateIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", scheduleNoteTemplateIndexView);

        $("#app").on("click", "#grd .k-grid-add", scheduleNoteTemplateIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", scheduleNoteTemplateIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", scheduleNoteTemplateIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", scheduleNoteTemplateIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        scheduleNoteTemplateEditView.bind("saved", function () {
            scheduleNoteTemplateIndexView.list.read();
        });
    }
);



