//TypeScriptコンパイラ用型定義
interface IMgmReportView extends kendo.data.ObservableObject {
    reportVisible(): boolean;
    afterFetchReport(data: IMgmReport): void;
    editReport(e?: any): void;
    onExecute(e?: any): void;
}

require(['Models/MgmReportComboList', 'Models/MgmReportParam', 'Models/MgmReport', 'ReportEditView'],
    function (MgmReportComboList: MgmReportComboListFn, MgmReportParam: MgmReportParamFn, MgmReport: MgmReportFn, reportEditView: IMgmReportEditView) {

        var myApp : IMgmReportView=<any> kendo.observable({
            mgmReport: null,
            status: "Loading...",
            error: "",

            reportVisible: function () { return !(this.get("status") || this.get("error")); },

            init: function (rep_cd) {

                MgmReport.fetch(rep_cd, myApp.afterFetchReport, false)
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        myApp.set("status", "");
                        myApp.set("error", errorThrown);
                    });
            },

            afterFetchReport: function (data) {
                myApp.set("status", "");
                myApp.set("mgmReport", data);

                //コンボリストの内容をセット
                $("#app").find(".parameter input[data-role='combobox']").each(function (ix, item) {
                    var name = $(item).attr("name");
                    var param = data.getParamByName(name);
                    var list_cd : string = param.get("list_cd");
                    MgmReportComboList.getList(list_cd)
                        .done(function (data) {
                            $(item).data("kendoComboBox").setDataSource(data);
                            $(item).data("kendoComboBox").list.width(420);
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                });

                //ラジオボタンの選択肢をセット
                $("#app").find(".parameter input[type='radio']").each(function (ix, item) {
                    var name = $(item).attr("name");
                    var param = data.getParamByName(name);
                    var list_cd: string = param.get("list_cd");
                    MgmReportComboList.getList(list_cd)
                        .done(function (data) {
                            $.each(data, function (ix, dataItem) {
                                var input = $("<input type='radio' />");
                                input.attr("name", name);
                                input.val(dataItem.value);
                                input.prop("checked", (dataItem.value == param.get("default_value")));
                                input.appendTo($(item).parent());
                                var span = $("<span/>");
                                span.text(dataItem.text);
                                span.insertAfter(input);
                            });
                            $(item).remove();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                });
            },

            editReport: function (e) {
                if (e) e.preventDefault();
                var rep_cd = $(e.target).closest("a.btn").data("repcd");
                reportEditView.openEditWindow(rep_cd, false);
            },

            onExecute: function (e) {
                if (e) e.preventDefault();
                var url = $(e.target).data("url");

                var report = myApp.get("mgmReport");
                if (!report) return;

                var values = "";
                var params = report.get("Params");
                var len = params.length;
                var validationErrors = [];
                for (var i = 0; i < len; i++) {
                    var p = params[i];
                    var name = p.get("param_name");
                    if (name !== "_dummy_") {
                        var input = $("#app").find("input[name='" + name + "']");
                        var v = "";
                        switch (p.get("param_type")) {
                            case "combo":
                                var selectedIndex = input.data("kendoComboBox").select();
                                v = selectedIndex >= 0 ? input.data("kendoComboBox").dataItem(selectedIndex).value : input.val();
                                break;
                            case "date":
                                v = kendo.toString(input.data("kendoDatePicker").value(), "yyyy/MM/dd");
                                break;
                            case "check":
                                v = input.prop("checked");
                                break;
                            case "radio":
                                $.each(input, function (ix, item) {
                                    if ($(item).prop("checked")) {
                                        v = $(item).val();
                                        return false;
                                    }
                                });
                                break;

                            case "text":      //Intentional fall-through to default.
                            default:
                                v = input.val();
                                break;
                        }
                        values += "&" + name + "=" + encodeURIComponent(v);

                        if (p.get("required") && !v) {
                            validationErrors.push({ error: "required", param_name: name });
                        }
                    }
                }

                if (validationErrors.length > 0) {
                    var msg = JSON.stringify(validationErrors);
                    App.Utils.ShowAlert(msg, true);
                } else {
                    url += "?_rep_cd=" + encodeURIComponent(report.rep_cd) + values;
                    window.open(url, "_blank");
                }
            }

        });

        kendo.bind("#app", myApp);
        myApp.init($("#hdnRepCd").val());

        reportEditView.bind("saved", function () {
            myApp.init($("#hdnRepCd").val());
        });
    }
);




