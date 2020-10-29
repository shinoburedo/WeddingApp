//TypeScriptコンパイラ用型定義
interface IItemTypeEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(item_type: string, is_copied: boolean): void;
    openEditWindow(agent_cd: String, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
}

define(['Models/ItemType'],
    function (ItemType: ItemTypeFn) {

        var itemTypeEditView: IItemTypeEditView = <any> kendo.observable({
            model: null,
            status: "",
            error: "",

            infoTypes: new kendo.data.DataSource({
                data: ["", "DLV", "MKS", "RCP", "TRN", "SHO", "WED", "COS"]
            }),

            init: function (item_type: string, is_copied: boolean) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading(true);
                ItemType.fetch(item_type, function (item) {
                            if (item_type == "" || is_copied) {
                                item.set("id", "");     //これによってisNew()がtrueになる。
                                item.dirty = true;
                            }
                            itemTypeEditView.set("model", item);
                        }
                    ).fail(function (jqXHR, textStatus, errorThrown) {
                        itemTypeEditView.set("error", errorThrown);
                    }).always(function () {
                        itemTypeEditView.set("status", "");
                        App.Utils.HideLoading(true);
                    });
            },

            openEditWindow: function (item_type : string, is_copied: boolean) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(item_type || "New");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, itemTypeEditView);
                itemTypeEditView.init(item_type, is_copied);
            },

            isCodeEditable: function () {
                var item = itemTypeEditView.get("model");
                if (!item) return true;
                return item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading(true);
                var item = itemTypeEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        itemTypeEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        itemTypeEditView.onClose();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return itemTypeEditView;
    }
);




