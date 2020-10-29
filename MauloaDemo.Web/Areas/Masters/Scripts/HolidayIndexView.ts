//TypeScriptコンパイラ用型定義
interface IHolidayIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
    }

require(['Models/Holiday', 'HolidayEditView'],
    function (Holiday : HolidayFn, holidayEditView : IHolidayEditView) {

        var holidayIndexView : IHolidayIndexView = <any> kendo.observable({

            search: {
                holiday: "",
                description: ""
            },
            list: Holiday.getDataSource(function () {
                return holidayIndexView.get("search").toJSON();
            }),
            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                holidayIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var holiday = $(e.target).closest("a.btn").data("key");
                holidayEditView.openEditWindow(holiday, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                holidayEditView.openEditWindow(null, false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var holiday = item.get("holiday");
                holidayEditView.openEditWindow(holiday, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IHoliday>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var holiday = item.get("holiday");
                var strHoliday = kendo.toString(kendo.parseDate(holiday), 'yyyy/MM/dd');
                if (confirm("Are you sure to delete '" + strHoliday + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + strHoliday + "' has been deleted.", false);
                            holidayIndexView.list.read();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", holidayIndexView);

        $("#app").on("click", "#grd .k-grid-add", holidayIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", holidayIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", holidayIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", holidayIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        holidayEditView.bind("saved", function () {
            holidayIndexView.list.read();
        });

    }
);



