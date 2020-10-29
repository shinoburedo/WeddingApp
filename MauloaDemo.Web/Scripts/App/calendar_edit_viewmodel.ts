define(['models/customer', 'models/weddingplan', 'models/planlistitem', 'customer_duplist'],
    function (Customer, WeddingPlan, PlanListItem, dupListViewModel) {

        //*-------------------------------- Calendar Edit View Model　--------------------------------*//
        var CalendarEditViewModel = <any>kendo.data.ObservableObject.extend({

            church_label: "Church",
            church_cd: "",
            sub_agent_cd: "",
            date_s: "",
            time_s: "",
            is_sunset: false,
            plan_type: "",
            area_cd: App.User.AreaCd,
            g_last: "",
            g_first: "",
            b_last: "",
            b_first: "",
            item_cd: "",
            item_name_jpn: "",
            staff: "",
            branch_staff: "",

            SelectedPlan: null,

            onAgentChurchChanged: function () {
                this.set("item_cd", null);
                this.set("item_name", null);
                this.set("item_name_jpn", null);
                this.set("price", 0);
                this.set("SelectedPlan", { PlanItems: [] });    //グリッドをクリアするために空の配列をセットする。(nullだと表示が残ってしまう。)

                var data = {
                    church_cd: this.get("church_cd"),
                    plan_type: this.get("plan_type"),
                    wed_date: this.get("date_s"),
                    sub_agent_cd: this.get("sub_agent_cd")
                };
                this.get("planList").read(data);
            },

            onPlanChanged: function () {
                var self = this;
                var combo = $(".create-dialog").find("#txtEditPlan").data("kendoComboBox");
                var item = combo.dataItem(); 
                //log("onPlanChanged ", item);
                if (item) {
                    //プランの名称を表示。
                    self.set("item_cd", item.item_cd);
                    self.set("item_name", item.item_name);
                    self.set("item_name_jpn", item.item_name_jpn);
                    self.set("price", item.d_net);
                    self.set("church_cd", item.church_cd);

                    var plan = new WeddingPlan();
                    plan = $.extend(plan, {
                        op_seq: 0,
                        item_cd: item.item_cd,
                        rq_default: item.rq_default,
                        req_church_cd: self.get("church_cd"),
                        req_wed_date: self.get("date_s"),
                        req_wed_time_s: self.get("time_s"),
                        is_sunset: self.get("is_sunset"),
                        book_status: "K"
                    });
                    plan.set("sub_agent_cd", self.get("sub_agent_cd"));         //オーバーライドされたsetメソッドでデフォルトのinv_agentを取得しているので、コンストラクタではなく明示的にsetメソッドを呼ぶ。
                    self.set("SelectedPlan", plan);

                    App.Utils.ShowLoading();
                    plan.getChildren({
                        success: function (list) {
                            for (var i = 0; i < list.length; i++) {
                                var sales = list[i];
                                sales.createInfoIfNull(sales.info_type, plan.get("req_wed_date"));
                                //log("PlanItem[" + i + "]: ", sales);
                            }
                            App.Utils.HideLoading();
                        },
                        fail: App.Utils.HandleServerError
                    });
                }
            },

            onPlanItemsDataBound: function (e) {
                //log("onPlanItemsDataBound");
                var grid = e.sender;

                if (App.Config.Lang === 'ja') {
                    grid.hideColumn("item_name");
                } else {
                    grid.hideColumn("item_name_jpn");
                }

            },

            isJapan: function () {
                return App.Config.Lang == 'ja';
            },

            isBranchStaffVisible: function () {
                return App.User.IsStaff || App.User.BranchStaffRequired;
            },

            isPlanEnabled: function () {
                return !!this.get("sub_agent_cd");
            },

            isChurchEnabled: function () {
                //return (this.get("plan_type") === "P");
                return false;
            },

            isOKEnabled: function () {
                //必須項目が全て入っていればOKボタンをクリック可能にする。
                return !!(this.get("SelectedPlan"))
                    && !!(this.get("SelectedPlan.PlanItems"))
                    //&& !!(this.get("g_last"))
                    //&& !!(this.get("area_cd"))
                    && !!(this.get("SelectedPlan.sub_agent_cd"));
            },

            onOkClick: function (e, noDupCheck) {
                if (e) e.preventDefault();

                //Kendo UIのValidationチェック
                var validator = $(".create-dialog form").data("kendoValidator");
                if (!validator.validate()) return;

                var self = this;
                var customer = new Customer({
                    g_last: this.get("g_last"),
                    g_first: this.get("g_first"),
                    b_last: this.get("b_last"),
                    b_first: this.get("b_first"),
                    area_cd: this.get("area_cd"),
                    sub_agent_cd: this.get("SelectedPlan.sub_agent_cd")
                });
                var plan = this.get("SelectedPlan");
                plan.set("staff", this.get("staff"));
                plan.set("branch_staff", this.get("branch_staff"));

                //log("Saving customer=%O, plan=%O", customer, plan);
                App.Utils.ShowLoading(true);
                WeddingPlan.createWithCustomer(customer, plan, {
                    noDupCheck: noDupCheck,
                    success: function (result) {
                        if (!result) {
                            App.Utils.ShowAlert("No results were received. The new order has not been saved yet.", true);
                            return;
                        } else if (result.status === "DUP") {
                            //log("DUP: ", result.dupList);
                            App.Utils.HideLoading(true);
                            dupListViewModel.openWindow(customer, result.dupList,
                                function () {
                                    //log("Ignore and continue.");
                                    self.trigger("close");
                                    self.onOkClick(null, /* noDupCheck = */ true);

                                }, function (existing_c_num) {
                                    //log("Open existing customer. " + existing_c_num);
                                    //self.trigger("close");
                                    var newUrl = App.Config.BaseUrl + "customers/home/edit/" + existing_c_num;
                                    window.open(newUrl, '_blank');    //別タブで開く。
                                });
                            return;
                        } else if (result.status === "error") {
                            App.Utils.ShowAlert(result.message, true);
                            return;
                        }

                        var s = "Data saved. Customer # is " + result.c_num + " ";
                        if (result.message) {
                            s += "\n\n" + result.message;
                            alert(s);
                        } else {
                            alert(s);
                        }

                        self.trigger("close");      //ダイアログを閉じる。
                        App.Utils.HideLoading(true);
                        self.trigger("saved");      //カレンダーを更新。

                        var newUrl = App.Config.BaseUrl + "customers/home/edit/" + result.c_num;
                        //window.location.href = newUrl;
                        window.open(newUrl, '_blank');    //別タブで開く。

                    }, fail: App.Utils.HandleServerError
                });
            }
        });

        CalendarEditViewModel.create = function (values) {
            return new CalendarEditViewModel(_.extend({
                planList: new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: App.getApiPath("items/SearchWithPrice"),
                            type: "POST",
                            dataType: "json",
                            serverFiltering: true,
                            data: {
                                church_cd: "",
                                plan_type: "-",
                                wed_date: "",
                                sub_agent_cd: App.User.SubAgentCd
                            }
                        },
                    },
                    schema: { model: PlanListItem },
                })
            }, values));
        };

        return CalendarEditViewModel;
    }
);

