
(function (App, $) {
    
    //JavaScriptからサーバー側の処理を呼び出す時に使うURLを返す。
    App.getUrl = function (target) {
        if (target.substr(0, 2) === '//' ||
            target.substr(0, 5) === 'http:' || 
            target.substr(0, 6) === 'https:') {
            return target;
        }
        return App.Config.BaseURL + target;
    };

    App.L = function (japanese, english) {
        return App.Config.Language == "E" ? english : japanese;
    };

    App.CSRFTOKEN_NAME = "__RequestVerificationToken";

    //任意のオブジェクトにCSRF対策用トークンをプロパティとして追加する。
    App.addAntiForgeryToken = function (data) {
        data[App.CSRFTOKEN_NAME] = App.Config.AntiForgeryToken;     
    };

    window.twttr = (function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0], t = window.twttr || {}; if (d.getElementById(id)) return; js = d.createElement(s); js.id = id; js.src = "https://platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); t._e = []; t.ready = function (f) { t._e.push(f); }; return t; }(document, "script", "twitter-wjs"));

    $(function () {

        //CSRF対策用トークンを格納。--> Ajax呼び出し時に使う。
        App.Config.AntiForgeryToken = $("#AntiForgeryTokenContainer")
                                        .find("input[name='" + App.CSRFTOKEN_NAME + "']")
                                        .val();

        //「ログイン・登録」メニューにツールチップを適用
        $("#login").kendoTooltip({
            filter: "a",
            content: kendo.template($("#template_login").html()),
            width: 180,
            height: 60,
            position: "bottom",
            autoHide: false,
            showOn: "click"
        });
        //$("#a_login").click(false);

        //「ヘルプ」メニューにツールチップを適用
        $("#help").kendoTooltip({
            filter: "a",
            content: kendo.template($("#template_help").html()),
            width: (App.Config.Language == "J" ? 320 : 300),
            //height: 244,
            position: "bottom",
            autoHide: false,
            showOn: "click"
        });
        //$("#a_help").click(false);

        //カート内の現在の商品点数を表示
        //var div_cart = $("div.head_middle_cart");
        //if (div_cart.length) {
        //    div_cart.text("");
        //    $.ajax({
        //        url: App.getUrl("Cart/GetCartCount"),
        //        type: "POST",
        //        processData: false,
        //        contentType: "application/json; charset=utf-8"
        //    }).done(function (data) {
        //        if (data.Result === "success" && data.Count > 0) {
        //            var new_text;
        //            new_text = data.Count;
        //            //if (App.Config.Language === "J") {
        //            //    //new_text = "カート　現在" + data.Count + "点";
        //            //} else {
        //            //    var item = data.Count > 1 ? " items" : " item";
        //            //    new_text = data.Count + item + " in Cart";
        //            //}
        //            div_cart.text(new_text);
        //            div_cart.addClass("head_middle_cart_count");
        //        }
        //    });
        //}

    });

})(window.App, jQuery);


