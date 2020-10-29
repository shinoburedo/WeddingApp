//TypeScript用型定義
interface IItemType extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ItemTypeFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItemType;
    prototype: IItemType;
    fetch(item_type: string, done: JQueryPromiseCallback<IItemType>): JQueryDeferred<IItemType>;
    getDataSource(data?: any): kendo.data.DataSource;
}
//クラス名を定義
declare var ItemType: ItemTypeFn;

define(function () {

    var ItemType: ItemTypeFn = <any>kendo.data.Model.define({
        id: "item_type",
        fields: {
            "item_type": { type: "string", validation: { required: true } },
            "desc_eng": { type: "string" },
            "desc_jpn": { type: "string" },
            "info_type": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ItemType.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemType.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    ItemType.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ItemTypes/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemType.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemTypes/Delete/" + encodeURIComponent(this.get("item_type"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemType.fetch = function (item_type, callback: JQueryPromiseCallback<IItemType>): JQueryDeferred<IItemType> {
        var deffered = $.ajax({
            url: App.getApiPath("ItemTypes/" + encodeURIComponent(item_type)),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new ItemType(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    ItemType.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemTypes/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemType }
        });
    };

    return ItemType;
});

