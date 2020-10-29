//TypeScript用型定義
interface IItemOption extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
    destroyoption(): JQueryXHR;
}
interface ItemOptionFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IItemOption;
    prototype: IItemOption;
    fetch(item_cd: string, done: JQueryPromiseCallback<IItemOption>): JQueryDeferred<IItemOption>;
    getDataSource(data?: any): kendo.data.DataSource;
    getOptionDataSource(data?: any): kendo.data.DataSource;
    saveList(data?: any);
}
//クラス名を定義
declare var ItemOption: ItemOptionFn;

define(function () {

    var ItemOption: ItemOptionFn = <any>kendo.data.Model.define({
        id: "item_cd",
        fields: {
            "item_cd": { type: "string", validation: { required: true } },
            "child_cd": { type: "string", validation: { required: true } },
            "item_type": { type: "string" },
            "item_name": { type: "string", editable: false },
            "item_name_jpn": { type: "string", editable: false },
            "is_new": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ItemOption.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ItemOption.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ItemOption.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("ItemOptions/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemOption.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemOptions/Delete/" + this.get("item_cd")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemOption.prototype.destroyoption = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ItemOptions/DeleteOption/" + this.get("item_cd") + "/" + this.get("child_cd")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ItemOption.fetch = function (item_cd, callback: JQueryPromiseCallback<IItemOption>): JQueryDeferred<IItemOption> {
        var deffered = $.ajax({
            url: App.getApiPath("ItemOptions/" + encodeURIComponent(item_cd)),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new ItemOption(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    ItemOption.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemOptions/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemOption }
        });
    };

    ItemOption.getOptionDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ItemOptions/SearchOptions"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ItemOption }
        });
    };

    ItemOption.saveList = function (data): JQueryXHR {
        var dataStr = JSON.stringify(data);
        var deffered = $.ajax({            
            url: App.getApiPath("ItemOptions/SaveList"),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8",
            data: dataStr
        });
        return deffered;
    };

    return ItemOption;
});

