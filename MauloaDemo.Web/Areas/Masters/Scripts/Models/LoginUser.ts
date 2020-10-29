//TypeScript用型定義
interface ILoginUser extends kendo.data.Model {
    //インスタンスメソッド・プロパティ
    parseJSON(): void;
    save(): JQueryXHR;
    destroy(): JQueryXHR;
}
interface LoginUserFn {
    //スタティックメソッド・プロパティ
    new (options?: any): ILoginUser;
    prototype: ILoginUser;
    fetch(login_id: string, callback: JQueryPromiseCallback<ILoginUser>): JQueryDeferred<ILoginUser>;
    getDataSource(data?: any): kendo.data.DataSource;
    setAutoPwd(): JQueryXHR;
}
//クラス名を定義
declare var LoginUser: LoginUserFn;


define(function () {

    var LoginUser: LoginUserFn = <any>kendo.data.Model.define({
        id: "login_id",
        fields: {
            "login_id": { type: "string", validation: { required: true } },
            "region_cd": { type: "string", defaultValue: "HWI" },
            "area_cd": { type: "string", defaultValue: "HNL" },
            "sub_agent_cd": { type: "string" },
            "busho": { type: "string" },
            "section": { type: "string" },
            "company": { type: "string" },
            "e_last_name": { type: "string" },
            "e_first_name": { type: "string" },
            "j_last_name": { type: "string" },
            "j_first_name": { type: "string" },
            "access_level": { type: "number", defaultValue: 3 },
            "access_count": { type: "number" },
            "user_type": { type: "string", defaultValue: "AGT" },
            "e_mail": { type: "string" },
            "phone": { type: "string" },
            "eff_from_pass": { type: "date", defaultValue: null },
            "eff_to_pass": { type: "date", defaultValue: null },
            "locked": { type: "boolean", defaultValue: false },
            "culture_name": { type: "string", defaultValue: "en-US" },
            "date_format": { type: "string", defaultValue: "MM/dd/yyyy" },
            "time_format": { type: "string", defaultValue: "HH:mm" },
            "last_person": { type: "string" },
            "update_date": { type: "date", defaultValue: null }
        }
    });

    LoginUser.prototype.toJSON = function(){
        var json = kendo.data.ObservableObject.prototype.toJSON.call(this); //Call the original toJSON method
        delete json.dirty;
        delete json.id;
        json.eff_from_pass = kendo.toString(this.eff_from_pass, "yyyy/MM/dd");
        json.eff_to_pass = kendo.toString(this.eff_to_pass, "yyyy/MM/dd");
        json.update_date = kendo.toString(this.update_date, "yyyy/MM/dd HH:mm:ss");
        return json;
    };

    LoginUser.prototype.parseJSON = function(){
        this.set("id", this.get(this.idField)); //idを明示的にセット。isNew()の値を正しく取得するために必要。
        this.eff_from_pass = App.Utils.convertToDateTime(this.eff_from_pass);
        this.eff_to_pass = App.Utils.convertToDateTime(this.eff_to_pass);
        this.update_date = App.Utils.convertToDateTime(this.update_date);
        this.dirty = false;
    };


    LoginUser.prototype.save = function(){
        this.set("is_new", this.isNew());
        var dataStr = JSON.stringify(this);

        var deffered = $.ajax({
            url: App.getApiPath("LoginUsers/Save"),
            type: "POST",
            data: dataStr,
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    LoginUser.prototype.destroy = function(){
        var deffered = $.ajax({
            url: App.getApiPath("LoginUsers/Delete/" + encodeURIComponent(this.get("login_id"))),
            type: "POST",
            processData: false,
            contentType: "application/json; charset=utf-8"
        });
        return deffered;
    };

    LoginUser.fetch = function (login_id, callback: JQueryPromiseCallback<ILoginUser>): JQueryDeferred<ILoginUser> {
        var deffered = $.ajax({
            url: App.getApiPath("LoginUsers/" + encodeURIComponent(login_id)),
            type: "GET",
            cache: false
        })
            .done(function (data) {
                var model = $.extend(new LoginUser(), data);
                model.parseJSON();
                if (callback) callback(model);
            });
        return deffered;
    };

    LoginUser.getDataSource = function (data): kendo.data.DataSource {
        return new kendo.data.DataSource({
            transport: {
                read: {
                    url: App.getApiPath("LoginUsers/Search"),
                    type: "POST",
                    dataType: "json",
                    data: data
                }
            },
            schema: { model: LoginUser }
        });
    };

    LoginUser.setAutoPwd = function (): JQueryXHR {
        var deffered = $.ajax({
            url: App.getApiPath("LoginUsers/SetAutoPwd"),
            type: "POST",
            cache: false
        });
        return deffered;
    };

    return LoginUser;
});

