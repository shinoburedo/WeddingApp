define(['models/itemwithprice'
        , 'models/sales'
        , 'models/delivery_info'
        , 'models/make_info'
        , 'models/reception_info'
        , 'models/shoot_info'
        , 'models/trans_info'
        , 'models/pickupplace'
        , 'models/arrangement'],
    function (ItemWithPrice
                , Sales
                , DeliveryInfo
                , MakeInfo
                , ReceptionInfo
                , ShootInfo
                , TransInfo
                , PickupPlace
                , Arrangement) {

        var optionDetailViewModel = App.optionDetailViewModel = <any>kendo.observable({
            element: "#OptionDetailDialog",
            sales: { op_seq: -1 },
            org_sales :{ op_seq: -1},

            Arrangements: Arrangement.getDataSource(function () {
                return {
                    op_seq: optionDetailViewModel.get("sales.op_seq")
                };
            }), 

            dirty: false,
            finalized: false,
            saved_callback: null,
            item_name: "",
            item_name_jpn: "",
            info_type: "",

            itemList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("items/searchWithPrice"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                plan_type: "O",
                                item_type: optionDetailViewModel.get("sales.item_type") || "-",
                                c_num: App.ViewModel.get("Customer.c_num"),
                                sub_agent_cd: optionDetailViewModel.get("sales.sub_agent_cd")
                                                || (App.User.IsAgent ? App.User.SubAgentCd : "")
                            };
                        }
                    }
                },
                schema: {
                    model: ItemWithPrice
                },
            }),

            pickupPlaceList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("pickupplaces/search"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                hotel_cd: optionDetailViewModel.get("sales.TransInfo.pickup_hotel") || App.ViewModel.get("Customer.hotel_cd")
                            };
                        }
                    }
                },
                schema: {
                    model: PickupPlace
                },
            }),

            dropoffPlaceList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("pickupplaces/search"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                hotel_cd: optionDetailViewModel.get("sales.TransInfo.dropoff_hotel") || App.ViewModel.get("Customer.hotel_cd")
                            };
                        }
                    }
                },
                schema: {
                    model: PickupPlace
                },
            }),

            openWindow: function (sales, finalized, saved_callback) {
                //前回のValidationメッセージをクリア。
                var validator = $(optionDetailViewModel.element).find("form[data-role='validator']").data("kendoValidator");
                validator.hideMessages();       

                optionDetailViewModel.set("sales", sales);
                optionDetailViewModel.set("org_sales", $.extend({}, sales));     //コピーを保持。
                optionDetailViewModel.set("finalized", finalized);
                optionDetailViewModel.set("dirty", false);
                optionDetailViewModel.set("saved_callback", saved_callback);
                optionDetailViewModel.refreshItemList();
                optionDetailViewModel.refreshArrangements();

                var win = $("#OptionDetailDialog");
                var kendoWindow = win.data("kendoWindow");
                kendoWindow.setOptions({
                    title: sales.op_seq ? (sales.parent_op_seq ? "Item Detail" : "Option Detail") : "New Option",
                    width: "86%",
                    minWidth: 500,
                    minHeight: 576,
                });
                kendoWindow.center().open();

                if (sales.op_seq !== 0) {
                    optionDetailViewModel.loadOption(sales.op_seq);
                }

                var grdArrangement = win.find("#grdArrangement").data("kendoGrid");
                grdArrangement.bind("edit", this.onEditArrangement);
                grdArrangement.bind("save", this.onSaveArrangement);
                grdArrangement.bind("cancel", this.onCancelArrangement);

                //店舗担当者名の必須制御。
                App.Utils.setBoolAttr(this.element + " .branch_staff", "required", App.User.BranchStaffRequired);
            },

            closeDialog: function (e) {
                if (e) e.preventDefault();
                $(".optiondetail-dialog").parent().data("kendoWindow").close();
            },

            loadOption: function (op_seq) {
                //既存レコードの場合はサーバーからデータを取得。
                App.Utils.ShowLoading(true);
                Sales.fetch(op_seq, {
                    success: function (sales_loaded) {
                        App.Utils.HideLoading(true);
                        optionDetailViewModel.set("sales", sales_loaded);
                        optionDetailViewModel.set("org_sales", $.extend({}, sales_loaded));     //コピーを保持。

                        //trans_infoのplaceリスト設定
                        var info_type = sales_loaded.get("info_type");
                        if (info_type === "TRN") {
                            optionDetailViewModel.pickupPlaceList.read();
                            optionDetailViewModel.dropoffPlaceList.read();
                        }
                        optionDetailViewModel.set("dirty", false);
                    },
                    fail: App.Utils.ShowAlertAjaxErr
                });
            },

            onOkClick: function (e) {
                if (e) e.preventDefault();
                if (e) {
                    var form = $(e.target).closest("form").data("kendoValidator")
                    if (!form.validate()) {
                        //var errors = form.errors();
                        //var msg = "";
                        //$(errors).each(function () { msg += (this + '\n\n'); });
                        //App.Utils.ShowAlert(msg, true);
                        return;
                    }
                }

                var sales = optionDetailViewModel.get("sales");
                optionDetailViewModel.saveOption(sales);
            },            

            onPickupHotelChanged: function () {
                this.pickupPlaceList.read();
            },

            onDropoffHotelChanged: function () {
                this.dropoffPlaceList.read();
            },

            onPriceChanged: function () {
                var sales = optionDetailViewModel.get("sales");
                var txt = $(".optiondetail-dialog .div_txt_price input");  //.data("kendoNumericTextBox");
                if (sales.get("price_changed")) {
                    txt.addClass("price_changed");
                    //log("class 'price_changed' added");
                } else {
                    txt.removeClass("price_changed");
                    //log("class 'price_changed' removed");
                }
            },

            isSaveButtonVisible: function () {
                return App.User.AccessLevel >= 3;
            },

            isCanceledOrFinalized: function () {
                var org_sales = optionDetailViewModel.get("org_sales");
                var finalized = optionDetailViewModel.get("finalized");
                return finalized
                    || (org_sales.book_status == "C" || org_sales.book_status == "N")
                    || App.User.AccessLevel < 3;
            },

            isPriceEditable: function () {
                var sales = optionDetailViewModel.get("sales");
                if (!sales) return false;
                if (!sales.isNew) return false;     //isNew()の戻り値ではなくisNew関数の存在をチェックしているので注意。

                var org_sales = optionDetailViewModel.get("org_sales");
                if (org_sales.book_status == "C" || org_sales.book_status == "N") return false;

                //価格変更出来るのはPROMユーザーのみ。かつ新規追加時は不可。
                return App.User.IsStaff && !sales.isNew() && !optionDetailViewModel.get("finalized");
            },

            isPriceChangeReasonEditable: function () {
                var sales = optionDetailViewModel.get("sales");
                if (!sales) return false;
                if (!sales.isNew) return false;     //isNew()の戻り値ではなくisNew関数の存在をチェックしているので注意。

                var org_sales = optionDetailViewModel.get("org_sales");
                if (org_sales.book_status == "C" || org_sales.book_status == "N") return false;

                //価格変更出来るのはPROMユーザーのみ。かつ新規追加時は不可。
                return sales.get("price_changed") && App.User.IsStaff && !sales.isNew() && !optionDetailViewModel.get("finalized");
            },

            isBranchStaffVisible: function () {
                return App.User.IsStaff || App.User.BranchStaffRequired;
            },

            isArrangementVisible: function () {
                return !App.User.IsAgent;
            },

            isJapan: function () {
                return App.Config.Lang == 'ja';
            },

            //isOKEnabled: function () {
            //    return optionDetailViewModel.get("dirty");
            //},

            refreshItemList: function () {
                optionDetailViewModel.itemList.data([]);
                optionDetailViewModel.itemList.read();
            },

            refreshArrangements: function () {
                optionDetailViewModel.Arrangements.data([]);
                optionDetailViewModel.Arrangements.read();
            },

            onItemTypeChanged: function () {
                optionDetailViewModel.refreshItemList();

                var itemType = $("#txtItemType", ".optiondetail-dialog").data("kendoComboBox").dataItem();
                var info_type = itemType ? itemType.info_type : null;
                optionDetailViewModel.set("info_type", info_type);

                var wed_date = App.ViewModel.get("Customer.wed_date");
                optionDetailViewModel.sales.createInfoIfNull(info_type, wed_date);
                //trans_infoのhotel_cdデフォルト設定
                if (info_type === "TRN") {
                    var hotel_cd = App.ViewModel.get("Customer.hotel_cd");
                    optionDetailViewModel.set("sales.TransInfo.pickup_hotel", hotel_cd);
                    optionDetailViewModel.set("sales.TransInfo.dropoff_hotel", hotel_cd);
                    this.pickupPlaceList.read();
                    this.dropoffPlaceList.read();
                }

                optionDetailViewModel.clearItemSelection();
            },

            onSubAgentChanged: function () {
                optionDetailViewModel.clearItemSelection();
                optionDetailViewModel.refreshItemList();
            },

            clearItemSelection: function () {
                optionDetailViewModel.set("sales.item_cd", null);
                optionDetailViewModel.set("sales.item_name", null);
                optionDetailViewModel.set("sales.item_name_jpn", null);
                optionDetailViewModel.set("item_name", null);
                optionDetailViewModel.set("item_name_jpn", null);
                optionDetailViewModel.set("sales.price", "");
            },

            saveOption: function (sales) {
                //log("saveOption", sales);

                if (!sales.get("item_cd")) {
                    App.Utils.ShowAlert("Please select an item code.", true);
                    return;
                }
                if (!sales.get("sub_agent_cd")) {
                    App.Utils.ShowAlert("Please select an agent.", true);
                    return;
                }

                App.Utils.ShowLoading(true);

                var arrangements = optionDetailViewModel.get("Arrangements");
                if (arrangements) {
                    var arrs = arrangements.data().toJSON();
                    sales.set("Arrangements", arrs);
                }

                sales.save({
                    success: function (result) {
                        if (result && result.op_seq > 0 && result.message === "ok") {
                            App.Utils.HideLoading(true);
                            App.Utils.ShowAlert("Data saved successfully.", false);
                            optionDetailViewModel.set("sales.op_seq", result.op_seq);
                            optionDetailViewModel.loadOption(result.op_seq);
                            optionDetailViewModel.refreshArrangements();

                            var callback = optionDetailViewModel.get("saved_callback");
                            if (callback) callback();
                        } else {
                            App.Utils.ShowAlert(result.message, true);
                        }
                    },
                    fail: App.Utils.ShowAlertAjaxErr
                });
            },

            visibleDLV: function () { return this.get("info_type") == "DLV"; },
            visibleMKS: function () { return this.get("info_type") == "MKS"; },
            visibleRCP: function () { return this.get("info_type") == "RCP"; },
            visibleSHT: function () { return this.get("info_type") == "SHO" || this.get("info_type") == "SHT"; },
            visibleTRN: function () { return this.get("info_type") == "TRN"; },
            visibleOthers: function () {
                //return !this.get("info_type") || "DLV|MKS|RCP|TRN|SHO|SHT".indexOf(this.get("info_type")) < 0;
                var dummy = this.get("info_type");
                return false;
            },

            onEditArrangement: function (e) {
                //log("onEditArrangement", e);
            },
            onSaveArrangement: function (e) {
                //log("onSaveArrangement", e);
            },
            onCancelArrangement: function(e) {
                //log("onCancelArrangement", e);
            },
            onAddArrangement: function(e) {
                //log("onAddArrangement", e);
                var grid = $("#OptionDetailDialog").find("#grdArrangement").data("kendoGrid");
                grid.addRow();
            },

            getSelectedArrangement: function () {
                var sales = optionDetailViewModel.get("sales");
                if (!sales) return null;

                var grid = $("#OptionDetailDialog").find("#grdArrangement").data("kendoGrid");
                var arrangement = grid.dataItem(grid.select());
                if (arrangement) {
                    return arrangement;
                }

                var ds = optionDetailViewModel.get("Arrangements");
                return ds.total() === 1 ? ds.at(0) : null;
            },

            onOrderSheetClick: function (e) {
                if (e) e.preventDefault();

                var arrangement = optionDetailViewModel.getSelectedArrangement();
                if (!arrangement) {
                    alert("Please select a vendor.");
                    return;
                }

                var arr_id = arrangement.get("arrangement_id");
                if (!arr_id) {
                    alert("Please save the vendor info first.");
                    return;
                }

                var url = $(e.target).data("contenturl");
                url += arr_id + "?c_num=" + arrangement.c_num;
                window.open(url, "_blank");
            }
        });

        optionDetailViewModel.bind("change", function (e) {
            //log("optionDetailViewModel.change", e.field, optionDetailViewModel[e.field]);

            if (e.field === "sales.item_type") {
                optionDetailViewModel.onItemTypeChanged();
            } else if (e.field === "sales.sub_agent_cd") {
                optionDetailViewModel.onSubAgentChanged();
            }

            if (e.field === "sales.item_cd" && optionDetailViewModel.get("sales.item_cd")) {
                var combo = $("#OptionDetailDialog").find("#txtItemCd").data("kendoComboBox");
                var item = combo.dataItem();
                if (item) {
                    optionDetailViewModel.set("sales.item_cd", item.item_cd);
                    optionDetailViewModel.set("item_name", item.item_name);
                    optionDetailViewModel.set("item_name_jpn", item.item_name_jpn);
                    optionDetailViewModel.set("sales.price", "");       //Priceは保存後にのみ表示する。
                }
            }

            if (e.field === "sales.price_changed" || e.field === "sales") {
                //price_changedの値が変わったもしくはsalesが読み込まれた時に価格の表示を変更。
                optionDetailViewModel.onPriceChanged();     //NumericTextBoxではStyleバインディングが使えないのでその代替策。
            }

            //フィールド名が「sales.」で始まる場合はdirtyフラグを立てる。
            if (e.field.indexOf("sales.") == 0 || e.field.indexOf("Arrangements") == 0) {
                optionDetailViewModel.set("dirty", true);
            }
        });

        return optionDetailViewModel;
    });
