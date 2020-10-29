//TypeScript用型定義
interface INotice extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface NoticeFn {
    //スタティックメソッド・プロパティ
    new (options?: any): INotice;
    prototype: INotice;
    fetch(notice_id: number, done: JQueryPromiseCallback<INotice>): JQueryDeferred<INotice>;
    getDataSource(data?: any): kendo.data.DataSource;
    getDataSourceForList(): kendo.data.DataSource;
}
//クラス名を定義
declare var Notice: NoticeFn;

define(function () {

    var Notice: NoticeFn = <any>kendo.data.Model.define({
        id: "notice_id",
        fields: {
            "notice_id": { type: "number", validation: { required: true } },
            "from_date": { type: "date", validation: { required: true } },
            "to_date": { type: "date" },
            "agent_cd": { type: "string" },
            "title_jpn": { type: "string" },
            "title_eng": { type: "string" },
            "notice_jpn": { type: "string" },
            "notice_eng": { type: "string" },
            "disp_seq": { type: "number" },
            "notice_type": { type: "string", validation: { required: true } },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Notice.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.from_date = kendo.toString(this.from_date, "yyyy/MM/dd HH:mm:ss");
        json.to_date = kendo.toString(this.to_date, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Notice.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.from_date = App.Utils.convertToDateTime(this.from_date);
        this.to_date = App.Utils.convertToDateTime(this.to_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    Notice.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Notices/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Notice.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Notices/Delete/" + this.get("notice_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Notice.fetch = function (notice_id, callback: JQueryPromiseCallback<INotice>): JQueryDeferred<INotice> {
        var deffered = $.ajax({
            url: App.getApiPath("Notices/" + notice_id),
            type: "GET",
            cache: false
        })
            .done(function (data) {
                var model = $.extend(new Notice(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    Notice.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Notices/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Notice }
        });
    };

    Notice.getDataSourceForList = function (): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Notices/searchForNoticeList"),
                    type: "POST",
                    dataType: "json"
                }
            },
            schema: { model: Notice }
        });
    };

    return Notice;
});

