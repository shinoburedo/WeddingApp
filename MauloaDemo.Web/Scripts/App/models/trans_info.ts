declare class TransInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- TransInfo Model　--------------------------------*//
    var TransInfo = <typeof TransInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number" }
            , pickup_date: { type: "date", nullable: true }
            , pickup_time: { type: "date", nullable: true }
            , pickup_time_s: { type: "string" }
            , pickup_hotel: { type: "string" }
            , pickup_place: { type: "string" }
            , dropoff_time: { type: "date", nullable: true }
            , dropoff_time_s: { type: "string" }
            , dropoff_hotel: { type: "string" }
            , dropoff_place: { type: "string" }
            , note: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date" }
        }
    });


    TransInfo.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.pickup_date = App.Utils.convertToDateTime(this.pickup_date);
        this.pickup_time = App.Utils.convertToDateTime(this.pickup_time)
        this.pickup_time_s = kendo.toString(this.pickup_time, "HH:mm");
        this.dropoff_time = App.Utils.convertToDateTime(this.dropoff_time)
        this.dropoff_time_s = kendo.toString(this.dropoff_time, "HH:mm");
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    TransInfo.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.pickup_date = kendo.toString(this.pickup_date, "yyyy/MM/dd");
        json.pickup_time = this.pickup_time_s ? json.pickup_date + " " + this.pickup_time_s : "";
        json.dropoff_time = this.dropoff_time_s ? json.pickup_date + " " + this.dropoff_time_s : "";
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return TransInfo;
});

