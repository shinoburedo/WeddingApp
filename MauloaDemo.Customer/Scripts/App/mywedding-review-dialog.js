(function () {
    'use strict';

    var vm = App.ViewModels.MyWeddingReviewDialog = kendo.observable({
        element: ".review-window",

        region_cd: null,
        trf_item_name: null,
        trf_item_cd: null,
        model: null,
        post_done: false,
        enable_edit: false,

        open: function (region_cd, wt_id, account_id, trf_item_cd, trf_item_name) {
            vm.set("region_cd", region_cd);
            vm.set("trf_item_cd", trf_item_cd);
            vm.set("trf_item_name", trf_item_name);
            vm.set("model", null);
            //vm.set("cancel_done", false);

            var div = $(this.get("element"));
            var win = div.data("kendoWindow");
            //win.setOptions({ title: "Option: " + id });
            win.unbind();                               //deactivateでunbindしているが念のため。
            win.bind("activate", function () {
                App.Models.WtReview.fetch(wt_id)
                    .done(function (model) {
                        App.Utils.HideLoading(true);
                        var item = new App.Models.WtReview(model);
                        item.parseJSON();
                        if (item.review_id == 0) {
                            item.set("wt_id", wt_id);
                            item.set("account_id", account_id);
                            item.set("star", 5);
                            vm.set("enable_edit", true);
                        }
                        vm.set("model", item);
                    });
            });
            win.bind("deactivate", function () {
                this.unbind();                          //イベントハンドラーの登録を解除。（これをしないとactivateのハンドラが多重呼び出しされてしまう。）
            });
            win.center().open();
        },

        close: function () {
            $(vm.element).data("kendoWindow").close();
        },

        onOkClick: function (e) {
            if (e) e.preventDefault();

            var model = vm.get("model");

            App.Utils.ShowLoading(true);
            App.Models.WtReview.postReview(model)
                .done(function (result) {
                    App.Utils.HideLoading(true);
                    if (result.status == "ok") {
                        vm.set("post_done", true);
                        var region_cd = vm.get("region_cd");
                        var wt_id = vm.get("wt_id");
                        vm.trigger("post", { region_cd: region_cd, wt_id: wt_id });
                        App.Utils.ShowAlert(App.L("登録しました。", "This review has been posted."));
                    } else {
                        App.Utils.ShowAlert(App.L("エラーが発生しました。", "An error occurred. ") + "\n\n" + result.message, true);
                    }
                });
        },

        onCloseClick: function (e) {
            if (e) e.preventDefault();
            vm.close();
        }

    });

    kendo.bind(vm.element, vm);

    ////DatePickerにユーザーの日付書式を反映。
    //App.Utils.applyUserDateFormat(vm.element);

})();