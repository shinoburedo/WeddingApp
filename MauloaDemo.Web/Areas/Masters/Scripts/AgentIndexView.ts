//TypeScriptコンパイラ用型定義
interface IAgentIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

// requireメソッドを呼ぶ前に、require.configメソッドでオプションを指定する。
// JavaScriptファイルをキャッシュしない様にする。（デバッグ中は毎回、本番は1時間毎に再取得。）
var urlArgs = 'bust=' +
    (App.Config.IsTest ? (new Date()).getTime() : moment().format('YYYYMMDDHH'));
require.config({ urlArgs: urlArgs });

require(['Models/Agent', 'AgentEditView'],
    function (Agent : AgentFn, agentEditView: IAgentEditView) {

        var agentIndexView : IAgentIndexView = <any> kendo.observable({

            search: {
                agent_cd: "",
                agent_name: "",
                agent_fit: "",
                area_cd: ""
            },

            list: Agent.getDataSource(function () {
                return agentIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                agentIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var agent_cd = $(e.target).closest("a.btn").data("key");
                agentEditView.openEditWindow(agent_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                agentEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var agent_cd = item.get("agent_cd");
                agentEditView.openEditWindow(agent_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IAgent>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var agent_cd = item.get("agent_cd");
                if (confirm("Are you sure to delete '" + agent_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + agent_cd + "' has been deleted.", false);
                            agentIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", agentIndexView);

        $("#app").on("click", "#grd .k-grid-add", agentIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", agentIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", agentIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", agentIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        agentEditView.bind("saved", function () {
            agentIndexView.list.read();
        });
    }
);



