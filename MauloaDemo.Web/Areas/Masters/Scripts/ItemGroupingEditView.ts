//TypeScriptコンパイラ用型定義
interface IItemGroupingEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(item_cd: string, is_copied: boolean): void;
    openEditWindow(item_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
    refreshItemCombo(e?: any): void;
    itemList: kendo.data.DataSource;
    createItem(e?: any);
    editItem(e?: any);
    deleteItem(e?: any);
    refreshList(e?: any);
}

define(['Models/ItemOption', 'Models/Item'], 
    function (ItemOption: ItemOptionFn, Item : ItemFn) {

        var itemGroupingEditView: IItemGroupingEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",
            itemList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("items/Search"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                item_type: itemGroupingEditView.get("model.item_type") || ""
                            };
                        }
                    }
                },
                schema: {
                    model: Item
                },
            }),
            itemOptions: [], //グリッド用データソース

            init: function (item_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                Item.fetch(item_cd, function (item) {
                    var list = ItemOption.getOptionDataSource({ item_cd: item_cd || "-" });
                    if (item_cd == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                        item.set("is_new", true);
                    //} else {
                    //    item.set("item_type", item.get("item_type"));
                    //    item.set("item_name", item.get("item_name"));
                    }
                    itemGroupingEditView.set("model", item);
                    itemGroupingEditView.set("itemOptions", list);
                }
                ).fail(function (jqXHR, textStatus, errorThrown) {
                    itemGroupingEditView.set("error", errorThrown);
                }).always(function () {
                    itemGroupingEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });

            },

            openEditWindow: function (item_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(item_cd || "New ItemOption");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, itemGroupingEditView);
                var combo = div.find("#txtItemCd").data("kendoComboBox");
                if (combo) combo.list.width(680);
                itemGroupingEditView.init(item_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = itemGroupingEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                var item = itemGroupingEditView.get("model");
                if (!item.item_cd) {
                    alert("Enter Item Code!");
                    return false;
                }

                var options = itemGroupingEditView.get("itemOptions");
                var opttions_data = options.data();
                if (options && opttions_data.length > 0) {
                    for (var i = 0; i < opttions_data.length; i++) {
                        var ds = opttions_data.at(i);
                        if (ds) {
                            if (!ds.item_type && !ds.child_cd && !ds.item_name && !ds.item_name_jpn) {
                                //空行は削除
                                options.remove(ds);
                            } else if (!ds.child_cd) {
                                //ItemCdが未入力の場合エラー
                                alert("Enter Item Code!");
                                return false;
                            }
                        }
                    }
                    App.Utils.ShowLoading();
                    var arrs = opttions_data.toJSON();
                    var data = {
                        item_cd: item.item_cd,
                        is_new: item.is_new,
                        list: arrs
                    };
                    ItemOption.saveList(data).done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        itemGroupingEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        itemGroupingEditView.init(item.item_cd, false);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                } else {
                    alert("Enter Child Code!");
                    return false;
                }
            },

            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            },

            refreshItemCombo: function () {
                itemGroupingEditView.itemList.read();
            },

            refreshList: function (e) {
                if (e) e.preventDefault();
                var item_cd = itemGroupingEditView.get("model.item_cd");
                var list = ItemOption.getOptionDataSource({ item_cd: item_cd || "-" });
                itemGroupingEditView.set("itemOptions", list);
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grdItemOption").data("kendoGrid");
                var item = <IItemOption>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var child_cd = item.get("child_cd");
                if (confirm("Are you sure to delete '" + child_cd + "'?")) {
                    App.Utils.ShowLoading();
                    var item_cd = itemGroupingEditView.get("model.item_cd");
                    item.set("item_cd", item_cd);
                    item.destroyoption()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + child_cd + "' has been deleted.", false);
                            itemGroupingEditView.refreshList();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        itemGroupingEditView.bind("change", function (e) {
            //log("optionDetailViewModel.change", e.field, optionDetailViewModel[e.field]);

            if (e.field === "model.item_type") {
                itemGroupingEditView.refreshItemCombo();
            }

            if (e.field === "model.item_cd" && itemGroupingEditView.get("model.item_cd")) {
                var combo = $("#editDiv").find("#txtItemCd").data("kendoComboBox");
                var item = combo.dataItem();
                if (item) {
                    itemGroupingEditView.set("model.item_cd", item.item_cd);
                    itemGroupingEditView.set("model.item_name", item.item_name);
                    itemGroupingEditView.set("model.item_name_jpn", item.item_name_jpn);
//                    itemGroupingEditView.refreshList();
                }
            }
        });

        $("#editDiv").on("click", "#grdItemOption .k-grid-delete", itemGroupingEditView.deleteItem);
        $("#editDiv").on("click", "#grdItemOption .k-grid-refresh", itemGroupingEditView.refreshList);
        $("#grdItemOption .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

        return itemGroupingEditView;
    }
);




