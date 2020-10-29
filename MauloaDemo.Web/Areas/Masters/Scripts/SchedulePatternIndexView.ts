//TypeScriptコンパイラ用型定義
interface ISchedulePatternIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    itemList: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
    onChangedItemTypeName(e?: any): void;
}

require(['Models/SchedulePattern', 'SchedulePatternEditView', 'Models/Item'],
    function (SchedulePattern: SchedulePatternFn, schedulePatternEditView: ISchedulePatternEditView, Item: ItemFn) {

        var schedulePatternIndexView : ISchedulePatternIndexView = <any> kendo.observable({

            search: {
                item_type: "",
                item_cd: "",
                description: ""
            },

            list: SchedulePattern.getDataSource(function () {
                return schedulePatternIndexView.get("search").toJSON();
            }),

            itemList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("items/Search"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                item_type: schedulePatternIndexView.get("search.item_type") || ""
                            };
                        }
                    }
                },
                schema: {
                    model: Item
                },
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                schedulePatternIndexView.get("list").read();
            },

            onChangedItemTypeName: function () {
                schedulePatternIndexView.itemList.read();
                schedulePatternIndexView.refreshList();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var sch_pattern_id = $(e.target).closest("a.btn").data("key");
                schedulePatternEditView.openEditWindow(sch_pattern_id, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                schedulePatternEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var sch_pattern_id = item.get("sch_pattern_id");
                schedulePatternEditView.openEditWindow(sch_pattern_id, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <ISchedulePattern>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var description = item.get("description");
                if (confirm("Are you sure to delete '" + description + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + description + "' has been deleted.", false);
                            schedulePatternIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", schedulePatternIndexView);

        var combo_type = $("#app").find("#cmbItemType").data("kendoComboBox");
        if (combo_type) combo_type.list.width(200);
        var combo_cd = $("#app").find("#cmbItemCd").data("kendoComboBox");
        if (combo_cd) combo_cd.list.width(700);

        $("#app").on("click", "#grd .k-grid-add", schedulePatternIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", schedulePatternIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", schedulePatternIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", schedulePatternIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        schedulePatternEditView.bind("saved", function () {
            schedulePatternIndexView.list.read();
        });

        schedulePatternIndexView.bind("change", function (e) {

            if (e.field === "search.item_type") {
                schedulePatternIndexView.onChangedItemTypeName();
            }

        });


    }
);



