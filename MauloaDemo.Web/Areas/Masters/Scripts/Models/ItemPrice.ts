//TypeScript用型定義
interface IItemPrice extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ItemPriceFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItemPrice;
    prototype: IItemPrice;
    fetch(item_price_id: number, done: JQueryPromiseCallback<IItemPrice>): JQueryDeferred<IItemPrice>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var ItemPrice: ItemPriceFn;

define(function () {

    var ItemPrice: ItemPriceFn = <any>kendo.data.Model.define({
        id: "item_price_id",
        fields: {
            "item_price_id": { type: "number", validation: { required: true } },
            "item_cd": { type: "string", validation: { required: true } },
            "agent_cd": { type: "string", validation: { required: true } },
            "plan_kind": { type: "string" },
            "eff_from": { type: "date", validation: { required: true } },
            "eff_to": { type: "date", defaultValue: null },
            "gross": { type: "number", validation: { required: true } },
            "d_net": { type: "number", validation: { required: true } },
            "y_net": { type: "number", validation: { required: true } },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ItemPrice.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.eff_from = kendo.toString(this.eff_from, "yyyy/MM/dd HH:mm:ss");
        json.eff_to = kendo.toString(this.eff_to, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemPrice.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.eff_from = App.Utils.convertToDateTime(this.eff_from);
        this.eff_to = App.Utils.convertToDateTime(this.eff_to);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    ItemPrice.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ItemPrices/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemPrice.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemPrices/Delete/" + this.get("item_price_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemPrice.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemPrices/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemPrice }
        });
    };

    return ItemPrice;
});

