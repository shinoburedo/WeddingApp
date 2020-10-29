//TypeScriptコンパイラ用型定義
interface IHotelIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/Hotel', 'HotelEditView'],
    function (Hotel: HotelFn, hotelEditView: IHotelEditView) {

        var hotelIndexView : IHotelIndexView = <any> kendo.observable({

            search: {
                hotel_cd: "",
                hotel_name: "",
                area_cd: ""
            },
            list: Hotel.getDataSource(function () {
                return hotelIndexView.get("search").toJSON();
            }),
            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                hotelIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var hotel_cd = $(e.target).closest("a.btn").data("key");
                hotelEditView.openEditWindow(hotel_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                hotelEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var hotel_cd = item.get("hotel_cd");
                hotelEditView.openEditWindow(hotel_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IHotel>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var hotel_cd = item.get("hotel_cd");
                if (confirm("Are you sure to delete '" + hotel_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + hotel_cd + "' has been deleted.", false);
                            hotelIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", hotelIndexView);

        $("#app").on("click", "#grd .k-grid-add", hotelIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", hotelIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", hotelIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", hotelIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        hotelEditView.bind("saved", function () {
            hotelIndexView.list.read();
        });

    }
);



