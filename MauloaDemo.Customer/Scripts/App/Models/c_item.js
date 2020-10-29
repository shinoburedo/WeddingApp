(function () {
    'use strict';

    App.Models.CItem = kendo.data.Model.define({
        id: "item_cd",
        fields: {
            item_cd: { type: "number" },
            item_type: { type: "string", defaultValue: "" },
            item_name: { type: "string", defaultValue: "" },
            last_person: { type: "string", defaultValue: "" },
            update_date: { type: "date" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");

            return json;
        },

        parseJSON: function () {
            this.dirty = false;
        },

        isPkg: function () {
            return this.get("item_type") === "PKG";
        }

    });

    App.Models.CItem.detail = function (id, lang, wed_date) {
        return $.ajax({
            url: App.getUrl("api/citems/detail"),
            type: "GET",
            data: { id: id, lang: lang, wed_date: wed_date },
            cache: false
        });
    };

    App.Models.CItem.ChurchAvail = function (id, wed_date) {
        return $.ajax({
            url: App.getUrl("api/citems/churchavail"),
            type: "GET",
            data: { id: id, wed_date: wed_date },
            cache: false
        });
    };

    App.Models.CItem.updatelOrder = function (data) {
        App.addAntiForgeryToken(data);      //CSRF対策用トークンを追加。

        return $.ajax({
            url: App.getUrl("MyWedding/UpdateOrder"),
            type: "POST",
            data: data,
        });
    };


})();

