//TypeScriptコンパイラ用型定義
interface IAreaEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(area_cd: string, is_copied: boolean): void;
    openEditWindow(area_cd: string, is_new: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
}

define(['Models/Area'],
    function (Area: AreaFn) {

        var areaEditView : IAreaEditView= <any> kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (area_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading(true);
                Area.fetch(area_cd, function (item) {
                        if (area_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        areaEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                            areaEditView.set("error", errorThrown);
                    }).always(function () {
                        areaEditView.set("status", "");
                        App.Utils.HideLoading(true);
                    });
            },

            openEditWindow: function (area_cd: string, is_copied: boolean) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(area_cd || "New Area");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, areaEditView);
                areaEditView.init(area_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = areaEditView.get("model");
                if (!item) return true;
                return item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading(true);
                var item = areaEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        }
                        areaEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        areaEditView.onClose();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return areaEditView;
    }
);




