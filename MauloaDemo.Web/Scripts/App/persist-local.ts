
///使用方法:
///
/// 1. 保存・復元したいエリア（div, table, formなど）に「class="persist-local"」を付けておく。
/// 2. $PL.loadForm()を呼ぶと復元される。
///     document.readyのタイミングで、Kendo UIを使っている場合はその適用後に実行する。
/// 3. $PL.saveForm()を呼ぶと保存される。
///     フォームのsubmitイベントや検索ボタンのclickイベントなどで実行する。
/// 4. $PL.loadForm(), $PL.saveForm()にはkeyPrefixパラメータを渡す。(ログインIDごとに保存先のキーを変える場合。)
///
///
/// （例）
/// 
///$(function () {
///    var userid = "@(UserHelper.LoginUser.login_id)";
/// 
///    $PL.loadForm(userid);
///    
///    $(document).on("submit", "form", function(){ 
///        $PL.saveForm(userid);
///    });
///});

define(function () {

    var persistLocal = function () {
        var parentSelector = ".persist-local";

        //ローカルストレージのキーを取得する。
        var getKey = function (prefix) {

            //URLのパスを得る
            var key = window.location.pathname;

            //Prefixを先頭に付ける
            if (prefix) key = prefix + "-" + key;
            key = key.toLowerCase();

            //末尾の"/"を除去
            if (key.substr(key.length - 1, 1) === "/") {
                key = key.substr(0, key.length - 1);
            }

            //末尾の"index"を除去
            if (key.substr(key.length - "index".length) === "index") {
                key = key.substr(0, key.length - "index".length);
            }

            //もう一度末尾の"/"を除去
            if (key.substr(key.length - 1, 1) === "/") {
                key = key.substr(0, key.length - 1);
            }

            return key;
        };

        var getElements = function () {
            var elements = $(parentSelector + " input:not(.nostore), " + parentSelector + " select:not(.nostore)");
            return elements;
        };

        return {
            parentSelector: parentSelector,
            storage: "local", //'local' or 'session'

            getStorage: function () {
                if (this.storage == 'local') {
                    return window.localStorage;
                } else {
                    return window.sessionStorage;
                }
            },

            loadObject: function (keyPrefix) {
                var storage = this.getStorage();
                if (!storage) return null;
                var key = getKey(keyPrefix);
                var json_string = storage.getItem(key);
                if (json_string === null) return null;
                var obj = JSON.parse(json_string);
                //log("loadObject done", key, obj);
                return obj;
            },

            saveObject: function (obj, keyPrefix) {
                var storage = this.getStorage();
                if (!storage) return;
                var key = getKey(keyPrefix);
                var json_string = JSON.stringify(obj);
                storage.setItem(key, json_string);
                //log("saveObject done", key, obj);
            },

            loadForm: function (keyPrefix) {
                var obj = this.loadObject(keyPrefix);
                if (!obj) return;
                //log("loadForm", obj);

                var elements = getElements();
                elements.each(function () {
                    var el = $(this);
                    var id = el.attr("id");
                    if (!id) {
                        if (el.attr("type") === "radio") {
                            var name = el.attr("name");
                            if (name) elements.filter("[type='radio'][name='" + name + "'][value='" + obj[name] + "']").prop("checked", true);
                        }
                        return true;
                    }

                    var value = obj[id];

                    if (el.attr("type") === "checkbox" || el.attr("type") === "radio") {
                        el.prop("checked", value);
                    } else {
                        if (el.attr("data-role") === "datepicker") {
                            el.data("kendoDatePicker").value(value);
                        } else if (el.attr("data-role") === "timepicker") {
                            el.data("kendoTimePicker").value(value);
                        } else if (el.attr("data-role") === "combobox") {
                            el.data("kendoComboBox").value(value);
                        } else if (el.attr("data-role") === "dropdownlist") {
                            el.data("kendoDropDownList").value(value);
                        }
                        el.val(value);
                    }
                });
            },

            saveForm: function (keyPrefix) {
                var obj = {};
                var elements = getElements();
                elements.each(function () {
                    var el = $(this);
                    var id = el.attr("id");
                    if (!id) {
                        if (el.attr("type") === "radio" && el.prop("checked") === true) {
                            var name = el.attr("name");
                            if (name) obj[name] = el.val();
                        }
                        return true;
                    }

                    var value = el.val();
                    if (el.attr("type") === "checkbox" || el.attr("type") === "radio") {
                        value = el.prop("checked");
                    }
                    obj[id] = value;
                });
                //log("saveForm", obj);
                this.saveObject(obj, keyPrefix);
            }
        };
    };

    return persistLocal();
});


