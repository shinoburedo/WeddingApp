declare class PlanListItem extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {

    //*-------------------------------- PlanListItem Model　--------------------------------*//
    var PlanListItem = <typeof PlanListItem>kendo.data.Model.define({
        id: "item_cd",
        fields: {
            church_cd: { type: "string" }
            , item_cd: { type: "string" }
            , item_name: { type: "string" }
            , item_name_jpn: { type: "string" }
            , rq_default: { type: "boolean" }
            , gross: { type: "number" }
            , d_net: { type: "number" }
            , y_net: { type: "number" }
        }
    });


    PlanListItem.prototype.parseJSON= function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.wed_date = App.Utils.convertToDateTime(this.wed_date);
        this.dirty = false;
    };

    PlanListItem.prototype.toJSON= function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.wed_date = kendo.toString(this.wed_date, "yyyy/MM/dd");
        return json;
    };

    return PlanListItem;
});

