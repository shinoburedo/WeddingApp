require(['models/customerlistitem', 'persist-local', '../../Areas/Masters/Scripts/Models/Item'],
  function (CustomerListItem, $PL, Item) {

      const defaultPageSize = 50;

    //*-------------------------------- viewModel　--------------------------------*//
    var viewModel = <any>kendo.observable({

        c_num: "",
        cust_name: "",
        date_from: moment().add(0, "months").toDate(),
        date_to: moment().add(12, "months").toDate(),
        area_cd: "",
        agent_cd: "",
        sub_agent_cd: App.User.IsAgent ? App.User.SubAgentCd : "",
        church_cd: "",
        date_type: "W",
        item_type: "",
        item_cd: "",
        book_status: "",
        action: "",
        include_archived: false,
        not_finalized_only: false,

        itemList: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("items/Search"),
                    type: "POST",
                    dataType: "json",
                    serverFiltering: true,
                    data: function () {
                        return {
                            item_type: viewModel.get("item_type") || ""
                        };
                    }
                }
            },
            schema: {
                model: Item
            },
        }),

        changeItemType: function () {
            viewModel.set("item_cd", "");
            viewModel.itemList.read();
            viewModel.DoSearchChanges();
            viewModel.DoSearchOrders();
        },

        listChanges: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Customers/Search"),
                    type: "POST",
                    dataType: "json",
                    data: function () { return viewModel.getSearchParams(0); }
                },
            },
            serverPaging: true,
            pageSize: defaultPageSize,
            schema: {
                model: CustomerListItem,
                data: "data",
                total: "total" // total is returned in the "total" field of the response
            },
            error: App.Utils.HandleServerError
        }),
        countChanges: function () {
            return this.get("listChanges").total();
        },

        listOrders: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Customers/Search"),
                    type: "POST",
                    dataType: "json",
                    data: function () { return viewModel.getSearchParams(1); }
                },
            },
            serverPaging: true,
            pageSize: defaultPageSize,
            schema: {
                model: CustomerListItem,
                data: "data",
                total: "total" // total is returned in the "total" field of the response
            },
            error: App.Utils.HandleServerError
        }),
        countOrders: function () {
            return this.get("listOrders").total();
        },

        listCustomers: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Customers/Search"),
                    type: "POST",
                    dataType: "json",
                    data: function () { return viewModel.getSearchParams(2); }
                },
            },
            serverPaging: true,
            pageSize: defaultPageSize,
            schema: {
                model: CustomerListItem,
                data: "data",
                total: "total" // total is returned in the "total" field of the response
            },
            error: App.Utils.HandleServerError
        }),
        countCustomers: function () {
            return this.get("listCustomers").total();
        },

        listDuplicateCheck: new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Customers/Search"),
                    type: "POST",
                    dataType: "json",
                    data: function () { return viewModel.getSearchParams(3); }
                },
            },
            serverPaging: true,
            pageSize: defaultPageSize,
            schema: {
                model: CustomerListItem,
                data: "data",
                total: "total" // total is returned in the "total" field of the response
            },
            error: App.Utils.HandleServerError
        }),
        countDuplicateCheck: function () {
            return this.get("listDuplicateCheck").total();
        },

        isSubAgentVisible: function () {
            return App.User.HasChildAgents;
        },

        isDatesEnabled: function () {
            return !(viewModel.get("c_num")) && !(viewModel.get("cust_name"));   //C#またはCust Nameが入力されたら日付の条件は無効。
        },

        getSearchParams: function (tab) {
            var srchTypes = ["change", "order", "customer", "duplicate"];
            return {
                tab: tab,
                srch_type: srchTypes[tab],
                c_num: viewModel.get("c_num"),
                cust_name: viewModel.get("cust_name"),
                date_from: kendo.toString(viewModel.get("date_from"), "yyyy/MM/dd"),
                date_to: kendo.toString(viewModel.get("date_to"), "yyyy/MM/dd"),
                area_cd: viewModel.get("area_cd"),
                agent_cd: viewModel.get("agent_cd"),
                sub_agent_cd: viewModel.get("sub_agent_cd"),
                church_cd: viewModel.get("church_cd"),
                date_type: viewModel.get("date_type"),
                item_type: viewModel.get("item_type"),
                item_cd: viewModel.get("item_cd"),
                book_status: viewModel.get("book_status"),
                action: viewModel.get("action"),
                include_archived: viewModel.get("include_archived"),
                include_dup_check_done: viewModel.get("include_dup_check_done"),
                not_finalized_only: viewModel.get("not_finalized_only")
        };
        },

        activateTab: function (tab) {
            $("ul.nav-tabs-plans li, div.tab-pane").removeClass("active");

            if (!tab || tab === 0) {
                $("#tabChanges, #panChanges").addClass("active");
            } else if (tab === 1) {
                $("#tabOrders, #panOrders").addClass("active");
            } else if (tab === 2) {
                $("#tabCustomers, #panCustomers").addClass("active");
            } else {
                $("#tabDuplicateCheck, #panDuplicateCheck").addClass("active");
            }
        },

        DoSearchChanges: function () {
            var data = viewModel.getSearchParams(0);
            viewModel.listChanges.read();
            $PL.saveObject(data, App.User.UserId);
        },

        DoSearchOrders: function () {
            var data = viewModel.getSearchParams(1);
            viewModel.listOrders.read();
            $PL.saveObject(data, App.User.UserId);
        },

        DoSearchCustomers: function () {
            var data = viewModel.getSearchParams(2);
            viewModel.listCustomers.read();
            $PL.saveObject(data, App.User.UserId);

        },

        DoSearchDuplicateCheck: function () {
            var data = viewModel.getSearchParams(3);
            viewModel.listDuplicateCheck.read();
            $PL.saveObject(data, App.User.UserId);
        },

        onChangeArchived: function (e) {
            //log("onChangeArchived", e);
            if (e) e.preventDefault();
            //var input = $(e.target);
        },

        onChangeDupCheckDone: function (e) {
            //log("onChangeArchived", e);
            if (e) e.preventDefault();
            //var input = $(e.target);
        },

        onAllCheckOn: function(e){
            //log("onAllCheckOn", e);
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }
            viewModel.onAllCheckChange(true);
        },
        onAllCheckOff: function (e) {
            //log("onAllCheckOff", e);
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }

            viewModel.onAllCheckChange(false);
        },

        onAllCheckChange: function (is_on) {
            var dataSource = viewModel.get("listChanges");
            var len = dataSource.data().length;
            if (len > 0) {
                App.Utils.ShowLoading(true);
                viewModel.onAllCheckChangeSub(0, is_on);
            } 
        },

        onAllCheckChangeSub: function (i, is_on) {
            var dataSource = viewModel.get("listChanges");
            var len = dataSource.data().length;
            var dr = dataSource.at(i);
            if (dr.archived !== is_on) {
                dr.set("archived", is_on);
            }

            if (i < len - 1) {
                //データの更新をDOMに反映するタイミングを与えるためにsetTimeoutを使って再帰呼び出し。
                setTimeout(function () {
                    viewModel.onAllCheckChangeSub(i + 1, is_on);
                }, 1);
            } else {
                App.Utils.HideLoading(true);
            }
        },

        onAllDoneCheckOn: function (e) {
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }
            viewModel.onAllDoneCheckChange(true);
        },
        onAllDoneCheckOff: function (e) {
            if (e) {
                e.preventDefault();
                e.stopPropagation();
            }

            viewModel.onAllDoneCheckChange(false);
        },

        onAllDoneCheckChange: function (is_on) {
            var dataSource = viewModel.get("listDuplicateCheck");
            var len = dataSource.data().length;
            if (len > 0) {
                App.Utils.ShowLoading(true);
                viewModel.onAllDoneCheckChangeSub(0, is_on);
            }
        },

        onAllDoneCheckChangeSub: function (i, is_on) {
            var dataSource = viewModel.get("listDuplicateCheck");
            var len = dataSource.data().length;
            var dr = dataSource.at(i);
            if (dr.dup_check_done !== is_on) {
                dr.set("dup_check_done", is_on);
            }

            if (i < len - 1) {
                //データの更新をDOMに反映するタイミングを与えるためにsetTimeoutを使って再帰呼び出し。
                setTimeout(function () {
                    viewModel.onAllDoneCheckChangeSub(i + 1, is_on);
                }, 1);
            } else {
                App.Utils.HideLoading(true);
            }
        },

        onSaveArchived: function (e) {
            //log("onSaveArchived", e);
            if (e) e.preventDefault();

            var data = { ids_off: [], ids_on: [] };
            var dataSource = viewModel.get("listChanges");
            var len = dataSource.data().length;
            for (var i = 0; i < len; i++) {
                var dr = dataSource.at(i);
                if (dr.dirty) {         // 変更された行のみ配列に格納。
                    var id = dr.get("log_id");
                    if (dr.archived){
                        data.ids_on.push(id);
                    } else {
                        data.ids_off.push(id);
                    }
                }
            }
            if (data.ids_off.length == 0 && data.ids_on.length == 0) {
                alert("No row has been changed.");
                return;
            }
            //log("data: ", data);

            App.Utils.ShowLoading(true);
            $.ajax({
                url: App.getApiPath("LogChanges/SaveArchived"),
                data: JSON.stringify(data),
                type: "POST",
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (result) {
                if (result == "ok") {
                    //App.Utils.ShowAlert("Data updated.");
                    App.Utils.HideLoading(true);
                    viewModel.DoSearchChanges();
                 } else {
                    App.Utils.ShowAlert(result, true);
                 }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                App.Utils.ShowAlert(errorThrown, true);
            });
        },

        onSaveChkDone: function (e) {
            //log("onSaveChkDone", e);
            if (e) e.preventDefault();

            var data = { c_num: [], dup_check_done: [] };
            var dataSource = viewModel.get("listDuplicateCheck");
            var len = dataSource.data().length;
            for (var i = 0; i < len; i++) {
                var dr = dataSource.at(i);
                if (dr.dirty) {         // 変更された行のみ配列に格納。
                    data.c_num.push(dr.get("c_num"));
                    data.dup_check_done.push(dr.get("dup_check_done"));
                }
            }
            if (data.c_num.length == 0) {
                alert("No row has been changed.");
                return;
            }

            App.Utils.ShowLoading(true);
            $.ajax({
                url: App.getApiPath("customers/saveDupChkDone"),
                data: JSON.stringify(data),
                type: "POST",
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (result) {
                if (result == "ok") {
                    App.Utils.HideLoading(true);
                    viewModel.DoSearchDuplicateCheck();
                } else {
                    App.Utils.ShowAlert(result, true);
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                App.Utils.ShowAlert(errorThrown, true);
            });
        }

    });
    //*-------------------------------- End of viewModel　--------------------------------*//


    //検索条件を復元。
    var obj = $PL.loadObject(App.User.UserId);
    if (obj) {
        _.each(obj, function (value, key: string) {
            if (key === "date_from" || key === "date_to") {
                value = kendo.parseDate(value);
            }
            viewModel.set(key, value);
        });
        viewModel.activateTab(viewModel.tab);
    }

    //タブの切り替え時に検索を実行。
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var href = $(e.target).attr("href");
        if (href === '#panChanges') {
            viewModel.DoSearchChanges();
        } else if (href === '#panOrders') {
            viewModel.DoSearchOrders();
        } else if (href === '#panCustomers') {
            viewModel.DoSearchCustomers();
        } else {
            viewModel.DoSearchDuplicateCheck();
        }
    })

    //ViewModelをDOMにバインド。
    kendo.bind($("#app"), viewModel);

    //グリッドのheaderTemplate内では何故かdata-bind属性が効かない様なので代わりにここでイベントハンドラをセット。
    $(".btn_check_on").on("click", function (e) {
        viewModel.onAllCheckOn(e);
    });
    //グリッドのheaderTemplate内では何故かdata-bind属性が効かない様なので代わりにここでイベントハンドラをセット。
    $(".btn_check_off").on("click", function (e) {
        viewModel.onAllCheckOff(e);
    });

    $(".btn_done_check_on").on("click", function (e) {
        viewModel.onAllDoneCheckOn(e);
    });
    $(".btn_done_check_off").on("click", function (e) {
        viewModel.onAllDoneCheckOff(e);
    });


    //Item Cdのドロップダウンリストの幅をセット。
    $("select[name=itemCd]").each(function (index, el) {
        var combo = $(el).data("kendoComboBox");
        if (combo) combo.list.width(680);
    });

  });



