//TypeScript用型定義
interface IChurchBlock extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ChurchBlockFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IChurchBlock;
    prototype: IChurchBlock;
    getDataSource(data?: any): kendo.data.DataSource;
    saveList(data?: any): JQueryXHR;
    deleteAll(church_cd: string): JQueryXHR;
    //getAvailList(data?: any): kendo.data.DataSource;    
    getAvailList(data?: any): JQueryDeferred<kendo.data.DataSource>;
}
//クラス名を定義
declare var ChurchBlockMst: ChurchBlockFn;


define(function () {

    var ChurchBlock: ChurchBlockFn = <any>kendo.data.Model.define({
        id: "church_block_id",
        fields: {
            "church_block_id": { type: "number", validation: { required: true } },
            "church_cd": { type: "string", validation: { required: true } },
            "block_date": { type: "date", defaultValue: null },
            "start_time": { type: "date", defaultValue: null },
            "block_status": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    ChurchBlock.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.block_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.start_time = kendo.toString(this.discon_date, "HH:mm");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    ChurchBlock.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.block_date = App.Utils.convertToDateTime(this.block_date);
        this.start_time = App.Utils.convertToDateTime(this.start_time);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    ChurchBlock.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("ChurchBlocks/Delete/" + this.get("church_time_id")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ChurchBlock.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("ChurchBlocks/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: ChurchBlock }
        });
    };

    //Church.fetch = function (church_cd, callback: JQueryPromiseCallback<IChurch>): JQueryDeferred<IChurch> {
    //    var deffered = $.ajax({
    //        url: App.getApiPath("Churches/" + encodeURIComponent(church_cd)),
    //        type: "GET",
    //        cache: false
    //    })
    //        .done(function (data) {
    //            var model = $.extend(new Church(), data);
    //            model.parseJSON();
    //            if (callback) callback(model);
    //        });
    //    return deffered;
    //};

    //ChurchBlock.getAvailList = function (data): JQueryDeferred<kendo.data.DataSource> {
    //    return new kendo.data.DataSource({
    //        transport: {
    //            read: {
    //                url: App.getApiPath("ChurchBlocks/GetAvailList"),
    //                type: "POST",
    //                dataType: "json",
    //                data: data
    //            }
    //        },
    //        requestEnd: function (e) {
    //            var response = e.response;
    //            var type = e.type;
    //            console.log(type); // displays "read"
    //            console.log(response.length); // displays "77"
    //        }
    //    });
    //};

    ChurchBlock.saveList = function (data): JQueryXHR {
        var dataStr = JSON.stringify(data);
        var deffered = $.ajax({
            url: App.getApiPath("ChurchBlocks/savelist"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    ChurchBlock.deleteAll = function (church_cd): JQueryXHR {
        var deffered = $.ajax({
            url: App.getApiPath("ChurchBlocks/Delete/" + encodeURIComponent(church_cd)),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    return ChurchBlock;
});

