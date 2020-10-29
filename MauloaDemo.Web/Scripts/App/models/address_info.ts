declare class AddressInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    save(options);
    remove(options);
    static search(c_num, options);
    static fetch(info_id, options);
}

define(function () {

    //*-------------------------------- AddressInfo Model　--------------------------------*//
    var AddressInfo = <typeof AddressInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number", defaultValue: 0 }
            , c_num: { type: "string" }
            , pax_type: { type: "string", defaultValue: "G" }
            , pax_name: { type: "string" }
            , jpn_zip: { type: "string" }
            , addr_kana1: { type: "string" }
            , addr_kana2: { type: "string" }
            , addr_kana3: { type: "string" }
            , addr_kanji1: { type: "string" }
            , addr_kanji2: { type: "string" }
            , addr_kanji3: { type: "string" }
            , home_tel: { type: "string" }
            , work_tel: { type: "string" }
            , cell_tel: { type: "string" }
            , e_mail: { type: "string" }
            , create_by: { type: "string" }
            , create_date: { type: "date", defaultValue: null }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: null }

            , address_kanji: { type: "string", editable: false }
        }
    });


    AddressInfo.prototype.parseJSON= function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.create_date = App.Utils.convertToDateTime(this.create_date);
        this.address_kanji = (this.addr_kanji1 || "") + " " + (this.addr_kanji2 || "") + " " + (this.addr_kanji3 || "");
        this.dirty = false;
    };

    AddressInfo.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.create_date;
        delete json.dirty;
        delete json.id;

        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    AddressInfo.prototype.save = function(options){
        //stringifyによってModelのtoJSON関数が呼ばれる。(日付型を正しくPOSTする為に必要。これをしないと時差分ずれる。)
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("customers/addressinfos"),
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

    AddressInfo.prototype.remove= function(options){
        $.ajax({
            url: App.getApiPath("customers/" + this.get("c_num") + "/addressinfos/" + this.get("info_id")),
            type: "DELETE"
        })
            .done(function (result) {
                if (options && options.success) options.success(result);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
    };

    AddressInfo.search = function (c_num, options) {
        $.ajax({
            url: App.getApiPath("customers/" + c_num + "/addressinfos"),
            type: "GET",
            cache: false
        }).done(function (result) {
            var list = [];
            for (var i = 0; i < result.length; i++) {
                var plan = new AddressInfo(result[i]);
                plan.parseJSON();
                list.push(plan);
            }
            if (options && options.success) options.success(list);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    AddressInfo.fetch = function (info_id, options) {
        $.ajax({
            url: App.getApiPath("customers/addressinfos/") + info_id,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var info = new AddressInfo(result);
            info.parseJSON();
            info.dirty = false;
            if (options && options.success) options.success(info);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    return AddressInfo;
});




