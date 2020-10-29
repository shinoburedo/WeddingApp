(function () {
    'use strict';

    App.Models.WtWedInfo = kendo.data.Model.define({
        id: "c_num",
        fields: {
            c_num: { type: "string", defaultValue: "" },
            family_name: { type: "string", defaultValue: "" },

            attend_g_father: { type: "boolean", defaultValue: false },
            attend_g_mother: { type: "boolean", defaultValue: false },
            attend_g_siblings: { type: "number", defaultValue: 0 },
            attend_g_relatives: { type: "number", defaultValue: 0 },
            attend_g_friends: { type: "number", defaultValue: 0 },
            attend_b_father: { type: "boolean", defaultValue: false },
            attend_b_mother: { type: "boolean", defaultValue: false },
            attend_b_siblings: { type: "number", defaultValue: 0 },
            attend_b_relatives: { type: "number", defaultValue: 0 },
            attend_b_friends: { type: "number", defaultValue: 0 },

            escort_name: { type: "string", defaultValue: "" },
            escort_relation: { type: "string", defaultValue: "" },

            cert_name1: { type: "string", defaultValue: "" },
            cert_bg1: { type: "string", defaultValue: "" },
            cert_relation1: { type: "string", defaultValue: "" },
            cert_name2: { type: "string", defaultValue: "" },
            cert_bg2: { type: "string", defaultValue: "" },
            cert_relation2: { type: "string", defaultValue: "" },
            cert_name3: { type: "string", defaultValue: "" },
            cert_bg3: { type: "string", defaultValue: "" },
            cert_relation3: { type: "string", defaultValue: "" },
            cert_name4: { type: "string", defaultValue: "" },
            cert_bg4: { type: "string", defaultValue: "" },
            cert_relation4: { type: "string", defaultValue: "" },

            create_by: { type: "string", defaultValue: "" },
            create_date: { type: "date" },
            last_person: { type: "string", defaultValue: "" },
            update_date: { type: "date" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
            return json;
        },

        parseJSON: function () {
            this.set("create_date", App.Utils.convertToDateTime(this.create_date));
            this.set("update_date", App.Utils.convertToDateTime(this.update_date));

            this.set("family_name", this.family_name || '');
            this.set("escort_name", this.escort_name || '');
            this.set("escort_relation", this.escort_relation || '');
            this.set("cert_name1", this.cert_name1 || '');
            this.set("cert_bg1", this.cert_bg1 || '');
            this.set("cert_relation1", this.cert_relation1 || '');
            this.set("cert_name2", this.cert_name2 || '');
            this.set("cert_bg2", this.cert_bg2 || '');
            this.set("cert_relation2", this.cert_relation2 || '');
            this.set("cert_name3", this.cert_name3 || '');
            this.set("cert_bg3", this.cert_bg3 || '');
            this.set("cert_relation3", this.cert_relation3 || '');
            this.set("cert_name4", this.cert_name4 || '');
            this.set("cert_bg4", this.cert_bg4 || '');
            this.set("cert_relation4", this.cert_relation4 || '');

            this.dirty = false;
        }

    });

    App.Models.WtWedInfo.fetch = function () {
        return $.ajax({
            url: App.getUrl("MyWedding/GetWtWedInfo"),
            type: "GET",
            cache: false
        });
    };

})();

