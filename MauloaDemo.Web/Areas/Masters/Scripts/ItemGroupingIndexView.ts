//TypeScriptコンパイラ用型定義
interface IItemGroupingIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/ItemOption', 'ItemGroupingEditView'],
    function (ItemOption: ItemOptionFn, itemGroupingEditView: IItemGroupingEditView) {

        var itemGroupingIndexView: IItemGroupingIndexView= <any> kendo.observable({

            search: {
                item_cd: "",
                item_type: ""
            },
            list: ItemOption.getDataSource(function () {
                return itemGroupingIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                itemGroupingIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var item_type = $(e.target).closest("a.btn").data("key");
                itemGroupingEditView.openEditWindow(item_type, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                itemGroupingEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var item_cd = item.get("item_cd");
                itemGroupingEditView.openEditWindow(item_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IItemOption>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var item_cd = item.get("item_cd");
                if (confirm("Are you sure to delete '" + item_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + item_cd + "' has been deleted.", false);
                            itemGroupingIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", itemGroupingIndexView);

        $("#app").on("click", "#grd .k-grid-add", itemGroupingIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", itemGroupingIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", itemGroupingIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", itemGroupingIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        itemGroupingEditView.bind("saved", function () {
            itemGroupingIndexView.list.read();
        });

    }
);



