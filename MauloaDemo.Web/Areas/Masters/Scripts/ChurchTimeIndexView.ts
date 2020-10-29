//TypeScriptコンパイラ用型定義
interface IChurchTimeIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    editItem(e?: any);
    refreshList(e?: any);
}

require(['Models/ChurchTime', 'ChurchTimeEditView', 'Models/Church' ],
    function (ChurchTime: ChurchTimeFn, churchGroupingEditView: IChurchTimeEditView, Church: ChurchFn) {

        var churchGroupingIndexView: IChurchTimeIndexView= <any> kendo.observable({

            search: {
                church_cd: "",
                church_name: ""
            },

            list: Church.getDataSource(function () {
                return churchGroupingIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                churchGroupingIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var church_type = $(e.target).closest("a.btn").data("key");
                churchGroupingEditView.openEditWindow(church_type, false);
            }

        });

        kendo.bind("#app", churchGroupingIndexView);

        churchGroupingEditView.bind("saved", function () {
            churchGroupingIndexView.list.read();
        });

    }
);



