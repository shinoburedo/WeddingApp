//TypeScript用型定義
interface IItemCost extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ItemCostFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItemCost;
    prototype: IItemCost;
    fetch(item_cost_id: number, done: JQueryPromiseCallback<IItemCost>): JQueryDeferred<IItemCost>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var ItemCost: ItemCostFn;

define(function () {

    var ItemCost: ItemCostFn = <any>kendo.data.Model.define({
        id: "item_cost_id",
        fields: {
            "item_cost_id": { type: "number", validation: { required: true } },
            "item_cd": { type: "string", validation: { required: true } },
            "vendor_cd": { type: "string", validation: { required: true } },
            "church_cd": { type: "string" },
            "cost": { type: "number" },
            "eff_from": { type: "date", defaultValue: null },
            "eff_to": { type: "date", defaultValue: null },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ItemCost.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.eff_from = kendo.toString(this.eff_from, "yyyy/MM/dd HH:mm:ss");
        json.eff_to = kendo.toString(this.eff_to, "yyyy/MM/dd HH:mm:ss");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemCost.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.eff_from = App.Utils.convertToDateTime(this.eff_from);
        this.eff_to = App.Utils.convertToDateTime(this.eff_to);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ItemCost.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ItemCosts/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemCost.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemCosts/Delete/" + this.get("item_cost_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemCost.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemCosts/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemCost }
        });
    };

    return ItemCost;
});

