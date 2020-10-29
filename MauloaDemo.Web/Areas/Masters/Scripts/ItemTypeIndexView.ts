//TypeScriptコンパイラ用型定義
interface IItemTypeIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/ItemType', 'ItemTypeEditView'],
    function (ItemType : ItemTypeFn, itemTypeEditView: IItemTypeEditView) {

        var itemTypeIndexView: IItemTypeIndexView= <any> kendo.observable({

            search: {
                item_type: "",
                description: ""
            },
            list: ItemType.getDataSource(function () {
                return itemTypeIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                itemTypeIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var item_type = $(e.target).closest("a.btn").data("key");
                itemTypeEditView.openEditWindow(item_type, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                itemTypeEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var item_type = item.get("item_type");
                itemTypeEditView.openEditWindow(item_type, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IItemType>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var item_type = item.get("item_type");
                if (confirm("Are you sure to delete '" + item_type + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + item_type + "' has been deleted.", false);
                            itemTypeIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", itemTypeIndexView);

        $("#app").on("click", "#grd .k-grid-add", itemTypeIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", itemTypeIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", itemTypeIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", itemTypeIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        itemTypeEditView.bind("saved", function () {
            itemTypeIndexView.list.read();
        });

    }
);



