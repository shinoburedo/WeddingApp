//TypeScriptコンパイラ用型定義
interface IPickupPlaceIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/PickupPlace', 'PickupPlaceEditView'],
    function (PickupPlace : PickupPlaceFn, pickupPlaceEditView: IPickupPlaceEditView) {

        var pickupPlaceIndexView : IPickupPlaceIndexView = <any> kendo.observable({

            search: {
                hotel_cd: "",
                getAllList: true
            },

            list: PickupPlace.getDataSource(function () {
                return pickupPlaceIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                pickupPlaceIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var place_id = $(e.target).closest("a.btn").data("key");
                pickupPlaceEditView.openEditWindow(place_id, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                pickupPlaceEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var place_id = item.get("place_id");
                pickupPlaceEditView.openEditWindow(place_id, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IPickupPlace>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var place_order = item.get("place_order");
                if (confirm("Are you sure to delete '" + place_order + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + place_order + "' has been deleted.", false);
                            pickupPlaceIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", pickupPlaceIndexView);

        $("#app").on("click", "#grd .k-grid-add", pickupPlaceIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", pickupPlaceIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", pickupPlaceIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", pickupPlaceIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        pickupPlaceEditView.bind("saved", function () {
            pickupPlaceIndexView.list.read();
        });
    }
);



