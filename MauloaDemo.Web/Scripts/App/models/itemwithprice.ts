declare class ItemWithPrice extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    static search(data, options);
}

define(function () {

    //*-------------------------------- Item Model　--------------------------------*//
    var ItemWithPrice = <typeof ItemWithPrice>kendo.data.Model.define({
        id: "item_cd",
        fields: {
            info_type: { type: "string" }
            , item_type: { type: "string" }
            , church_cd: { type: "string" }
            , item_cd: { type: "string" }
            , item_name: { type: "string" }
            , item_name_jpn: { type: "string" }
            , abbrev: { type: "string" }
            , gross: { type: "number" }
            , d_net: { type: "number" }
            , y_net: { type: "number" }
            , tentative: { type: "boolean" }
        }
    });


    ItemWithPrice.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ItemWithPrice.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        // Remove the fields that do not need to be sent back
        delete json.dirty;
        delete json.id;

        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemWithPrice.search = function (data, options) {
        $.ajax({
            url: App.getApiPath("items/searchWithPrice"),
            data: {
                plan_type: "O",
                item_type: data.item_type,
                c_num: data.c_num,
                sub_agent_cd: App.User.IsAgent ? App.User.SubAgentCd : ""
            },
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

    return ItemWithPrice;
});

