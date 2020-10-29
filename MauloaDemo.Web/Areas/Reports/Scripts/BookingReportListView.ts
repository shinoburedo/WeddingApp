//TypeScriptコンパイラ用型定義
interface IBookingReportIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    itemList: kendo.data.DataSource;
    preview(e?: any);
    refreshList(e?: any);
}

require(['../../../Scripts/App/models/item', 'ReportEditView'],
    function (Item, reportEditView: IBookingReportIndexView) {

        var date_from = moment();
        var date_to = moment();

        var myApp: IBookingReportIndexView = <any> kendo.observable({
            search: {
                date_from: date_from.toDate(),
                date_to: date_to.toDate(),
                agent_cd: null,
                church_cd: null,
                area_cd: null,
                vendor_cd: null,
                item_type: null,
                item_cd: null,
                sort_by: "c_num",
                include_cust_cxl: false,
                include_sales_cxl: false,
                not_finalized_only: false,

                toJSON: function () {
                    var json = kendo.data.ObservableObject.prototype.toJSON.call(this);
                    json.date_from = kendo.toString(kendo.parseDate(this.date_from), "yyyy/MM/dd");
                    json.date_to = kendo.toString(kendo.parseDate(this.date_to), "yyyy/MM/dd");
                    return json;
                }
            },

            itemList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("items/Search"),
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                item_type: myApp.get("search.item_type") || ""
                            };
                        }
                    }
                },
                schema: {
                    model: Item
                },
            }),

            changeItemType: function () {
                myApp.set("search.item_cd", "");
                myApp.itemList.read();
                //myApp.DoSearchChanges();
                //myApp.DoSearchOrders();
            },


            list: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("sales/bookingreport"),
                        type: "GET",
                        dataType: "json",
                        serverFiltering: true,
                        cache: false,
                        data: function () {
                            return myApp.get("search").toJSON();
                        }
                    }
                }
                //schema: { model: Arrangement }
            }),

            record_count: function () {
                return this.get("list").total();
            },

            preview: function (e) {
                if (e) e.preventDefault();
                var param = myApp.get("search");

                var url = "Preview";
                url += "?date_from=" + kendo.toString(kendo.parseDate(param.date_from), "yyyy/MM/dd");
                url += "&date_to=" + kendo.toString(kendo.parseDate(param.date_to), "yyyy/MM/dd");
                if (param.vendor_cd) url += "&vendor_cd=" + param.vendor_cd;
                if (param.agent_cd) url += "&agent_cd=" + param.agent_cd;
                if (param.church_cd) url += "&church_cd=" + param.church_cd;
                if (param.item_cd) url += "&item_cd=" + param.item_cd;
                if (param.item_type) url += "&item_type=" + param.item_type;
                url += "&include_cust_cxl=" + param.include_cust_cxl;
                url += "&include_sales_cxl=" + param.include_sales_cxl;
                url += "&not_finalized_only=" + param.not_finalized_only;
                if (param.sort_by) url += "&sort_by=" + param.sort_by;
                window.open(url, "_blank");
            },

            refreshList: function (e) {
                if (e) e.preventDefault();
                myApp.list.read();
            }

        });

      kendo.bind("#app", myApp);

      $("#refresh").on("click", myApp.refreshList);
      $("#report").on("click", myApp.preview);
      $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。

      //Item Cdのドロップダウンリストの幅をセット。
      $("select[name=txtItemCd]").each(function (index, el) {
          var combo = $(el).data("kendoComboBox");
          if (combo) combo.list.width(680);
      });

      $("select[name=txtSortBy]").each(function (index, el) {
          var combo = $(el).data("kendoComboBox");
          if (combo) combo.list.width(400);
      });

      

  });



