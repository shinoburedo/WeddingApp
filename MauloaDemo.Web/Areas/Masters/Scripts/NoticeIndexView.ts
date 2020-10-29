//TypeScriptコンパイラ用型定義
interface INoticeIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/Notice', 'NoticeEditView'],
    function (Notice : NoticeFn, noticeEditView: INoticeEditView) {

        var noticeIndexView : INoticeIndexView = <any> kendo.observable({

            search: {
                select_date: null,
                agent_cd: "",
                text: ""
            },

            list: Notice.getDataSource(function () {
                var param = noticeIndexView.get("search");
                param.select_date = kendo.toString(param.select_date, "yyyy/MM/dd HH:mm:ss");
                return param.toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                noticeIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var notice_id = $(e.target).closest("a.btn").data("key");
                noticeEditView.openEditWindow(notice_id, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                noticeEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var notice_id = item.get("notice_id");
                noticeEditView.openEditWindow(notice_id, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <INotice>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var disp_seq = item.get("disp_seq");
                if (confirm("Are you sure to delete '" + disp_seq + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + disp_seq + "' has been deleted.", false);
                            noticeIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", noticeIndexView);

        $("#app").on("click", "#grd .k-grid-add", noticeIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", noticeIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", noticeIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", noticeIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        noticeEditView.bind("saved", function () {
            noticeIndexView.list.read();
        });
    }
);



