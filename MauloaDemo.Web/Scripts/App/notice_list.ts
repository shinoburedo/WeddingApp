//TypeScriptコンパイラ用型定義
interface INoticeListView extends kendo.data.ObservableObject {
    list: kendo.data.DataSource;
}

// requireメソッドを呼ぶ前に、require.configメソッドでオプションを指定する。
if (App.Config.IsTest) {
    require.config({
        urlArgs: "bust=" + (new Date()).getTime()      //JavaScriptファイルをキャッシュしない様にする。(デバッグ用)
    });
}

require(['../../Areas/Masters/Scripts/Models/Notice'],
    function (Notice : NoticeFn) {

        var noticeListView : INoticeListView = <any> kendo.observable({

            list: Notice.getDataSourceForList()
        });

        kendo.bind("#app", noticeListView);

    }
);



