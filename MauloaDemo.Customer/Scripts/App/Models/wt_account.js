(function () {
    'use strict';

    App.Models.WtAccount = kendo.data.Model.define({
        id: "account_id",
        fields: {
            account_id: { type: "number", defaultValue: 0 },
            c_num: { type: "string", defaultValue: "" },
            is_japan: { type: "boolean", defaultValue: true },
            e_last_name: { type: "string", defaultValue: "" },
            e_first_name_jpn: { type: "string", defaultValue: "" },
            is_groom: { type: "boolean", defaultValue: false },
            email: { type: "string", defaultValue: "" },
            primary_id: { type: "number", defaultValue: 0 },
            eff_from_pass: {type: "date"},
            eff_to_pass: {type: "date"},
            culture_name: { type: "string", defaultValue: "ja-JP" },
            date_format: { type: "string", defaultValue: "yyyy/MM/dd" },
            time_format: { type: "string", defaultValue: "HH:mm" },
            create_date: { type: "date" },
            last_person: { type: "string", defaultValue: "" },
            update_date: { type: "date" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
            json.eff_from_pass = kendo.toString(this.eff_from_pass, "yyyy/MM/dd");
            json.eff_to_pass = kendo.toString(this.eff_to_pass, "yyyy/MM/dd");
            return json;
        },

        parseJSON: function () {
            this.set("create_date", App.Utils.convertToDateTime(this.create_date));
            this.set("update_date", App.Utils.convertToDateTime(this.update_date));
            this.set("eff_from_pass", App.Utils.convertToDateTime(this.eff_from_pass));
            this.set("eff_to_pass", App.Utils.convertToDateTime(this.eff_to_pass));
            this.dirty = false;
        },

        cnumToDisplay: function () {
            var s = this.get("c_num") || App.L("---", "----");
            return s;
        }

    });

    App.Models.WtAccount.fetch = function () {
        return $.ajax({
            url: App.getUrl("MyWedding/GetWtAccount"),
            type: "GET",
            cache: false
        });
    };

    App.Models.WtAccount.fetchMyWeddingInfo = function () {
        return $.ajax({
            url: App.getUrl("MyWedding/GetMyWeddingInfo"),
            type: "GET",
            data: {is_japan: App.Config.Language != "E"},
            cache: false
        });
    };

})();

