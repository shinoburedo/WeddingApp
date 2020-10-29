(function () {
    'use strict';

    App.Models.WtBooking = kendo.data.Model.define({
        id: "wt_id",
        fields: {
            wt_id: { type: "number" },
            account_id: { type: "number" },
            c_num: { type: "string", defaultValue: "" },
            region_cd: { type: "string", defaultValue: "" },
            area_cd: { type: "string", defaultValue: "" },
            agent_cd: { type: "string", defaultValue: "" },
            sub_agent_cd: { type: "string", defaultValue: "" },
            op_seq: { type: "number" },
            item_cd: { type: "string", defaultValue: "" },
            trf_item_cd: { type: "string", defaultValue: "" },
            item_type: { type: "string", defaultValue: "" },
            wed_date: { type: "date" },
            create_date: { type: "date" },
            quantity: { type: "number", defaultValue: 1 },
            price: { type: "number", defaultValue: 0 },
            price_type: { type: "number", defaultValue: 0 },
            price_cur: { type: "string", defaultValue: "" },

            cxl_charge: { type: "number", defaultValue: 0 },

            service_date: { type: "date" },
            service_time: { type: "date" },
            rcp_private_room: { type: "boolean" },
            rcp_room_id: { type: "number" },
            rcp_seat_only: { type: "number" },
            rcp_time_flag: { type: "number" },
            mks_time_flag: { type: "number" },
            bga: { type: "string", defaultValue: "" },
            payment_status: { type: "string", defaultValue: "" },
            del_date: { type: "date" },
            payment_id: { type: "string", defaultValue: "" },
            wed_h_seq: { type: "number" },
            land_wb_id: { type: "number" },

            alb_mount: { type: "string", defaultValue: "" },
            alb_mount_name: { type: "string", defaultValue: "" },
            alb_cover: { type: "string", defaultValue: "" },
            alb_cover_name: { type: "string", defaultValue: "" },
            alb_type: { type: "string", defaultValue: "" },
            alb_type_name: { type: "string", defaultValue: "" },
            dvd_menucolor: { type: "string", defaultValue: "" },
            dvd_menucolor_name: { type: "string", defaultValue: "" },

            trf_kind: { type: "string", defaultValue: "" },
            trf_type: { type: "string", defaultValue: "" },
            trf_cat: { type: "string", defaultValue: "" },
            period_id: { type: "number" },

            order_num: { type: "number" },
            cnt_picture_s: { type: "number" },
            trf_item_name: { type: "string", defaultValue: "" },
            price_charge: { type: "number" },
            price_cur_charge: { type: "string", defaultValue: "" },
            reserve_pkg: { type: "boolean" },
            last_person: { type: "string", defaultValue: "" },
            update_date: { type: "date" }
        },

        toJSON: function () {
            var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
            json.wed_date = kendo.toString(this.wed_date, "yyyy/MM/dd");
            json.service_date = kendo.toString(this.service_date, "yyyy/MM/dd");
            json.service_time = kendo.toString(this.service_time, "HH:mm");
            json.del_date = kendo.toString(this.del_date, "yyyy/MM/dd");
            json.create_date = kendo.toString(this.create_date, "yyyy/MM/dd HH:mm:ss");
            json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");

            delete json.service_date_s;
            delete json.service_time_s;
            delete json.del_date_s;
            return json;
        },

        parseJSON: function () {
            this.set("wed_date", App.Utils.convertToDateTime(this.wed_date));
            this.set("service_date", App.Utils.convertToDateTime(this.service_date));
            this.set("service_time", App.Utils.convertToDateTime(this.service_time));
            this.set("del_date", App.Utils.convertToDateTime(this.del_date));
            this.set("create_date", App.Utils.convertToDateTime(this.create_date));
            this.set("update_date", App.Utils.convertToDateTime(this.update_date));

            this.set("service_date_s", kendo.toString(this.service_date, App.Config.DateFormat));
            this.set("service_time_s", kendo.toString(this.service_time, App.Config.TimeFormat));
            this.set("del_date_s", kendo.toString(this.del_date, App.Config.DateFormat));
            this.dirty = false;
        },

        isPkg: function () {
            return this.get("item_type") === "PKG";
        },

        save: function () {
            var dataStr = JSON.stringify(this);
            return $.ajax({
                url: App.getUrl("MyWedding/UpdateOrder"),
                type: "POST",
                data: dataStr,
                processData: false,
                contentType: "application/json; charset=utf-8"
            });
        }

    });

    App.Models.WtBooking.fetch = function (region_cd, wt_id) {
        return $.ajax({
            url: App.getUrl("MyWedding/GetBooking"),
            type: "GET",
            data: {region_cd: region_cd, wt_id: wt_id},
            cache: false
        });
    };

    App.Models.WtBooking.updatelOrder = function (data) {
        App.addAntiForgeryToken(data);      //CSRF対策用トークンを追加。

        return $.ajax({
            url: App.getUrl("MyWedding/UpdateOrder"),
            type: "POST",
            data: data,
        });
    };

    App.Models.WtBooking.cancelOrder = function (region_cd, wt_id) {
        var data = { region_cd: region_cd, wt_id: wt_id };
        App.addAntiForgeryToken(data);      //CSRF対策用トークンを追加。

        return $.ajax({
            url: App.getUrl("MyWedding/CancelOrder"),
            type: "POST",
            data: data,
        });
    };

    App.Models.WtBooking.makePayment = function (region_cd, wt_id) {
        var data = { region_cd: region_cd, wt_id: wt_id };
        App.addAntiForgeryToken(data);      //CSRF対策用トークンを追加。

        return $.ajax({
            url: App.getUrl("MyWedding/MakePayment"),
            type: "POST",
            data: data,
        });
    };

    App.Models.WtBooking.getDataSource = function (region_cd, c_num, show_cxl) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getUrl("MyWedding/GetBookingList"),
                    data: { region_cd: region_cd, c_num: c_num, show_cxl: show_cxl },
                    dataType: "json"
                }
            }
        });
    };

    App.Models.WtBooking.getWinkOptionList = function (region_cd, c_num) {
        console.log("WtBooking.getWinkOptionList", region_cd, c_num);
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getUrl("MyWedding/GetWinkOptionList"),
                    data: { region_cd: region_cd, c_num: c_num },
                    dataType: "json"
                }
            }
        });
    };

    App.Models.WtBooking.getReceiptList = function (region_cd, c_num) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getUrl("MyWedding/GetReceiptList"),
                    data: { region_cd: region_cd, c_num: c_num },
                    dataType: "json"
                }
            }
        });
    };


})();

