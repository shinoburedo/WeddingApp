declare class Arrangement extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    save(options);
    static fetch(arrangement_id, options);
    static getDataSource(data);
    static getVendorConfirmationDataSource(data);
}

define(function () {

    //*-------------------------------- Arrangement Model　--------------------------------*//
    var Arrangement = <typeof Arrangement>kendo.data.Model.define({
        id: "arrangement_id",
        fields: {
            arrangement_id: { type: "number" }
            , op_seq: { type: "number" }
            , c_num: { type: "string" }
            , vendor_cd: { type: "string" }
            , cfmd: { type: "boolean" }
            , cfmd_by: { type: "string" }
            , cfmd_date: { type: "date", nullable: true, defaultValue: null }
            , cxl: { type: "boolean" }
            , cxl_by: { type: "string" }
            , cxl_date: { type: "date", nullable: true, defaultValue: null }
            , note: { type: "string" }
            , quantity: { type: "number" }
            , cost: { type: "number" }
            , jnl_started: { type: "boolean" }
            , create_by: { type: "string" }
            , create_date: { type: "date", defaultValue: null }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: null }
        }
    });

    Arrangement.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.cfmd_date = App.Utils.convertToDateTime(this.cfmd_date);
        this.cxl_date = App.Utils.convertToDateTime(this.cxl_date)
        this.create_date = App.Utils.convertToDateTime(this.create_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    Arrangement.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;

        json.cfmd_date = kendo.toString(this.cfmd_date, "yyyy/MM/dd");
        json.cxl_date = kendo.toString(this.cxl_date, "yyyy/MM/dd");
        json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Arrangement.prototype.save = function(options){
        //console.log("arrangement.save. %O", this.toJSON());

        //stringifyによってModelのtoJSON関数が呼ばれる。(日付型を正しくPOSTする為に必要。これをしないと時差分ずれる。)
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("Arrangements"),
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

    Arrangement.fetch = function (arrangement_id, options) {
        $.ajax({
            url: App.getApiPath("Arrangements/") + arrangement_id,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var arr = new Arrangement(result);
            arr.parseJSON();
            arr.dirty = false;
            if (options && options.success) options.success(arr);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    Arrangement.getDataSource = function (data) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Arrangements/ByOpSeq"),
                    type: "GET",
                    dataType: "json",
                    serverFiltering: true,
                    cache: false,
                    data: data
                },
                update: {
                    url: App.getApiPath("Arrangements"),
                    type: "POST"
                }
            },
            schema: { model: Arrangement }
        });
    };

    Arrangement.getVendorConfirmationDataSource = function (data) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Arrangements/ForVendorConfirmation"),
                    type: "GET",
                    dataType: "json",
                    serverFiltering: true,
                    cache: false,
                    data: data
                },
                update: {
                    url: App.getApiPath("Arrangements"),
                    type: "POST"
                }
            },
            schema: { model: Arrangement }
        });
    };

    return Arrangement;
});

