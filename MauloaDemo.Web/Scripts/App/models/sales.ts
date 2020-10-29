declare class Sales extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;

    StatusList: kendo.data.ObservableArray;
    DeliveryInfo: any;
    MakeInfo: any;
    ReceptionInfo: any;
    ShootInfo: any;
    TransInfo: any;

    updateAmount();
    createInfoIfNull(info_type, wed_date);

    save(options);
    getDefaultInvAgent();

    static search(c_num, options);
    static fetch(op_seq, options);
    static BookStatus: any;
}

define(['models/delivery_info'
    , 'models/make_info'
    , 'models/reception_info'
    , 'models/shoot_info'
    , 'models/trans_info'
    , 'models/arrangement'],
    function (DeliveryInfo
        , MakeInfo
        , ReceptionInfo
        , ShootInfo
        , TransInfo
        , Arrangement) {
        //*-------------------------------- Sales Model　--------------------------------*//
        var Sales = <typeof Sales>kendo.data.Model.define({
            id: "op_seq",
            fields: {
                op_seq: { type: "number", defaultValue: 0 }
                , c_num: { type: "string", defaultValue: "" }
                , item_type: { type: "string" }
                , item_cd: { type: "string" }
                , agent_cd: { type: "string" }
                , sub_agent_cd: { type: "string" }
                , inv_agent: { type: "string" }
                , staff: { type: "string", defaultValue: (App.User.StaffRequired ? "" : App.User.UserName) }
                , branch_staff: { type: "string" }
                , quantity: { type: "number", defaultValue: 1 }
                , note: { type: "string" }
                , parent_op_seq: { type: "number", nullable: true }
                , upgrade_op_seq: { type: "number", nullable: true }
                , cust_collect: { type: "boolean" }
                , org_price: { type: "number" }
                , price: { type: "number" }
                , amount: { type: "number" }
                , price_changed: { type: "boolean", defaultValue: false }
                , price_change_reason: { type: "string" }
                , book_status: { type: "string" }
                , sales_post_date: { type: "date", defaultValue: null }
                , create_date: { type: "date", defaultValue: null }
                , create_date_s: { type: "string" }
                , create_by: { type: "string" }
                , last_person: { type: "string" }
                , update_date: { type: "date", defaultValue: null }
                , update_date_s: { type: "string" }
                , update_date_stamp: { type: "date", defaultValue: null }

                , item_name: { type: "string" }
                , item_name_jpn: { type: "string" }
                , info_type: { type: "string" }
            }
        });


        Sales.prototype.parseJSON= function(){
            this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
            this.sales_post_date = App.Utils.convertToDateTime(this.sales_post_date);
            this.create_date = App.Utils.convertToDateTime(this.create_date);
            this.update_date = App.Utils.convertToDateTime(this.update_date);
            this.update_date_stamp = App.Utils.convertToDateTime(this.update_date_stamp);
            this.dirty = false;

            this.create_date_s = kendo.toString(this.create_date, "M/dd HH:mm:ss");
            this.update_date_s = kendo.toString(this.update_date, "M/dd HH:mm:ss");

            //if (this.Arrangements && this.Arrangements.length) {
            //    var len = this.Arrangements.length;
            //    for (var i = 0; i < len; i++) {
            //        var arr = new Arrangement(this.Arrangements[i]);
            //        arr.parseJSON();
            //        this.Arrangements[i] = arr;
            //    }
            //}
        };

        Sales.prototype.toJSON= function(){
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this);

            // Remove the fields that do not need to be sent back
            delete json.create_date;
            delete json.create_date_s;
            delete json.update_date;
            delete json.update_date_s;
            delete json.dirty;
            delete json.id;

            json.sales_post_date = kendo.toString(this.sales_post_date, "yyyy/MM/dd");
            json.update_date_stamp = kendo.toString(this.update_date_stamp, "yyyy/MM/dd HH:mm:ss");
            return json;
        };

        Sales.prototype.set = function(field, value){
            //kendo.data.Model.fn.set.call(this, field, value); //継承元のメソッドを呼ぶ。
            kendo.data.Model.prototype.set.call(this, field, value);      //継承元のメソッドを呼ぶ。

            if (field === "quantity") {
                this.updateAmount();
            }

            if (field === "price") {
                var price = this.get("price") || 0;
                var org_price = this.get("org_price") || 0;
                this.set("price_changed", price !== org_price);
                this.updateAmount();
            }

            if (field === "sub_agent_cd") {
                this.getDefaultInvAgent();
            }
        };

        Sales.prototype.getDefaultInvAgent= function(){
            var self = this;
            $.ajax({
                url: App.getApiPath("SubAgents/GetDefaultInvAgent"),
                data: { sub_agent_cd: this.get("sub_agent_cd"), invoice_type: "O" },
                type: "GET",
                cache: false
            })
                .done(function (result) {
                    self.set("inv_agent", result);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    //console.log("Sales::getDefaultInvAgent failed.", jqXHR);
                });
        };

        Sales.prototype.StatusList = new kendo.data.ObservableArray([{ value: "Q", text: "RQ" }, { value: "K", text: "OK" }]);
        Sales.prototype.DeliveryInfo = null;
        Sales.prototype.MakeInfo = null;
        Sales.prototype.ReceptionInfo = null;
        Sales.prototype.ShootInfo = null;
        Sales.prototype.TransInfo = null;

        Sales.prototype.updateAmount= function(){
            var qty = this.get("quantity") || 0;
            var price = this.get("price") || 0;
            this.set("amount", qty * price);
        };

        Sales.prototype.createInfoIfNull = function(info_type, wed_date){
            var info;
            switch (info_type) {
                case "DLV":
                    if (!this.get("DeliveryInfo")) {
                        info = new DeliveryInfo();
                        info.set("delivery_date", wed_date);
                        this.set("DeliveryInfo", info);
                    }
                    break;
                case "MKS":
                    if (!this.get("MakeInfo")) {
                        info = new MakeInfo();
                        info.set("make_date", wed_date);
                        this.set("MakeInfo", info);
                    }
                    break;
                case "RCP":
                    if (!this.get("ReceptionInfo")) {
                        info = new ReceptionInfo();
                        info.set("party_date", wed_date);
                        this.set("ReceptionInfo", info);
                    }
                    break;
                case "SHT":
                    if (!this.get("ShootInfo")) {
                        info = new ShootInfo();
                        info.set("shoot_date", wed_date);
                        this.set("ShootInfo", info);
                    }
                    break;
                case "TRN":
                    if (!this.get("TransInfo")) {
                        info = new TransInfo();
                        info.set("pickup_date", wed_date);
                        this.set("TransInfo", info);
                    }
                    break;
            }
        };

        Sales.prototype.save = function(options){
            //stringifyによってModelのtoJSON関数が呼ばれる。(日付型を正しくPOSTする為に必要。これをしないと時差分ずれる。)
            var dataStr = JSON.stringify(this);

            $.ajax({
                url: App.getApiPath("sales"),
                type: "POST",
                data: dataStr,
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

    Sales.search = function (c_num, options) {
        $.ajax({
            url: App.getApiPath("sales/getoptions"),
            data: { c_num: c_num },
            type: "GET",
            cache: false
        }).done(function (results) {
            var list = [];
            for (var i = 0; i < results.length; i++) {
                var sales = $.extend(new Sales(), results[i]);
                sales.parseJSON();
                list.push(sales);
            }
            if (options && options.success) options.success(list);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    Sales.fetch = function (op_seq, options) {
        $.ajax({
            url: App.getApiPath("Sales/") + op_seq,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var sales = new Sales(result);
            sales.parseJSON();

            if (result.DeliveryInfo) {
                var info = new DeliveryInfo(result.DeliveryInfo);
                info.parseJSON();
                sales.set("DeliveryInfo", info);
            }
            if (result.MakeInfo) {
                var info = new MakeInfo(result.MakeInfo);
                info.parseJSON();
                sales.set("MakeInfo", info);
            }
            if (result.ReceptionInfo) {
                var info = new ReceptionInfo(result.ReceptionInfo);
                info.parseJSON();
                sales.set("ReceptionInfo", info);
            }
            if (result.ShootInfo) {
                var info = new ShootInfo(result.ShootInfo);
                info.parseJSON();
                sales.set("ShootInfo", info);
            }
            if (result.TransInfo) {
                var info = new TransInfo(result.TransInfo);
                info.parseJSON();
                sales.set("TransInfo", info);
            }

            sales.dirty = false;
            if (options && options.success) options.success(sales);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    //スタティックなプロパティとして連想配列を公開する。(Sales.BookStatus["C"] --> "CXL")
    Sales.BookStatus = { "Q": "RQ", "K": "OK", "N": "NG", "X": "CXLRQ", "C": "CXL" };

    return Sales;

});