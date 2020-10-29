require(['calendar_edit_viewmodel', 'calendar_edit_view', 'customer_duplist', 'persist-local'],
    function (CalendarEditViewModel, CalendarEditView, dupListViewModel, $PL) {

        var DEFAULT_WEEKS = 4;
        var CHURCH_CD_PHOTO = "PHOTO";

        var d = moment();
        if (d.isBefore(App.Config.MinWedDate, "day")) {
            d = moment(App.Config.MinWedDate);
        }
        var initialDate = d.toDate();
        //log("initial date:", initialDate);
   
        var calendarViewModel = <any> kendo.observable({
            calendar: {},
            search: {
                start_date: initialDate,
                church_cd: (App.User.IsAgent || App.User.AccessLevel <= 1) ? CHURCH_CD_PHOTO : "",
                location: "",
                sub_agent_cd: App.User.HasChildAgents ? "" : App.User.SubAgentCd,
                weeks: DEFAULT_WEEKS,

                toJSON: function () {
                    var json = kendo.data.ObservableObject.prototype.toJSON.call(this);
                    json.start_date = kendo.toString(kendo.parseDate(this.start_date), "yyyy/MM/dd");
                    return json;
                }
            },

            churchList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Churches/ForCalendar"),
                        type: "GET",
                        dataType: "json"
                    }
                }
            }),

            locationList: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.getApiPath("Churches/forphotoplan"),
                        type: "GET",
                        dataType: "json"
                    }
                }
            }),

            isUpdatingDisplay: false,

            isSubAgentVisible: function () {
                return App.User.HasChildAgents;
            },
            locationVisible: function () {
                return this.get("search.church_cd") === CHURCH_CD_PHOTO;
            },

            isPickUpMsgVisible: function () {
                return this.get("search.church_cd") === CHURCH_CD_PHOTO
                    && this.get("search.location")
                    && (this.get("search.sub_agent_cd") || App.User.IsAgent);
            },

            onDayItemClick: function (e) {
                if (e) e.preventDefault();
                var div = $(e.target).closest('.calendar_dayitem');
                var status = div.data("status");
                if (status === 'X') {
                    App.Utils.ShowAlert('This block is not available.', true);
                    return;
                }

                var c_num = div.data("cnum");
                if (c_num) {
                    var url = App.Config.BaseUrl + "customers/home/edit/" + c_num;
                    window.open(url, "_blank");
                } else {
                    var date = div.data("date");
                    var time = div.data("time");
                    var is_sunset = div.data("issunset");
                    this.openCreateDialog(date, time, is_sunset);
                }
            },

            openCreateDialog: function (date_s, time_s, is_sunset) {
                var church_cd = this.get("search.church_cd");
                var location = this.get("search.location") || "";

                if (church_cd === CHURCH_CD_PHOTO && !location) {
                    alert("Please select a location first.");
                    return;
                }

                var calendarEditViewModel = CalendarEditViewModel.create({
                    church_cd: church_cd === CHURCH_CD_PHOTO ? location : church_cd,
                    sub_agent_cd: this.get("search.sub_agent_cd"),
                    date_s: date_s,
                    time_s: time_s,
                    is_sunset: is_sunset,
                    plan_type: church_cd === CHURCH_CD_PHOTO ? "P" : "W",
                    staff: App.User.StaffRequired ? "" : App.User.UserName
                });
                calendarEditViewModel.bind("saved", function () {
                    calendarViewModel.doSearch();
                });
                //log("openCreateDialog: ", calendarEditViewModel);

                var calendarEditView = new CalendarEditView("create_dialog", { model: calendarEditViewModel });
                calendarEditView.render("#dialog");
                calendarEditView.openWindow();
            },

            doSearch: function (e) {
                if (e) e.preventDefault();
                if (calendarViewModel.isUpdatingDisplay) return;
                if (!calendarViewModel.get("search.weeks")) {
                    calendarViewModel.set("search.weeks", DEFAULT_WEEKS);
                }
                if (calendarViewModel.get("search.church_cd") !== CHURCH_CD_PHOTO) {
                    calendarViewModel.set("search.location", "");
                }
                var url = App.getApiPath("Calendar");
                var data = calendarViewModel.get("search").toJSON();
                //log("data to send: ", data);

                App.Utils.ShowLoading(false);
                $.getJSON(url, data)
                    .done(function (calendar) {
                        App.Utils.HideLoading(false);
                        calendarViewModel.parseDates(calendar);
                        calendarViewModel.updateDisplay(calendar);

                        ////検索条件を保存。
                        //$PL.saveObject(data, App.User.UserId);

                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        App.Utils.HideLoading(false);
                        App.Utils.ShowAlertAjaxErr(jqXHR, textStatus, errorThrown);
                    });
            },

            parseDates: function (calendar) {
                calendar.SearchDate = kendo.parseDate(calendar.SearchDate);
                calendar.StartDate = kendo.parseDate(calendar.StartDateStr);
                calendar.EndDate = kendo.parseDate(calendar.EndDateStr);

                var len = calendar.Days.length;
                for (var i = 0; i < len; i++) {
                    var day = calendar.Days[i];
                    day.Date = kendo.parseDate(day.Date);

                    for (var j = 0; j < day.Items.length; j++) {
                        var item = day.Items[j];
                        item.Date = kendo.parseDate(item.Date);
                        item.StartTime = kendo.parseDate(item.StartTime);
                        item.EndTime = kendo.parseDate(item.EndTime);
                    }
                }
                //log("after parseDate: %O", calendar);
            },

            updateDisplay: function (calendar) {
                calendarViewModel.isUpdatingDisplay = true;
                calendarViewModel.set("calendar", calendar);
                calendarViewModel.set("search.start_date", calendar.SearchDate);
                calendarViewModel.set("search.weeks", calendar.Weeks);
                calendarViewModel.isUpdatingDisplay = false;
            },

            prevWeek: function (e) {
                calendarViewModel.changeDate(e, -1, 'weeks');
            },
            nextWeek: function (e) {
                calendarViewModel.changeDate(e, 1, 'weeks');
            },
            prevMonth: function (e) {
                calendarViewModel.changeDate(e, -1, 'months');
            },
            nextMonth: function (e) {
                calendarViewModel.changeDate(e, 1, 'months');
            },
            changeDate: function (e, number, period) {
                if (e) e.preventDefault();
                var d = moment(calendarViewModel.get("search.start_date")).add(number, period).toDate();
                calendarViewModel.set("search.start_date", d);
            }

        });

        ////検索条件を復元。
        //var obj = $PL.loadObject(App.User.UserId);
        //if (obj) {
        //    _.each(obj, function (value, key) {
        //        if (key === "start_date") {
        //            value = kendo.parseDate(value);
        //        }
        //        calendarViewModel.set("search." + key, value);
        //    });
        //}

        //Viewを生成して表示。
        var calendarView = new kendo.View(
            "index", // the id of the template that contains the view
            {
                model: calendarViewModel,
                init: function () {
                    var combo = $("#txtChurchCd").data("kendoComboBox");
                    if (combo) combo.list.width(400);

                    combo = $("#txtLocation").data("kendoComboBox")
                    if (combo) combo.list.width(400);

                    combo = $("#txtChurchCd").data("kendoComboBox")
                    if (combo) combo.text(calendarViewModel.get("search.church_cd"));
                }
            });
        calendarView.render("#app");

        //共通Comboboxの初期化など。
        //Viewをrenderした後に呼ぶ必要がある。(Templateを使っているため)
        App.initKendo($("#app"));

        //画面を開いた時に自動的に検索を実行。
        setTimeout(function () {
            calendarViewModel.doSearch();

            //検索条件が変わったら検索を実行。
            calendarViewModel.search.bind('change', calendarViewModel.doSearch);
        }, 200);


        //ダイアログ部分のViewModelをバインド。(念のため時間差を付けて確実にKendoUIの初期化後に実行される様にしている。)
        setTimeout(function () {
            kendo.bind($("#DupListDialog"), dupListViewModel);
        }, 300);

    });
 