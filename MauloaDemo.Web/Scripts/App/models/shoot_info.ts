declare class ShootInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- ShootInfo Model　--------------------------------*//
    var ShootInfo = <typeof ShootInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number" }
            , shoot_date: { type: "date", nullable: true }
            , shoot_time: { type: "date", nullable: true }
            , shoot_time_s: { type: "string" }
            , shoot_place: { type: "string" }
            , note: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date" }
        }
    });


    ShootInfo.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.shoot_date = App.Utils.convertToDateTime(this.shoot_date);
        this.shoot_time = App.Utils.convertToDateTime(this.shoot_time)
        this.shoot_time_s = kendo.toString(this.shoot_time, "HH:mm");
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ShootInfo.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.shoot_date = kendo.toString(this.shoot_date, "yyyy/MM/dd");
        json.shoot_time = this.shoot_time_s ? json.shoot_date + " " + this.shoot_time_s : "";
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return ShootInfo;
});

