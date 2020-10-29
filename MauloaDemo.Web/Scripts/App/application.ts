interface IApp {
    Config: {
        CultureName: string;
        Lang: string;
        DateFormat: string;
        TimeFormat: string;
        TimeZone: string;
        ApiPath: string;
        BaseUrl: string;
        SunsetBlockTime: string;
        MinWedDate: Date;
        Photo_RQ_LimitDays: number;
        Photo_RQ_Places: string;
        IsTest: boolean;
    };
    User: {
        UserId: string;
        UserName: string;
        AreaCd: string;
        AgentCd: string;
        SubAgentCd: string;
        IsAgent: boolean;
        IsStaff: boolean;
        AccessLevel: number;
        HasChildAgents: boolean;
        StaffRequired: boolean;
        BranchStaffRequired: boolean;
    };
    Utils: {
        KendoComboBoxChangeFix(kendoCombo: kendo.ui.ComboBox, input_element: EventTarget): void;
        ShowOverlay(id?: string, zorder?: number): void;
        HideOverlay(id?: string): void;
        ShowLoading(withOverlay?: boolean): void;
        HideLoading(withOverlay?: boolean): void;
        ShowAlert(msg: string, isError?: boolean, title?: string): void;
        ShowAlertAjaxErr(jqXHR: JQueryXHR, textStatus: string, errorThrown: string): void;
        HandleServerError(e: any): void;
        AjaxError(event: JQueryEventObject, jqXHR: JQueryXHR, ajaxSettings: JQueryAjaxSettings, thrownError: any): any;
        setAutoComplete(obj: any, url: string): void;
        kendoComboBoxClose(comboId: string): void;
        convertToDateTime(value: string): Date;
        fillZero(n: number, width: number): string;
        setBoolAttr(selector: string, attr_name: string, boolean_value: boolean): void;
        getBoolAttr(selector: string, attr_name: string): boolean;
        toggleBoolAttr(selector: string, attr_name: string): void;
        getUTCDateStr(d?: Date): string;
        getRegionNow(): Date;
        getRegionDate(): Date;
    };

    data: any;
    getApiPath(target: string): string;
    isJPN(): boolean;
    L(japanese: string, english: string): string;
    initDataSources(): void;
    initKendo(parent?: any): void;

    ViewModel: any;
    DupListViewModel: any;
    optionDetailViewModel: any;
}
declare var App: IApp;

(function () {

    var jpn_suffix = (App.Config.Lang == 'ja' ? "_jpn" : "");
    var eng_suffix = (App.Config.Lang == 'ja' ? "" : "_eng");

    App.data = kendo.observable({});

    App.getApiPath = function (target) {
        return App.Config.ApiPath + target;
    };

    App.isJPN = function () {
        return App.Config.Lang == "ja";
    };

    App.L = function (japanese, english) {
        return App.isJPN() ? japanese : english;
    };

    App.initDataSources = function () {
        //Area一覧のデータソース
        App.data.set("areaList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Areas"),
                        dataType: "json"
                    }
                }
            })
        );

        //Vendor一覧のデータソース (可変のパラメータ(item_cd)を渡せる様に関数として定義)
        App.data.set("vendorList", function (data) {
            return new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Vendors/Search"),
                        type: "POST",
                        dataType: "json",
                        data: data
                    }
                }
            });
        });

        //Agent一覧のデータソース
        App.data.set("agentList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Agents/Search"),
                        type: "POST",
                        dataType: "json"
                    }
                }
            })
        );

        //Sub Agent一覧のデータソース
        App.data.set("subAgentList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("SubAgents/GetChildList"),
                        dataType: "json"
                    }
                }
            })
        );

        //Inv Agent一覧のデータソース
        App.data.set("invAgentList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("SubAgents/GetInvAgentList"),
                        dataType: "json"
                    }
                }
            })
        );

        //Hotel一覧のデータソース
        App.data.set("hotelList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Hotels/Search"),
                        type: "POST",
                        dataType: "json"
                    }
                }
            })
        );

        //Church一覧のデータソース
        App.data.set("churchList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Churches/GetList"),
                        dataType: "json"
                    }
                }
            })
        );

        //Photoプラン用Church一覧のデータソース
        App.data.set("churchListForPhotoPlan",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Churches/ForPhotoPlan"),
                        dataType: "json"
                    }
                }
            })
        );

        //Weddingプラン用Church一覧のデータソース
        App.data.set("churchListForWeddingPlan",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Churches/ForWeddingPlan"),
                        dataType: "json"
                    }
                }
            })
        );

        //ItemType一覧のデータソース(全件取得)
        App.data.set("itemTypeList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("ItemTypes/search"),
                        type: "POST",
                        dataType: "json"
                    }
                }
            })
        );

        //ItemType一覧のデータソース(Optionのみ)
        App.data.set("optionItemTypeList",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("ItemTypes/search"),
                        type:"POST",
                        dataType: "json",
                        data: {plan_type: "O", has_items: true}
                    }
                }
            })
        );

        //ItemType一覧のデータソース(空白行付き、かつ'*PK', '*OP'付き。Search-CHANGESタブで使用)
        App.data.set("itemTypeListWithBlankPkgOpt",
            new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("ItemTypes/search"),
                        type: "POST",
                        dataType: "json",
                        data: { add_blank: true, add_pkg_opt: true, has_items: true }
                    }
                }
            })
            );

        //PickUp Place一覧のデータソース (可変のパラメータ(hotel_cd)を渡せる様に関数として定義)
        App.data.set("getPickupPlaceList", function (data) {
            return new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("PickupPlaces/search"),
                        type: "POST",
                        dataType: "json",
                        data: data
                    }
                }
            });
        });

    };

    App.initKendo = function (parent) {
        if (!parent) parent = $(document);

        //Area一覧コンボボックス
        parent.find(".select-area").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "area_cd",
                dataValueField: "area_cd",
                height: 200,
                template: "<dl><dt>${ data.area_cd }</dt><dd>${ data.desc" + jpn_suffix + eng_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.areaList);
                combo.list.width(160);
            }
        });

        //Vendor一覧コンボボックス
        parent.find(".select-vendor").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "vendor_cd",
                dataValueField: "vendor_cd",
                height: 500,
                template: "<dl><dt style='width:90px;'>${ data.vendor_cd }</dt><dd>${ data.vendor_name }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.vendorList());
                combo.list.width(440);
            }
        });


        //Agent一覧コンボボックス
        parent.find(".select-agent").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "agent_cd",
                dataValueField: "agent_cd",
                height: 500,
                template: "<dl><dt>${ data.agent_cd }</dt><dd>${ data.agent_name" + jpn_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.agentList);
                combo.list.width(300);
            }
        });

        //Sub Agent一覧コンボボックス
        parent.find(".select-subagent").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "child_cd",
                dataValueField: "child_cd",
                height: 500,
                template: "<dl><dt>${ data.child_cd }</dt><dd>${ data.agent_area_cd ? ('(' + data.agent_area_cd + ') ') : ''}${ data.company_name" + eng_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.subAgentList);
                combo.list.width(440);
            }
        });

        //Inv Agent一覧コンボボックス
        parent.find(".select-invagent").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "child_cd",
                dataValueField: "child_cd",
                height: 500,
                template: "<dl><dt>${ data.child_cd }</dt><dd>${ data.company_name" + eng_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.invAgentList);
                combo.list.width(300);
            }
        });


        //Hotel一覧コンボボックス
        parent.find(".select-hotel").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "hotel_cd",
                dataValueField: "hotel_cd",
                height: 400,
                template: "<dl><dt style='width:40px;'>${ data.hotel_cd }</dt><dd>${ data.hotel_name" + jpn_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.hotelList);
                combo.list.width(400);
            }
        });


        //Church一覧コンボボックス
        parent.find(".select-church").each(function () {
            if ($(this).data("kendoComboBox")) {
                return true;
            }

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "church_cd",
                dataValueField: "church_cd",
                height: 300,
                template: "<dl><dt style='width:60px;'>${ data.church_cd }</dt>"
                + "<dd>${ data.church_name" + jpn_suffix + " }</dd>"
                + "</dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.churchList);
                combo.list.width(320);
            }
        });

        //ItemType一覧コンボボックス
        parent.find(".select-itemtype").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            var plantype = $(this).data("plantype");
            var addpkgopt = $(this).data("addpkgopt");

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "item_type",
                dataValueField: "item_type",
                height: 500,
                template: "<dl><dt style='width:40px;'>${ data.item_type }</dt><dd>${ data.desc" + jpn_suffix + eng_suffix + " }</dd></dl>"
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                if (plantype === "O") {
                    combo.setDataSource(App.data.optionItemTypeList);
                } else if (addpkgopt == true) {
                    combo.setDataSource(App.data.itemTypeListWithBlankPkgOpt);
                } else {
                    combo.setDataSource(App.data.itemTypeList);
                }
                combo.list.width(220);
            }
        });

        //PickUp Place一覧コンボボックス
        parent.find(".select-pickupplace").each(function () {
            if ($(this).data("kendoComboBox")) return true;

            $(this).kendoComboBox({
                autoBind: true,
                valuePrimitive: true,
                dataTextField: "place_name",
                dataValueField: "place_name",
                height: 400
            });

            var combo = $(this).data("kendoComboBox");
            if (combo) {
                combo.setDataSource(App.data.getPickupPlaceList());
                combo.list.width(220);
            }
        });




        //全てのDatePickerの日付書式をセット
        parent.find("input[data-role='datepicker']").each(function () {
            var dtp = $(this).data("kendoDatePicker");
            if (dtp) {
                dtp.options.format = App.Config.DateFormat;
            }
        });

    };

})();

$(function () {
    App.initDataSources();
    App.initKendo();
});



