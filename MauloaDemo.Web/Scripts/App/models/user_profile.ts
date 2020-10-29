declare class UserProfile extends kendo.data.Model {
    constructor(options: any);
    parseJSON(): void;
    toJSON(): any;
    set(): void;
    getPwdExpiration(): string;
    isSavePasswordEnabled() : boolean;
}

define(function () {

    var UserProfile = <typeof UserProfile> kendo.data.Model.define( {
        id: "login_id",
        fields: {
            "login_id": { type: "string", editable: false, validation: { required: true } },
            "culture_name": { type: "string", validation: { required: true } },
            "date_format": { type: "string", validation: { required: true } },
            "time_format": { type: "string", validation: { required: true } },

            "e_last_name": { type: "string", editable: false },
            "e_first_name": { type: "string", editable: false },
            "j_last_name": { type: "string", editable: false },
            "j_first_name": { type: "string", editable: false },
            "section": { type: "string", editable: false },
            "sub_agent_cd": { type: "string", editable: false },
            "e_mail": { type: "string", editable: true },

            "cur_password": { type: "string", validation: { required: true } },
            "new_password": { type: "string", validation: { required: true } },
            "new_password_conf": { type: "string", validation: { required: true } },
            "eff_to_pass": { type: "date", nullable: true, validation: { required: false } },

            isSaveUserInfoEnabled: { type: "boolean", defaultValue: false }
        }
    });

    UserProfile.DATE_FMT_EN = "MM/dd/yyyy";
    UserProfile.DATE_FMT_JA = "yyyy/MM/dd";
    UserProfile.TIME_FMT_EN = "HH:mm";
    UserProfile.TIME_FMT_JA = "HH:mm";

    UserProfile.prototype.getPwdExpiration = function(){
        var s = "";
        var effTo = this.get("eff_to_pass");
        if (effTo) {
            var d = kendo.parseDate(effTo);
            s = kendo.toString(d, this.get("date_format"));
        }
        return s;
    };

    UserProfile.prototype.isSavePasswordEnabled = function(){
        var cur_pwd = this.get("cur_password");
        var new_pwd = this.get("new_password");
        var new_pwd_conf = this.get("new_password_conf");
        return (cur_pwd && new_pwd && new_pwd === new_pwd_conf);
    }

    return UserProfile;
});