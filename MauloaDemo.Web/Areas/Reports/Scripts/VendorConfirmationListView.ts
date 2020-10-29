//TypeScriptコンパイラ用型定義
interface IVendorConfirmationIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    refreshList(e?: any);
}

require(['../../../Scripts/App/models/arrangement', 'ReportEditView'],
    function (Arrangement, reportEditView: IVendorConfirmationIndexView) {

        var date_from = moment().add("months", 2).date(1).hours(0).minutes(0).seconds(0).milliseconds(0);
        var date_to = moment().add("months", 3).date(1).hours(0).minutes(0).seconds(0).milliseconds(0);
        date_to.add("days", -1);

        var myApp: IVendorConfirmationIndexView = <any> kendo.observable({
            search: {
                date_from: date_from.toDate(),
                date_to: date_to.toDate(),

                toJSON: function () {
                    var json = kendo.data.ObservableObject.prototype.toJSON.call(this);
                    json.date_from = kendo.toString(kendo.parseDate(this.date_from), "yyyy/MM/dd");
                    json.date_to = kendo.toString(kendo.parseDate(this.date_to), "yyyy/MM/dd");
                    return json;
                }
            },

            list: Arrangement.getVendorConfirmationDataSource(function () {
                return myApp.get("search").toJSON();
            }),

            preview: function (e) {
                if (e) e.preventDefault();
                var vendor_cd = e.data.vendor_cd;
                var date_from = myApp.get("search.date_from");
                var date_to = myApp.get("search.date_to");
                var url = "Preview";
                url += "?vendor_cd=" + vendor_cd;
                url += "&date_from=" + kendo.toString(kendo.parseDate(date_from), "yyyy/MM/dd");
                url += "&date_to=" + kendo.toString(kendo.parseDate(date_to), "yyyy/MM/dd");
                window.open(url, "_blank");
            },

            refreshList: function (e) {
                if (e) e.preventDefault();
                myApp.list.read();
            }

        });

      kendo.bind("#app", myApp);

      $("#refresh").on("click", myApp.refreshList);
      $("#grd .k-grid-refresh").addClass("pull-right");     //Toolbar内のRefreshボタンを右端に移動。


  });



