//TypeScriptコンパイラ用型定義
interface ILoginUserIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/LoginUser', 'LoginUserEditView'],
    function (LoginUser : LoginUserFn, editView: ILoginUserEditView) {

        var indexView : ILoginUserIndexView = <any> kendo.observable({

            search: {
                login_id: "",
                user_name: "",
                sub_agent_cd: ""
            },

            list: LoginUser.getDataSource(function () {
                var data = indexView.get("search").toJSON();
                return data;
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                indexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var login_id = $(e.target).closest("a.btn").data("key");
                editView.openEditWindow(login_id, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                editView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var login_id = item.get("login_id");
                editView.openEditWindow(login_id, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <ILoginUser>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var login_id = item.get("login_id");
                if (confirm("Are you sure to delete '" + login_id + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + login_id + "' has been deleted.", false);
                            indexView.refreshList();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            },

            onSetAutoPwd: function (e) {
                if (e) e.preventDefault();
                if (!confirm("パスワードが設定されていないユーザーにデフォルトの一時パスワードをセットします。よろしいですか?")) return;
                var btn = $(e.target);

                App.Utils.ShowLoading();
                LoginUser.setAutoPwd()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        App.Utils.ShowAlert("Completed successfully!", false);
                        indexView.refreshList();
                        btn.hide();

                    }).fail(App.Utils.ShowAlertAjaxErr);
            }
        });

        kendo.bind("#app", indexView);

        $("#app").on("click", "#grd .k-grid-add", indexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", indexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", indexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", indexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        editView.bind("saved", function () {
            indexView.refreshList();
        });
    }
);



