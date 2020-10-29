//TypeScriptコンパイラ用型定義
interface ILoginUserEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(login_id: string, is_copied: boolean): void;
    openEditWindow(login_id: string, is_copied: boolean): void;
    onSave(e?: any): void;
    onClose(e? : any): void;
}

define(['Models/LoginUser'],
    function (LoginUser : LoginUserFn) {

        var editView : ILoginUserEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",

            accessLevelList: new kendo.data.DataSource({
                data: [
                    { value: 0, text: '0 - No access (Account disabled)' },
                    { value: 1, text: '1 - Calendar only' },
                    { value: 2, text: '2 - User with less permission' },
                    { value: 3, text: '3 - Regular user' },
                    { value: 4, text: '4 - Power user' },
                    { value: 5, text: '5 - Supervisor' },
                    { value: 6, text: '6 - Manager' },
                    { value: 7, text: '7 - Administrator' }
                ]
            }),

            init: function (login_id, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                App.Utils.ShowLoading();
                LoginUser.fetch(login_id, function (item) {
                    if (login_id == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                    }
                    editView.set("model", item);
                    //console.log("LoginUser model=", item);

                }).fail(function (jqXHR, textStatus, errorThrown) {
                        editView.set("error", errorThrown);
                    }).always(function () {
                        editView.set("status", "");
                        App.Utils.HideLoading();
                    });
            },

            openEditWindow: function (login_id, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(login_id || "New LoginUser");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, editView);
                editView.init(login_id, is_copied);
            },

            isCodeEditable: function () {
                var item = editView.get("model");
                return item && item.isNew();
            },

            onSave: function (e) {
                if (e) e.preventDefault();

                var validator = $("#editDiv form").data("kendoValidator");
                if (!validator.validate()) return;

                App.Utils.ShowLoading();
                var item = editView.get("model");
                item.save()
                    .done(function (result) {
                        var has_error = false;
                        switch (result) {
                            case "ok0":
                                App.Utils.ShowAlert(App.L("正常に保存しました。", "User saved successfully!"), false);
                                break;
                            case "ok1":
                                App.Utils.HideLoading(true);
                                alert(App.L("正常に保存しました。\n\n"
                                          + "但しAccess Levelが1に変更されたためパスワードを「カレンダー閲覧専用パスワード」に変更しました。",
                                            "User saved successfully. \n\n"
                                          + "Please note that the password was reset for 'Calendar only' user."));
                                break;
                            case "ok2":
                                App.Utils.HideLoading(true);
                                alert(App.L("正常に保存しました。\n\n" 
                                          + "但しAccess Levelが1から他の値に変更されたためパスワードをデフォルトの「テンポラリパスワード」に変更しました。"
                                          + "次回ログイン時にユーザー自身で再度リセットしてもらう必要があります。",
                                            "User saved successfully. \n\n"
                                          + "Please note that the password was reset to the temporary one because the access level was changed from 1 to another value. \n\n"
                                          + "The user must reset her password at the next login."));
                                break;
                            default:
                                App.Utils.ShowAlert(result, true);
                                break;
                        }
                        if (!has_error) {
                            editView.trigger("saved");
                            editView.onClose();
                        }

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.ShowAlert(errorThrown, true);
                    });
            },
            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            },

            onResetPassword: function (e) {
                if (e) e.preventDefault();
                if (!confirm("本当にパスワードをリセットしてよろしいですか？")) return;

                App.Utils.setBoolAttr('#btnResetPassword', "disabled", true);
                App.Utils.ShowLoading(true);

                var model = editView.get("model");

                $.post(
                    App.getApiPath("LoginUsers/ResetPassword"), model.toJSON())
                    .done(function (data) {
                        //log("ResetPassword done:", data);
                        if (data.result === 'ok') {
                            model.set("new_password", data.new_password);
                            model.set("eff_to_pass", kendo.parseDate(data.EffToPass));
                        } else {
                            var msg = data.message || data.result;
                            App.Utils.ShowAlert(msg, true);
                        }
                    }).fail(App.Utils.ShowAlertAjaxErr)
                    .always(function () {
                        App.Utils.HideLoading(true);
                        App.Utils.setBoolAttr('#btnResetPassword', "disabled", false);
                    });
            }

        });

        return editView;
    }
);




