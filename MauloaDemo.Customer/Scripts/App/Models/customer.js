(function () {
    'use strict';

    App.Models.Customer = kendo.data.Model.define({
        id: "c_num",
        fields: {
            c_num: { type: "string", defaultValue: "" },
            pkg_cd : { type: "string", defaultValue: "" },
            order_date : { type: "date" },
            g_last : { type: "string", defaultValue: "" },
            g_first : { type: "string", defaultValue: "" },
            b_last : { type: "string", defaultValue: "" },
            b_first : { type: "string", defaultValue: "" },
            agent_cd : { type: "string", defaultValue: "" },
            tour_by : { type: "string", defaultValue: "" },
            tour_cd : { type: "string", defaultValue: "" },
            area_cd : { type: "string", defaultValue: "" },
            cust_kind : { type: "string", defaultValue: "" },
            wed_cust : { type: "boolean", defaultValue: true },
            church_cd : { type: "string", defaultValue: "" },
            wed_date : { type: "date" },
            wed_time : { type: "date" },
            bf_date : { type: "date" },
            bf_time : { type: "date" },
            ft_date : { type: "date" },
            ft_time : { type: "date" },
            htl_pick : { type: "date" },
            flight_date1 : { type: "date" },
            flight1 : { type: "string", defaultValue: "" },
            org1 : { type: "string", defaultValue: "" },
            dep_time1 : { type: "date" },
            dst1 : { type: "string", defaultValue: "" },
            arr_time1 : { type: "date" },
            flight_date2 : { type: "date" },
            flight2 : { type: "string", defaultValue: "" },
            org2 : { type: "string", defaultValue: "" },
            dep_time2 : { type: "date" },
            dst2 : { type: "string", defaultValue: "" },
            arr_time2: { type: "date" },

            gb_hotel1: { type: "string", defaultValue: "" },
            gb_hotel1_name : { type: "string", defaultValue: "" },
            gb_hotel1_name_jpn : { type: "string", defaultValue: "" },

            gb_room1 : { type: "string", defaultValue: "" },
            gb_checkin1 : { type: "date" },
            gb_checkout1 : { type: "date" },
            gb_hotel2 : { type: "string", defaultValue: "" },
            gb_room2 : { type: "string", defaultValue: "" },
            gb_checkin2 : { type: "date" },
            gb_checkout2 : { type: "date" },
            //att_hotel1 : { type: "string", defaultValue: "" },
            //att_room1 : { type: "string", defaultValue: "" },
            //att_checkin1 : { type: "date" },
            //att_checkout1 : { type: "date" },
            //att_hotel2 : { type: "string", defaultValue: "" },
            //att_room2 : { type: "string", defaultValue: "" },
            //att_checkin2 : { type: "date" },
            //att_checkout2 : { type: "date" },
            //att_count : { type: "number", defaultValue: 0 },
            //att_name : { type: "string", defaultValue: "" },
            g_custtype : { type: "string", defaultValue: "" },
            b_custtype : { type: "string", defaultValue: "" },
            //wed_order : { type: "string", defaultValue: "" },
            //options : { type: "string", defaultValue: "" },
            //rs_info : { type: "string", defaultValue: "" },
            //cos_info : { type: "string", defaultValue: "" },
            //jpn_info : { type: "string", defaultValue: "" },
            //glo_option : { type: "string", defaultValue: "" },
            //glo_dress : { type: "string", defaultValue: "" },
            note : { type: "string", defaultValue: "" },
            //due_date : { type: "date" },
            //due_desc : { type: "string", defaultValue: "" },
            //w_history : { type: "string", defaultValue: "" },
            //rs_history : { type: "string", defaultValue: "" },
            //recep_history : { type: "string", defaultValue: "" },
            wed_cxl : { type: "boolean", defaultValue: false },
            cxl_date : { type: "date" },
            cxl_by : { type: "string", defaultValue: "" },
            cxl_charge : { type: "number", defaultValue: 0 },
            disc_reason : { type: "string", defaultValue: "" },
            create_date : { type: "date" },
            create_by : { type: "string", defaultValue: "" },
            final_date : { type: "date" },
            final_by : { type: "string", defaultValue: "" },
            last_person : { type: "string", defaultValue: "" },
            update_date : { type: "date" },
            sub_agent_cd : { type: "string", defaultValue: "" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method

            json.order_date = kendo.toString(this.order_date, "yyyy/MM/dd");
            json.wed_date = kendo.toString(this.wed_date, "yyyy/MM/dd");
            json.wed_time = kendo.toString(this.wed_time, "HH:mm");
            json.bf_date = kendo.toString(this.bf_date, "yyyy/MM/dd");
            json.bf_time = kendo.toString(this.bf_time, "HH:mm");
            json.ft_date = kendo.toString(this.ft_date, "yyyy/MM/dd");
            json.ft_time = kendo.toString(this.ft_time, "HH:mm");
            json.htl_pick = kendo.toString(this.htl_pick, "HH:mm");

            json.flight_date1 = kendo.toString(this.flight_date1, "yyyy/MM/dd");
            json.dep_time1 = kendo.toString(this.dep_time1, "HH:mm");
            json.arr_time1 = kendo.toString(this.arr_time1, "yyyy/MM/dd HH:mm");
            json.flight_date2 = kendo.toString(this.flight_date2, "yyyy/MM/dd");
            json.dep_time2 = kendo.toString(this.dep_time2, "HH:mm");
            json.arr_time2 = kendo.toString(this.arr_time2, "yyyy/MM/dd HH:mm");

            json.gb_checkin1 = kendo.toString(this.gb_checkin1, "yyyy/MM/dd HH:mm");
            json.gb_checkout1 = kendo.toString(this.gb_checkout1, "yyyy/MM/dd HH:mm");
            json.gb_checkin2 = kendo.toString(this.gb_checkin2, "yyyy/MM/dd HH:mm");
            json.gb_checkout2 = kendo.toString(this.gb_checkout2, "yyyy/MM/dd HH:mm");
            //json.att_checkin1 = kendo.toString(this.att_checkin1, "yyyy/MM/dd HH:mm");
            //json.att_checkout1 = kendo.toString(this.att_checkout1, "yyyy/MM/dd HH:mm");
            //json.att_checkin2 = kendo.toString(this.att_checkin2, "yyyy/MM/dd HH:mm");
            //json.att_checkout2 = kendo.toString(this.att_checkout2, "yyyy/MM/dd HH:mm");
            //json.due_date = kendo.toString(this.due_date, "yyyy/MM/dd");
            json.cxl_date = kendo.toString(this.cxl_date, "yyyy/MM/dd");
            json.final_date = kendo.toString(this.final_date, "yyyy/MM/dd");

            json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
            return json;
        },

        parseJSON: function () {
            this.set("create_date", App.Utils.convertToDateTime(this.create_date));
            this.set("update_date", App.Utils.convertToDateTime(this.update_date));

            this.set("order_date", App.Utils.convertToDateTime(this.order_date));
            this.set("wed_date", App.Utils.convertToDateTime(this.wed_date));
            this.set("wed_time", App.Utils.convertToDateTime(this.wed_time));
            this.set("bf_date", App.Utils.convertToDateTime(this.bf_date));
            this.set("bf_time", App.Utils.convertToDateTime(this.bf_time));
            this.set("ft_date", App.Utils.convertToDateTime(this.ft_date));
            this.set("ft_time", App.Utils.convertToDateTime(this.ft_time));
            this.set("htl_pick", App.Utils.convertToDateTime(this.htl_pick));

            this.set("flight_date1", App.Utils.convertToDateTime(this.flight_date1));
            this.set("dep_time1", App.Utils.convertToDateTime(this.dep_time1));
            this.set("arr_time1", App.Utils.convertToDateTime(this.arr_time1));
            this.set("flight_date2", App.Utils.convertToDateTime(this.flight_date2));
            this.set("dep_time2", App.Utils.convertToDateTime(this.dep_time2));
            this.set("arr_time2", App.Utils.convertToDateTime(this.arr_time2));
            this.set("gb_checkin1", App.Utils.convertToDateTime(this.gb_checkin1));
            this.set("gb_checkout1", App.Utils.convertToDateTime(this.gb_checkout1));
            this.set("gb_checkin2", App.Utils.convertToDateTime(this.gb_checkin2));
            this.set("gb_checkout2", App.Utils.convertToDateTime(this.gb_checkout2));
            //this.set("att_checkin1", App.Utils.convertToDateTime(this.att_checkin1));
            //this.set("att_checkout1", App.Utils.convertToDateTime(this.att_checkout1));
            //this.set("att_checkin2", App.Utils.convertToDateTime(this.att_checkin2));
            //this.set("att_checkout2", App.Utils.convertToDateTime(this.att_checkout2));

            //this.set("due_date", App.Utils.convertToDateTime(this.due_date));
            this.set("cxl_date", App.Utils.convertToDateTime(this.cxl_date));
            this.set("final_date", App.Utils.convertToDateTime(this.final_date));

            this.set("pkg_cd", this.pkg_cd || '');
            this.set("g_last", this.g_last || '');
            this.set("g_first", this.g_first || '');
            this.set("b_last", this.b_last || '');
            this.set("b_first", this.b_first || '');
            this.set("agent_cd", this.agent_cd || '');
            this.set("tour_by", this.tour_by || '');
            this.set("tour_cd", this.tour_cd || '');
            this.set("area_cd", this.area_cd || '');
            this.set("cust_kind", this.cust_kind || '');
            this.set("church_cd", this.church_cd || '');
            this.set("flight1", this.flight1 || '');
            this.set("org1", this.org1 || '');
            this.set("dst1", this.dst1 || '');
            this.set("flight2", this.flight2 || '');
            this.set("org2", this.org2 || '');
            this.set("dst2", this.dst2 || '');
            this.set("gb_hotel1", this.gb_hotel1 || '');
            this.set("gb_hotel1_name", this.gb_hotel1_name || '');
            this.set("gb_hotel1_name_jpn", this.gb_hotel1_name_jpn || '');
            this.set("gb_room1", this.gb_room1 || '');
            this.set("gb_hotel2", this.gb_hotel2 || '');
            this.set("gb_room2", this.gb_room2 || '');
            this.set("g_custtype", this.g_custtype || '');
            this.set("b_custtype", this.b_custtype || '');
            this.set("note", this.note || '');
            this.set("cxl_by", this.cxl_by || '');
            this.set("disc_reason", this.disc_reason || '');
            this.set("final_by", this.final_by || '');
            this.set("sub_agent_cd", this.sub_agent_cd || '');

            this.dirty = false;
        },

        flight_date1_s: function () {
            if (!this.get("flight_date1")) return '';
            return kendo.toString(this.get("flight_date1"), App.Config.DateFormat);
        },

        dep_time1_s: function () {
            if (!this.get("dep_time1")) return '';
            return kendo.toString(this.get("dep_time1"), App.Config.TimeFormat);
        },

        arr_date1_s: function () {
            if (!this.get("arr_time1")) return '';
            return kendo.toString(this.get("arr_time1"), App.Config.DateFormat);
        },

        arr_time1_s: function () {
            if (!this.get("arr_time1")) return '';
            return kendo.toString(this.get("arr_time1"), App.Config.TimeFormat);
        },

        flight_date2_s: function () {
            if (!this.get("flight_date2")) return '';
            return kendo.toString(this.get("flight_date2"), App.Config.DateFormat);
        },

        dep_time2_s: function () {
            if (!this.get("dep_time2")) return '';
            return kendo.toString(this.get("dep_time2"), App.Config.TimeFormat);
        },

        arr_date2_s: function () {
            if (!this.get("arr_time2")) return '';
            return kendo.toString(this.get("arr_time2"), App.Config.DateFormat);
        },

        arr_time2_s: function () {
            if (!this.get("arr_time2")) return '';
            return kendo.toString(this.get("arr_time2"), App.Config.TimeFormat);
        }

    });

    App.Models.Customer.fetch = function () {
        return $.ajax({
            url: App.getUrl("MyWedding/GetCustomer"),
            type: "GET",
            cache: false
        });
    };

})();

