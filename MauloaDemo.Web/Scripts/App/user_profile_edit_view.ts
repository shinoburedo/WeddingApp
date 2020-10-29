define(['models/user_profile', 'module'],
function (UserProfile, module) {

    var UserProfileEditView = kendo.View.extend({
        kendoWindow: null,

        init: function(content, options) {
            (<any>(kendo.View.fn)).init.call(this, content, options);
            var self = this;

            //log("UserProfileEditView initialized.", content, options);
        },

        render: function (container, tab) {
            (<any>(kendo.View.fn)).render.call(this, container);

            var self = this;
            var el = self.element;
            var model = self.model;

            if (tab === 1) {
                el.find('.nav-tabs-profile li').removeClass("active");
                el.find('.nav-tabs-profile li:eq(1)').addClass("active");

                el.find('.tab-content .tab-pane').removeClass("active");
                el.find('.tab-content .tab-pane:eq(1)').addClass("active");
            }

            model.bind("change", function (e) {
                if (e.field === "e_mail"
                    || e.field === "culture_name"
                    || e.field === "date_format") {
                    model.set("isSaveUserInfoEnabled", true);
                }
            });

            el.find('#btnResetPassword').on("click", function (e) {
                if (e) e.preventDefault();
                if (!model.get("cur_password")) {
                    App.Utils.ShowAlert("Current password is required.", true);
                    return;
                }

                var mydata = model.toJSON();
                App.Utils.setBoolAttr('#btnResetPassword', "disabled", true);
                App.Utils.ShowLoading(true);

                $.post(
                    App.getApiPath("LoginUsers/ResetPassword"),
                    mydata
                ).done(function (data) {
                    if (data.result === 'ok') {
                        model.set("cur_password", "");
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
            });

            el.find('#btnSaveUserInfo').on("click", function (e) {
                e.preventDefault();
                model.set("isSaveUserInfoEnabled", false);
                App.Utils.ShowLoading(true);

                //Kendo MVVMのData BindingだとDropDownの場合はタイミング的に最新の値が反映されない事があるのでここで確実に値を取得。
                model.set("date_format", el.find("#date_format").val());
                model.set("print_date_format", el.find("#print_date_format").val());
                model.set("time_format", el.find("#time_format").val());
                var mydata = model.toJSON();        //KendoのObservableからPlain Objectに変換。

                $.post(
                    App.getApiPath("LoginUsers/UpdateUserProfile"),
                    mydata
                ).done(function (data) {
                    if (data.result == 'ok') {
                        $.post(self.AfterUpdateActionPath)
                        .done(function (data) {
                            //log("AfterUpdateProfile done", data);
                            if (data.result === "ok") {
                                App.Utils.ShowAlert("Saved successfully!");
                            } else {
                                App.Utils.ShowAlert("Saved successfully. However, you need to log out and log in again.", true);
                            }
                        });

                    } else {
                        App.Utils.ShowAlert(data.message, true);
                        model.set("isSaveUserInfoEnabled", true);
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    App.Utils.ShowAlertAjaxErr(jqXHR, textStatus, errorThrown);
                    model.set("isSaveUserInfoEnabled", true);
                });
            });

            App.initKendo(el);
        }
    });

    return UserProfileEditView;
});
    


