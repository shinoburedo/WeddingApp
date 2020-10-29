declare class Customer extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    GroomName(): string;
    BrideName(): string;
    getWedDateStr(): string;
    save(options);
    static fetch(c_num, options);
    static updateFinalInfo(c_num, finalize, options);
}

define(function () {

    //*-------------------------------- Customer Model　--------------------------------*//
    var Customer = <typeof Customer>kendo.data.Model.define({
        id: "c_num",
        fields: {
            c_num: { type: "string", defaultValue: "" }

            , g_last: { type: "string", validation: { required: true } }
            , g_first: { type: "string" }
            , b_last: { type: "string" }
            , b_first: { type: "string" }

            , g_last_kana: { type: "string" }
            , g_first_kana: { type: "string" }
            , b_last_kana: { type: "string" }
            , b_first_kana: { type: "string" }

            , g_last_kanji: { type: "string" }
            , g_first_kanji: { type: "string" }
            , b_last_kanji: { type: "string" }
            , b_first_kanji: { type: "string" }

            , agent_cd: { type: "string", validation: { required: true } }
            , sub_agent_cd: { type: "string", validation: { required: true } }
            , area_cd: { type: "string", validation: { required: true } }
            , tour_cd: { type: "string" }
            , church_cd: { type: "string" }

            , wed_date: { type: "date", nullable: true, defaultValue: null }
            , wed_time: { type: "date", nullable: true, defaultValue: null }
            , wed_time_s: { type: "string" }
            , htl_pick: { type: "date", nullable: true, defaultValue: null }
            , htl_pick_s: { type: "string" }

            , bf_date: { type: "date", nullable: true, defaultValue: null }
            , bf_time: { type: "date", nullable: true, defaultValue: null }
            , bf_time_s: { type: "string" }
            , bf_place: { type: "string" }

            , ft_date: { type: "date", nullable: true, defaultValue: null }
            , ft_time: { type: "date", nullable: true, defaultValue: null }
            , ft_time_s: { type: "string" }
            , ft_place: { type: "string" }

            , in_flight: { type: "string" }
            , in_dep: { type: "string" }
            , in_dep_date: { type: "date", nullable: true, defaultValue: null }
            , in_dep_time: { type: "date", nullable: true, defaultValue: null }
            , in_dep_time_s: { type: "string" }
            , in_arr: { type: "string" }
            , in_arr_date: { type: "date", nullable: true, defaultValue: null }
            , in_arr_time: { type: "date", nullable: true, defaultValue: null }
            , in_arr_time_s: { type: "string" }

            , out_flight: { type: "string" }
            , out_dep: { type: "string" }
            , out_dep_date: { type: "date", nullable: true, defaultValue: null }
            , out_dep_time: { type: "date", nullable: true, defaultValue: null }
            , out_dep_time_s: { type: "string" }
            , out_arr: { type: "string" }
            , out_arr_date: { type: "date", nullable: true, defaultValue: null }
            , out_arr_time: { type: "date", nullable: true, defaultValue: null }
            , out_arr_time_s: { type: "string" }

            , hotel_cd: { type: "string" }
            , room_number: { type: "string" }
            , checkin_date: { type: "date", nullable: true, defaultValue: null }
            , checkout_date: { type: "date", nullable: true, defaultValue: null }
            , note: { type: "string" }
            , staff_note: { type: "string" }
            , attend_count: { type: "number", defaultValue: 0 }
            , attend_name: { type: "string" }
            , attend_memo: { type: "string" }

            , cust_cxl: { type: "boolean", defaultValue: false }
            , cxl_date: { type: "date", nullable: true, defaultValue: null }
            , cxl_date_s: { type: "string" }
            , cxl_by: { type: "string" }

            , create_date: { type: "date", defaultValue: null }
            , create_date_s: { type: "string" }
            , create_by: { type: "string" }
            , final_date: { type: "date", nullable: true, defaultValue: null }
            , final_by: { type: "string" }
            , update_date: { type: "date", defaultValue: null }
            , update_date_s: { type: "string" }
            , update_date_stamp: { type: "date", defaultValue: null }
        }
    });

    Customer.prototype.GroomName = function(){
        return this.get("g_first") + " " + this.get("g_last");
    };

    Customer.prototype.BrideName = function() {
        return this.get("b_first") + " " + this.get("b_last");
    };

    Customer.prototype.parseJSON = function() {
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.wed_date = App.Utils.convertToDateTime(this.wed_date);
        this.wed_time = App.Utils.convertToDateTime(this.wed_time)
        this.wed_time_s = kendo.toString(this.wed_time, "HH:mm");
        if (this.is_sunset) {
            this.wed_time_s = "Sunset";
        }
        this.htl_pick = App.Utils.convertToDateTime(this.htl_pick)
        this.htl_pick_s = kendo.toString(this.htl_pick, "HH:mm");
        this.bf_date = App.Utils.convertToDateTime(this.bf_date);
        this.bf_time = App.Utils.convertToDateTime(this.bf_time)
        this.bf_time_s = kendo.toString(this.bf_time, "HH:mm");
        this.ft_date = App.Utils.convertToDateTime(this.ft_date);
        this.ft_time = App.Utils.convertToDateTime(this.ft_time)
        this.ft_time_s = kendo.toString(this.ft_time, "HH:mm");
        this.in_dep_date = App.Utils.convertToDateTime(this.in_dep_date);
        this.in_dep_time = App.Utils.convertToDateTime(this.in_dep_time)
        this.in_dep_time_s = kendo.toString(this.in_dep_time, "HH:mm");
        this.in_arr_date = App.Utils.convertToDateTime(this.in_arr_date);
        this.in_arr_time = App.Utils.convertToDateTime(this.in_arr_time)
        this.in_arr_time_s = kendo.toString(this.in_arr_time, "HH:mm");
        this.out_dep_date = App.Utils.convertToDateTime(this.out_dep_date);
        this.out_dep_time = App.Utils.convertToDateTime(this.out_dep_time)
        this.out_dep_time_s = kendo.toString(this.out_dep_time, "HH:mm");
        this.out_arr_date = App.Utils.convertToDateTime(this.out_arr_date);
        this.out_arr_time = App.Utils.convertToDateTime(this.out_arr_time)
        this.out_arr_time_s = kendo.toString(this.out_arr_time, "HH:mm");
        this.checkin_date = App.Utils.convertToDateTime(this.checkin_date);
        this.checkout_date = App.Utils.convertToDateTime(this.checkout_date);
        this.cxl_date = App.Utils.convertToDateTime(this.cxl_date);
        this.create_date = App.Utils.convertToDateTime(this.create_date);
        this.final_date = App.Utils.convertToDateTime(this.final_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.update_date_stamp = App.Utils.convertToDateTime(this.update_date_stamp);

        this.cxl_date_s = kendo.toString(this.cxl_date, "M/dd HH:mm:ss");
        this.create_date_s = kendo.toString(this.create_date, "M/dd HH:mm:ss");
        this.update_date_s = kendo.toString(this.update_date, "M/dd HH:mm:ss");

        this.dirty = false;
    };

    Customer.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this);
        delete json.create_date;
        delete json.cxl_date_s;
        delete json.create_date_s;
        delete json.update_date;
        delete json.update_date_s;
        delete json.dirty;
        delete json.id;

        json.wed_date = kendo.toString(this.wed_date, "yyyy/MM/dd");
        if (this.is_sunset) {
            json.wed_time_s = App.Config.SunsetBlockTime;
        }
        json.wed_time = this.wed_time_s;
        json.htl_pick = this.htl_pick_s;
        json.bf_date = kendo.toString(this.bf_date, "yyyy/MM/dd");
        json.bf_time = this.bf_time_s;
        json.ft_date = kendo.toString(this.ft_date, "yyyy/MM/dd");
        json.ft_time = this.ft_time_s;
        json.in_dep_date = kendo.toString(this.in_dep_date, "yyyy/MM/dd");
        json.in_dep_time = this.in_dep_time_s;
        json.in_arr_date = kendo.toString(this.in_arr_date, "yyyy/MM/dd");
        json.in_arr_time = this.in_arr_time_s;
        json.out_dep_date = kendo.toString(this.out_dep_date, "yyyy/MM/dd");
        json.out_dep_time = this.out_dep_time_s;
        json.out_arr_date = kendo.toString(this.out_arr_date, "yyyy/MM/dd");
        json.out_arr_time = this.out_arr_time_s;
        json.checkin_date = kendo.toString(this.checkin_date, "yyyy/MM/dd");
        json.checkout_date = kendo.toString(this.checkout_date, "yyyy/MM/dd");
        json.final_date = kendo.toString(this.final_date, "yyyy/MM/dd HH:mm:ss");
        json.update_date_stamp = kendo.toString(this.update_date_stamp, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Customer.prototype.getWedDateStr = function(){
        var wd = this.get("wed_date");
        return kendo.toString(wd, "M/dd/yyyy");
    };

    Customer.prototype.save = function(options){
        var data = this.toJSON();
        data.noDupCheck = options.noDupCheck;
        var dataStr = JSON.stringify(data);
        //console.log("Customer.save: dataStr = " + dataStr);

        $.ajax({
            url: App.getApiPath("Customers"),
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

    Customer.prototype.hasBriefingInfo = function(){
        return this.get("bf_date") && this.get("ft_date");
    };

    Customer.prototype.hasInFlightInfo = function(){
        return this.get("in_arr_date") && this.get("in_arr_time_s");
    };

    Customer.prototype.hasOutFlightInfo = function(){
        return this.get("out_dep_date") && this.get("out_dep_time_s");
    };

    Customer.prototype.hasHotelInfo = function(){
        return this.get("hotel_cd");
    };

    Customer.prototype.hasAttendantInfo = function(){
        return this.get("attend_count") || this.get("attend_name") || this.get("attend_memo");
    };

    Customer.prototype.hasAddressInfo = function(){
        var list = this.get("AddressInfos");
        return list && list.length;
    };

    Customer.prototype.hasCostumeInfo = function(){
        var list = this.get("CosInfos");
        return list && list.length;
    };

    Customer.fetch = function (c_num, options) {
        $.ajax({
            url: App.getApiPath("Customers"),
            data: { id: c_num },
            type: "GET",
            cache: false
        }).done(function(result){
            var c = new Customer(result);
            c.parseJSON();
            c.dirty = false;
            if (options && options.success) options.success(c);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    Customer.updateFinalInfo = function (c_num, finalize, options) {
        $.ajax({
            url: App.getApiPath("customers/" + c_num + "/finalize/" + finalize),
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

    Customer.saveNote = function (c_num, note, edit_all, options) {
        var data = {
            c_num: c_num,
            note: note,
            edit_all: edit_all
        };
        var dataStr = JSON.stringify(data);
        //console.log("Customer.save: dataStr = " + dataStr);

        $.ajax({
            url: App.getApiPath("customers/note"),
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

    return Customer;
});