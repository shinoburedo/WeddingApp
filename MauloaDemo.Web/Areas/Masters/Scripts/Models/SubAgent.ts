//TypeScript用型定義
interface ISubAgent extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface SubAgentFn {
    //スタティックメソッド・プロパティ
    new (options?: any): ISubAgent;
    prototype: ISubAgent;
    fetch(child_cd: string, callback: JQueryPromiseCallback<ISubAgent> ): JQueryDeferred<ISubAgent>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var SubAgent: SubAgentFn;


define(function () {

    var SubAgent: SubAgentFn = <any>kendo.data.Model.define({
        id: "child_cd",
        fields: {
            "child_cd": { type: "string", validation: { required: true } },
            "parent_cd": { type: "string" },
            "inv_agent": { type: "string" },
            "invoice_type": { type: "string" },
            "indep_flg": { type: "boolean" },
            "contact_name": { type: "string" },
            "company_name": { type: "string" },
            "company_name_eng": { type: "string" },
            "address1": { type: "string" },
            "address2": { type: "string" },
            "address3": { type: "string" },
            "tel1": { type: "string" },
            "tel2": { type: "string" },
            "fax": { type: "string" },
            "email": { type: "string" },
            "agent_area_cd": { type: "string" },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    SubAgent.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    SubAgent.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    SubAgent.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("SubAgents/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    SubAgent.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("SubAgents/Delete/" + encodeURIComponent(this.get("child_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    SubAgent.fetch = function (child_cd, callback: JQueryPromiseCallback<ISubAgent>): JQueryDeferred<ISubAgent> {
        var deffered = $.ajax({
            url: App.getApiPath("SubAgents/" + encodeURIComponent(child_cd)),
                type: "GET",
                cache: false
            })
            .done(function (data) {
                var model = $.extend(new SubAgent(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    SubAgent.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("SubAgents/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: SubAgent }
        });
    };

    return SubAgent;
});

