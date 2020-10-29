declare class DeliveryInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- DeliveryInfo Model　--------------------------------*//
    var DeliveryInfo = <typeof DeliveryInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number" }
            , delivery_date: { type: "date", nullable: true, defaultValue: null }
            , delivery_time: { type: "date", nullable: true, defaultValue: null }
            , delivery_time_s: { type: "string" }
            , delivery_place: { type: "string" }
            , note: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date", defaultValue: null }
        }
    });


    DeliveryInfo.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.delivery_date = App.Utils.convertToDateTime(this.delivery_date);
        this.delivery_time = App.Utils.convertToDateTime(this.delivery_time)
        this.delivery_time_s = kendo.toString(this.delivery_time, "HH:mm");
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    DeliveryInfo.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.delivery_date = kendo.toString(this.delivery_date, "yyyy/MM/dd");
        json.delivery_time = this.delivery_time_s ? json.delivery_date + " " + this.delivery_time_s : "";
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return DeliveryInfo;
});

