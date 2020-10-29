declare class MakeInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- MakeInfo Model　--------------------------------*//
    var MakeInfo = <typeof MakeInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number" }
            , make_date: { type: "date", nullable: true }
            , make_time: { type: "date", nullable: true }
            , make_time_s: { type: "string" }
            , make_place: { type: "string" }
            , make_in_time: { type: "date", nullable: true }
            , make_in_time_s: { type: "string" }
            , note: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date" }
        }
    });


    MakeInfo.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.make_date = App.Utils.convertToDateTime(this.make_date);
        this.make_time = App.Utils.convertToDateTime(this.make_time)
        this.make_time_s = kendo.toString(this.make_time, "HH:mm");
        this.make_in_time = App.Utils.convertToDateTime(this.make_in_time)
        this.make_in_time_s = kendo.toString(this.make_in_time, "HH:mm");
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    MakeInfo.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.make_date = kendo.toString(this.make_date, "yyyy/MM/dd");
        json.make_time = this.make_time_s ? json.make_date + " " + this.make_time_s : "";
        json.make_in_time = this.make_in_time_s ? json.make_date + " " + this.make_in_time_s : "";
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return MakeInfo;
});

