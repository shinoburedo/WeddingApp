//TypeScriptコンパイラ用型定義
interface IChurchTimeEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(church_cd: string, is_copied: boolean): void;
    openEditWindow(church_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
    refreshItemCombo(e?: any): void;
    itemList: kendo.data.DataSource;
    addItem(e?: any);
    editItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

define(['Models/ChurchTime', 'Models/Church'], 
    function (ChurchTime: ChurchTimeFn, Church: ChurchFn) {

        var churchTimeEditView: IChurchTimeEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",
            churchTimes: [], //グリッド用データソース

            init: function (church_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                Church.fetch(church_cd, function (item) {
                    var list = ChurchTime.getDataSource({ church_cd: church_cd || "-" });
                    if (church_cd == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                        item.set("is_new", true);
                    }
                    churchTimeEditView.set("model", item);
                    churchTimeEditView.set("churchTimes", list);
                }
                ).fail(function (jqXHR, textStatus, errorThrown) {
                    churchTimeEditView.set("error", errorThrown);
                }).always(function () {
                    churchTimeEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });

            },

            openEditWindow: function (church_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(church_cd || "New ChurchTime");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, churchTimeEditView);
                churchTimeEditView.init(church_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = churchTimeEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                var item = churchTimeEditView.get("model");
                if (!item.church_cd) {
                    alert("Enter Item Code!");
                    return false;
                }

                var dataSource = churchTimeEditView.get("churchTimes");
                var total = dataSource.total();
                var churchTimes = [];

                for (var i = 0; i < total; i++) {
                    var ds = dataSource.at(i);
                    if (ds && ds.dirty) {
                        if (!ds.start_time) {
                            //空行の場合エラー
                            alert("Empty record is not allowed.");
                            return false;
                        }

                        churchTimes.push({
                            church_time_id: ds.church_time_id,
                            church_cd: item.church_cd,
                            start_time: kendo.toString(ds.start_time, 'HH:mm')
                        });
                    }
                }

                if (churchTimes.length > 0) {
                    App.Utils.ShowLoading();
                    var data = {
                        list: churchTimes
                    };
                    ChurchTime.saveList(data).done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        churchTimeEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        churchTimeEditView.init(item.church_cd, false);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            },

            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            },

            //addItem: function (e) {
            //    if (e) e.preventDefault();
            //    var grid = $("#grdChurchTime").data("kendoGrid");
            //    var dataSource = grid.dataSource;
            //    dataSource.add({ start_time: "" });
            //    grid.select(grid.element.find('tbody tr:last'));
            //    grid.editRow(grid.tbody.children().last());
            //},

            refreshList: function (e) {
                if (e) e.preventDefault();
                var church_cd = churchTimeEditView.get("model.church_cd");
                var list = ChurchTime.getDataSource({ church_cd: church_cd || "-" });
                churchTimeEditView.set("churchTimes", list);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grdChurchTime").data("kendoGrid");
                var item = <IChurchTime>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var start_time = kendo.toString(item.get("start_time"), "HH:mm");

                if (confirm("Are you sure to delete '" + start_time + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + start_time + "' has been deleted.", false);
                            churchTimeEditView.refreshList();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        //$("#editDiv").on("click", "#grdChurchTime .k-grid-add", churchTimeEditView.addItem);
        $("#editDiv").on("click", "#grdChurchTime .k-grid-delete", churchTimeEditView.deleteItem);
        $("#editDiv").on("click", "#grdChurchTime .k-grid-refresh", churchTimeEditView.refreshList);
        $("#grdChurchTime .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        return churchTimeEditView;
    }
);




