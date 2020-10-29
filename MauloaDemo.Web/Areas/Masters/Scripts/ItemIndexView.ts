//TypeScriptコンパイラ用型定義
interface IItemIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/Item', 'ItemEditView'],
    function (Item: ItemFn, itemEditView: IItemEditView) {

        var itemIndexView: IItemIndexView= <any> kendo.observable({

            search: {
                item_cd: "",
                item_name: "",
                item_type: ""
            },
            list: Item.getDataSource(function () {
                return itemIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                itemIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var item_type = $(e.target).closest("a.btn").data("key");
                itemEditView.openEditWindow(item_type, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                itemEditView.openEditWindow("", false);
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
                itemEditView.openEditWindow(item_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IItem>grid.dataItem(grid.select());
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
                            itemIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", itemIndexView);

        $("#app").on("click", "#grd .k-grid-add", itemIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", itemIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", itemIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", itemIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        itemEditView.bind("saved", function () {
            itemIndexView.list.read();
        });

    }
);



