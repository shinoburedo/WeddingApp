//TypeScriptコンパイラ用型定義
interface IItemEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(item_cd: string, is_copied: boolean): void;
    openEditWindow(item_cd: String, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
}

define(['Models/Item', 'Models/ItemPrice', 'Models/ItemVendor', 'Models/ItemCost', 'ItemPriceEditView', 'ItemVendorEditView', 'ItemCostEditView'],
    function (Item: ItemFn, ItemPrice: ItemPriceFn, ItemVendor: ItemVendorFn, ItemCost: ItemCostFn, itemPriceEditViewModel, itemVendorEditViewModel, itemCostEditViewModel) {

        var itemEditView: IItemEditView = <any> kendo.observable({
            model: null,
            status: "",
            error: "",

            itemPriceList: [],
            itemVendorList: [],
            itemCostList: [],

            init: function (item_cd: string, is_copied: boolean) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);
                itemEditView.set("itemPriceList", new kendo.data.DataSource([]));
                itemEditView.set("itemVendorList", new kendo.data.DataSource([]));
                itemEditView.set("itemCostList", new kendo.data.DataSource([]));

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading(true);
                Item.fetch(item_cd, function (item) {
                    if (item_cd == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                    } else {
                        var data = {
                            item_cd: item_cd
                        };
                        var itemPricelist = ItemPrice.getDataSource(data);
                        itemEditView.set("itemPriceList", itemPricelist);
                        var itemVendorlist = ItemVendor.getDataSource(data);
                        itemEditView.set("itemVendorList", itemVendorlist);
                    }
                    itemEditView.set("model", item);
                }
                ).fail(function (jqXHR, textStatus, errorThrown) {
                    itemEditView.set("error", errorThrown);
                }).always(function () {
                    itemEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });
            },

            openEditWindow: function (item_cd : string, is_copied: boolean) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(item_cd || "New");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, itemEditView);
                itemEditView.init(item_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = itemEditView.get("model");
                if (!item) return true;
                return item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading(true);
                var item = itemEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        itemEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        itemEditView.onClose();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },

            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            },

            onItemPriceNew: function (e) {
                if (e) e.preventDefault();
                //log('onAddressInfoNew');
                var item_cd = itemEditView.get("model.item_cd");
                var price = new ItemPrice({ item_cd: item_cd });

                itemPriceEditViewModel.openWindow(price, function () {
                    var data = {
                        item_cd: item_cd
                    };
                    var itemPricelist = ItemPrice.getDataSource(data);
                    itemEditView.set("itemPriceList", itemPricelist);
                });
            },

            onItemPriceEdit: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = grid.dataItem(tr);

                //log("Selected address info: ", selected);
                var price = new ItemPrice(selected);

                itemPriceEditViewModel.openWindow(price, function () {
                    //詳細の保存後は一覧を再取得する。
                    var item_cd = itemEditView.get("model.item_cd");
                    var data = {
                        item_cd: item_cd
                    };
                    var itemPricelist = ItemPrice.getDataSource(data);
                    itemEditView.set("itemPriceList", itemPricelist);
                });
            },

            onItemPriceDelete: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = grid.dataItem(tr);

                if (!confirm("Are you sure to DELETE this record?")) return;

                var price = new ItemPrice(selected);
                price.destroy()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        App.Utils.ShowAlert("The record has been deleted.", false);
                        var item_cd = itemEditView.get("model.item_cd");
                        var data = {
                            item_cd: item_cd
                        };
                        var itemPricelist = ItemPrice.getDataSource(data);
                        itemEditView.set("itemPriceList", itemPricelist);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },

            onItemVendorNew: function (e) {
                if (e) e.preventDefault();
                //log('onAddressInfoNew');
                var item_cd = itemEditView.get("model.item_cd");
                var vendor = new ItemVendor({ item_cd: item_cd });

                itemVendorEditViewModel.openWindow(vendor, function () {
                    var data = {
                        item_cd: item_cd
                    };
                    var list = ItemVendor.getDataSource(data);
                    itemEditView.set("itemVendorList", list);
                });
            },

            onItemVendorEdit: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = grid.dataItem(tr);

                var vendor = new ItemVendor(selected);

                itemVendorEditViewModel.openWindow(vendor, function () {
                    //詳細の保存後は一覧を再取得する。
                    var item_cd = itemEditView.get("model.item_cd");
                    var data = {
                        item_cd: item_cd
                    };
                    var list = ItemVendor.getDataSource(data);
                    itemEditView.set("itemVendorList", list);
                });
            },

            onItemVendorDelete: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = grid.dataItem(tr);

                if (!confirm("Are you sure to DELETE this record?")) return;

                var vendor = new ItemVendor(selected);
                vendor.destroy()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        App.Utils.ShowAlert("The record has been deleted.", false);
                        var item_cd = itemEditView.get("model.item_cd");
                        var data = {
                            item_cd: item_cd
                        };
                        var list = ItemVendor.getDataSource(data);
                        itemEditView.set("itemVendorList", list);
                        itemEditView.get("itemCostList").read();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },

            onVendorDataBound: function (e) {
                var grid = e.sender;
                grid.select(grid.element.find('tbody tr:first'));
            },

            onVendorGridChange: function (e) {
                var grid = e.sender;
                var selectedRow = grid.dataItem(grid.select());
                if (selectedRow) {
                    var data = {
                        item_cd: itemEditView.get("model.item_cd"),
                        vendor_cd: selectedRow.vendor_cd
                    };
                    var itemCostlist = ItemCost.getDataSource(data);
                    itemEditView.set("itemCostList", itemCostlist);
                }
            },
                       
            onItemCostNew: function (e) {
                if (e) e.preventDefault();
                var item_cd = itemEditView.get("model.item_cd");
                var grid = $("#grdVendor").data("kendoGrid");
                var selectedRow = <IItemCost>grid.dataItem(grid.select());
                if (selectedRow) {
                    var vendor_cd = selectedRow.get("vendor_cd");
                    var cost = new ItemCost({ item_cd: item_cd, vendor_cd: vendor_cd });

                    itemCostEditViewModel.openWindow(cost, function () {
                        var data = {
                            item_cd: item_cd,
                            vendor_cd: vendor_cd
                        };
                        var list = ItemCost.getDataSource(data);
                        itemEditView.set("itemCostList", list);
                    });
                } else {
                    alert("Select a ItemVendor record.")
                }
            },

            onItemCostEdit: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = <IItemCost>grid.dataItem(tr);

                var cost = new ItemCost(selected);

                itemCostEditViewModel.openWindow(cost, function () {
                    //詳細の保存後は一覧を再取得する。
                    var item_cd = itemEditView.get("model.item_cd");
                    var data = {
                        item_cd: item_cd,
                        vendor_cd: selected.get("vendor_cd")
                    };
                    var list = ItemCost.getDataSource(data);
                    itemEditView.set("itemCostList", list);
                });
            },

            onItemCostDelete: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = <IItemCost>grid.dataItem(tr);

                if (!confirm("Are you sure to DELETE this record?")) return;

                var cost = new ItemCost(selected);
                cost.destroy()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        App.Utils.ShowAlert("The record has been deleted.", false);
                        var item_cd = itemEditView.get("model.item_cd");
                        var data = {
                            item_cd: item_cd,
                            vendor_cd: selected.get("vendor_cd")
                        };
                        var list = ItemCost.getDataSource(data);
                        itemEditView.set("itemCostList", list);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            }
        });

        //ダイアログ部分のViewModelをバインド。(念のため時間差を付けて確実にKendoUIの初期化後に実行される様にしている。)
        setTimeout(function () {
            kendo.bind($("#ItemPriceDialog"), itemPriceEditViewModel);
            kendo.bind($("#ItemVendorDialog"), itemVendorEditViewModel);
            kendo.bind($("#ItemCostDialog"), itemCostEditViewModel);
        }, 300);

        return itemEditView;
    }
);




