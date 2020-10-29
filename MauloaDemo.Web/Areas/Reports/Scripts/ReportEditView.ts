//TypeScriptコンパイラ用型定義
interface IMgmReportEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(agent_cd: string, is_copied: boolean): void;
    openEditWindow(rep_cd: string, is_new: boolean): void;
    onSave(e?: any): void;
    onClose(e?: any): void;

    menuCdList: kendo.data.DataSource;
    storedProcList: kendo.data.DataSource;
    outputTypeList: kendo.data.DataSource;
    isRepCdEditable(): boolean;
    isExcel(): boolean;
    loadParams(): void;
    createParam(e?: any): void;
    copyParam(e?: any): void;
    onFocusRepSp(): void;
    onBlurRepSp(): void;
    confirmLoadParams(): void;
}

define(['Models/MgmReportComboList', 'Models/MgmReportParam', 'Models/MgmReport'],
function (MgmReportComboList: MgmReportComboListFn, MgmReportParam: MgmReportParamFn, MgmReport: MgmReportFn) {

    var reportEditView: IMgmReportEditView = <any> kendo.observable({
        mgmReport: null,
        status: "",
        error: "",

        menuCdList: new kendo.data.DataSource({ data: MgmReport.MenuCdList }),

        storedProcList: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("MgmReport/GetReportStoredProcs"),
                    dataType: "json"
                }
            },
            serverFiltering: true
        }),

        outputTypeList: new kendo.data.DataSource({
            data: [{ value: "csv", text: 'CSV' }, { value: "excel", text: 'Excel' }]
        }),

        init: function (rep_cd: string, is_copied: boolean) {
            this.set("status", "Loading...");
            this.set("error", "");
            if (this.get("mgmReport")) this.set("mgmReport.Params", []);      //Paramsのグリッドをクリアするために必要。
            this.set("mgmReport", null);

            App.Utils.ShowLoading(true);
            MgmReport.fetch(rep_cd, function (report) {
                    if (rep_cd == "" || is_copied) {
                        report.set("id", "");     //これによってisNew()がtrueになる。
                        report.dirty = true;
                    }
                    reportEditView.set("mgmReport", report);
                },
                true).fail(function (jqXHR, textStatus, errorThrown) {
                    reportEditView.set("error", errorThrown);
                }).always(function () {
                    reportEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });
        },

        openEditWindow: function (rep_cd: string, is_copied: boolean) {
            var win = $("#editWindow").data("kendoWindow");
            win.title(rep_cd || "New Report");
            win.center().open();

            var div = $("#editDiv");
            kendo.unbind(div);
            kendo.bind(div, reportEditView);
            reportEditView.init(rep_cd, is_copied);
        },

        isRepCdEditable: function () {
            var report = this.get("mgmReport");
            if (!report) return true;
            return report.isNew();
        },

        isExcel: function () {
            return (this.get("mgmReport.output_type") == "excel");
        },

        onFocusRepSp: function () {
            $("#txtRepSp").data("kendoAutoComplete").search($("#txtRepSp").val());
        },

        onBlurRepSp: function () {
            var report = reportEditView.get("mgmReport");
            if (!report) return true;
            if (report.get("Params") && report.get("Params").length > 0) return;

            reportEditView.loadParams();
        },

        confirmLoadParams: function () {
            var report = reportEditView.get("mgmReport");
            if (!report) return;
            if (report.get("Params") && report.get("Params").length > 0) {
                if (!confirm("Reload parameters from database overwritting the current ones?")) return;
            }
            reportEditView.loadParams();
        },

        loadParams: function () {
            var rep_sp = $("#txtRepSp").val();
            MgmReportParam.getStoredProcParams(rep_sp)
                .done(function (result) {
                    if (result) {
                        var report = reportEditView.get("mgmReport");
                        var params = [];
                        $.each(result, function (ix, item) {
                            var seq = (params.length + 1) * 10;
                            var param_name = item.ParamName.replace("@", "");
                            var type = "text";
                            switch (item.TypeName) {
                                case "datetime":
                                case "smalldatetime":
                                    type = "date";
                                    break;
                                case "bit":
                                    type = "check";
                                    break;
                                default:
                                    type = "text";
                                    break;
                            }
                            var new_param = new MgmReportParam({
                                rep_cd: report.rep_cd,
                                param_seq: seq,
                                param_name: param_name,
                                param_type: type,
                                caption: param_name,
                                break_after: false,
                                field_length: type == "date" ? 10 : item.MaxLength,
                                decimal_length: 0,
                                required: false,
                                default_value: "",
                                hidden: false
                            });
                            params.push(new_param);
                        });
                        reportEditView.set("mgmReport.Params", params);
                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    App.Utils.HideLoading();
                    App.Utils.ShowAlert(errorThrown, true);
                }).always(function () {
                    App.Utils.HideLoading();
                });
        },

        createParam: function (e) {
            if (e) e.preventDefault();
            var grid = $("#reportEditGrid").data("kendoGrid");
            var report = reportEditView.get("mgmReport");

            var last_seq = 0;
            if (report.get("Params")) {
                var last_param = report.get("Params")[report.get("Params").length - 1];
                if (last_param) last_seq = last_param.get("param_seq");
            }

            var new_param = new MgmReportParam();
            new_param.set("param_id", 0);
            new_param.set("param_seq", last_seq + 1);
            grid.dataSource.add(new_param);
        },

        copyParam: function (e) {
            if (e) e.preventDefault();
            var grid = $("#reportEditGrid").data("kendoGrid");
            var p = grid.dataItem(grid.select());
            if (!p) {
                alert("Please select an existing parameter to copy.");
                return;
            }

            var new_param = new MgmReportParam(p.toJSON());
            new_param.set("id", "");
            new_param.set("param_id", 0);
            new_param.set("param_seq", p.get("param_seq") + 1);
            grid.dataSource.add(new_param);
        },

        onSave: function (e) {
            if (e) e.preventDefault();

            App.Utils.ShowLoading(true);
            var report = reportEditView.get("mgmReport");
            report.save()
                .done(function (data) {
                    if (data !== "ok") {
                        App.Utils.ShowAlert(data, true);
                        return;
                    }
                    reportEditView.trigger("saved");
                    App.Utils.ShowAlert("Data saved successfully!", false);
                    reportEditView.onClose();
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    App.Utils.ShowAlert(errorThrown, true);
                });
        },
        onClose: function (e) {
            if (e) e.preventDefault();
            $("#reportEditClose").closest("[data-role='window']").data("kendoWindow").close();
        }

    });

    $("#editDiv").on("click", "#reportEditGrid .k-grid-new", reportEditView.createParam);
    $("#editDiv").on("click", "#reportEditGrid .k-grid-copy", reportEditView.copyParam);
    $("#editDiv").on("focus", "#txtRepSp", reportEditView.onFocusRepSp);
    $("#editDiv").on("blur", "#txtRepSp", reportEditView.onBlurRepSp);
    $("#editDiv").on("click", "#btnGetParams", reportEditView.confirmLoadParams);

    return reportEditView;
});




