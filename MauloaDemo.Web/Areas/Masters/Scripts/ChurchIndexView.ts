//TypeScriptコンパイラ用型定義
interface IChurchIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/Church', 'ChurchEditView'],
    function (Church : ChurchFn, churchEditView: IChurchEditView) {

        var churchIndexView : IChurchIndexView = <any> kendo.observable({

            search: {
                church_cd: "",
                church_name: "",
                church_fit: "",
                area_cd: ""
            },

            list: Church.getDataSource(function () {
                return churchIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                churchIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var church_cd = $(e.target).closest("a.btn").data("key");
                churchEditView.openEditWindow(church_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                churchEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var church_cd = item.get("church_cd");
                churchEditView.openEditWindow(church_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IChurch>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var church_cd = item.get("church_cd");
                if (confirm("Are you sure to delete '" + church_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + church_cd + "' has been deleted.", false);
                            churchIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", churchIndexView);

        $("#app").on("click", "#grd .k-grid-add", churchIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", churchIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", churchIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", churchIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        churchEditView.bind("saved", function () {
            churchIndexView.list.read();
        });
    }
);



