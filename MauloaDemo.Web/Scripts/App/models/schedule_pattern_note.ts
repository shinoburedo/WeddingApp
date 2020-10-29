//TypeScriptコンパイラ用型定義
declare class SchedulePatternNote extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    search(c_num: string, options: any): void;
    fetch(info_id: Number, options: any): void;
    getDataSource(data: any): kendo.data.DataSource;
    saveList(data: kendo.data.ObservableArray, c_num: string): JQueryXHR;
}

define(function () {

    //*-------------------------------- SchedulePatternNote Model　--------------------------------*//
    var SchedulePatternNote = <typeof SchedulePatternNote>kendo.data.Model.define({
        id: "row_id",
        fields: {
            row_id: { type: "number", defaultValue: 0 }
            , sch_pattern_id: { type: "number" }
            , template_cd: { type: "string" }
            , disp_seq: { type: "number" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: new Date().getUTCDate() }
        }
    });


    SchedulePatternNote.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    SchedulePatternNote.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    SchedulePatternNote.prototype.save = function(options){
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("SchedulePatternNotes/Save"),
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

    SchedulePatternNote.prototype.destroy = function(options){
        $.ajax({
            url: App.getApiPath("SchedulePatternNotes/Delete" + this.get("row_id")),
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

    SchedulePatternNote.fetch = function (id, options) {
        $.ajax({
            url: App.getApiPath("SchedulePatternNotes/") + id,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var info = new SchedulePatternNote(result);
            info.parseJSON();
            info.dirty = false;
            if (options && options.success) options.success(info);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    SchedulePatternNote.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SchedulePatternNotes/Search"),
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: SchedulePatternNote }
        });
    };

    SchedulePatternNote.saveList = function (data: kendo.data.ObservableArray, sch_pattern_id: number): JQueryXHR {
        data.forEach(function (item: kendo.data.ObservableObject, index: number, source: kendo.data.ObservableArray) {
            item.set("sch_pattern_id", sch_pattern_id);
            item.set("disp_seq", (index + 1) * 10);
        });
        var dataStr = JSON.stringify(data);
        var deffered = $.ajax({
            url: App.getApiPath("SchedulePatternNotes/SaveList"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };


    return SchedulePatternNote;
});




