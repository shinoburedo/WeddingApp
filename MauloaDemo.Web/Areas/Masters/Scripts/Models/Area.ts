//TypeScript用型定義
interface IArea extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface AreaFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IArea;
    prototype: IArea;
    fetch(area_cd: string, done: JQueryPromiseCallback<IArea> ): JQueryDeferred<IArea>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Area: AreaFn;

define(function () {

    var Area: AreaFn = <any>kendo.data.Model.define({
        id: "area_cd",
        fields: {
            "area_cd": { type: "string", validation: { required: true } },
            "desc_eng": { type: "string" },
            "desc_jpn": { type: "string" },
            "area_seq": { type: "number", defaultValue: 0 },
            "address_name": { type: "string", validation: { required: true } },
            "add_name_jpn": { type: "string", validation: { required: true } },
            "main_add1": { type: "string", validation: { required: true } },
            "main_add2": { type: "string" },
            "main_add3": { type: "string" },
            "main_add4": { type: "string" },
            "main_tel": { type: "string" },
            "main_fax": { type: "string" },
            "emg_contact1": { type: "string" },
            "emg_tel1": { type: "string" },
            "emg_contact2": { type: "string" },
            "emg_tel2": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        },
    });

    Area.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Area.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Area.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Areas/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Area.prototype.destroy  = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Areas/Delete/" + encodeURIComponent(this.get("area_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Area.fetch = function (area_cd, callback: JQueryPromiseCallback <IArea>) : JQueryDeferred<IArea> {
        var deffered = $.ajax({
            url: App.getApiPath("Areas/" + encodeURIComponent(area_cd)),
            type: "GET",
            cache: false
        })
        .done(function (data) {
            var model = $.extend(new Area(), data);
            model.parseJSON();
            if (callback) callback(model);
        });
        return deffered;
    };

    Area.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Areas/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Area }
        });
    };

    return Area;
});

