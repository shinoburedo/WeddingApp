//TypeScript用型定義
interface IScheduleNoteTemplate extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR; 
    destroy(): JQueryXHR;
}
interface ScheduleNoteTemplateFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IScheduleNoteTemplate;
    prototype: IScheduleNoteTemplate;
    fetch(template_cd: string, callback: JQueryPromiseCallback<IScheduleNoteTemplate> ): JQueryDeferred<IScheduleNoteTemplate>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var ScheduleNoteTemplateMst: ScheduleNoteTemplateFn;


define(function () {

    var ScheduleNoteTemplate: ScheduleNoteTemplateFn = <any>kendo.data.Model.define({
        id: "template_cd",
        fields: {
            "template_cd": { type: "string", validation: { required: true } },
            "title_jpn": { type: "string" },
            "title_eng": { type: "string" },
            "note_jpn": { type: "string" },
            "note_eng": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ScheduleNoteTemplate.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ScheduleNoteTemplate.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
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

    ScheduleNoteTemplate.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ScheduleNoteTemplates/Delete/" + this.get("template_cd")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ScheduleNoteTemplate.fetch = function (template_cd, callback: JQueryPromiseCallback<IScheduleNoteTemplate>): JQueryDeferred<IScheduleNoteTemplate> {
        var deffered = $.ajax({
                url: App.getApiPath("ScheduleNoteTemplates/" + template_cd),
                type: "GET",
                cache: false
            })
            .done(function (data) {
                var model = $.extend(new ScheduleNoteTemplate(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    ScheduleNoteTemplate.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ScheduleNoteTemplates/SearchPost"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ScheduleNoteTemplate }
        });
    };

    return ScheduleNoteTemplate;
});

