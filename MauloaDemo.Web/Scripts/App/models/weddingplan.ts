declare class WeddingPlan extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    getText(): string;
    getDefaultInvAgent();
    getChildren(options: any);
    prepareBookStatus(): void;
    static search(data: any, options: any);
    static savePlans(plans, options);
    static createWithCustomer(customer, plan, options);
}

define(['models/customer', 'models/sales'],
    function (Customer, Sales) {

        //*-------------------------------- Wedding Plan Model　--------------------------------*//
        var WeddingPlan = <typeof WeddingPlan>kendo.data.Model.define({
            id: "info_id",
            fields: {
                info_id: { type: "number", defaultValue: 0 }
                , op_seq: { type: "number", defaultValue: 0 }
                , c_num: { type: "string", defaultValue: "" }
                , req_church_cd: { type: "string" }
                , req_wed_date: { type: "date", defaultValue: null }
                , req_wed_time: { type: "date", defaultValue: null }
                , req_wed_time_s: { type: "string" }
                , is_irregular_time: { type: "boolean" }
                , note: { type: "string" }
                , create_date: { type: "date", defaultValue: null }
                , create_date_s: { type: "string" }
                , create_by: { type: "string" }
                , last_person: { type: "string" }
                , update_date: { type: "date", defaultValue: null }
                , update_date_s: { type: "string" }

                , item_type: { type: "string" }
                , item_cd: { type: "string" }
                , item_name: { type: "string" }
                , item_name_jpn: { type: "string" }
                , rq_default: { type: "boolean" }
                , church_name: { type: "string" }
                , church_name_jpn: { type: "string" }
                , agent_cd: { type: "string" }
                , sub_agent_cd: { type: "string" }
                , inv_agent: { type: "string" }
                , cust_collect: { type: "boolean", defaultValue: false }
                , quantity: { type: "number", defaultValue: 1 }
                , price: { type: "number", defaultValue: 0 }
                , amount: { type: "number", defaultValue: 0 }
                , book_status: { type: "string", defaultValue: "Q" }
                , staff: { type: "string", defaultValue: (App.User.StaffRequired ? "" : App.User.UserName) }
                , branch_staff: { type: "string" }
                , vendor_cd: { type: "string" }
                , cfmd: { type: "boolean", defaultValue: false }

                , text: { type: "string" }
                , is_sunset: { type: "boolean", defaultValue: false }
            }
        });


        WeddingPlan.prototype.PlanItems = [];
        WeddingPlan.prototype.StatusList = new kendo.data.ObservableArray([{ value: "Q", text: "RQ" }, { value: "K", text: "OK" }]);

        WeddingPlan.prototype.getText = function(i, len){
            var s = "(New)";
            if (this.get("op_seq")) {
                s = (i + 1) + " / " + len + " (" + Sales.BookStatus[this.get("book_status")] + ")";
            }
            return s;
        };

        WeddingPlan.prototype.parseJSON = function(){
            this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。

            this.req_wed_date = App.Utils.convertToDateTime(this.req_wed_date);

            this.req_wed_time = App.Utils.convertToDateTime(this.req_wed_time)
            this.req_wed_time_s = kendo.toString(this.req_wed_time, "HH:mm");

            if (this.is_sunset) this.req_wed_time_s = "Sunset";

            this.create_date = App.Utils.convertToDateTime(this.create_date);
            this.create_date_s = kendo.toString(this.create_date, "M/dd HH:mm:ss");
            this.update_date = App.Utils.convertToDateTime(this.update_date);
            this.update_date_s = kendo.toString(this.update_date, "M/dd HH:mm:ss");

            this.dirty = false;
        };

        WeddingPlan.prototype.toJSON = function(){
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this);

            // Remove the fields that do not need to be sent back
            delete json.create_date;
            delete json.create_date_s;
            delete json.update_date_s;
            delete json.dirty;
            delete json.id;

            json.req_wed_date = kendo.toString(this.req_wed_date, "yyyy/MM/dd");
            if (this.get("is_sunset")) {
                this.req_wed_time_s = App.Config.SunsetBlockTime;
            }
            json.req_wed_time = json.req_wed_date + " " + this.req_wed_time_s;
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");

            //log("WeddingPlan.toJSON:", json);
            return json;
        };

        WeddingPlan.prototype.set = function(key, value){
            //kendo.data.Model.fn.set.call(this, key, value); //継承元のメソッドを呼ぶ。
            kendo.data.Model.prototype.set.call(this, key, value);      //継承元のメソッドを呼ぶ。

            if (key === "sub_agent_cd") {
                this.getDefaultInvAgent();
            }
        };

        WeddingPlan.prototype.getDefaultInvAgent = function(){
            var self = this;
            $.ajax({
                url: App.getApiPath("SubAgents/GetDefaultInvAgent"),
                data: { sub_agent_cd: this.get("sub_agent_cd"), invoice_type: "W" },
                type: "GET",
                cache: false
            })
                .done(function (result) {
                    self.set("inv_agent", result);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    //log("WeddingPlan::getDefaultInvAgent failed.", jqXHR);
                });
        };

        WeddingPlan.prototype.getChildren = function(options){
            var self = this;
            $.ajax({
                url: App.getApiPath("Sales/getchildren"),
                data: { op_seq: this.get("op_seq"), item_cd: this.get("item_cd") },
                type: "GET",
                cache: false
            })
                .done(function (results) {
                    var list = [];
                    for (var i = 0; i < results.length; i++) {
                        var sales = new Sales(results[i]);
                        sales.parseJSON();
                        list.push(sales);
                        if (sales.arrangements.length > 0) {
                            if (sales.arrangements.length === 1) {
                                sales.cfmd = sales.arrangements[0].cfmd;
                                sales.vendor_cd = sales.arrangements[0].vendor_cd;
                            } else {
                                var cfmd = true;
                                for (var j = 0; j < sales.arrangements.length; j++) {
                                    if (!sales.arrangements[j].cfmd) {
                                        cfmd = false;
                                        break;
                                    }
                                }
                                sales.cfmd = cfmd;
                                sales.vendor_cd = sales.arrangements[0].vendor_cd + "...";
                            }
                        }
                    }
                    self.set("PlanItems", list);
                    if (options && options.success) options.success(list);

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
                }).always(function () {
                    if (options && options.always) options.always();
                });
        };

        //保存前にデフォルトのステータスを決定する。
        WeddingPlan.prototype.prepareBookStatus = function(plan_type: string){
            var new_status_list = [];

            if (plan_type === "P") {
                //フォトプランの場合

                if (this.get("rq_default")) {
                    this.set("book_status", "Q");
                    new_status_list = [{ value: "Q", text: "RQ" }];
                } else {
                    if (App.User.IsAgent) {
                        //Agentユーザーの場合

                        //1. 特定の撮影場所は常に「RQ」。
                        var is_RQ_place = App.Config.Photo_RQ_Places.indexOf('[' + this.get("req_church_cd") + ']') >= 0;

                        //2. 挙式日まで2週間未満の場合は「RQ」。
                        var wed_date = moment(this.get("req_wed_date")).toDate();
                        var limitDate = moment(App.Utils.getRegionDate()).add(App.Config.Photo_RQ_LimitDays, 'days').toDate();
                        var less_than_limit_days = wed_date < limitDate;
                        //console.log("wed_date=", wed_date, "limitDate=", limitDate, "less=", less_than_limit_days, "is_RQ_place=", is_RQ_place);

                        if (is_RQ_place || less_than_limit_days) {
                            this.set("book_status", "Q");
                            new_status_list = [{ value: "Q", text: "RQ" }];
                        } else {
                            new_status_list = [{ value: "K", text: "OK" }];
                        }
                    } else {
                        //MauloaDemoユーザーならデフォルト「OK」で「RQ」も選択可能。
                        this.set("book_status", "K");
                        new_status_list = [{ value: "Q", text: "RQ" }, { value: "K", text: "OK" }];
                    }
                }

            } else {
                //挙式プランの場合はユーザーがAgentでもMauloaDemoでも常に「RQ」。
                this.set("book_status", "Q");
                new_status_list = [{ value: "Q", text: "RQ" }];
            }

            //*** ↓↓ this.set("StatusList", ...) だと上手く動かないので注意。
            this.StatusList = new kendo.data.ObservableArray(new_status_list);
        };

        WeddingPlan.search = function (data, options) {
            $.ajax({
                url: App.getApiPath("weddingplans"),
                data: data,
                type: "GET",
                cache: false
            }).done(function (result) {
                var list = [];
                for (var i = 0; i < result.length; i++) {
                    var plan = new WeddingPlan(result[i]);
                    plan.parseJSON();
                    plan.getChildren({
                        plan: plan,
                        success: function(){
                            this.plan.dirty = false;
                        }
                    });
                    list.push(plan);
                }
                if (options && options.success) options.success(list);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
        };

        //複数のPlanを一度に保存。(カスタマー画面のプランの保存ボタンを押した時はこれが呼ばれる。)
        WeddingPlan.savePlans = function (plans, options) {

            var dataStr = JSON.stringify(plans);
            $.ajax({
                url: App.getApiPath("weddingplans"),
                data: dataStr,
                type: "POST",
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (result) {
                if (options && options.success) options.success(result);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
        };

        //カスタマー登録とプラン保存を一度に実行。（カレンダー画面で日時をクリックして保存した場合はこちらが呼ばれる。）
        WeddingPlan.createWithCustomer = function (customer, plan, options) {
            var data = { Customer: customer, WeddingPlans: [plan] };
            data.Customer.noDupCheck = options.noDupCheck;
            var dataStr = JSON.stringify(data);
            //console.log("WeddingPlan.createWithCustomer: dataStr = " + dataStr);

            $.ajax({
                url: App.getApiPath("weddingplans/CreateWithCustomer"),
                data: dataStr,
                type: "POST",
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (result) {
                if (options && options.success) options.success(result);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
        };

        return WeddingPlan;
    }
);