//TypeScript用型定義
interface IItem extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ItemFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItem;
    prototype: IItem;
    fetch(item_cd: string, done: JQueryPromiseCallback<IItem>): JQueryDeferred<IItem>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var Item: ItemFn;

define(function () {

    var Item: ItemFn = <any>kendo.data.Model.define({
        id: "item_cd",
        fields: {
            "item_cd": { type: "string", validation: { required: true } },
            "item_name": { type: "string" },
            "item_name_jpn": { type: "string" },
            "item_type": { type: "string" },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "auto_ok": { type: "boolean" },
            "special": { type: "boolean" },
            "selective": { type: "boolean" },
            "church_cd": { type: "string" },
            "unit": { type: "number" },
            "abbrev": { type: "string" },
            "note": { type: "string" },
            "open_to_japan": { type: "boolean" },
            "rq_default": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Item.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Item.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Item.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Items/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Item.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Items/Delete/" + encodeURIComponent(this.get("item_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Item.fetch = function (item_cd, callback: JQueryPromiseCallback<IItem>): JQueryDeferred<IItem> {
        var deffered = $.ajax({
            url: App.getApiPath("Items/" + encodeURIComponent(item_cd)),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new Item(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    Item.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Items/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Item }
        });
    };

    return Item;
});

