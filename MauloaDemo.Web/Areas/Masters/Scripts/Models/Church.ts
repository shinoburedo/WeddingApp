//TypeScript用型定義
interface IChurch extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface ChurchFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IChurch;
    prototype: IChurch;
    fetch(church_cd: string, callback: JQueryPromiseCallback<IChurch> ): JQueryDeferred<IChurch>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Church: ChurchFn;


define(function () {

    var Church: ChurchFn = <any>kendo.data.Model.define({
        id: "church_cd",
        fields: {
            "church_cd": { type: "string", validation: { required: true } },
            "church_name": { type: "string" },
            "church_name_jpn": { type: "string" },
            "plan_kind": { type: "string" },
            "abbrev_jpn": { type: "string" },
            "abbrev_eng": { type: "string" },
            "area_cd": { type: "string" },
            "exclusive": { type: "string" },
            "release_days": { type: "string" },
            "op_address1": { type: "string" },
            "op_address2": { type: "string" },
            "op_address3": { type: "string" },
            "op_address4": { type: "string" },
            "op_tel": { type: "string" },
            "op_fax": { type: "string" },
            "ac_address1": { type: "string" },
            "ac_address2": { type: "string" },
            "ac_address3": { type: "string" },
            "ac_address4": { type: "string" },
            "ac_tel": { type: "string" },
            "ac_fax": { type: "string" },
            "note": { type: "string" },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "disp_seq": { type: "string" },
            "default_pickup": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Church.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Church.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Church.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Churches/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Church.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Churches/Delete/" + encodeURIComponent(this.get("church_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    
    Church.fetch = function (church_cd, callback: JQueryPromiseCallback<IChurch>) : JQueryDeferred<IChurch> {
        var deffered = $.ajax({
            url: App.getApiPath("Churches/" + encodeURIComponent(church_cd)),
                type: "GET",
                cache: false
            })
            .done(function (data) {
                var model = $.extend(new Church(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    Church.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Churches/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Church }
        });
    };

    return Church;
});

