//TypeScriptコンパイラ用型定義
interface IAgentEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(agent_cd: string, is_copied: boolean): void;
    openEditWindow(agent_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/Agent'],
    function (Agent : AgentFn) {

        var agentEditView : IAgentEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            init: function (agent_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                Agent.fetch(agent_cd, function (item) {
                        if (agent_cd == "" || is_copied) {
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

            openEditWindow: function (agent_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(agent_cd || "New Agent");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, agentEditView);
                agentEditView.init(agent_cd, is_copied);
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




