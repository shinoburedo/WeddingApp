//TypeScriptコンパイラ用型定義
interface IMgmReportComboListEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(list_cd: string, is_copied: boolean): void;
    openEditWindow(list_cd: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;
    isListCdEditable(): boolean;
}

define(['Models/MgmReportComboList'],
function (MgmReportComboList: MgmReportComboListFn) {

    var comboListEditView: IMgmReportComboListEditView= <any> kendo.observable({
        comboList: null,
        status: "",
        error: "",

        init: function (list_cd: string, is_copied: boolean){
            this.set("status", "Loading...");
            this.set("error", "");
            this.set("comboList", null);

            App.Utils.ShowLoading(true);
            MgmReportComboList.fetch(list_cd, function (item) {
                    if (list_cd == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                    }
                    comboListEditView.set("comboList", item);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    comboListEditView.set("error", errorThrown);
                }).always(function () {
                    comboListEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });
        },

        openEditWindow: function (list_cd: string, is_copied: boolean) {
            var win = $("#editWindow").data("kendoWindow");
            win.title(list_cd || "New Combo List");
            win.center().open();

            var div = $("#editDiv");
            kendo.unbind(div);
            kendo.bind(div, comboListEditView);
            comboListEditView.init(list_cd, is_copied);
        },

        isListCdEditable: function () {
            var item = comboListEditView.get("comboList");
            return item && item.isNew();
        },

        onSave: function (e) {
            if (e) e.preventDefault();

            App.Utils.ShowLoading(true);
            var item = comboListEditView.get("comboList");
            item.save()
                .done(function (data) {
                    if (data !== "ok") {
                        App.Utils.ShowAlert(data, true);
                        return;
                    }
                    comboListEditView.trigger("saved");
                    App.Utils.ShowAlert("Data saved successfully!", false);
                    comboListEditView.onClose();
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    App.Utils.ShowAlert(errorThrown, true);
                });
        },
        onClose: function (e) {
            if (e) e.preventDefault();
            $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
        }

    });

    return comboListEditView;
});




