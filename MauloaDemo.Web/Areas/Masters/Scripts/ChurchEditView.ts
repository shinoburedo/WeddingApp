//TypeScriptコンパイラ用型定義
interface IChurchEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(church_cd: string, is_copied: boolean): void;
    openEditWindow(church_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/Church'],
    function (Church : ChurchFn) {

        var churchEditView : IChurchEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (church_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                Church.fetch(church_cd, function (item) {
                        if (church_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        churchEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        churchEditView.set("error", errorThrown);
                    }).always(function () {
                        churchEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (church_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(church_cd || "New Church");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, churchEditView);
                churchEditView.init(church_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = churchEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = churchEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        churchEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        churchEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return churchEditView;
    }
);




