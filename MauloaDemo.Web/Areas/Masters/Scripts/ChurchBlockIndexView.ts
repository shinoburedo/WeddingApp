//TypeScriptコンパイラ用型定義
interface IChurchBlockIndexView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
    editItem(e?: any);
    refreshList(e?: any);
}

require(['Models/ChurchBlock', 'ChurchBlockEditView', 'Models/Church' ],
    function (ChurchBlock, churchGroupingEditView: IChurchBlockEditView, Church: ChurchFn) {

        var churchBlockIndexView: IChurchBlockIndexView= <any> kendo.observable({

            search: {
                church_cd: "",
                church_name: ""
            },

            list: Church.getDataSource(function () {
                return churchBlockIndexView.get("search").toJSON();
            }),

            status: "Loading...",
            error: "",

            refreshList: function (e) {
                if (e) e.preventDefault();
                churchBlockIndexView.get("list").read();
            },

            editItem: function (e) {
                if (e) e.preventDefault();
                var church_type = $(e.target).closest("a.btn").data("key");
                churchGroupingEditView.openEditWindow(church_type, false);
            }

        });

        kendo.bind("#app", churchBlockIndexView);

        churchGroupingEditView.bind("saved", function () {
            churchBlockIndexView.list.read();
        });

    }
);



