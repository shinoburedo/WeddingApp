declare class PickupPlaceMst extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    save(options: any);
    static search(data: any, options: any);
    static fetch(id:number, options: any);
    static getDataSource(data: any): kendo.data.DataSource;
}

define(function () {

    //*-------------------------------- Item Model　--------------------------------*//
    var PickupPlace = <typeof PickupPlace>kendo.data.Model.define({
        id: "place_id",
        fields: {
            place_id: { type: "number" }
            , place_name: { type: "string" }
            , place_name_eng: { type: "string" }
            , place_order: { type: "number" }
            , hotel_cd: { type: "string" }
        }
    });


    PickupPlace.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.dirty = false;
    };

    PickupPlace.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        return json;
    };

    PickupPlace.prototype.save= function(options){
        //stringifyによってModelのtoJSON関数が呼ばれる。(日付型を正しくPOSTする為に必要。これをしないと時差分ずれる。)
        var dataStr = JSON.stringify(this);

        $.ajax({
            url: App.getApiPath("PickupPlaces"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        }).done(function (result) {
            if (options && options.success) options.success(result);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    PickupPlace.search = function (data, options) {
        $.ajax({
            url: App.getApiPath("PickupPlaces/search"),
            data: { plan_name: data.plan_name, hotel_cd: data.hotel_cd },
            type: "POST",
            dataType: "json"
        }).done(function (result) {
            if (options && options.success) options.success(result);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    PickupPlace.fetch = function (id, options) {
        $.ajax({
            url: App.getApiPath("PickupPlaces/") + id,
            type: "GET",
            cache: false
        }).done(function (result) {
            var place = new PickupPlace(result);
            place.parseJSON();
            if (options && options.success) options.success(place);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            if (options && options.fail) options.fail(jqXHR, textStatus, errorThrown);
        }).always(function () {
            if (options && options.always) options.always();
        });
    };

    PickupPlace.getDataSource = function (data) {
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

