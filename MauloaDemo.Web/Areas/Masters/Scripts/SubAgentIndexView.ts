//TypeScriptコンパイラ用型定義
interface ISubSubAgentIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/SubAgent', 'SubAgentEditView'],
    function (SubAgent : SubAgentFn, subAgentEditView: ISubAgentEditView) {

        var subAgentIndexView : ISubSubAgentIndexView = <any> kendo.observable({

            search: {
                child_cd: "",
                agent_cd: ""
            },

            list: SubAgent.getDataSource(function () {
                return subAgentIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                subAgentIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var child_cd = $(e.target).closest("a.btn").data("key");
                subAgentEditView.openEditWindow(child_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                subAgentEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var child_cd = item.get("child_cd");
                subAgentEditView.openEditWindow(child_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <ISubAgent>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var child_cd = item.get("child_cd");
                if (confirm("Are you sure to delete '" + child_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + child_cd + "' has been deleted.", false);
                            subAgentIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", subAgentIndexView);

        $("#app").on("click", "#grd .k-grid-add", subAgentIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", subAgentIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", subAgentIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", subAgentIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        subAgentEditView.bind("saved", function () {
            subAgentIndexView.list.read();
        });
    }
);



