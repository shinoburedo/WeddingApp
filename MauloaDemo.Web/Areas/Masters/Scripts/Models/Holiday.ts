//TypeScript用型定義
interface IHoliday extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface HolidayFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IHoliday;
    prototype: IHoliday;
    fetch(holiday: Date, done: JQueryPromiseCallback<IHoliday> ): JQueryDeferred<IHoliday>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Holiday: HolidayFn;

define(function () {

    var Holiday: HolidayFn = <any>kendo.data.Model.define({
        id: "holiday",
        fields: {
            "holiday": { type: "date", validation: { required: true } },
            "description": { type: "string" },
            "st_flag": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Holiday.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.holiday = kendo.toString(this.holiday, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };
    Holiday.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.holiday = App.Utils.convertToDateTime(this.holiday);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Holiday.prototype.save = function(){
        //this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Holidays/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Holiday.prototype.destroy = function(){
        var strHoliday = kendo.toString(kendo.parseDate(this.get("holiday")), 'yyyyMMddHHmmss');
        var deffered = $.ajax({
            url: App.getApiPath("Holidays/Delete/" + strHoliday),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Holiday.fetch = function (holiday, callback: JQueryPromiseCallback<IHoliday>): JQueryDeferred<IHoliday> {
        var str_holiday = (holiday == null) ? "" : kendo.toString(kendo.parseDate(holiday), "yyyyMMddHHmmss");
        var deffered = $.ajax({
            url: App.getApiPath("Holidays/" + str_holiday),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new Holiday(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    Holiday.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Holidays/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Holiday }
        });
    };

    return Holiday;
});

