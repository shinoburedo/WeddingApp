//TypeScript用型定義
interface IVendor extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface VendorFn {
    //スタティックメソッド・プロパティ
    new (options?: any): IVendor;
    prototype: IVendor;
    fetch(vendor_cd: string, callback: JQueryPromiseCallback<IVendor> ): JQueryDeferred<IVendor>;
    getDataSource(data? : any): kendo.data.DataSource;
}
//クラス名を定義
declare var Vendor: VendorFn;


define(function () {

    var Vendor: VendorFn = <any>kendo.data.Model.define({
        id: "vendor_cd",
        fields: {
            "vendor_cd": { type: "string", validation: { required: true } },
            "vendor_name": { type: "string" },
            "vendor_name_j": { type: "string" },
            "vendor_type": { type: "string" },
            "op_address1": { type: "string" },
            "op_address2": { type: "string" },
            "op_address3": { type: "string" },
            "op_address4": { type: "string" },
            "op_tel": { type: "string" },
            "op_fax": { type: "string" },
            "ac_address1": { type: "string" },
            "ac_address2": { type: "string" },
            "ac_address3": { type: "string" },
            "ac_address4": { type: "string" },
            "ac_tel": { type: "string" },
            "ac_fax": { type: "string" },
            "ac_contact": { type: "string" },
            "acct_cd": { type: "string" },
            "def_bank_cd": { type: "string" },
            "tax_id": { type: "string" },
            "region": { type: "string" },
            "area_cd": { type: "string" },
            "discon_date": { type: "date", nullable: true, defaultValue: null },
            "employee": { type: "boolean" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    Vendor.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.discon_date = kendo.toString(this.discon_date, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    Vendor.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.discon_date = App.Utils.convertToDateTime(this.discon_date);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    Vendor.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("Vendors/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Vendor.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("Vendors/Delete/" + encodeURIComponent(this.get("vendor_cd"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    Vendor.fetch = function (vendor_cd, callback: JQueryPromiseCallback<IVendor>): JQueryDeferred<IVendor> {
        var deffered = $.ajax({
            url: App.getApiPath("Vendors/" + encodeURIComponent(vendor_cd)),
                type: "GET",
                cache: false
            })
            .done(function (data) {
                var model = $.extend(new Vendor(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    Vendor.getDataSource = function (data) : kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("Vendors/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: Vendor }
        });
    };

    return Vendor;
});

