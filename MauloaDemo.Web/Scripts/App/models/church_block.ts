declare class ChurchBlock extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    save(options);
    static search(data, options);
    static fetch(id, options);
}

define(function () {

    //*-------------------------------- ChurchBlock Model　--------------------------------*//
    var ChurchBlock = <typeof ChurchBlock>kendo.data.Model.define({
        id: "church_block_id",
        fields: {
            church_block_id: { type: "number", defaultValue: 0 }
            , church_cd: { type: "string" }
            , block_date: { type: "date", defaultValue: null }
            , start_time: { type: "date", defaultValue: null }
            , start_time_s: { type: "string" }
            , block_status: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: null }

            , pickup_time: { type: "date", defaultValue: null }
            , pickup_time_s: { type: "string" }
            , is_sunset: { type: "boolean", defaultValue: false }
            , agent_cd: { type: "string" }
            , sub_agent_cd: { type: "string" }
            , c_num: { type: "string" }
            , g_last: { type: "string" }
            , g_last_kanji: { type: "string" }
        }
    });


    ChurchBlock.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.block_date = App.Utils.convertToDateTime(this.block_date);
        this.start_time = App.Utils.convertToDateTime(this.start_time);
        this.start_time_s = kendo.toString(this.start_time, "HH:mm");
        this.pickup_time = App.Utils.convertToDateTime(this.pickup_time);
        this.pickup_time_s = kendo.toString(this.pickup_time, "HH:mm");
        if (this.get("is_sunset")) {
            this.start_time_s = 'Sunset';
        }
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ChurchBlock.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this);
        delete json.dirty;
        delete json.id;

        json.block_date = kendo.toString(this.block_date, "yyyy/MM/dd");
        if (this.get("is_sunset")) {
            this.start_time_s = App.Config.SunsetBlockTime;
        }
        json.start_time = json.block_date + " " + this.start_time_s;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ChurchBlock.prototype.save = function(options){
        //stringifyによってModelのtoJSON関数が呼ばれる。(日付型を正しくPOSTする為に必要。これをしないと時差分ずれる。)
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("ChurchBlocks"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        }).done(function (result) {
            if (options && options.success) options.success(result);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };
            
        ChurchBlock.search = function (data, options) {
            data.block_date = kendo.toString(data.block_date, "yyyy/MM/dd");

            $.ajax({
                url: App.getApiPath("ChurchBlocks"),
                data: data,
                type: "GET",
                cache: false
            }).done(function (result) {
                var list = [];
                for (var i = 0; i < result.length; i++) {
                    var block = new ChurchBlock(result[i]);
                    block.parseJSON();
                    list.push(block);
                }
                if (options && options.success) options.success(list);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
        };

        ChurchBlock.fetch = function (id, options) {
            $.ajax({
                url: App.getApiPath("ChurchBlocks/") + id,
                type: "GET",
                cache: false
            }).done(function (result) {
                var block = new ChurchBlock(result);
                block.parseJSON();
                block.dirty = false;
                if (options && options.success) options.success(block);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
            }).always(function () {
                if (options && options.always) options.always();
            });
        };

        return ChurchBlock;
    }
);


