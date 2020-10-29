//TypeScriptコンパイラ用型定義
declare class ScheduleNoteTemplate extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    search(c_num: string, options: any): void;
    fetch(info_id: Number, options: any): void;
    getDataSource(data: any): kendo.data.DataSource;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}

define(function () {

    //*-------------------------------- ScheduleNoteTemplate Model　--------------------------------*//
    var ScheduleNoteTemplate = <typeof ScheduleNoteTemplate>kendo.data.Model.define({
        id: "template_cd",
        fields: {
            template_cd: { type: "string", defaultValue: "" }
            , title_jpn: { type: "string" }
            , title_eng: { type: "string" }
            , note_jpn: { type: "string" }
            , note_eng: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: new Date().getUTCDate() }
        }
    });


    ScheduleNoteTemplate.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ScheduleNoteTemplate.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ScheduleNoteTemplate.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ScheduleNoteTemplates/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ScheduleNoteTemplate.prototype.destroy= function(){
        var deffered = $.ajax({
            url: App.getApiPath("ScheduleNoteTemplates/Delete/" + this.get("template_cd")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ScheduleNoteTemplate.fetch = function (template_cd, options) {
        $.ajax({
            url: App.getApiPath("ScheduleNoteTemplates/") + template_cd,
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var info = new ScheduleNoteTemplate(result);
            info.parseJSON();
            info.dirty = false;
            if (options && options.success) options.success(info);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    ScheduleNoteTemplate.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ScheduleNoteTemplates/Search"),
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ScheduleNoteTemplate }
        });
    };

    return ScheduleNoteTemplate;
});




