declare class ReceptionInfo extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- ReceptionInfo Model　--------------------------------*//
    var ReceptionInfo = <typeof ReceptionInfo>kendo.data.Model.define({
        id: "info_id",
        fields: {
            info_id: { type: "number" }
            , party_date: { type: "date", nullable: true }
            , party_time: { type: "date", nullable: true }
            , party_time_s: { type: "string" }
            , rest_cd: { type: "string" }
            , note: { type: "string" }
            , last_person: { type: "string" }
            , update_date: { type: "date" }
        }
    });


    ReceptionInfo.prototype.parseJSON= function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.party_date = App.Utils.convertToDateTime(this.party_date);
        this.party_time = App.Utils.convertToDateTime(this.party_time)
        this.party_time_s = kendo.toString(this.party_time, "HH:mm");
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ReceptionInfo.prototype.toJSON= function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.party_date = kendo.toString(this.party_date, "yyyy/MM/dd");
        json.party_time = this.party_time_s ? json.party_date + " " + this.party_time_s : "";
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return ReceptionInfo;
});

