//TypeScriptコンパイラ用型定義
interface IMgmReportComboListIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/MgmReportComboList', 'ComboListEditView'],
  function (MgmReportComboList: MgmReportComboListFn, comboListEditView: IMgmReportComboListEditView) {

      var myApp : IMgmReportComboListIndexView = <any> kendo.observable({

          search: {
              list_cd: "",
              description: ""
          },
          list: MgmReportComboList.getDataSource(function () {
              return myApp.get("search").toJSON();
          }),
          status: "Loading...",
          error: "",

          refreshList: function (e) {
              if (e) e.preventDefault();
              myApp.get("list").read();
          },

          editItem: function (e) {
              if (e) e.preventDefault();
              var list_cd = $(e.target).closest("a.btn").data("listcd");
              comboListEditView.openEditWindow(list_cd, false);
          },

          createItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              grid.clearSelection();
              comboListEditView.openEditWindow("", false);
          },

          copyItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              var item = grid.dataItem(grid.select());
              if (!item) {
                  alert("Please select a row.");
                  return;
              }
              var list_cd = item.get("list_cd");
              comboListEditView.openEditWindow(list_cd, true);
          },

          deleteItem: function (e) {
              if (e) e.preventDefault();
              var grid = $("#grd").data("kendoGrid");
              var item = <IMgmReportComboList>grid.dataItem(grid.select());
              if (!item) {
                  alert("Please select a row.");
                  return;
              }

              var list_cd = item.get("list_cd");
              if (confirm("Are you sure to delete the combo list '" + list_cd + "'?")) {
                  App.Utils.ShowLoading();
                  item.destroy()
                      .done(function (result) {
                          if (result !== "ok") {
                              App.Utils.ShowAlert(result, true);
                              return;
                          }
                          App.Utils.ShowAlert("The combo list '" + list_cd + "' has been deleted.", false);
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

      comboListEditView.bind("saved", function () {
          myApp.list.read();
      })

  });



