//declare class Item extends kendo.data.Model {
//    constructor(options: any);
//    parseJSON(): void;
//    toJSON(): any;
//}

//define(function () {

//    //*-------------------------------- Item Model　--------------------------------*//
//    var Item = <typeof Item> kendo.data.Model.define({
//        id: "item_cd",
//        fields: {
//            item_cd: { type: "string" }
//            , item_name: { type: "string" }
//            , item_name_jpn: { type: "string" }
//            , item_type: { type: "string" }
//            , discon_date: { type: "date", nullable: true }
//            , auto_ok: { type: "boolean" }
//            , special: { type: "boolean" }
//            , church_cd: { type: "string" }
//            , unit: { type: "number", nullable: true }
//            , abbrev: { type: "string" }
//            , note: { type: "string" }
//            , last_person: { type: "string" }
//            , update_date: { type: "date" }
//        },

//        parseJSON: function () {
//            this.discon_date = App.Utils.convertToDateTime(this.discon_date);
//            this.update_date = App.Utils.convertToDateTime(this.update_date);
//            this.dirty = false;
//        },

//        toJSON: function () {
//            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
//            // Remove the fields that do not need to be sent back
//            delete json.dirty;
//            delete json.id;

//            json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
//            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
//            return json;
//        }
//    });

//    return Item;
//});

