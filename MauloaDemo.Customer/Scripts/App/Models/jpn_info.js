(function () {
    'use strict';

    App.Models.JpnInfo = kendo.data.Model.define({
        id: "c_num",
        fields: {
            c_num: { type: "string", defaultValue: "" },
            g_last_kana: { type: "string", defaultValue: "" },
            g_first_kana: { type: "string", defaultValue: "" },
            g_last_kanji: { type: "string", defaultValue: "" },
            g_first_kanji: { type: "string", defaultValue: "" },
            b_last_kana: { type: "string", defaultValue: "" },
            b_first_kana: { type: "string", defaultValue: "" },
            b_last_kanji: { type: "string", defaultValue: "" },
            b_first_kanji: { type: "string", defaultValue: "" },
            jpn_zip: { type: "string", defaultValue: "" },
            addr_kana1: { type: "string", defaultValue: "" },
            addr_kana2: { type: "string", defaultValue: "" },
            addr_kana3: { type: "string", defaultValue: "" },
            addr_kanji1: { type: "string", defaultValue: "" },
            addr_kanji2: { type: "string", defaultValue: "" },
            addr_kanji3: { type: "string", defaultValue: "" },
            home_tel: { type: "string", defaultValue: "" },
            work_tel: { type: "string", defaultValue: "" },
            e_mail: { type: "string", defaultValue: "" },
            e_mail_b: { type: "string", defaultValue: "" },

            do_not_send_g: { type: "boolean", defaultValue: false },
            send_date_g: { type: "date" },
            send_count_g: { type: "number", defaultValue: 0 },
            do_not_send_b: { type: "boolean", defaultValue: false },
            send_date_b: { type: "date" },
            send_count_b: { type: "number", defaultValue: 0 },

            access_key_g: { type: "string", defaultValue: "" },
            access_key_b: { type: "string", defaultValue: "" },
            mp_do_not_send_g: { type: "boolean", defaultValue: false },
            mp_send_date_g: { type: "date" },
            mp_send_count_g: { type: "number", defaultValue: 0 },
            mp_do_not_send_b: { type: "boolean", defaultValue: false },
            mp_send_date_b: { type: "date" },
            mp_send_count_b: { type: "number", defaultValue: 0 },
            mp_access_key_g: { type: "string", defaultValue: "" },
            mp_access_key_b: { type: "string", defaultValue: "" },
            disclosure_date: { type: "date" },
            disclosure_by: { type: "string", defaultValue: "" },

            last_person: { type: "string", defaultValue: "" },
            update_date: { type: "date" },
        },


        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.send_date_g = kendo.toString(this.send_date_g, "yyyy/MM/dd");
            json.send_date_b = kendo.toString(this.send_date_b, "yyyy/MM/dd");
            json.mp_send_date_g = kendo.toString(this.mp_send_date_g, "yyyy/MM/dd");
            json.mp_send_date_b = kendo.toString(this.mp_send_date_b, "yyyy/MM/dd");
            json.disclosure_date = kendo.toString(this.disclosure_date, "yyyy/MM/dd");
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
            return json;
        },

        parseJSON: function () {
            this.set("send_date_g", App.Utils.convertToDateTime(this.send_date_g));
            this.set("send_date_b", App.Utils.convertToDateTime(this.send_date_b));
            this.set("mp_send_date_g", App.Utils.convertToDateTime(this.mp_send_date_g));
            this.set("mp_send_date_b", App.Utils.convertToDateTime(this.mp_send_date_b));
            this.set("disclosure_date", App.Utils.convertToDateTime(this.disclosure_date));
            this.set("update_date", App.Utils.convertToDateTime(this.update_date));

            this.set("g_last_kana", this.g_last_kana || '');
            this.set("g_first_kana", this.g_first_kana || '');
            this.set("g_last_kanji", this.g_last_kanji || '');
            this.set("g_first_kanji", this.g_first_kanji || '');
            this.set("b_last_kana", this.b_last_kana || '');
            this.set("b_first_kana", this.b_first_kana || '');
            this.set("b_last_kanji", this.b_last_kanji || '');
            this.set("b_first_kanji", this.b_first_kanji || '');
            this.set("jpn_zip", this.jpn_zip || '');
            this.set("addr_kana1", this.addr_kana1 || '');
            this.set("addr_kana2", this.addr_kana2 || '');
            this.set("addr_kana3", this.addr_kana3 || '');
            this.set("addr_kanji1", this.addr_kanji1 || '');
            this.set("addr_kanji2", this.addr_kanji2 || '');
            this.set("addr_kanji3", this.addr_kanji3 || '');
            this.set("home_tel", this.home_tel || '');
            this.set("work_tel", this.work_tel || '');
            this.set("e_mail", this.e_mail || '');
            this.set("e_mail_b", this.e_mail_b || '');
            this.set("access_key_g", this.access_key_g || '');
            this.set("access_key_b", this.access_key_b || '');
            this.set("mp_access_key_g", this.mp_access_key_g || '');
            this.set("mp_access_key_b", this.mp_access_key_b || '');
            this.set("disclosure_by", this.disclosure_by || '');

            this.dirty = false;
        }

    });

    App.Models.JpnInfo.fetch = function () {
        return $.ajax({
            url: App.getUrl("MyWedding/GetJpnInfo"),
            type: "GET",
            cache: false
        });
    };

})();

