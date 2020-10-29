//TypeScript用型定義
interface IHotel extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface HotelFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IHotel;
    prototype: IHotel;
    fetch(hotel_cd: string, done: JQueryPromiseCallback<IHotel> ): JQueryDeferred<IHotel>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Hotel: HotelFn;

define(function () {

    var Hotel: HotelFn = <any>kendo.data.Model.define({
        id: "hotel_cd",
        fields: {
            "hotel_cd": { type: "string", validation: { required: true } },
            "hotel_name": { type: "string" },
            "hotel_name_jpn": { type: "string" },
            "tel": { type: "string" },
            "area_cd": { type: "string" },
            "sort_order": { type: "number", defaultValue: 0 },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Hotel.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Hotel.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Hotel.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Hotels/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Hotel.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Hotels/Delete/" + encodeURIComponent(this.get("hotel_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Hotel.fetch = function (hotel_cd, callback: JQueryPromiseCallback<IHotel>): JQueryDeferred<IHotel> {
        var deffered = $.ajax({
            url: App.getApiPath("Hotels/" + encodeURIComponent(hotel_cd)),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new Hotel(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    Hotel.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Hotels/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Hotel }
        });
    };

    return Hotel;
});

