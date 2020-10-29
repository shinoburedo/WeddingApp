//TypeScriptコンパイラ用型定義
interface INoticeEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(notice_id: string, is_copied: boolean): void;
    openEditWindow(notice_id: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/Notice'],
    function (Notice : NoticeFn) {

        var noticeEditView : INoticeEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",
            notice_type_list: [{ name: 'システム', notice_type: 'SYS' },
                { name: '業務連絡', notice_type: 'OPE' }],

            init: function (notice_id, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                Notice.fetch(notice_id, function (item) {
                        if (notice_id == "" || is_copied) {
                            item.dirty = true;
                            item.set("is_new", true);
                            item.set("id", 0);
                            item.set("notice_id", 0);
                        }
                        noticeEditView.set("model", item);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        noticeEditView.set("error", errorThrown);
                    }).always(function () {
                        noticeEditView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (notice_id, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(notice_id || "New Notice");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, noticeEditView);
                noticeEditView.init(notice_id, is_copied);
            },

            isCodeEditable: function () {
                var item = noticeEditView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = noticeEditView.get("model");
                item.save()
                    .done(function (result) {
                        if (result !== "ok") {
                            App.Utils.ShowAlert(result, true);
                            return;
                        } 
                        noticeEditView.trigger("saved");
                        App.Utils.ShowAlert("Data saved successfully!", false);
                        noticeEditView.onClose();

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            }

        });

        var combo_type = $("#editWindow").find("#cmbNoticeType").data("kendoComboBox");
        if (combo_type) combo_type.list.width(200);

        return noticeEditView;
    }
);




