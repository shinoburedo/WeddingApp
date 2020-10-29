require([
        'models/customer'
        , 'models/sales'
        , 'models/weddingplan'
        , 'models/address_info'
        , 'models/cos_info'
        , 'customer_plansearch'
        , 'customer_optiondetail'
        , 'customer_addressinfodetail'
        , 'customer_cosinfodetail'
        , 'customer_note'
        , 'customer_schedule'
        , 'customer_file'
        , 'customer_duplist'], function (
        Customer
        , Sales
        , WeddingPlan
        , AddressInfo
        , CosInfo
        , planSearchViewModel
        , optionDetailViewModel
        , addressInfoViewModel
        , cosInfoViewModel
        , noteViewModel
        , schedulePhraseViewModel
        , customerFileViewModel
        , dupListViewModel
    ) {

    //*-------------------------------- View Model　--------------------------------*//
    var viewModel = <any>kendo.observable({

        dirty: false,
        customer_loaded: false,
        address_loaded: false,
        plans_loaded: false,
        options_loaded: false,
        costume_loaded: false,

        Customer: {},

        PhotoPlans: [],
        CurrentPhotoPlan: null,
        showCxlPhoto: false,
        PhotoPlans_dirty: false,

        WeddingPlans: [],
        CurrentWeddingPlan: null,
        showCxlWedding: false,
        WeddingPlans_dirty: false,

        Options: [],
        CurrentOption: null,
        showCxlOption: false,
        FileNumber: 0,

        CNumText: function () {
            var c_num = this.get("Customer.c_num");
            return c_num ? "#" + c_num : "(New)";
        },

        all_loaded: function () {
            return viewModel.get("customer_loaded")
                    && viewModel.get("address_loaded")
                    && viewModel.get("costume_loaded")
                    && viewModel.get("plans_loaded")
                    && viewModel.get("options_loaded");
        },

        //isPlansDirty: function (plans) {
        //    var len = plans.length;
        //    for (var i = 0; i < len; i++) {
        //        if (plans[i].dirty) return true;
        //    }
        //    return false;
        //},

        //saveEnabled: function () {
        //    var customer = viewModel.get("Customer");
        //    if (!customer) return false;                         //customerがNULLでない事をチェック。
        //    if (typeof customer.isNew === "undefined") return false;      //customerにisNew関数がある事をチェック。
        //    return customer.isNew() || viewModel.get("dirty");
        //},

        isJapan: function () {
            return App.Config.Lang == 'ja';
        },

        isSaveButtonVisible: function () {
            return App.User.AccessLevel >= 3;
        },

        savePhotoPlanEnabled: function () {
            var customer = viewModel.get("Customer");
            if (!customer) return false;
            if (!customer.isNew) return false;

            var plans_dirty = viewModel.get("PhotoPlans_dirty");
            return !customer.isNew()
                && plans_dirty
                && App.User.AccessLevel >= 3;
        },

        saveWeddingPlanEnabled: function () {
            var customer = viewModel.get("Customer");
            if (!customer) return false;
            if (!customer.isNew) return false;

            var plans_dirty = viewModel.get("WeddingPlans_dirty");
            return !customer.isNew()
                && plans_dirty
                && App.User.AccessLevel >= 3;
        },

        isBranchStaffVisible: function () {
            return App.User.IsStaff || App.User.BranchStaffRequired;
        },

        isChurchOrderVisible: function () {
            return App.User.IsStaff;
        },

        photoPlanSearchEnabled: function () {
            var cur = viewModel.get("CurrentPhotoPlan");
            if (!cur) return false;
            return cur.op_seq == 0;
        },
        photoPlanAddVisible: function () {
            var new_cnt = 0;
            var plans = viewModel.get("PhotoPlans");
            for (var i = 0; i < plans.length; i++) {
                var p = plans[i];
                if (p.op_seq == 0) new_cnt++;
            }
            return new_cnt == 0;
        },
        photoPlanAddEnabled: function () {
            if (!viewModel.get("Customer.c_num")) return false;
            if (!!viewModel.get("Customer.final_date")) return false;

            var new_cnt = 0;
            //var ok_cnt = 0;
            var plans = viewModel.get("PhotoPlans");
            for (var i = 0; i < plans.length; i++) {
                var p = plans[i];
                //if (p.book_status === 'K') ok_cnt++;
                if (p.op_seq == 0) new_cnt++;
            }
            //return ok_cnt == 0;
            return new_cnt == 0 && App.User.AccessLevel >= 3;
        },
        photoPlanReadOnly: function () {
            var cur = viewModel.get("CurrentPhotoPlan");
            if (!cur) return true;
            return cur.op_seq != 0;
        },

        isPhotoPlanStatusVisible: function () {
            var cur = viewModel.get("CurrentPhotoPlan");
            if (!cur) return false;
            return App.User.IsStaff || !cur.isNew();
        },

        isPhotoPlanAgentEnabled: function () {
            return !!viewModel.get("CurrentPhotoPlan") &&
                    !viewModel.get("Customer.final_date");
        },

        isWeddingPlanStatusVisible: function () {
            var cur = viewModel.get("CurrentWeddingPlan");
            if (!cur) return false;
            if (App.User.IsAgent) {
                return !cur.isNew();
            } else {
                return true;
            }
        },

        weddingPlanSearchEnabled: function () {
            var cur = viewModel.get("CurrentWeddingPlan");
            if (!cur) return false;
            return cur.op_seq == 0;
        },
        weddingPlanAddEnabled: function () {
            if (!viewModel.get("Customer.c_num")) return false;
            if (!!viewModel.get("Customer.final_date")) return false;

            var cnt = 0;
            var plans = viewModel.get("WeddingPlans");
            for (var i = 0; i < plans.length; i++) {
                var p = plans[i];
                if (p.op_seq == 0) cnt++;
            }
            return cnt == 0 && App.User.AccessLevel >= 3;
        },
        weddingPlanAddVisible: function () {
            var cnt = 0;
            var plans = viewModel.get("WeddingPlans");
            for (var i = 0; i < plans.length; i++) {
                var p = plans[i];
                if (p.op_seq == 0) cnt++;
            }
            return cnt == 0;
        },

        isWeddingPlanAgentEnabled: function () {
            return !!viewModel.get("CurrentWeddingPlan") &&
                    !viewModel.get("Customer.final_date");
        },

        weddingPlanReadOnly: function () {
            var cur = viewModel.get("CurrentWeddingPlan");
            if (!cur) return true;
            return cur.op_seq != 0;
        },

        hasPlanOrders: function () {
            return !!viewModel.get("Customer.c_num")
                    && (viewModel.getPhotoPlansCount() > 0 || viewModel.getWeddingPlansCount() > 0);
        },

        isOptionOnly: function () {
            return viewModel.get("customer_loaded") && !viewModel.hasPlanOrders();
        },

        optionAddEnabled: function () {
            if (!!viewModel.get("Customer.final_date")) return false;
            if (App.User.AccessLevel < 3) return false;

            if (App.User.IsAgent) {
                //Agentユーザーは先にプランを入れないとオプション追加は出来ない。
                return !!viewModel.get("Customer.c_num")
                        && (viewModel.getPhotoPlansCount() > 0 || viewModel.getWeddingPlansCount() > 0);
            } else {
                //MauloaDemoユーザーの場合はプランを入れなくても挙式日を手動で入力していればオプション追加が可能。
                return !!viewModel.get("Customer.c_num") && !!viewModel.get("Customer.wed_date");
            }
        },

        getCustomer: function (c_num) {
            if (!c_num) {
                var customer = new Customer();
                customer.set("area_cd", App.User.AreaCd);
                customer.set("sub_agent_cd", App.User.IsAgent ? App.User.SubAgentCd : "");
                viewModel.set("Customer", customer);
                return;
            } 

            viewModel.set("dirty", false);
            viewModel.set("customer_loaded", false);
            viewModel.set("address_loaded", false);
            viewModel.set("costume_loaded", false);
            viewModel.set("plans_loaded", false);
            viewModel.set("options_loaded", false);

            App.Utils.ShowLoading();
            Customer.fetch(c_num, {
                success: function (customer) {
                    App.Utils.HideLoading();
                    viewModel.set("Customer", customer);
                    viewModel.set("customer_loaded", true);
                    viewModel.getAddressInfos(c_num);       //住所一覧を取得
                    viewModel.getCosInfos(c_num);           //Costume一覧を取得
                    viewModel.getPlans(c_num);              //プラン一覧を取得
                    viewModel.getOptions(c_num);            //オプション一覧を取得
                    viewModel.getFiles(c_num);                   //File数を取得
                },
                fail: function (jqXHR, textStatus, errorThrown) {
                    App.Utils.HideLoading();
                    App.Utils.ShowAlert(errorThrown, true);
                }
            });
        },

        getFiles: function (c_num) {
            var strData = JSON.stringify({
                c_num: c_num
            });

            var request = $.ajax({
                url: "../../customerFolder/Count",
                type: "POST",
                data: strData,
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (data) {
                if (data.Result == "success") {
                    viewModel.set("FileNumber", data.Count);
                } else {
                    alert(data.Message);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert("Unexpected error:\n\n" + errorThrown);
            });
        },

        getAddressInfos: function (c_num) {
            AddressInfo.search(c_num, {
                success: function (list) {
                    viewModel.set("Customer.AddressInfos", list);
                    viewModel.set("address_loaded", true);
                },
                fail: function (jqXHR, textStatus, errorThrown) { App.Utils.ShowAlert(errorThrown, true); }
            });
        },

        getCosInfos: function (c_num) {
            CosInfo.search(c_num, {
                success: function (list) {
                    viewModel.set("Customer.CosInfos", list);
                    viewModel.set("costume_loaded", true);
                },
                fail: function (jqXHR, textStatus, errorThrown) { App.Utils.ShowAlert(errorThrown, true); }
            });
        },

        getPlans: function (c_num) {
            viewModel.set("CurrentPhotoPlan", null);
            viewModel.set("CurrentWeddingPlan", null);

            var data = { c_num: c_num, plan_type: "" };
            WeddingPlan.search(data, {
                success: function (list) {
                    viewModel.separatePlans(list);
                    viewModel.set("plans_loaded", true);
                },
                fail: function (jqXHR, textStatus, errorThrown) { App.Utils.ShowAlert(errorThrown, true); }
            });
        },

        separatePlans: function (list) {
            var photoPlans = [], wedPlans = []; 

            for (var i = 0; i < list.length; i++) {
                if (list[i].get("item_type") === "PKG") {
                    wedPlans.push(list[i]);
                } else {
                    photoPlans.push(list[i]);
                }
            }
            viewModel.setPlanText(photoPlans);
            viewModel.setPlanText(wedPlans);

            for (var i = 0; i < list.length; i++) {
                list[i].dirty = false;
            }
            //log("Plans list:", list);

            viewModel.set("PhotoPlans", photoPlans);
            if (photoPlans && photoPlans.length > 0) {
                setTimeout(function () {
                    viewModel.set("CurrentPhotoPlan", photoPlans[0]);
                }, 100);
            } 

            viewModel.set("WeddingPlans", wedPlans);
            if (wedPlans && wedPlans.length > 0) {
                setTimeout(function () {
                    viewModel.set("CurrentWeddingPlan", wedPlans[0]);
                }, 100);
            } 

            viewModel.activatePlanTab(list);
        },

        activatePlanTab: function (list) {
            var latest = null;
            for (var i = 0; i < list.length; i++) {
                if (list[i].book_status === "K" || list[i].book_status === "Q") {
                    latest = list[i];
                    break;
                }
            }
            if (!latest && list.length > 0) latest = list[0];
            if (latest) {
                $("#tabWedding, #tabPhoto, #tabOption, #panWedding, #panPhoto, #panOption").removeClass("active");
                if (latest.item_type === "PKG") {
                    $("#tabWedding, #panWedding").addClass("active");
                } else {
                    $("#tabPhoto, #panPhoto").addClass("active");
                }
            }
        },

        setPlanText: function (list) {
            for (var i = 0; i < list.length; i++) {
                list[i].set("text", list[i].getText(list.length - i - 1, list.length));
            }
        },

        getPhotoPlansCount: function () {
            return viewModel.get("PhotoPlans").length;
        },

        getWeddingPlansCount: function () {
            return viewModel.get("WeddingPlans").length;
        },

        getOptionsCount: function () {
            return viewModel.get("Options").length;
        },

        getOptions: function (c_num) {
            viewModel.set("Options", []);

            Sales.search(c_num, {
                success: function (list) {
                    ///log("getOptions done. ", list);
                    for (var i = 0; i < list.length; i++) {
                        var sales = list[i];
                        if (sales.arrangements.length > 0) {
                            if (sales.arrangements.length === 1) {
                                sales.cfmd = sales.arrangements[0].cfmd;
                                sales.vendor_cd = sales.arrangements[0].vendor_cd;
                            } else {
                                var cfmd = true;
                                for (var j = 0; j < sales.arrangements.length; j++) {
                                    if (!sales.arrangements[j].cfmd) {
                                        cfmd = false;
                                        break;
                                    }
                                }
                                sales.cfmd = cfmd;
                                sales.vendor_cd = sales.arrangements[0].vendor_cd + "...";
                            }
                        }
                    }
                    viewModel.set("Options", list);
                    viewModel.set("CurrentOption", null);
                    viewModel.set("options_loaded", true);
                },
                fail: function (jqXHR, textStatus, errorThrown) { App.Utils.ShowAlert(errorThrown, true); }
            });
        },

        onFolderSheetClick: function (e) {
            if (e) e.preventDefault();
            var customer = viewModel.get("Customer");
            if (!customer) return false;
            var contentUrl = $(e.target).data("contenturl");
            contentUrl += customer.c_num;
            window.open(contentUrl, "_blank");
        },

        isScheduleSheetEnabled: function () {
            var c_num = viewModel.get("Customer.c_num");
            return !!c_num;
        },

        isEditScheduleVisible: function () {
            return !App.User.IsAgent && App.User.AccessLevel >= 3;
        },

        getOKPlanItemCd: function () {
            var item_cd = null;
            var photoPlans = viewModel.get("PhotoPlans");
            for (var i = 0; i < photoPlans.length; i++) {
                var plan = photoPlans[i];
                if (plan.get("book_status") === 'K') {
                    item_cd = plan.get("item_cd");
                }
            }

            var wedPlans = viewModel.get("WeddingPlans");
            for (var i = 0; i < wedPlans.length; i++) {
                var plan = wedPlans[i];
                if (plan.get("book_status") === 'K') {
                    item_cd = plan.get("item_cd");
                }
            }
            return item_cd;
        },

        onEditScheduleClick: function (e) {
            if (e) e.preventDefault();
            var c_num = viewModel.get("Customer.c_num");
            if (!c_num) return false;
            var item_cd = viewModel.getOKPlanItemCd();
            var contentUrl = $(e.target).data("contenturl");
            var hotel_cd = viewModel.get("Customer.hotel_cd");
            schedulePhraseViewModel.openWindow(c_num, item_cd, hotel_cd, contentUrl);
        },

        onPreviewScheduleSheetClick: function (e) {
            if (e) e.preventDefault();
            var c_num = viewModel.get("Customer.c_num");
            if (!c_num) return false;
            var contentUrl = $(e.target).data("contenturl") + c_num;
            window.open(contentUrl, "_blank");
        },

        onSaveClick: function (e) {
            if (e) e.preventDefault();

            ////Kendo UIのValidationチェック
            //var validator = $("#frmCustomer").data("kendoValidator");
            //if (!validator.validate()) {
            //    //var errors = validator.errors();
            //    //var msg = "";
            //    //$(errors).each(function () { msg += (this + '\n\n'); });
            //    //App.Utils.ShowAlert(msg, true);
            //    return;
            //}
            viewModel.saveAll();
        },

        saveAll: function () {
            //Kendo UIのValidationチェック
            var validator = $("#frmCustomer").data("kendoValidator");
            if (!validator.validate()) return;

            viewModel.saveCustomer(function (c_num) {

                viewModel.savePlansSub("Photo", function (msg) {
                    if (msg) {
                        App.Utils.ShowAlert(msg, true);
                        viewModel.afterSave(c_num);
                        return;
                    }

                    viewModel.savePlansSub("Wedding", function (msg) {
                        if (msg) {
                            App.Utils.ShowAlert(msg, true);
                            viewModel.afterSave(c_num);
                            return;
                        }

                        App.Utils.ShowAlert("Data saved successfully. C#: " + c_num, false);
                        viewModel.afterSave(c_num);
                    });
                });
            });
        },

        afterSave: function (c_num) {
            viewModel.getCustomer(c_num);
            var newUrl = "../edit/" + c_num;
            history.pushState(null, null, newUrl);
        },

        onFinalizeClick: function (e) {
            if (e) e.preventDefault();
            var c_num = viewModel.get("Customer.c_num");
            if (!c_num) return false;
            viewModel.finalizeCustomer(c_num, true);
        },

        onUndoFinalizeClick: function (e) {
            if (e) e.preventDefault();
            var c_num = viewModel.get("Customer.c_num");
            if (!c_num) return false;
            viewModel.finalizeCustomer(c_num, false);
        },

        isFinalized: function () {
            return !!(viewModel.get("Customer.final_date"));
        },

        isFinalizeVisible: function () {
            if (App.User.AgentCd === 'JTBO' || App.User.AgentCd === 'JTBW') return false;
            return !(viewModel.get("Customer.final_date"));
        },

        isUndoFinalizeVisible: function () {
            if (App.User.IsAgent) return false;
            return !!(viewModel.get("Customer.final_date"));
        },

        undoFinalizeEnabled: function () {
            return !App.User.IsAgent;
        },

        undoFinalizeText: function () {
            return App.User.IsAgent ? "FINALIZED" : "Undo Finalize";
        },

        onPhotoPlanSaveClick: function (e) {
            if (e) e.preventDefault();
            viewModel.saveAll();
        },

        onWeddingPlanSaveClick: function (e) {
            if (e) e.preventDefault();
            viewModel.saveAll();
        },

        saveCustomer: function (callback, noDupCheck) {
            App.Utils.ShowLoading(true);
            var customer = viewModel.get("Customer");
            customer.save({
                noDupCheck: noDupCheck,
                success: function (result) {
                    if (result.message == "DUP") {
                        //log("DUP: ", result.dupList);
                        App.Utils.HideLoading(true);
                        dupListViewModel.openWindow(customer, result.dupList, 
                            function () {
                                //log("Ignore and continue.");
                                viewModel.saveCustomer(callback, /* noDupCheck = */ true);
                            }, function (existing_c_num) {
                                //log("Open existing customer. " + existing_c_num);
                                var newUrl = "../edit/" + existing_c_num;
                                window.open(newUrl, '_blank');    //別タブで開く。
                                //location.href = newUrl;
                            });
                        return;
                    } else if (result.message != "ok") {
                        App.Utils.ShowAlert(result.message, true);
                        return;
                    }
                    //log("Customer.save OK. C#: " + result.c_num);
                    viewModel.set("Customer.c_num", result.c_num);

                    if (callback) {
                        callback(result.c_num);
                        return;
                    }
                    var s = "Data saved successfully. C#: " + customer.c_num;
                    App.Utils.ShowAlert(s, false);

                    viewModel.getCustomer(result.c_num);

                    var newUrl = "../edit/" + result.c_num;
                    history.pushState(null, null, newUrl);
                },
                fail: App.Utils.ShowAlertAjaxErr
            });
        },

        finalizeCustomer: function (c_num, finalize) {
            if (!confirm("Are you sure to " + (finalize ? "finalize" : "UNDO finalize") + "?")) return;

            App.Utils.ShowLoading(true);
            Customer.updateFinalInfo(c_num, finalize, {
                success: function (result) {
                    if (result.message == "ok") {
                        var s = "Successfully updated customer #: " + result.c_num;
                        App.Utils.ShowAlert(s, false);
                        viewModel.getCustomer(result.c_num);
                    } else {
                        App.Utils.ShowAlert(result.message, true);
                    }
                },
                fail: App.Utils.ShowAlertAjaxErr
            });
        },


        addPhotoPlan: function (e) {
            if (e) e.preventDefault();

            var plans = viewModel.get("PhotoPlans");
            var p = new WeddingPlan();
            p.set("StatusList", [{ value: "Q", text: "RQ" }, { value: "K", text: "OK" }]);
            p.set("book_status", "K");      //フォトプランはデフォルトでOKステータス。(２週間を切っていたらサーバー側のチェックでRQになる。)
            p.set("text", p.getText(plans.length, plans.length + 1));

            //if (!App.User.StaffRequired) {
            //    p.set("staff", App.User.UserName);
            //}

            viewModel.openPlanSearchDialog("P", p);
        },

        removePhotoPlan: function (e) {
            if (e) e.preventDefault();
            var plans = viewModel.get("PhotoPlans");
            if (!plans) return;

            for (var i = plans.length - 1; i >= 0; i--) {
                if (!plans[i].op_seq) {
                    plans.splice(i, 1);              //i番目の要素を削除。
                }
            }
            viewModel.set("PhotoPlans", plans);

            var cur = null;
            if (plans.length > 0) cur = plans[0];
            setTimeout(function () {
                viewModel.set("CurrentPhotoPlan", cur);
            }, 200);
        },

        savePlansSub: function (caption, afterProc) {
            var customer = viewModel.get("Customer");
            if (!customer) return;
            var plans = viewModel.get(caption + "Plans");

            var dirty_plans = [];
            for (var i = 0; i < plans.length; i++) {
                var plan = plans[i];
                if (plan.dirty) {
                    plan.c_num = customer.c_num;
                    dirty_plans.push(plan);
                }
            }
            if (dirty_plans.length === 0) {
                //App.Utils.ShowAlert("No plans to save.");
                if (afterProc) {
                    afterProc();
                }
                return;
            }

            App.Utils.ShowLoading(true);
            WeddingPlan.savePlans(dirty_plans, {
                success: function (results) {
                    if (!results) {
                        App.Utils.ShowAlert("No results were received after saving " + caption + " Plans.", true);
                        return;
                    }
                    var msg = '';
                    var has_error = false;
                    for (var i = 0; i < results.length; i++) {
                        var res = results[i];
                        if (res.status === "error") {
                            has_error = true;
                            res.message = res.message || "An error has occurred.";
                        }
                        msg += res.message ? (caption + " Plan: " + plans[i].get("text") + " " + res.message + "<br />") : "";
                    }

                    if (has_error) {
                        App.Utils.ShowAlert(msg, true);
                    } else {
                        viewModel.set(caption + "Plans_dirty", false);

                        if (afterProc) {
                            afterProc(msg);
                            return;
                        }
                        viewModel.getCustomer(customer.c_num);
                    }
                },
                fail: function (jqXHR, textStatus, errorThrown) {
                    App.Utils.ShowAlert(errorThrown, true);
                }
            });
        },

        onAddressInfoNew: function (e) {
            if (e) e.preventDefault();
            //log('onAddressInfoNew');
            var c_num = viewModel.get("Customer.c_num");

            var info = new AddressInfo({ c_num: c_num, pax_type: "G" });
            var finalized = !!viewModel.get("Customer.final_date");
            addressInfoViewModel.openWindow(info, finalized, function () {
                viewModel.getAddressInfos(c_num);
            });
        },

        onAddressInfoEdit: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = grid.dataItem(tr);

            //log("Selected address info: ", selected);
            var info = new AddressInfo(selected);
            var finalized = !!viewModel.get("Customer.final_date");

            //AddressInfo詳細画面を開く。
            addressInfoViewModel.openWindow(info, finalized, function () {
                //詳細の保存後は一覧を再取得する。
                viewModel.getAddressInfos(c_num);
            });
        },

        onAddressInfoDelete: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = grid.dataItem(tr);

            if (!confirm("Are you sure to DELETE this address?")) return;

            var info = new AddressInfo(selected);
            info.remove({
                success: function (result) {
                    viewModel.getAddressInfos(c_num);   //住所の一覧を再取得。
                },
                fail: App.Utils.HandleServerError
            });
        },

        onCosInfoNew: function (e) {
            if (e) e.preventDefault();
            //log('onAddressInfoNew');
            var c_num = viewModel.get("Customer.c_num");

            var info = new CosInfo({ c_num: c_num, pax_type: "G" });
            var finalized = !!viewModel.get("Customer.final_date");

            cosInfoViewModel.openWindow(info, finalized, function () {
                viewModel.getCosInfos(c_num);
            });
        },

        onCosInfoEdit: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = grid.dataItem(tr);

            //log("Selected address info: ", selected);
            var info = new CosInfo(selected);
            var finalized = !!viewModel.get("Customer.final_date");

            //CosInfo詳細画面を開く。
            cosInfoViewModel.openWindow(info, finalized, function () {
                //詳細の保存後は一覧を再取得する。
                viewModel.getCosInfos(c_num);
            });
        },

        onCosInfoDelete: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = grid.dataItem(tr);

            if (!confirm("Are you sure to DELETE this costume?")) return;

            var info = new CosInfo(selected);
            info.remove({
                success: function (result) {
                    viewModel.getCosInfos(c_num);   //Costumeの一覧を再取得。
                },
                fail: App.Utils.HandleServerError
            });
        },

        onOrderSheetClick: function (e) {
            if (e) e.preventDefault();
            var c_num = viewModel.get("Customer.c_num");
            if (!c_num) {
                alert("No customer data.");
                return;
            }
            var url = $(e.target).data("contenturl");
            url += "?c_num=" + c_num;
            window.open(url, "_blank");
        },

        onNoteClick: function (e) {
            if (e) e.preventDefault();

            var note = viewModel.get("Customer.note");
            var customer = viewModel.get("Customer");

            //Note詳細画面を開く。
            noteViewModel.openWindow(note, customer, function () {
                viewModel.getCustomer(c_num);
            });
        },

        onPlanItemEdit: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr")
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = <any>grid.dataItem(tr);

            //選択された行から詳細ダイアログのViewModelに商品名などをコピー。
            optionDetailViewModel.set("item_name", selected.item_name);
            optionDetailViewModel.set("item_name_jpn", selected.item_name_jpn);
            optionDetailViewModel.set("info_type", selected.info_type);

            //選択された行(SalesListItem)からSalesモデルにデータをコピー。
            var sales = $.extend(new Sales(), selected);
            var c_num = viewModel.get("Customer.c_num");
            var finalized = !!viewModel.get("Customer.final_date");

            //アイテム詳細画面を開く。
            optionDetailViewModel.openWindow(sales, finalized, function () {
                //アイテムの保存後はプラン一覧を再取得する。
                viewModel.getPlans(c_num);
            });
        },

        onPlanItemsDataBound: function (e) {
            var grid = e.sender;
            if (!grid) return;

            if (grid.dataSource.total() == 0) return;
            var item = grid.dataItem("tbody tr:eq(0)");
            if (!item) return;

            setTimeout(function () {
                //新規またはエージェントユーザーの場合はEditボタンの列を非表示に。
                if (!item.get("op_seq") || App.User.IsAgent) {
                    //grid.hideColumn(grid.columns.length - 1);
                    grid.showColumn('op_seq'); //一旦showしないとhide出来ずに列がずれる。
                    grid.hideColumn('op_seq');
                } else {
                    //grid.showColumn(grid.columns.length - 1);
                    grid.hideColumn('op_seq'); //一旦hideしないとshow出来ずに列がずれる。
                    grid.showColumn('op_seq');
                }
            }, 200);
        },

        addWeddingPlan: function (e) {
            if (e) e.preventDefault();

            var plans = viewModel.get("WeddingPlans");
            var p = new WeddingPlan();
            p.set("StatusList", [{ value: "Q", text: "RQ" }]);
            p.set("book_status", "Q");      //挙式プランはデフォルトでRQステータス。
            p.set("text", p.getText(plans.length, plans.length + 1));

            //if (!App.User.StaffRequired) {
            //    p.set("staff", App.User.UserName);
            //}

            viewModel.openPlanSearchDialog("W", p);
        },

        removeWeddingPlan: function (e) {
            if (e) e.preventDefault();
            var plans = viewModel.get("WeddingPlans");
            if (!plans) return;

            for (var i = plans.length - 1; i >= 0; i--) {
                if (!plans[i].op_seq) {
                    plans.splice(i, 1);              //i番目の要素を削除。
                }
            }
            viewModel.set("WeddingPlans", plans);

            var cur = null;
            if (plans.length > 0) cur = plans[0];
            setTimeout(function () {
                viewModel.set("CurrentWeddingPlan", cur);
            }, 200);
        },

        //挙式・フォトプラン選択ダイアログを開く
        openPlanSearchDialog: function (plan_type, plan) {
            plan.set("sub_agent_cd", App.ViewModel.get("Customer.sub_agent_cd"));
            plan.set("c_num", App.ViewModel.get("Customer.c_num"))
            plan.set("req_wed_date", App.ViewModel.get("Customer.wed_date"))
            plan.set("req_church_cd", App.ViewModel.get("Customer.church_cd"))

            var win = $("#PlanSearchDialog"),
                wd = null,
                sub_agent_cd = null;

            var kendoWindow = win.data("kendoWindow");
            kendoWindow.setOptions({
                title: plan_type === "W" ? "Wedding Plans" : "Photo Plans",
                width: "70%",
                minWidth: 360,
                minHeight: 600,
            });

            planSearchViewModel.clearSearch(plan_type, plan);
            kendoWindow.center().open();
        },

        getSelectedOption: function(){
            var grid = $("#grdOptions").data("kendoGrid");
            var row = grid.select();
            if (!row) return null;
            var op = grid.dataItem(row);
            if (!op) return null;
            return op;
        },

        onOptionGridChange: function (e) {
            var op = viewModel.getSelectedOption();
            viewModel.set("CurrentOption", op);
        },

        addOption: function (e) {
            if (e) e.preventDefault();

            var sales = new Sales();
            var customer = viewModel.get("Customer");
            sales.set("c_num", customer.c_num);
            sales.set("agent_cd", customer.agent_cd);
            sales.set("sub_agent_cd", customer.sub_agent_cd);
            sales.set("quantity", 1);
            sales.set("price", "");
            sales.set("book_status", "K");
            sales.set("item_type", "");
            sales.set("sales_post_date", customer.get("wed_date"));

            optionDetailViewModel.set("item_name", null);
            optionDetailViewModel.set("item_name_jpn", null);
            optionDetailViewModel.set("info_type", null);

            var finalized = !!viewModel.get("Customer.final_date");

            //オプション詳細画面を開く。
            optionDetailViewModel.openWindow(sales, finalized, function () {
                viewModel.getOptions(viewModel.get("Customer.c_num"));
            });
        },

        editOption: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr")
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = <any>grid.dataItem(tr);

            //選択された行から詳細ダイアログのViewModelに商品名などをコピー。
            optionDetailViewModel.set("item_name", selected.item_name);
            optionDetailViewModel.set("item_name_jpn", selected.item_name_jpn);
            optionDetailViewModel.set("info_type", selected.info_type);

            //選択された行(SalesListItem)からSalesモデルにデータをコピー。
            var sales = $.extend(new Sales(), selected);
            var finalized = !!viewModel.get("Customer.final_date");

            //オプション詳細画面を開く。
            optionDetailViewModel.openWindow(sales, finalized, function () {
                viewModel.getOptions(viewModel.get("Customer.c_num"));
            });
        },

        onChurchOrderClick: function (e) {
            if (e) e.preventDefault();

            var plan = viewModel.get("CurrentWeddingPlan");
            if (!plan ) return false;

            var url = $(e.target).data("contenturl");
            url += plan.info_id + "?c_num=" + plan.c_num
            window.open(url);
        },

        onOptionsDataBound: function (e) {
            viewModel.showCxlOptionChanged();
        },

        showCxlOptionChanged: function (e) {
            if (e) e.preventDefault();
            var show = viewModel.get("showCxlOption");
            var grid = $("#grdOptions").data("kendoGrid");
            if (!grid.dataSource) return;
            if (grid.dataSource.total() == 0) return;
            var len = grid.dataSource.total();
            for (var i = 0; i < len; i++) {
                var row = grid.tbody.find("tr:eq(" + i.toString() + ")");
                var data = <any>grid.dataItem(row);
                if (data.book_status === "C") {
                    if (show) {
                        row.fadeIn();
                    } else {
                        row.fadeOut();
                    }
                }
            }
        },

        orderSheetDisabled: function () {
            var cnt = 0;
            var plans = viewModel.get("CurrentPhotoPlan.PlanItems");
            if (plans) {
                for (var i = 0; i < plans.length; i++) {
                    var p = plans[i];
                    if (p.item_type == "COS" && (p.book_status == "K" || p.book_status == "Q")) {
                        cnt++;
                    }
                }
                if (cnt > 0) return false;
            }
            var wed_plans = viewModel.get("CurrentWeddingPlan.PlanItems");
            if (wed_plans) {
                for (var i = 0; i < wed_plans.length; i++) {
                    var p = wed_plans[i];
                    if (p.item_type == "COS" && (p.book_status == "K" || p.book_status == "Q")) {
                        cnt++;
                    }
                }
                if (cnt > 0) return false;
            }
            var options = viewModel.get("Options");
            if (options) {
                for (var i = 0; i < options.length; i++) {
                    var p = options[i];
                    if (p.item_type == "COS" && (p.book_status == "K" || p.book_status == "Q")) {
                        cnt++;
                    }
                }
                if (cnt > 0) return false;
            }
            return true;
        },

        onFileClick: function (e) {
            if (e) e.preventDefault();
            var c_num = this.get("Customer.c_num");
            if (c_num) {
                customerFileViewModel.openWindow(c_num);
            }
        }
    });

    planSearchViewModel.bind('selectPlan', function (e) {
        var plan_type = e.plan_type;
        var plan = e.plan;
        if (!plan_type || !plan) return;

        var plans;
        if (plan_type == 'P') {
            plans = viewModel.get("PhotoPlans");
        } else {
            plans = viewModel.get("WeddingPlans");
        }
        plans.unshift(plan);            //配列の先頭に追加。

        //予約ステータスを決定する。
        plan.prepareBookStatus(plan_type);

        if (plan_type == 'P') {
            viewModel.set("CurrentPhotoPlan", plan);
            viewModel.set("PhotoPlans_dirty", true);
        } else {
            viewModel.set("CurrentWeddingPlan", plan);
            viewModel.set("WeddingPlans_dirty", true);
        }

        //プラン内の子アイテムの一覧を取得してグリッドに表示。
        App.Utils.ShowLoading();
        plan.getChildren({
            success: function (list) {
                for (var i = 0; i < list.length; i++) {
                    var sales = list[i];
                    sales.createInfoIfNull(sales.info_type, plan.get("req_wed_date"));
                    //log("PlanItem[" + i + "]: ", sales);
                }
                App.Utils.HideLoading();
            },
            fail: App.Utils.HandleServerError
        });

    });

    viewModel.bind('change', function (e) {
        //log('viewModel.change: %s = %O', e.field, viewModel.get(e.field));

        //フィールド名が「Customer.」で始まる場合はdirtyフラグを立てる。
        if (viewModel.all_loaded() && e.field.indexOf("Customer.") == 0 && e.field.indexOf("Customer.AddressInfo") < 0) {
            viewModel.set("dirty", true);
        }
        if (viewModel.all_loaded() && e.field.indexOf("Customer.") == 0 && e.field.indexOf("Customer.CosInfo") < 0) {
            viewModel.set("dirty", true);
        }

        if (viewModel.all_loaded() && e.field.indexOf("CurrentPhotoPlan.") == 0 && e.field.indexOf(".PlanItems") < 0) {
            viewModel.set("PhotoPlans_dirty", true);
        }
        if (viewModel.all_loaded() && e.field.indexOf("CurrentWeddingPlan.") == 0 && e.field.indexOf(".PlanItems") < 0) {
            viewModel.set("WeddingPlans_dirty", true);
        }

    });



    //全体のViewModelをバインド。
    kendo.bind($("#app"), viewModel);
    App.ViewModel = viewModel;

    //ダイアログ部分のViewModelをバインド。(念のため時間差を付けて確実にKendoUIの初期化後に実行される様にしている。)
    setTimeout(function () {
        kendo.bind($("#PlanSearchDialog"), planSearchViewModel);
        kendo.bind($("#OptionDetailDialog"), optionDetailViewModel);
        kendo.bind($("#AddressInfoDialog"), addressInfoViewModel);
        kendo.bind($("#CosInfoDialog"), cosInfoViewModel);
        kendo.bind($("#NoteDialog"), noteViewModel);
        kendo.bind($("#DupListDialog"), dupListViewModel);

        var combo = $("#OptionDetailDialog").find("#txtItemCd").data("kendoComboBox");
        if (combo) combo.list.width(680);

        var grid = $("#PlanSearchDialog").find("#grdPlanSearch").data("kendoGrid");
        grid.bind("dataBinding", planSearchViewModel.gridDataBinding);

    }, 300);

    //File情報が変わったらFile数を取得
    customerFileViewModel.bind("changed", function () {
        viewModel.getFiles(c_num);
    });

    //店舗担当者名の必須制御。
    App.Utils.setBoolAttr(".branch_staff", "required", App.User.BranchStaffRequired);

    ////Kendoのバリデータを初期化。
    //$("#frmCustomer").kendoValidator();

    //カスタマー情報の取得を開始。
    var c_num = $("#hdnCNum").val();
    viewModel.getCustomer(c_num);

    $("#Customer_note").bind("click", function () {
        viewModel.onNoteClick();
    });



});

