//TypeScript用型定義
interface IItemVendor extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ItemVendorFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItemVendor;
    prototype: IItemVendor;
    fetch(item_vendor_id: number, done: JQueryPromiseCallback<IItemVendor>): JQueryDeferred<IItemVendor>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var ItemVendor: ItemVendorFn;

define(function () {

    var ItemVendor: ItemVendorFn = <any>kendo.data.Model.define({
        id: "item_vendor_id",
        fields: {
            "item_vendor_id": { type: "number", validation: { required: true } },
            "item_cd": { type: "string", validation: { required: true } },
            "vendor_cd": { type: "string", validation: { required: true } },
            "order_name": { type: "string" },
            "default_vendor": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ItemVendor.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemVendor.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ItemVendor.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ItemVendors/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemVendor.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemVendors/Delete/" + this.get("item_vendor_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemVendor.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemVendors/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemVendor }
        });
    };

    return ItemVendor;
});

