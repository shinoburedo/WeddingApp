//TypeScript用型定義
interface IAgent extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface AgentFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IAgent;
    prototype: IAgent;
    fetch(agent_cd: string, callback: JQueryPromiseCallback<IAgent> ): JQueryDeferred<IAgent>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Agent: AgentFn;


define(function () {

    var Agent: AgentFn = <any>kendo.data.Model.define({
        id: "agent_cd",
        fields: {
            "agent_cd": { type: "string", validation: { required: true } },
            "agent_name": { type: "string" },
            "agent_name_jpn": { type: "string" },
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
            "ac_contact": { type: "string" },
            "agent_fit": { type: "string" },
            "ac_contact1": { type: "string" },
            "acct_cd": { type: "string" },
            "comm_acct_cd": { type: "string" },
            "def_bank": { type: "string" },
            "region": { type: "string" },
            "area_cd": { type: "string" },
            "block_release": { type: "number" },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "staff_required": { type: "boolean" },
            "branch_staff_required": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Agent.prototype.toJSON = function (){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Agent.prototype.parseJSON = function (){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Agent.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Agents/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Agent.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Agents/Delete/" + encodeURIComponent(this.get("agent_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    
    Agent.fetch = function (agent_cd, callback: JQueryPromiseCallback<IAgent>) : JQueryDeferred<IAgent> {
        var deffered = $.ajax({
            url: App.getApiPath("Agents/" + encodeURIComponent(agent_cd)),
                type: "GET",
                cache: false
            })
            .done(function (data) {
                var model = $.extend(new Agent(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    Agent.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Agents/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Agent }
        });
    };

    return Agent;
});

