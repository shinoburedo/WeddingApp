//TypeScript用型定義
interface IMgmReportComboList extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface MgmReportComboListFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IMgmReportComboList;
    prototype: IMgmReportComboList;
    fetch(rep_cd: string, callback: JQueryPromiseCallback<IMgmReportComboList>): JQueryDeferred<IMgmReportComboList>;
    getDataSource(data?: any): kendo.data.DataSource;
    getList(list_cd: string): any;
}
//クラス名を定義
declare var MgmReportComboList: MgmReportComboListFn;

define(function () {

    var MgmReportComboList: MgmReportComboListFn = <any>kendo.data.Model.define({
        id: "list_cd",
        fields: {
            "list_cd": { type: "string" },
            "query": { type: "string" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    MgmReportComboList.prototype.parseJSON = function(){
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    MgmReportComboList.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("MgmReportComboList/SaveComboList"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    MgmReportComboList.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("MgmReportComboList/DeleteComboList/" + this.get("list_cd")),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    MgmReportComboList.fetch = function (list_cd: string, callback) {
        var deffered = $.ajax({
            url: App.getApiPath("MgmReportComboList/GetComboList/" + list_cd),
            type: "GET",
            cache: false
        })
        .done(function (result) {
            var data = new MgmReportComboList(result);
            data.parseJSON();
            if (callback) callback(data);
        });
        return deffered;
    };

    MgmReportComboList.getList = function (list_cd : string) {
        var deffered = $.ajax({
            url: App.getApiPath("MgmReportComboList/GetComboListData/" + list_cd),
            type: "GET",
            cache: false
        });
        return deffered;
    };

    MgmReportComboList.getDataSource = function (data) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("MgmReportComboList/SearchComboLists"),
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: MgmReportComboList }
        });
    };

    return MgmReportComboList;
});

