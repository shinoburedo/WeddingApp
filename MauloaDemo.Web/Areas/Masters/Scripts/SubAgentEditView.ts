//TypeScriptコンパイラ用型定義
interface ISubAgentEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(child_cd: string, is_copied: boolean): void;
    openEditWindow(child_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/SubAgent'],
    function (SubAgent : SubAgentFn) {

        var agentEditView : ISubAgentEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (child_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                SubAgent.fetch(child_cd, function (item) {
                        if (child_cd == "" || is_copied) {
                            item.set("id", "");     //これによってisNew()がtrueになる。
                            item.dirty = true;
                        }
                        agentEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        agentEditView.set("error", errorThrown);
                    }).always(function () {
                        agentEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (child_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(child_cd || "New SubAgent");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, agentEditView);
                agentEditView.init(child_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = agentEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = agentEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        agentEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        agentEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        return agentEditView;
    }
);




