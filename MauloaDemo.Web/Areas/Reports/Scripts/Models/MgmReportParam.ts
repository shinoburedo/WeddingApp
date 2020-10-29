//TypeScript用型定義
interface IMgmReportParam extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
}
interface MgmReportParamFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IMgmReportParam;
    prototype: IMgmReportParam;
    getStoredProcParams(proc_name: string): JQueryXHR;
}
//クラス名を定義
declare var MgmReportParam: MgmReportParamFn;

define(function () {

    var MgmReportParam: MgmReportParamFn = <any> kendo.data.Model.define({
        id: "param_id",
        fields: {
            "param_id": { type: "number" },
            "rep_cd": { type: "string" },
            "param_seq": { type: "number" },
            "param_name": { type: "string" },
            "param_type": { type: "string", defaultValue: "text" },     //text, combo
            "caption": { type: "string" },
            "prefix": { type: "string", defaultValue: "" },
            "suffix": { type: "string", defaultValue: "" },
            "break_after": { type: "boolean", defaultValue: false },
            "field_length": { type: "number", defaultValue: 5 },
            "decimal_length": { type: "number", defaultValue: 0 },
            "required": { type: "boolean", defaultValue: false },
            "default_value": { type: "string", defaultValue: "" },
            "list_cd": { type: "string", defaultValue: "" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    MgmReportParam.prototype.parseJSON = function(){
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };

    MgmReportParam.getStoredProcParams = function (proc_name: string) {
        var deffered = $.ajax({
            url: App.getApiPath("MgmReport/GetStoredProcParams"),
            data: { proc_name: proc_name },
            type: "GET",
            cache: false
        });
        return deffered;
    };

    return MgmReportParam;
});

