//TypeScriptコンパイラ用型定義
interface IChurchBlockEditView extends kendo.data.ObservableObject {
    init(options?: any): void;
    init(church_cd: string, is_copied: boolean): void;
    openEditWindow(church_cd: string, is_copied: boolean): void;
    createGrid(e?: any);
    churchBlockClick(e?: any);
    onSave(e?: any): void;
    onClose(e? : any): void;
    btnGo2Click(e?: any);
    getSelKind(e?: any);
    refreshList(e?: any);
}

define(['Models/ChurchBlock', 'Models/Church'], 
    function (ChurchBlock: ChurchBlockFn, Church: ChurchFn) {

        var churchBlockEditView: IChurchBlockEditView = <any>kendo.observable({
            model: null,
            status: "",
            error: "",
            year: null,
            month: null,
            updated: false,

            avail_data: null,

            init: function (church_cd, is_copied) {
                this.set("status", "Loading...");
                this.set("error", "");
                this.set("model", null);

                var validator = $("#editDiv form").data("kendoValidator");
                validator.hideMessages();       //前回のValidationメッセージをクリア。

                // Grid初期化
                $("#availGrid").html("");

                Church.fetch(church_cd, function (item) {
                    if (church_cd == "" || is_copied) {
                        item.set("id", "");     //これによってisNew()がtrueになる。
                        item.dirty = true;
                        item.set("is_new", true);
                    }
                    churchBlockEditView.set("model", item);
                }
                ).fail(function (jqXHR, textStatus, errorThrown) {
                    churchBlockEditView.set("error", errorThrown);
                }).always(function () {
                    churchBlockEditView.set("status", "");
                    App.Utils.HideLoading(true);
                });

            },

            createGrid: function () {
                var data = churchBlockEditView.get("avail_data");
                var headdata = data.headers;
                var blockdata = data.blocks;
                
                var grid = $("#availGrid");
                // Grid初期化
                grid.html("");

                var content = $("<div>", { "style": "height:550px;" });
                content.width($("#availGrid thead").width());  // Widthにヘッダーの幅とスクロールバーの幅を足す。

                // スクロールバーが表示されているかチェック。
                if (grid.prop("scrollWidth") != grid.prop("clientWidth")) {
                    content.height(content.height() - 17);          // Heightからスクロールバーの高さを引く。
                }
                grid.append(content);

                var table = $("<table>", { "style": "table-layout: fixed;" });
                content.append(table);

                //var thead = $("<thead>");
                //table.append(thead);

                var tbody = $("<tbody>");
                table.append(tbody);

                var tr = $("<tr>");
                tbody.append(tr);

                for (i = 0; i < headdata.length; i++) {
                    var timeTh;
                    if (i < 2) {
                        timeTh = $("<th>", { "class": "k-header", "style": "width:45px;" });
                    } else {
                        timeTh = $("<th>", { "class": "k-header", "style": "width:65px;" });
                    }
                    timeTh.text(headdata[i].header_name);
                    tr.append(timeTh);
                }

                for (var i = 0; i < blockdata.length; i++) {

                    var tr = $("<tr>", { "role": "row" });
                    tbody.append(tr);

                    // Date
                    var dateTd = $("<td>", { "role": "grid-cell", "style": "width:45px;" });
                    dateTd.text(blockdata[i].date);
                    tr.append(dateTd);

                    // Day
                    var dayTd = $("<td>", { "role": "grid-cell", "style": "width:45px;" });
                    dayTd.text(blockdata[i].day);

                    if (App.Utils.getBoolAttr("#chkCloseSun", "checked") && blockdata[i].day == "SUN") {
                        dayTd.css("background-color", "blue");
                        dayTd.css("color", "white");
                    }
                    if (blockdata[i].is_holiday) {
                        dayTd.css("background-color", "red");
                        dayTd.css("color", "white");
                    }
                    tr.append(dayTd);

                    // Blocks
                    var block_date = App.Utils.convertToDateTime(blockdata[i].block_date);
                    var blocks = blockdata[i].blocks;
                    for (var j = 0; j < blocks.length; j++) {
                        var status = blocks[j].status;
                        //var agentcd = blocks[j].agent_cd;

                        var td = $("<td>", { "role": "grid-cell", "style": "width:65px;" });
                        td.css("cursor", "pointer");
                        td.data("block-data", {
                            block_date: block_date,
                            start_time: App.Utils.convertToDateTime(blocks[j].start_time)
                        });
                        td.on("click", churchBlockEditView.churchBlockClick);

                        switch (status) {
                            case "X":
                                td.text("Closed");
                                td.addClass("closed");
                                break;
                            //case "Booked":
                            //    td.text(agentcd);
                            //    td.addClass("booked");
                            //    break;
                            //case "Block":
                            //    td.text(agentcd);
                            //    td.addClass("block");
                            //    break;
                            //case "Avail":
                            //    td.text("");
                            //    td.addClass("available");
                            //    break;
                            //case "Unknown":
                            //    td.text("Unknown");
                            //    td.addClass("unknown");
                            //    break;
                            default: break;
                        }

                        if (blocks[j].is_edited) {
                            td.addClass("edit");
                        }

                        tr.append(td);
                    }
                }

            },

            churchBlockClick: function () {
                var block = $(this);

                // クリックしたBlockが分かるように色を付ける。
                $("td.select-block").removeClass("select-block");
                block.addClass("select-block");

                var blockData = block.data("block-data");
                var fromDate = $("#fromDateRange1").data("kendoDatePicker");
                var toDate = $("#toDateRange1").data("kendoDatePicker");
                var fromTime2 = $("#fromTimeRange2").data("kendoTimePicker");
                var toTime2 = $("#toTimeRange2").data("kendoTimePicker");

                fromDate.value(blockData.block_date);
                toDate.value(blockData.block_date);
                fromTime2.value(blockData.start_time);
                toTime2.value(blockData.start_time);
            },

            openEditWindow: function (church_cd, is_copied) {
                var win = $("#editWindow").data("kendoWindow");
                win.title(church_cd || "New ChurchBlock");
                win.center().open();

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, churchBlockEditView);
                churchBlockEditView.init(church_cd, is_copied);
            },

            isCodeEditable: function () {
                var item = churchBlockEditView.get("model");
                return item && item.isNew();
            },

            onClose: function (e) {
                if (e) e.preventDefault();
                $("#btnEditClose").closest("[data-role='window']").data("kendoWindow").close();
            },

            getSelKind: function (e) {
                var selkind = 0;
                var lblSelKind = $("div.select-selkind").text();
                switch (lblSelKind) {
                    case "Closed": selkind = 0; break;
                    case "Open": selkind = 1; break;
                    //case "Block": selkind = 2; break;
                    //case "Booked": selkind = 3; break;
                    //case "Unknown": selkind = 4; break;
                    default: selkind = 0; break;
                }
                return selkind;
            },

            onSave: function (e) {
                var year = $("#year").data("kendoDatePicker").value();
                var month = $("#month").data("kendoDatePicker").value();
                if (!year || !month) {
                    return false;
                }

                //var total = ClientViewModel.AvailGridData.blocks.length;
                //var count = 0;

                ////プログレスバーを表示。
                //ShowOverlay();
                //$("#progress").show();
                App.Utils.ShowLoading();
                var list = [];
                var blocks = churchBlockEditView.get("avail_data.blocks");

                $.each(blocks, function (i) {

                    var avails = [];
                    var row = blocks[i];
                    ////ChurchBlockにレコードが存在しない場合は編集されていなくてもSaveする
                    //var exist = churchBlockEditView.get("avail_data.blockExist");

                    if (row.is_edited) {
                        $.each(row.blocks, function (j) {
                            var block = row.blocks[j];
                            if (block.is_edited) {
                                var data = {
                                    start_time: block.start_time,
                                    status: block.status,
                                    block_date: row.block_date
                                };
                                avails.push(data);
                            }
                        });
                        if (avails.length > 0) {
                            list.push(avails);
                        }
                    }
                });

                if (list.length === 0) {
                    alert("No data updated!");
                    return false;
                }

                var availData = {
                    church_cd: churchBlockEditView.get("model.church_cd"),
                    avails: list
                };

                $.ajax({
                    url: App.getApiPath("ChurchBlocks/SaveAvailList"),
                    type: "POST",
                    data: JSON.stringify(availData),
                    processData: false,
                    contentType: "application/json; charset=utf-8"

                }).done(function (result) {
                    if (result !== "ok") {
                        App.Utils.ShowAlert(result, true);
                        return;
                    }
                    churchBlockEditView.trigger("saved");
                    App.Utils.ShowAlert("Data saved successfully!", false);
                    churchBlockEditView.refreshList();

                }).fail(function (jqXHR, textStatus, errorThrown) {
                    alert("Unexpected error:\n\n" + errorThrown);
                    return false;
                }).always(function () {
                    App.Utils.HideLoading();
                });
            },

            btnGo2Click: function (e) {
                if (e) e.preventDefault();
                // 入力チェック
                //var agentCombo = $("#agentCombo2").data("kendoComboBox");
                //var agentCd = agentCombo.value();
                //if (!checkAgentCd(agentCombo)) {
                //    return false;
                //}

                var fromTimeRange = $("#fromTimeRange2").data("kendoTimePicker").value();
                var toTimeRange = $("#toTimeRange2").data("kendoTimePicker").value();
                if (!fromTimeRange || !toTimeRange) {
                    return false;
                }

                var selkind = churchBlockEditView.getSelKind();
                if (selkind == 3) {
                    alert("You can't set to Booked in Church Master.");
                    return false;
                }

                // Block編集
                var optDate = $("input:radio[name=optDate]:checked").val();
                var fromTime = fromTimeRange;
                var toTime = toTimeRange;
                var fromDate = 1;
                var toDate = 1;
                var fromDateRange = $("#fromDateRange1").data("kendoDatePicker").value();
                var toDateRange = $("#toDateRange1").data("kendoDatePicker").value();
                if (fromDateRange && toDateRange) {
                    fromDate = moment(fromDateRange).date();
                    toDate = moment(toDateRange).date();
                }

                var blocks = churchBlockEditView.get("avail_data.blocks");
                for (var i = 0; i < blocks.length; i++) {
                    var row = blocks[i];
                    var blockDate = moment(App.Utils.convertToDateTime(row.block_date)).date();
                    var blockDay = moment(App.Utils.convertToDateTime(row.block_date)).day();

                    if (blockDate >= fromDate && blockDate <= toDate) {
                        var edit = true;
                        // Individuall
                        if (optDate == 1) {
                            if (blockDay == 0 && !App.Utils.getBoolAttr("#chkSun", "checked")) edit = false;
                            else if (blockDay == 1 && !App.Utils.getBoolAttr("#chkMon", "checked")) edit = false;
                            else if (blockDay == 2 && !App.Utils.getBoolAttr("#chkTue", "checked")) edit = false;
                            else if (blockDay == 3 && !App.Utils.getBoolAttr("#chkWed", "checked")) edit = false;
                            else if (blockDay == 4 && !App.Utils.getBoolAttr("#chkThu", "checked")) edit = false;
                            else if (blockDay == 5 && !App.Utils.getBoolAttr("#chkFri", "checked")) edit = false;
                            else if (blockDay == 6 && !App.Utils.getBoolAttr("#chkSat", "checked")) edit = false;
                        }

                        // Block編集
                        if (edit) {
                            for (var j = 0; j < row.blocks.length; j++) {
                                var block = row.blocks[j];
                                var blockTime = App.Utils.convertToDateTime(block.start_time);                                

                                if (block.status != "Booked" && (blockTime >= fromTime && blockTime <= toTime)) {

                                    switch (selkind) {
                                        case 0: block.status = "X"; block.is_edited = true; row.is_edited = true; break;
                                        case 1: block.status = ""; block.is_edited = true; row.is_edited = true; break;
                                        //case 2:
                                        //    block.status = "Block";
                                        //    block.is_edited = true;
                                        //    row.is_edited = true;
                                        //    break;
                                        //case 3: break;
                                        //case 4: block.status = "Unknown"; block.is_edited = true; row.is_edited = true; break;
                                        default: break;
                                    }
                                }
                            }
                        }
                    }
                }

                // AvailGridを再表示
                churchBlockEditView.createGrid();

                //agentCombo.value(null);
                $("#fromTimeRange2").data("kendoTimePicker").value(null);
                $("#toTimeRange2").data("kendoTimePicker").value(null);
                //ClientViewModel.AvailUpdate = true;
            },

            refreshList: function (e) {
                if (e) e.preventDefault();

                var data = {
                    church_cd: churchBlockEditView.get("model.church_cd") || "-",
                    year: churchBlockEditView.get("year"),
                    month: churchBlockEditView.get("month"),
                    fdHol: false,
                    stHol: false,
                    sun: false
                }

                $.ajax({
                    url: App.getApiPath("ChurchBlocks/GetAvailList"),
                    type: "POST",
                    data: JSON.stringify(data),
                    processData: false,
                    contentType: "application/json; charset=utf-8"

                }).done(function (data) {
                    churchBlockEditView.set("avail_data", data);
                    churchBlockEditView.createGrid();

                    // RangeDateのMIN,MAXの設定。
                    var fromDate = $("#fromDateRange1").data("kendoDatePicker");
                    fromDate.value(null);
                    fromDate.min(App.Utils.convertToDateTime(data.minDate));
                    fromDate.max(App.Utils.convertToDateTime(data.maxDate));

                    var toDate = $("#toDateRange1").data("kendoDatePicker");
                    toDate.value(null);
                    toDate.min(App.Utils.convertToDateTime(data.minDate));
                    toDate.max(App.Utils.convertToDateTime(data.maxDate));

                    // RangeTimeの時間指定範囲の設定。
                    var timeList = [];
                    for (var i = 0; i < data.times.length; i++) {
                        timeList.push(App.Utils.convertToDateTime(data.times[i].start_time));
                    }

                    $("#fromTimeRange1").css("width", "100px");
                    $("#fromTimeRange1").kendoTimePicker({
                        dates: timeList,
                        format: "@(UserHelper.LoginUser.time_format)"
                    });

                    $("#toTimeRange1").css("width", "100px");
                    $("#toTimeRange1").kendoTimePicker({
                        dates: timeList,
                        format: "@(UserHelper.LoginUser.time_format)"
                    });

                    $("#fromTimeRange2").css("width", "100px");
                    $("#fromTimeRange2").kendoTimePicker({
                        dates: timeList,
                        format: "@(UserHelper.LoginUser.time_format)"
                    });

                    $("#toTimeRange2").css("width", "100px");
                    $("#toTimeRange2").kendoTimePicker({
                        dates: timeList,
                        format: "@(UserHelper.LoginUser.time_format)"
                    });
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    App.Utils.ShowAlert(errorThrown, true);
                }).always(function () {
                    App.Utils.HideLoading();
                });
            },

            deleteItem: function (e) {
                if (e) e.preventDefault();
                var grid = $("#grdChurchBlock").data("kendoGrid");
                var item = <IChurchBlock>grid.dataItem(grid.select());
                if (!item) {
                    alert("Please select a row.");
                    return;
                }

                var start_time = kendo.toString(item.get("start_time"), "HH:mm");

                if (confirm("Are you sure to delete '" + start_time + "'?")) {
                    App.Utils.ShowLoading();
                    item.destroy()
                        .done(function (result) {
                            if (result !== "ok") {
                                App.Utils.ShowAlert(result, true);
                                return;
                            }
                            App.Utils.ShowAlert("The item '" + start_time + "' has been deleted.", false);
                            churchBlockEditView.refreshList();
                        }).fail(function (jqXHR, textStatus, errorThrown) {
                            App.Utils.ShowAlert(errorThrown, true);
                        });
                }
            }
        });

        $("#editDiv").on("click", "#btnRefresh", churchBlockEditView.refreshList);
        $("#editDiv").on("click", "#btnGo2", churchBlockEditView.btnGo2Click);
        $("#editDiv").on("click", "#btnSaveBlockInfo", churchBlockEditView.onSave);


        return churchBlockEditView;
    }
);




