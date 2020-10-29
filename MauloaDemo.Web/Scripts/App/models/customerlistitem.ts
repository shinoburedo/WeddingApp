declare class CustomerListItem extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
}

define(function () {
    //*-------------------------------- CustomerListItem Model　--------------------------------*//
    var CustomerListItem = <typeof CustomerListItem>kendo.data.Model.define({
        id: "c_num",
        fields: {
            c_num: { type: "string" }
            , g_last: { type: "string" }
            , g_first: { type: "string" }
            , b_last: { type: "string" }
            , b_first: { type: "string" }
            , g_last_kana: { type: "string" }
            , g_first_kana: { type: "string" }
            , b_last_kana: { type: "string" }
            , b_first_kana: { type: "string" }
            , g_last_kanji: { type: "string" }
            , g_first_kanji: { type: "string" }
            , b_last_kanji: { type: "string" }
            , b_first_kanji: { type: "string" }
            , agent_cd: { type: "string" }
            , sub_agent_cd: { type: "string" }
            , area_cd: { type: "string" }
            , church_cd: { type: "string" }
            , wed_date: { type: "date", nullable: true, defaultValue: null }
            , wed_time: { type: "date", nullable: true, defaultValue: null }
            , wed_time_s: { type: "string" }
            , cxl_date: { type: "date", nullable: true, defaultValue: null }
            , create_date: { type: "date", defaultValue: null }
            , update_date: { type: "date", defaultValue: null }
            , CustomerNames: { type: "string" }

            , log_id: { type: "number" }
            , log_datetime: { type: "date", defaultValue: null }
            , log_by: { type: "string" }
            , table_name: { type: "string" }
            , action: { type: "string" }
            , changes: { type: "string" }
            , ChangesStrForStaff: { type: "string" }
            , ChangesStrForAgent: { type: "string" }
            , archived: { type: "boolean" }
            , archive_by: { type: "string" }
            , archive_datetime: { type: "date", nullable: true, defaultValue: null }
        }
    });


    CustomerListItem.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.wed_date = App.Utils.convertToDateTime(this.wed_date);
        this.wed_time = App.Utils.convertToDateTime(this.wed_time)
        this.wed_time_s = kendo.toString(this.wed_time, "HH:mm");
        this.cxl_date = App.Utils.convertToDateTime(this.cxl_date);
        this.create_date = App.Utils.convertToDateTime(this.create_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.log_datetime = App.Utils.convertToDateTime(this.log_datetime);
        this.archive_datetime = App.Utils.convertToDateTime(this.archive_datetime);
        this.dirty = false;
    };

    CustomerListItem.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this);

        // Remove the fields that do not need to be sent back
        delete json.create_date;
        delete json.dirty;
        delete json.id;

        json.wed_date = kendo.toString(this.wed_date, "yyyy/MM/dd");
        json.wed_time = json.wed_date + " " + this.wed_time_s;
        json.cxl_date = kendo.toString(this.cxl_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        json.log_datetime = kendo.toString(this.log_datetime, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    return CustomerListItem;
});

