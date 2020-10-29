/// アプリケーション全体で使うスクリプト
///

//IE8以下でtrim()がサポートされていないのでその対応。
if (typeof String.prototype.trim !== 'function') {
    //Stringクラスにtrim関数が無い場合にtrim関数を追加。
    String.prototype.trim = function () {
        return this.replace(/\s+$/, "");
    }
};

//// カーソル位置にテキストを挿入する
//$.fn.extend({
//    insertAtCaret: function (v) {
//        var o = this.get(0);
//        o.focus();

//        if (!jQuery.support.opacity) {
//            var r = document.selection.createRange();
//            r.text = v;
//            r.select();
//        } else {
//            var s = o.value;
//            var p = o.selectionStart;
//            var np = p + v.length;
//            o.value = s.substr(0, p) + v + s.substr(p);
//            o.setSelectionRange(np, np);
//        }
//    }
//});


$(function () {

    //Ajax処理の共通エラーハンドラを登録。
    $(document).ajaxError(App.Utils.AjaxError);

    //toupperクラスの付いたTextBoxの内容を自動的に大文字にする。
    $(document).on("blur", "input.toupper", function () {
        var e = $(this);
        var str = e.val();
        if (str) {
            var new_str = str.toUpperCase();
            if (new_str != str){
                e.val(new_str);
                e.trigger("change");        //changeイベントを呼ばないとKendoUIのData bindに反映されないので必要。
            }
        }
        return true;
    });

    //tolowerクラスの付いたTextBoxの内容を自動的に小文字にする。
    $(document).on("blur", "input.tolower", function () {
        var e = $(this);
        var str = e.val();
        if (str) {
            var new_str = str.toLowerCase();
            if (new_str != str) {
                e.val(new_str);
                e.trigger("change");        //changeイベントを呼ばないとKendoUIのData bindに反映されないので必要。
            }
        }
        return true;
    });

    //tohankakuクラスの付いたTextBoxの内容をOnBlurイベントで自動的に半角にする。
    $(document).on("blur", "input.tohankaku", function () {
        var e = $(this);
        var str = e.val();
        if (str) {
            // 全角英数字(および全角括弧、全角＝、全角ピリオド、全角ハイフン、全角アンダーバー、全角＠)を半角に変換する。
            var new_str = str.replace(/[Ａ-Ｚａ-ｚ０-９（）＝．－＿＠]/g, function (s) {
                return String.fromCharCode(s.charCodeAt(0) - 0xFEE0);
            });
            if (new_str != str) {
                e.val(new_str);
                e.trigger("change");        //changeイベントを呼ばないとKendoUIのData bindに反映されないので必要。
            }
        }
        return true;
    });


    //todateクラスの付いたTextBoxの内容をOnBlurイベントで日付に変換する。
    $(document).on("blur", "input.todate", function () {
        var e = $(this);
        var s = e.val();
        if (s && s.length == 8) {
            var valid = moment(s, "MMDDYYYY").isValid();
            if (valid) {
                var d = moment(s, "MMDDYYYY").toDate();
                s = kendo.toString(d, App.Config.DateFormat);
            } else {
                s = "";
            }
            e.val(s);
            e.trigger("change");        //changeイベントを呼ばないとKendoUIのData bindに反映されないので必要。
        }
        return true;
    });

    //totimeクラスの付いたTextBoxの内容をOnBlurイベントで時間に変換する。
    $(document).on("blur", "input.totime", function () {
        var e = $(this);
        var s = e.val();
        if (s && s.length == 4 && s.indexOf(":") < 0) {
            var valid = moment(s, "HHmm").isValid();
            if (valid) {
                var time = moment(s, "HHmm");
                s = time.format("HH:mm");
            } else {
                s = "";
            }
            e.val(s);
            e.trigger("change");        //changeイベントを呼ばないとKendoUIのData bindに反映されないので必要。
        }
        return true;
    });

    //Kendo UI のタブストリップ内の<a>タグのテキスト部分のクリック時に画面がスクロールしてしまう現象を防ぐ。
    // (但しこの方法を使うとURLの末尾の#記号以下の情報が変わらなくなるので注意。
    //  この結果、以下の副作用がある。
    //  - タブ選択状態込みでリンクを送ったりブックマークしたりする事が出来なくなる。
    //  - F5キー等で画面をリフレッシュするとタブの選択状態が初期化される。)
    //URLの#記号以下を更新しながら、かつ不要なスクロールを発生させない方法があればそちらを採用する事。
    $(document).on("click", ".k-tabstrip-items a", function () {
        return false;
    });

    //kendoComboBoxで、フォーカスが離れた時にvalueとtextの値に不一致が生じる場合があるのでその対応。
    $(document).on("blur", ".k-combobox input[data-role='combobox']", null, function (e) {
        var el = $(e.target);
        var combo = el.data("kendoComboBox");
        if (combo == null) return;
        var text = combo.text();
        var value = combo.value();

        if (text != value) {
            combo.value(text);      //valueとtextを一致させる。

            //valueをセットしただけではchangeイベントが発生しないので、
            //手動でkendoComboBoxのchangeイベントのハンドラを呼ぶ。(複数ありえるのでループする。)
            App.Utils.KendoComboBoxChangeFix(combo, e.target);
        }
    });


    //カスタマー番号入力用テキストボックスでEnterキーが押された時の処理
    $(document).on("keypress", ".horizontal_menu li.cust_num input", null, function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) { 
            var c_num = $(e.target).val();
            if (c_num) {
                var url = App.Config.BaseUrl + "customers/home/edit/" + c_num;
                window.open(url, "_blank");
            }            
        }
    });

});





// KendoComboBoxにJavaScriptからvalueをセットした場合にchangeイベントが発生しない場合があるので
// 手動でkendoComboBoxのchangeイベントのハンドラを呼ぶ。(複数ありえるのでループする。)
App.Utils.KendoComboBoxChangeFix = function (kendoCombo: kendo.ui.ComboBox, input_element: EventTarget) {
    var cbo = <any> kendoCombo;
    if (cbo._events && cbo._events.change) {
        for (var i = 0, len = cbo._events.change.length; i < len; i++) {
            cbo._events.change[i].call(kendoCombo, { sender: { element: [input_element] } });
        }
    }
};




// Overlay表示
App.Utils.ShowOverlay = function(id? :string, zorder? : number) {
    id = id || "shadow";
    var hash_id = "#" + id;

    if ($(hash_id).length == 0) {         //同じidで複数回appendされるのを防ぐ。
        //log("Creating a div for overlay.");
        $("body").append("<div id='" + id +"'></div>");
    }
    
    $(hash_id).css({
        opacity: '0.15',
        display: 'none',
        position: 'absolute',
        top: '0',
        left: '0',
        minWidth: $(document).width(),
        width: '100%',
        minHeight: $(document).height(),
        height: '100%',
        background: '#000',
        zIndex: zorder || 1000
    });

    $(hash_id).show();
}


// Overlay非表示
App.Utils.HideOverlay = function(id?: string) {
    id = "#" + (id || "shadow");
    $(id).hide();
    $(id).remove();
}


// Loading表示
App.Utils.ShowLoading = function(withOverlay? : boolean)
{
    if (withOverlay) App.Utils.ShowOverlay();

    var h = $("#_loading").height();
    var w = $("#_loading").width();
    var top = $(window).height() / 2 + $(document).scrollTop() - h;
    var left = $(window).width() / 2 + $(document).scrollLeft() - (w/2);

    $("#_loading").css({  top: top, left: left });
    $("#_loading").show();
}


// Loading非表示
App.Utils.HideLoading = function(withOverlay? : boolean) {
    if (withOverlay) App.Utils.HideOverlay();

    $("#_loading").hide();
}


// Alert表示
App.Utils.ShowAlert = function (msg: string, isError? : boolean, title?: string) {
    App.Utils.HideLoading(true);

    title = title || isError ? "Error" : "Info"
    if (msg) msg = msg.replace(/\n/g, "<br />");    //「g」オプション付きの正規表現で検索しないと最初にマッチした部分だけしか置き換えられないので注意。

    if (isError) {
        alertify.alert(msg, function () { });
    } else {
        //alertify.success(msg, 2000);
        toastr.success(msg, title, { positionClass: "toast-top-full-width", timeOut: 3000 });
    }
};


App.Utils.ShowAlertAjaxErr = function(jqXHR: JQueryXHR, textStatus: string, errorThrown: string) {
    App.Utils.HideLoading(true);

    //失敗時、アラートを表示。
    var s = errorThrown;
    if (jqXHR && jqXHR.responseJSON) {
        var res = jqXHR.responseJSON;
        s = res.ExceptionMessage || res.Message;
        if (res.ModelState) {
            s = JSON.stringify(res.ModelState);
        }
    }
    App.Utils.ShowAlert(s, true);
}



App.Utils.HandleServerError = function (e: any) {
    App.Utils.HideLoading(true);

    var msg = e.status + ": " + e.errorThrown;
    var respJSON = null;
    if (e.responseJSON) respJSON = e.responseJSON;
    if (e.xhr && e.xhr.responseJSON) respJSON = e.xhr.responseJSON;

    if (respJSON) {
        msg = e.status || (e.xhr ? e.xhr.status : "");
        msg += ": " + respJSON.Message;
        if (respJSON.ExceptionMessage) {
            msg += "<br /><br />" + respJSON.ExceptionMessage + " (" + respJSON.ExceptionType + ")";
        }
        if (respJSON.ModelState) {
            msg += "<br /><br />" + kendo.stringify(respJSON.ModelState);
        }
    }
    App.Utils.ShowAlert(msg, true);
    //alertify.error(msg, 0);
}



// 共通のAjaxErrorハンドラ。
// セッション切れの場合にログイン画面に飛ばす。
App.Utils.AjaxError = function (event: JQueryEventObject, jqXHR: JQueryXHR, ajaxSettings: JQueryAjaxSettings, thrownError: any) {
    App.Utils.HideLoading(true);

    var xhr = (<any>event).xhr || (<any>jqXHR).xhr || jqXHR;
    var status = xhr.status;
    var statusText = xhr.statusText;

    //If server returned 401 (Unauthorized), then reload the page in order to redirect to the login page.
    if (status === 401) {
        window.location.reload();
        return;
    }

    if (console && console.error) {
        console.error("AjaxError: " + status + " " + statusText + "\n" + ajaxSettings + "\n" + thrownError);
    }
};
 



// AutoComplete用のメソッド
App.Utils.setAutoComplete = function (obj: any, url: string) {
    $(obj).keyup(function (event) {
        var charCode = String.fromCharCode(event.keyCode);
        if (charCode.match(/[0-9A-Z]/) && obj.val().length == 1) {
            $.get(
                '' + url +'',
                {
                    text: obj.val(),
                    isCheckMode: false
                },
                function (data) {
                    $(obj).autocomplete({ source: data, minLength: 2 });
                }
            );
        }
    });
};


// 2文字未満になった場合にComboBoxのリストを閉じる
App.Utils.kendoComboBoxClose = function (comboId: string) {
    var comboInput = $("input[name=" + comboId + "_input]");
    $(comboInput).keyup(function (event) {
        if ($(comboInput).val().length <= 1) {
            var kendoCombo = $("#" + comboId).data("kendoComboBox");
            kendoCombo.close();
        }
    })
};


// Json経由のDate値変換
App.Utils.convertToDateTime = function (value: string) {
    if (!value) return null;
    var date = kendo.parseDate(value);
    return date;
};


App.Utils.fillZero = function (n: number, width: number) {
    var s: string = n.toString();
    width -= s.length;
    if (width > 0) {
        return new Array(width + (/\./.test(s) ? 2 : 1)).join('0') + s;
    }
    return s;
};


//"disabled", "checked"などの属性をboolean値からセットする。
App.Utils.setBoolAttr = function(selector: string, attr_name: string, boolean_value: boolean) {
    var e = $(selector);
    if (e.length <= 0) return;

    if (boolean_value === true) {
        e.attr(attr_name, attr_name);
    } else {
        e.removeAttr(attr_name);
    }

    if (attr_name === "checked") {
        //CheckBoxの選択状態を変更するにはこちらの方が安全。(http://api.jquery.com/prop/)
        e.prop("checked", boolean_value);
    }
};


//"disabled", "checked"などの属性をboolean値として取得する。
//(厳密にはdisabled="disabled"の様に入っているべきだが、一応念のために値がtrue, "", "true"の場合もtrueを返す。)
App.Utils.getBoolAttr = function(selector: string, attr_name: string) {
    var e = $(selector);
    if (e.length <= 0) return false;

    if (attr_name === "checked") {
        //CheckBoxの現在の選択状態を取得するにはこちらの方が安全。(http://api.jquery.com/prop/)
        return e.prop("checked");
    }

    if (e.attr(attr_name) === attr_name
        || e.attr(attr_name) === ""
        //|| e.attr(attr_name) == true
        || e.attr(attr_name) == "true") {
        return true;
    }

    return false;
};

App.Utils.toggleBoolAttr = function(selector: string, attr_name: string) {
    App.Utils.setBoolAttr(selector, attr_name, !App.Utils.getBoolAttr(selector, attr_name));
};

//現在のUTC時刻を「yyyy/MM/dd HH:mm:ss」の形式の文字列として返す。
//TimeZone情報をDateオブジェクトから故意に除去する際に使用する。
App.Utils.getUTCDateStr = function(d? : Date) {
    if (!d) d = new Date()

    var s = d.getUTCFullYear()
        + "/" + (d.getUTCMonth() + 1)
        + "/" + d.getUTCDate()
        + " " + d.getUTCHours()
        + ":" + d.getUTCMinutes()
        + ":" + d.getUTCSeconds()

    return s;
};

App.Utils.getRegionNow = function () {
    return moment()
        .utc()
        .add(App.Config.TimeZone, "hours")
        .toDate();
};

App.Utils.getRegionDate = function () {
    var d = App.Utils.getRegionNow();
    return moment(d)
        .startOf('day')
        .toDate();
};

