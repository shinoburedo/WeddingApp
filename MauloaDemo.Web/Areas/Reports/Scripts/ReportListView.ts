//TypeScriptコンパイラ用型定義
interface IMgmReportIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    menuCdList: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/MgmReportComboList', 'Models/MgmReportParam', 'Models/MgmReport', 'ReportEditView'],
  function (MgmReportComboList: MgmReportComboListFn, MgmReportParam: MgmReportParamFn, MgmReport: MgmReportFn, reportEditView: IMgmReportEditView) {

      var myApp : IMgmReportIndexView = <any> kendo.observable({

          search: {
              menu_cd: "",
              rep_cd: "",
              rep_name: ""
          },
          list: MgmReport.getDataSource(function () {
              return myApp.get("search").toJSON();
          }),
          menuCdList: new kendo.data.DataSource({ data: MgmReport.MenuCdList }),
          status: "Loading...",
          error: "",

          refreshList: function (e) {
              if (e) e.preventDefault();
              myApp.list.read();
          },

          editItem: function (e) {
              if (e) e.preventDefault();
              var rep_cd = $(e.target).closest("a.btn").data("repcd");
              reportEditView.openEditWindow(rep_cd, false);
          },

          createItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              grid.clearSelection();
              reportEditView.openEditWindow("", false);
          },

          copyItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              var item = grid.dataItem(grid.select());
              if (!item) {
                  alert("Please select a report.");
                  return;
              }
              var rep_cd = item.get("rep_cd");
              reportEditView.openEditWindow(rep_cd, true);
          },

          deleteItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              var item = <IMgmReport>grid.dataItem(grid.select());
              if (!item) {
                  alert("Please select a report.");
                  return;
              }

              var rep_cd = item.get("rep_cd");
              if (confirm("Are you sure to delete the report '" + rep_cd + "'?")) {
                  App.Utils.ShowLoading();
                  item.destroy()
                      .done(function (result) {
                          if (result !== "ok") {
                              App.Utils.ShowAlert(result, true);
                              return;
                          }
                          App.Utils.ShowAlert("The report '" + rep_cd + "' has been deleted.", false);
                          myApp.list.read();
                      }).fail(function (jqXHR, textStatus, errorThrown) {
                          App.Utils.ShowAlert(errorThrown, true);
                      });
              }
          }

      });

      kendo.bind("#app", myApp);

      $("#app").on("click", "#grd .k-grid-add", myApp.createItem);
      $("#app").on("click", "#grd .k-grid-copy", myApp.copyItem);
      $("#app").on("click", "#grd .k-grid-delete", myApp.deleteItem);
      $("#app").on("click", "#grd .k-grid-refresh", myApp.refreshList);
      $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

      reportEditView.bind("saved", function () {
          myApp.list.read();
      })

  });



