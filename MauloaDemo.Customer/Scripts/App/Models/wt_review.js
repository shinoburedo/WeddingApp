(function () {
    'use strict';

    App.Models.WtReview = kendo.data.Model.define({
        id: "review_id",
        fields: {
            review_id: { type: "number", defaultValue: 0 },
            account_id: { type: "number", defaultValue: 0 },
            wt_id: { type: "number", defaultValue: 0 },
            nickname: { type: "string", defaultValue: "" },
            star: { type: "number", defaultValue: 5 },
            review: { type: "string", defaultValue: "" },
            create_date: { type: "date" },
            proc_date: { type: "date" },
            proc_by: { type: "string", defaultValue: "" },
            status: { type: "string", defaultValue: "" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
            json.proc_date = kendo.toString(this.proc_date, "yyyy/MM/dd HH:mm:ss");
            return json;
        },

        parseJSON: function () {
            this.set("create_date", App.Utils.convertToDateTime(this.create_date));
            this.set("proc_date", App.Utils.convertToDateTime(this.proc_date));
            this.dirty = false;
        }

    });

    App.Models.WtReview.postReview = function (item) {
        var data = item.toJSON();
        App.addAntiForgeryToken(data);  //CSRF対策用トークンを追加。

        return $.ajax({
            url: App.getUrl("MyWedding/PostReview"),
            type: "POST",
            dataType: "json",
            data: data
        });
    };

    App.Models.WtReview.fetch = function (wt_id) {
        return $.ajax({
            url: App.getUrl("MyWedding/GetReview"),
            type: "GET",
            data: { wt_id: wt_id },
            cache: false
        });
    };


})();

