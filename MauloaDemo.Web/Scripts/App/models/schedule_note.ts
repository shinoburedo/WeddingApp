//TypeScriptコンパイラ用型定義
declare class ScheduleNote extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    search(c_num: string, options: any): void;
    fetch(info_id: Number, options: any): void;
    getDataSource(data: any): kendo.data.DataSource;
    GetNotesFromPatternId(c_num: string, sch_pattern_id: number, callback: JQueryPromiseCallback<any>): JQueryDeferred<any>;
    saveList(data: kendo.data.ObservableArray, c_num : string): JQueryXHR;
}

define(function () {

    //*-------------------------------- ScheduleNote Model　--------------------------------*//
    var ScheduleNote = <typeof ScheduleNote>kendo.data.Model.define({
        id: "sch_note_id",
        fields: {
            sch_note_id: { type: "number", defaultValue: 0 }
            , c_num: { type: "string" }
            , template_cd: { type: "string", defaultValue: "" }
            , title_jpn: { type: "string" }
            , title_eng: { type: "string" }
            , note_jpn: { type: "string" }
            , note_eng: { type: "string" }
            , disp_seq: { type: "number" }
            , deleted: { type: "bool" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: new Date().getUTCDate() }
        }
    });


    ScheduleNote.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.date = App.Utils.convertToDateTime(this.date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ScheduleNote.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ScheduleNote.prototype.save= function(options){
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("ScheduleNotes/Save"),
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

    ScheduleNote.prototype.destroy= function(options){
        $.ajax({
            url: App.getApiPath("ScheduleNotes/Delete" + this.get("sch_note_id")),
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

    ScheduleNote.fetch = function (id, options) {
        $.ajax({
            url: App.getApiPath("ScheduleNotes/") + id,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var info = new ScheduleNote(result);
            info.parseJSON();
            info.dirty = false;
            if (options && options.success) options.success(info);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    ScheduleNote.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ScheduleNotes/Search"),
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ScheduleNote }
        });
    };

    ScheduleNote.GetNotesFromPatternId = function (c_num: string, sch_pattern_id: number, callback: JQueryPromiseCallback<any>) : JQueryDeferred<any> {
        var deffered = $.ajax({
            url: App.getApiPath("ScheduleNotes/GetNotesFromPatternId"),
            type: "GET",
            data: { c_num: c_num, sch_pattern_id: sch_pattern_id },
            cache: false
        }).done(function (result) {
            var list = [];
            for (var i = 0; i < result.length; i++) {
                var info = new ScheduleNote(result[i]);
                info.parseJSON();
                list.push(info);
            }
            if (callback) callback(list);
        });
        return deffered;
    };

    ScheduleNote.saveList = function (data: kendo.data.ObservableArray, c_num: string): JQueryXHR {
        data.forEach(function (item: kendo.data.ObservableObject, index : number, source : kendo.data.ObservableArray) {
            item.set("c_num", c_num);
            item.set("disp_seq", (index+1) * 10);
        });
        var dataStr = JSON.stringify(data);

        var deffered = $.ajax({
            url: App.getApiPath("ScheduleNotes/SaveList"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };


    return ScheduleNote;
});




