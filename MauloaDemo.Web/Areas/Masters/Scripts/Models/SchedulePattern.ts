//TypeScript用型定義
interface ISchedulePattern extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface SchedulePatternFn {
    //スタティックメソッド・プロパティ
    new (options?: any): ISchedulePattern;
    prototype: ISchedulePattern;
    fetch(sch_pattern_id: number, done: JQueryPromiseCallback<ISchedulePattern>): JQueryDeferred<ISchedulePattern>;
    getDataSource(data?: any): kendo.data.DataSource;
    getItemDataSource(data?: any): kendo.data.DataSource;
    getNoteDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var SchedulePatternM: SchedulePatternFn;

define(function () {

    var SchedulePattern: SchedulePatternFn = <any>kendo.data.Model.define({
        id: "sch_pattern_id",
        fields: {
            "sch_pattern_id": { type: "number", validation: { required: true } },
            "description": { type: "string", validation: { required: true } },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    SchedulePattern.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    SchedulePattern.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    SchedulePattern.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("SchedulePatterns/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    SchedulePattern.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("SchedulePatterns/Delete/" + this.get("sch_pattern_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    SchedulePattern.fetch = function (sch_pattern_id, callback: JQueryPromiseCallback<ISchedulePattern>): JQueryDeferred<ISchedulePattern> {
        var deffered = $.ajax({
            url: App.getApiPath("SchedulePatterns/Find/" + sch_pattern_id),
            type: "GET",
            cache: false
        })
            .done(function (data) {
                var model = $.extend(new SchedulePattern(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    SchedulePattern.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SchedulePatterns/Search"),
                    type: "GET",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: SchedulePattern }
        });
    };

    SchedulePattern.getItemDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SchedulePatterns/SearchItem"),
                    type: "GET",
                    dataType: "json",
                    data: data
                }
            },
            schema: {
                model: {
                    id: "row_id",
                    fields: {
                        row_id: { type: "number" },
                        sch_pattern_id: { type: "number" },
                        item_cd: { type: "string" },
                        item_type: { type: "string" },
                        item_name: { type: "string", editable: false },
                        last_person: { type: "string" },
                        update_date: { type: "date" }
                    }
                }
            }
        });
    };

    SchedulePattern.getNoteDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SchedulePatterns/SearchNote"),
                    type: "GET",
                    dataType: "json",
                    data: data
                }
            },
            schema: {
                model: {
                    id: "row_id",
                    fields: {
                        row_id: { type: "number" },
                        sch_pattern_id: { type: "number" },
                        template_cd: { type: "string" },
                        disp_seq: { type: "number" },                        
                        note_jpn: { type: "string", editable: false },                        
                        last_person: { type: "string" },
                        update_date: { type: "date" }
                    }
                }
            }
        });
    };

    return SchedulePattern;
});

