define(['models/planlistitem'
        , 'models/church_block']
        , function (PlanListItem, ChurchBlock) {

    var planSearchViewModel = <any>kendo.observable({

        church_label: "Church",

        search: {
            church_cd: "",
            item_cd: "",
            item_name: "",
            plan_type: "-",
            sub_agent_cd: App.User.SubAgentCd
        },

        plan: null,

        clearSearch: function (plan_type, plan) {
            this.set("search.church_cd", plan.req_church_cd || "");
            this.set("search.item_cd", "");
            this.set("search.item_name", "");
            this.set("search.plan_type", plan_type);
            this.set("search.sub_agent_cd", plan.sub_agent_cd);
            this.set("wed_date", plan.req_wed_date);
            this.set("church_label", (plan_type === 'W') ? "Church" : "Location");
            this.set("plan", plan);
            this.set("is_irregular_time", false);
            this.set("irregular_time", "");

            var churchCombo = $(".plan-dialog").find("#txtChurchCd").data("kendoComboBox");
            churchCombo.list.width(380);
            if (plan_type === 'P') {
                churchCombo.setDataSource(App.data.churchListForPhotoPlan);
            } else {
                churchCombo.setDataSource(App.data.churchListForWeddingPlan);
            }

            this.doSearch();
        },

        selectedItem: null,
        blocks: [],
        wed_date: null,
        is_irregular_time: false,
        irregular_time: "",

        planList: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("items/SearchWithPrice"),
                    type: "POST",
                    dataType: "json",
                    serverFiltering: true,
                    data: function () {
                        return {
                            church_cd: planSearchViewModel.get("search.church_cd"),
                            item_cd: planSearchViewModel.get("search.item_cd"),
                            item_name: planSearchViewModel.get("search.item_name"),
                            plan_type: planSearchViewModel.get("search.plan_type"),
                            wed_date: kendo.toString(planSearchViewModel.get("wed_date"), "yyyy/MM/dd"),
                            sub_agent_cd: planSearchViewModel.get("search.sub_agent_cd")
                        };
                    }
                },
            },
            schema: {
                model: PlanListItem
            },
        }),

        doSearch: function () {
            planSearchViewModel.set("selectedItem", null);
            planSearchViewModel.set("blocks", []);
            planSearchViewModel.planList.read();
        },

        canEnterIrregularTime: function () {
            return !App.User.IsAgent;
        },

        isIrregularTime: function () {
            return planSearchViewModel.get("is_irregular_time");
        },

        getSelectedRow: function () {
            var grid = $(".plan-dialog").find("#grdPlanSearch").data("kendoGrid");
            var selectedRows = <any>grid.select();
            if (selectedRows && selectedRows.length == 1) {
                return selectedRows[0];
            } else {
                return null;
            }
        },
        getSelectedItem: function () {
            var row = this.getSelectedRow();
            if (!row) return null;
            var grid = $(".plan-dialog").find("#grdPlanSearch").data("kendoGrid");
            return grid.dataItem(row);
        },

        selectionChanged: function (e) {
            planSearchViewModel.set("selectedItem", this.getSelectedItem());

            this.getBlocks();
        },

        getWedDateStr: function () {
            var wd = planSearchViewModel.get("wed_date");
            var s = kendo.toString(wd, App.Config.DateFormat);
            return s;
        },

        wedDateChanged: function(){
            var wd = planSearchViewModel.get("wed_date");
            this.doSearch();
        },

        getBlocks: function () {
            var selected = planSearchViewModel.get("selectedItem");
            if (!selected) return;

            var wed_date = planSearchViewModel.get("wed_date");
            if (!wed_date) return;

            var data = {
                church_cd: selected.church_cd,
                block_date: kendo.toString(wed_date, "yyyy/MM/dd"),
                plan_kind: planSearchViewModel.get("search.plan_type")
            };
            App.Utils.ShowLoading();
            ChurchBlock.search( data, {
                success: function (result) {
                    if (result) {
                        //今開いているカスタマー自身の予約は「*SELF*」と表示。
                        for (var i = 0; i < result.length; i++) {
                            var block = result[i];
                            if (block.get("block_status") === "*BKD*"
                                && block.get("agent_cd") === App.ViewModel.get("Customer.agent_cd")
                                && block.get("c_num") === App.ViewModel.get("Customer.c_num")) {
                                block.set("block_status", "*SELF*");
                            }
                        }

                        planSearchViewModel.set("blocks", result);
                    } else {
                        App.Utils.ShowAlert("No block data found.", true);
                    }
                },
                fail: function (jqXHR, textStatus, errorThrown) { App.Utils.ShowAlert(errorThrown, true); },
                always: function () { App.Utils.HideLoading(); }
            });
        },

        closeDialog: function (e) {
            e.preventDefault();
            $(".plan-dialog").parent().data("kendoWindow").close();
        },

        selectPlan: function (e) {
            e.preventDefault();
            var selected = planSearchViewModel.get("selectedItem");
            if (!selected) {
                alert("Please select a plan.");
                return;
            }

            var msg;
            var church_cd = planSearchViewModel.get("selectedItem.church_cd");
            var wed_date = planSearchViewModel.get("wed_date");
            var item_cd = planSearchViewModel.get("selectedItem.item_cd");
            var item_name = planSearchViewModel.get("selectedItem.item_name");
            var item_name_jpn = planSearchViewModel.get("selectedItem.item_name_jpn");
            var d_net = planSearchViewModel.get("selectedItem.d_net");
            var plan_type = planSearchViewModel.get("search.plan_type");
            var sub_agent_cd = planSearchViewModel.get("search.sub_agent_cd");
            var is_irregular_time = planSearchViewModel.get("is_irregular_time");
            var irregular_time = planSearchViewModel.get("irregular_time");
            var rq_default = planSearchViewModel.get("selectedItem.rq_default");

            if (!wed_date) {
                msg = App.Config.Lang === "en" ? "Please select or input a date."
                                               : "日付を入力して下さい。";
                App.Utils.ShowAlert(msg, true);
                return;
            }

            if (is_irregular_time) {
                if (!irregular_time) {
                    msg = App.Config.Lang === "en" ? "Please select or input a time."
                                                   : "時間を入力して下さい。";
                    App.Utils.ShowAlert(msg, true);
                    return;
                }
            } else {
                var block = null;
                var selectedDivs = $("#blocksList").data("kendoListView").select();
                if (selectedDivs && selectedDivs.length > 0) {
                    //選択された時間枠のDIV要素を取得。
                    var div = $(selectedDivs[0]);

                    //DIV要素に対応するblockのオブジェクトを取得。
                    var block_filtered = $.grep(planSearchViewModel.get("blocks"), function (e: any) { return (e.uid === div.data("uid")) });
                    if (block_filtered && block_filtered.length == 1) {
                        block = block_filtered[0];
                    }
                }

                if (!block) {
                    msg = App.Config.Lang === "en" ? "Please select or select a time."
                                                   : "時間を選択して下さい。";
                    App.Utils.ShowAlert(msg, true);
                    return;
                }

                if (block.block_status === 'X') {
                    msg = App.Config.Lang === "en" ? "Selected date/time is closed. Please choose another date or time."
                                                    : "選択された日時はクローズされています。別の日時を選んで下さい。";
                    App.Utils.ShowAlert(msg, true);
                    return;
                }

                if (block.block_status && block.block_status !== "*SELF*" && block.block_status !== App.ViewModel.get("Customer.agent_cd")) {
                    msg = App.Config.Lang === "en" ? "Selected date/time is already booked. Please choose another date or time."
                                                    : "選択された日時は既に予約が入っています。別の日時を選んで下さい。";
                    App.Utils.ShowAlert(msg, true);
                    return;
                }

                if (block.block_status === "*SELF*") {
                    msg = App.Config.Lang === "en" ? "Selected date/time is booked by this customer. Are you changing the plan?"
                                                    : "選択された日時はこのカスタマーが予約済です。同一日時でプランを変更しますか？";
                    if (!confirm(msg)) return;
                }
            }

            var cur = planSearchViewModel.get("plan");
            cur.set("item_cd", item_cd);
            cur.set("item_name", item_name);
            cur.set("item_name_jpn", item_name_jpn);
            cur.set("req_church_cd", church_cd);
            cur.set("req_wed_date", wed_date);

            cur.set("is_irregular_time", is_irregular_time);

            if (is_irregular_time) {
                cur.set("req_wed_time_s", irregular_time);
            } else if (block.is_sunset) {
                cur.set("req_wed_time_s", App.Config.SunsetBlockTime);
            } else {
                cur.set("req_wed_time_s", block ? block.start_time_s : null);
            }

            cur.set("price", d_net);
            cur.set("amount", d_net);
            cur.set("sub_agent_cd", sub_agent_cd);
            cur.set("rq_default", rq_default);

            planSearchViewModel.trigger("selectPlan", {
                plan_type: plan_type,
                plan: cur
            });

            $(".plan-dialog").parent().data("kendoWindow").close();
        },

        gridDataBinding: function (e) {
            var grid = e.sender;
            var thead = $(grid.thead);
            var ths = thead.find("th");

            //日本語の時は英語名称を非表示にし、英語の時は日本語名称を非表示にする。
            grid.hideColumn(App.L("item_name", "item_name_jpn"));   

            if (App.User.IsAgent) {
                var caption_gross = "Gross";
                var caption_d_net = "Price";

                if (App.User.AgentCd == 'JTBO') {
                    caption_d_net = "Net";
                } else {
                    caption_gross = "";
                    grid.hideColumn("gross");
                }

                for (var i = 0; i < grid.columns.length; i++) {
                    var col = grid.columns[i];
                    switch (col.field) {
                        case 'gross':
                            $(ths[i]).find("a").text(caption_gross);
                            break;
                        case 'd_net':
                            $(ths[i]).find("a").text(caption_d_net);
                            break;
                    }
                }
            } else {
                for (var i = 0; i < grid.columns.length; i++) {
                    var col = grid.columns[i];
                    if (col.field == 'gross' || col.field == 'd_net') {
                        //col.title = col.field;    //これだと反映されないので下のやり方で。
                        $(ths[i]).find("a").text(col.field);
                    }
                }
            }
        }

    });

    //planSearchViewModel.bind("change", function (e) {
    //    //console.log("planSearchViewModel.change", e.field, planSearchViewModel.get(e.field));
    //});

    return planSearchViewModel;
});

