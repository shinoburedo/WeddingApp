//TypeScript用型定義
interface IMgmReport extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;

    getParamByName(name: string): any;
    paramsCount(): number;
    setBreakBeforeFlag(): void;
    removeDummyParam(): void;
}
interface MgmReportFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IMgmReport;
    prototype: IMgmReport;
    fetch(rep_cd: string, callback: JQueryPromiseCallback<IMgmReport>, removeDummyParam: boolean): JQueryDeferred<IMgmReport>;
    getDataSource(data?: any): kendo.data.DataSource;

    MenuCdList: Array<string>;
}
//クラス名を定義
declare var MgmReport: MgmReportFn;

define(['Models/MgmReportComboList', 'Models/MgmReportParam']
    , function (MgmReportComboList, MgmReportParam) {

        var MgmReport: MgmReportFn = <any>kendo.data.Model.define({
            id: "rep_cd",
            fields: {
                "rep_cd": { type: "string" },
                "menu_cd": { type: "string", defaultValue: "Management" },
                "rep_seq": { type: "number" },
                "rep_name": { type: "string" },
                "rep_sp": { type: "string", defaultValue: "usp_mgm_rpt_" },
                "description": { type: "string" },
                "output_type": { type: "string", defaultValue: "csv" },
                "output_name": { type: "string" },
                "excel_name": { type: "string" },
                "sheet_num": { type: "number" },
                "start_pos": { type: "string" },
                "write_header": { type: "boolean", defaultValue: true },
                "hidden": { type: "boolean", defaultValue: false },
                "last_person": { type: "string" },
                "update_date": { type: "date", defaultValue: null }
            }
        });

        MgmReport.prototype.parseJSON = function(){
            this.update_date = App.Utils.convertToDateTime(this.update_date);

            var _params = this.get("Params");
            if (_params) {
                var newParams = [];
                $.each(_params, function (index, item) {
                    var newItem = new MgmReportParam(item);
                    newItem.parseJSON();
                    newParams.push(newItem);
                });
                this.set("Params", newParams);
            }

            this.dirty = false;
        };

        MgmReport.prototype.paramsCount = function(){
            return this.Params.length;
        };

        MgmReport.prototype.getParamByName = function(name){
            var len = this.Params.length;
            for (var i = 0; i < len; i++) {
                if (this.Params[i].get("param_name") === name) {
                    return this.Params[i];
                }
            }
            return null;
        };

        MgmReport.prototype.removeDummyParam = function(){
            var _params = this.get("Params");
            if (_params) {
                var new_params = [];
                var len = _params.length;
                var break_before = false;
                for (var i = 0; i < len; i++) {
                    var p = _params[i];
                    p.set("break_before", break_before);
                    if (p.get("param_name") !== "_dummy_") new_params.push(p);
                    break_before = !!(p.get("break_after"));
                }
                this.set("Params", new_params);
            }
        };

        MgmReport.prototype.setBreakBeforeFlag = function(){
            var _params = this.get("Params");
            if (_params) {
                var len = _params.length;
                var break_before = false;
                for (var i = 0; i < len; i++) {
                    var p = _params[i];
                    p.set("break_before", break_before);
                    break_before = p.get("break_after");
                }
            }
        };

        MgmReport.prototype.save = function(){
            this.set("is_new", this.isNew());
            var dataStr = JSON.stringify(this);

            var deffered = $.ajax({
                url: App.getApiPath("MgmReport/Save"),
                type: "POST",
                data: dataStr,
                processData: false,
                contentType: "application/json; charset=utf-8"
            });
            return deffered;
        };

        MgmReport.prototype.destroy = function(){
            var deffered = $.ajax({
                url: App.getApiPath("MgmReport/Delete/" + this.get("rep_cd")),
                type: "POST",
                processData: false,
                contentType: "application/json; charset=utf-8"
            });
            return deffered;
        };

        MgmReport.fetch = function (rep_cd: string, callback: JQueryPromiseCallback<IMgmReport>, removeDummyParam: boolean) {
            var deffered = $.ajax({
                url: App.getApiPath("MgmReport/" + rep_cd),
                type: "GET",
                cache: false
            })
            .done(function (result) {
                var data : IMgmReport = new MgmReport(result);
                data.parseJSON();
                data.setBreakBeforeFlag();
                if (removeDummyParam) {
                    data.removeDummyParam();
                }
                data.dirty = false;
                if (callback) callback(data);
            });
            return deffered;
        };

        MgmReport.MenuCdList = ["Management", "Reservation", "Showroom", "Cashier", "Delivery", "Survey", "Accounting", "Master", "Admin"];

        MgmReport.getDataSource = function (data) {
            return new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("MgmReport/SearchReports"),
                        dataType: "json",
                        data: data
                    }
                },
                schema: { model: MgmReport }
            });
        };

        return MgmReport;
    });

