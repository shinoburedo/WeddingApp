//TypeScript用型定義
interface IPickupPlace extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface PickupPlaceFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IPickupPlace;
    prototype: IPickupPlace;
    fetch(place_id: number, done: JQueryPromiseCallback<IPickupPlace>): JQueryDeferred<IPickupPlace>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var PickupPlace: PickupPlaceFn;

define(function () {

    var PickupPlace: PickupPlaceFn = <any>kendo.data.Model.define({
        id: "place_id",
        fields: {
            "place_id": { type: "number", validation: { required: true } },
            "place_name": { type: "string", validation: { required: true } },
            "place_name_eng": { type: "string" },
            "place_order": { type: "number" },
            "hotel_cd": { type: "string" },
            "create_by": { type: "string" },
            "create_date": { type: "date", defaultValue: null },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    PickupPlace.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    PickupPlace.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.create_date = App.Utils.convertToDateTime(this.create_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    PickupPlace.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("PickupPlaces/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    PickupPlace.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("PickupPlaces/Delete/" + this.get("place_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    PickupPlace.fetch = function (place_id, callback: JQueryPromiseCallback<IPickupPlace>): JQueryDeferred<IPickupPlace> {
        var deffered = $.ajax({
            url: App.getApiPath("PickupPlaces/" + place_id),
            type: "GET",
            cache: false
        })
            .done(function (data) {
                var model = $.extend(new PickupPlace(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    PickupPlace.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("PickupPlaces/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: PickupPlace }
        });
    };

    return PickupPlace;
});

