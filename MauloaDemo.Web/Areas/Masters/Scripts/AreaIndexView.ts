//TypeScriptコンパイラ用型定義
interface IAreaIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
    }

require(['Models/Area', 'AreaEditView'],
    function (Area : AreaFn, areaEditView : IAreaEditView) {

        var areaIndexView : IAreaIndexView = <any> kendo.observable({

            search: {
                area_cd: "",
                description: ""
            },
            list: Area.getDataSource(function () {
                return areaIndexView.get("search").toJSON();
            }),
            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                areaIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var area_cd = $(e.target).closest("a.btn").data("key");
                areaEditView.openEditWindow(area_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                areaEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var area_cd = item.get("area_cd");
                areaEditView.openEditWindow(area_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IArea>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var area_cd = item.get("area_cd");
                if (confirm("Are you sure to delete '" + area_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + area_cd + "' has been deleted.", false);
                            areaIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", areaIndexView);

        $("#app").on("click", "#grd .k-grid-add", areaIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", areaIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", areaIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", areaIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        areaEditView.bind("saved", function () {
            areaIndexView.list.read();
        });

    }
);



