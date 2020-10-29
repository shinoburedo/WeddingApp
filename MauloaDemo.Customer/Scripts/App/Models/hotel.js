(function () {
    'use strict';

    App.Models.Hotel = kendo.data.Model.define({
        id: "hotel_cd",
        fields: {
            hotel_cd: { type: "string" },
            hotel_name : { type: "string", defaultValue: "" },
            hotel_name_jpn : { type: "string", defaultValue: "" },
            tel : { type: "string", defaultValue: "" },
            area_cd : { type: "string", defaultValue: "" },
            zone_cd : { type: "string", defaultValue: "" },
            sort_order : { type: "number", defaultValue: 0 },
            discon_date : { type: "date" },
            last_person: { type: "string" },
            update_date: { type: "date" }
        }
    });

    App.Models.Hotel.getDataSource = function (region_cd, area_cd) {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getUrl("api/Hotels"),
                    data: {
                        region_cd: region_cd,
                        area_cd: area_cd
                    },
                    dataType: "json"
                }
            }
        });
    };

})();

