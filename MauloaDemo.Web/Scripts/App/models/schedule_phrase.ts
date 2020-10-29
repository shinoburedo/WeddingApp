//TypeScriptコンパイラ用型定義
declare class SchedulePhrase extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    search(c_num: string, options: any): void;
    fetch(info_id: Number, options: any): void;
    getDataSource(data: any): kendo.data.DataSource;
    GetPhrasesFromPatternId(c_num: string, sch_pattern_id: number, callback: JQueryPromiseCallback<any>): JQueryDeferred<any>;
    saveList(data: kendo.data.ObservableArray, c_num : string): JQueryXHR;
}

define(function () {

    //*-------------------------------- SchedulePhrase Model　--------------------------------*//
    var SchedulePhrase = <typeof SchedulePhrase>kendo.data.Model.define({
        id: "sch_phrase_id",
        fields: {
            sch_phrase_id: { type: "number", defaultValue: 0 }
            , c_num: { type: "string" }
            , sch_pattern_line_id: { type: "number", defaultValue: 0 }
            , date: { type: "date" }
            , time: { type: "string" }
            , disp_seq: { type: "number" }
            , place: { type: "string" }
            , place_eng: { type: "string" }
            , title: { type: "string" }
            , title_eng: { type: "string" }
            , description: { type: "string" }
            , description_eng: { type: "string" }
            , item_type: { type: "string" }
            , op_seq: { type: "number" }
            , deleted: { type: "bool" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: new Date().getUTCDate() }
        }
    });


    SchedulePhrase.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.date = App.Utils.convertToDateTime(this.date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    SchedulePhrase.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;

        json.date = kendo.toString(this.date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    SchedulePhrase.prototype.save = function(options){
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("SchedulePhrases/Save"),
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

    SchedulePhrase.prototype.destroy = function(options){
        $.ajax({
            url: App.getApiPath("SchedulePhrases/Delete" + this.get("sch_phrase_id")),
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

    SchedulePhrase.fetch = function (info_id, options) {
        $.ajax({
            url: App.getApiPath("SchedulePhrases/") + info_id,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var info = new SchedulePhrase(result);
            info.parseJSON();
            info.dirty = false;
            if (options && options.success) options.success(info);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    SchedulePhrase.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SchedulePhrases/Search"),
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: SchedulePhrase }
        });
    };

    SchedulePhrase.GetPhrasesFromPatternId = function (c_num: string, sch_pattern_id: number, callback: JQueryPromiseCallback<any>) : JQueryDeferred<any> {
        var deffered = $.ajax({
            url: App.getApiPath("SchedulePhrases/GetPhrasesFromPatternId"),
            type: "GET",
            data: { c_num: c_num, sch_pattern_id: sch_pattern_id },
            cache: false
        }).done(function (result) {
            var list = [];
            for (var i = 0; i < result.length; i++) {
                var info = new SchedulePhrase(result[i]);
                info.parseJSON();
                list.push(info);
            }
            //var ds = new kendo.data.DataSource({ data: list });
            if (callback) callback(list);
        });
        return deffered;
    };

    SchedulePhrase.saveList = function (data: kendo.data.ObservableArray, c_num: string): JQueryXHR {
        data.forEach(function (item: kendo.data.ObservableObject, index : number, source : kendo.data.ObservableArray) {
            item.set("c_num", c_num);
        });
        var dataStr = JSON.stringify(data);
        var deffered = $.ajax({
            url: App.getApiPath("SchedulePhrases/SaveList"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };


    return SchedulePhrase;
});




