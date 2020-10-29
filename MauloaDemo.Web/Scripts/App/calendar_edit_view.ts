define(['models/customer' , 'models/weddingplan', 'calendar_edit_viewmodel'],
    function (Customer, WeddingPlan, CalendarEditViewModel) {

        var CalendarEditView = <any>kendo.View.extend({
            kendoWindow: null,

            init: function(content, options) {
                (<any>(kendo.View.fn)).init.call(this, content, options);

                var self = this;
                self.model.bind("close", function (e) {
                    //オーダー保存成功後にcloseイベントが起きる。
                    if (self.kendoWindow) {
                        self.kendoWindow.close();
                    }
                });

                self.model.set("church_label", (self.model.get("plan_type") === "W") ? "Church" : "Location");
            },

            render: function (container) {
                (<any>(kendo.View.fn)).render.call(this, container);

                var self = this;
                var el = self.element;
                self.kendoWindow = el.kendoWindow({
                    title: 'Select a plan',
                    width: "70%",
                    minWidth: 480,
                    minHeight: 400,
                    visible: false,
                    modal: true
                }).data("kendoWindow");
                App.initKendo(el);

                //コンボリストの幅をセット。
                el.find("#txtEditPlan").data("kendoComboBox").list.width(680);
                el.find("#txtEditChurchCd").data("kendoComboBox").list.width(380);

                if (self.model.get("plan_type") === 'P') {
                    el.find("#txtEditChurchCd").data("kendoComboBox").setDataSource(App.data.churchListForPhotoPlan);
                } else {
                    el.find("#txtEditChurchCd").data("kendoComboBox").setDataSource(App.data.churchListForWeddingPlan);
                }

                //店舗担当者名の必須制御。
                App.Utils.setBoolAttr(".branch_staff", "required", App.User.BranchStaffRequired);
            },

            openWindow: function () {
                var self = this;
                var el = self.element;

                self.kendoWindow.center().open();

                //初期フォーカスをセット。
                var focusTo = (self.model.get("sub_agent_cd") ? "#txtEditPlan" : "#txtEditSubAgent");
                var focusToCombo = el.find(focusTo).data("kendoComboBox");
                self.kendoWindow.bind("activate", function () {
                    setTimeout(function () {
                        if (focusToCombo) focusToCombo.focus();
                    }, 100);
                });

                self.model.onAgentChurchChanged();

                //Closeボタンの処理
                el.on("click", "#btnClose", function (e) {
                    if (e) e.preventDefault();
                    self.kendoWindow.close();
                });

                //クローズ後はDOMから削除。(Closeボタン以外にXボタン、ESCキーにも対応)
                self.kendoWindow.bind("deactivate", function () {
                    setTimeout(function () {
                        self.kendoWindow.destroy();
                        self.kendoWindow = null;
                        self.destroy();
                    }, 400);
                });



                ////Kendo UIのValidationチェックを実行して、必須項目を明示する。
                //var validator = $(".create-dialog form").data("kendoValidator");
                //validator.validate();
            }
        });
        return CalendarEditView;
    }
);

