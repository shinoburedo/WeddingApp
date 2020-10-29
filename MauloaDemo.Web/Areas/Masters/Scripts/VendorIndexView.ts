//TypeScriptコンパイラ用型定義
interface IVendorIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    copyItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

require(['Models/Vendor', 'VendorEditView'],
    function (Vendor : VendorFn, vendorEditView: IVendorEditView) {

        var vendorIndexView : IVendorIndexView = <any> kendo.observable({

            search: {
                vendor_cd: "",
                vendor_name: "",
                vendor_fit: "",
                area_cd: ""
            },

            list: Vendor.getDataSource(function () {
                return vendorIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                vendorIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var vendor_cd = $(e.target).closest("a.btn").data("key");
                vendorEditView.openEditWindow(vendor_cd, false);
            },

            createItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                grid.clearSelection();
                vendorEditView.openEditWindow("", false);
            },

            copyItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }
                var vendor_cd = item.get("vendor_cd");
                vendorEditView.openEditWindow(vendor_cd, true);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grd").data("kendoGrid");
                var item = <IVendor>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var vendor_cd = item.get("vendor_cd");
                if (confirm("Are you sure to delete '" + vendor_cd + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            } 
                            App.Utils.ShowAlert("The item '" + vendor_cd + "' has been deleted.", false);
                            vendorIndexView.list.read();

                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        kendo.bind("#app", vendorIndexView);

        $("#app").on("click", "#grd .k-grid-add", vendorIndexView.createItem);
        $("#app").on("click", "#grd .k-grid-copy", vendorIndexView.copyItem);
        $("#app").on("click", "#grd .k-grid-delete", vendorIndexView.deleteItem);
        $("#app").on("click", "#grd .k-grid-refresh", vendorIndexView.refreshList);
        $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        vendorEditView.bind("saved", function () {
            vendorIndexView.list.read();
        });
    }
);



