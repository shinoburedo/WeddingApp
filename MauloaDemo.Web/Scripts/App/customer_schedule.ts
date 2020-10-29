//TypeScriptコンパイラ用型定義
interface ISchedulePhraseViewModel extends kendo.data.ObservableObject {
    doSearch(): void;
    phrases: kendo.data.DataSource;
    notes: kendo.data.DataSource;
    openWindow(c_num: string, contentUrl: string): void;
    closeWindow(): void;
}


define(['models/schedule_phrase',
        'models/schedule_note_template',
        'models/schedule_note',
        'models/schedule_pattern',
        'models/pickupplace'],

    function (SchedulePhrase: SchedulePhrase,
            ScheduleNoteTemplate: ScheduleNoteTemplate,
            ScheduleNote: ScheduleNote,
            SchedulePattern: SchedulePattern,
            PickupPlace: any) {

        var jpn_suffix = (App.Config.Lang == 'ja' ? "_jpn" : "");
        var eng_suffix = (App.Config.Lang == 'ja' ? "" : "_eng");

        var schedulePhraseViewModel = App.data.schedulePhraseViewModel = <ISchedulePhraseViewModel> kendo.observable({
            c_num: "",
            item_cd: "",
            hotel_cd: "",
            sch_pattern_id: null,

            patternList: SchedulePattern.getDataSource(function () {
                var item_cd = schedulePhraseViewModel.get("item_cd");
                return { item_cd: item_cd };
            }),

            phrases: SchedulePhrase.getDataSource(function () {
                return { c_num: schedulePhraseViewModel.get("c_num") };
            }),

            notes: ScheduleNote.getDataSource(function () {
                return { c_num: schedulePhraseViewModel.get("c_num") };
            }),

            doSearch: function () {
                schedulePhraseViewModel.phrases.read();
                schedulePhraseViewModel.notes.read();
            },

            recreateData: function () {
                if ((this.get("phrases") && this.get("phrases").data().length > 0)
                    || (this.get("notes") && this.get("notes").data().length > 0)) {
                    if (!confirm("Are you sure to recreate the schedule?")) return;
                }
                var c_num : string = schedulePhraseViewModel.get("c_num");
                var sch_pattern_id : number = schedulePhraseViewModel.get("sch_pattern_id");

                App.Utils.ShowLoading(false);
                $.when(
                        SchedulePhrase.GetPhrasesFromPatternId(c_num, sch_pattern_id, function (phrases) {
                            var cur_list: kendo.data.DataSource = schedulePhraseViewModel.get("phrases");
                            cur_list.data().forEach(function (item: kendo.data.ObservableObject, index: number, source: kendo.data.ObservableArray) {
                                item.set("deleted", true);
                            });

                            for (var i = 0; i < phrases.length; i++) {
                                var model = phrases[i];
                                cur_list.add(model);
                            }
                        }),

                        ScheduleNote.GetNotesFromPatternId(c_num, sch_pattern_id, function (notes) {
                            var cur_list: kendo.data.DataSource = schedulePhraseViewModel.get("notes");
                            cur_list.data().forEach(function (item: kendo.data.ObservableObject, index: number, source: kendo.data.ObservableArray) {
                                item.set("deleted", true);
                            });

                            for (var i = 0; i < notes.length; i++) {
                                var model = notes[i];
                                cur_list.add(model);
                            }
                        })

                    ).fail(App.Utils.ShowAlertAjaxErr)
                    .always(function () {
                        App.Utils.HideLoading(false);
                    });
            },

            recreateButtonValue: function () {
                var s = this.get("phrases").data().length ? "Create again" : "Create";
                return s;
            },

            recreateButtonEnabled: function () {
                return schedulePhraseViewModel.get("sch_pattern_id");
            },

            openWindow: function (c_num: string, item_cd: string, hotel_cd: string, contentUrl: string) {
                this.set("c_num", c_num);
                this.set("item_cd", item_cd);
                this.set("hotel_cd", hotel_cd);
                this.set("sch_pattern_id", null);

                var div = $("<div class='schedule-dialog'><div class='loading'><h3>Loading...</h3><img src='/Content/Images/wait_circle.gif' /></div></div>");
                div.appendTo($(document.body));
                div.kendoWindow({
                    title: "Schedule",
                    width: "90%",
                    minWidth: 600,
                    minHeight: 840,
                    modal: true,
                    content: contentUrl
                });
                var kendoWindow : kendo.ui.Window = div.data("kendoWindow");

                kendoWindow.bind("refresh", function () {
                    kendo.bind($(".schedule-dialog"), schedulePhraseViewModel);
                    $(".schedule-dialog #txtPattern").data("kendoComboBox").list.width("500");
                    schedulePhraseViewModel.doSearch();
                });
                kendoWindow.bind("deactivate", function () {
                    kendoWindow.destroy();      //destroy()を呼ぶとWindowのDIV自体がDOMから削除される。
                    //log("deactivate. kendoWindow was destroyed.");
                });

                kendoWindow.center().open();
            },

            closeWindow: function (e) {
                if (e) e.preventDefault();
                $(".schedule-dialog").data("kendoWindow").close();
            },

            saveData: function (e) {
                if (e) e.preventDefault(); 

                var phrases: kendo.data.DataSource = schedulePhraseViewModel.get("phrases");
                var notes: kendo.data.DataSource = schedulePhraseViewModel.get("notes");
                if (!phrases && !notes) return;

                var c_num = schedulePhraseViewModel.get("c_num");
                var data_phrases = phrases.data();
                var data_notes = notes.data();

                App.Utils.ShowLoading(false);
                $.when(
                        SchedulePhrase.saveList(data_phrases, c_num)
                            .fail(App.Utils.ShowAlertAjaxErr),
                        ScheduleNote.saveList(data_notes, c_num)
                            .fail(App.Utils.ShowAlertAjaxErr)
                    )
                    .done(function (result) {
                        App.Utils.HideLoading(false);
                        schedulePhraseViewModel.doSearch();
                    });
            },

            placeEditor: function (container, options) {
                var input = $("<input/>");
                input.attr("name", options.field);
                input.appendTo(container);
                input.kendoComboBox({
                    valuePrimitive: true,
                    dataValueField: "place_name",
                    dataTextField: "place_name",
                    dataSource: PickupPlace.getDataSource(function () {
                        var data = { hotel_cd: schedulePhraseViewModel.get("hotel_cd") };
                        return data;
                    })
                });
                var combo = input.data("kendoComboBox");
                combo.list.width("400");
                combo.bind('change', function () {
                    var selectedIndex = combo.select();
                    if (selectedIndex < 0) return;
                    var item = combo.dataItem(selectedIndex);
                    if (!item) return;
                    options.model.set("place_eng", item.get("place_name_eng"));
                });
            },

            placeEngEditor: function (container, options) {
                var input = $("<input/>");
                input.attr("name", options.field);
                input.appendTo(container);
                input.kendoComboBox({
                    valuePrimitive: true,
                    dataValueField: "place_name_eng",
                    dataTextField: "place_name_eng",
                    dataSource: PickupPlace.getDataSource(function () {
                        var data = { hotel_cd: schedulePhraseViewModel.get("hotel_cd") };
                        return data;
                    })
                });
                var combo = input.data("kendoComboBox");
                combo.list.width("400");
                combo.bind('change', function () {
                    var selectedIndex = combo.select();
                    if (selectedIndex < 0) return;
                    var item = combo.dataItem(selectedIndex);
                    if (!item) return;
                    options.model.set("place", item.get("place_name"));
                });
            },

            templateCdEditor: function (container, options) {
                var input = $("<input/>");
                input.attr("name", options.field);
                input.appendTo(container);
                input.kendoComboBox({
                    valuePrimitive: true,
                    dataValueField: "template_cd",
                    dataTextField: "template_cd",
                    dataSource: ScheduleNoteTemplate.getDataSource({ template_cd: "", title: "" }),
                    template: "<dl><dt style='width:120'>${ data.template_cd }</dt><dd>${ data.title" + jpn_suffix + eng_suffix + " }</dd></dl>"
                });
                var combo = input.data("kendoComboBox");
                combo.list.width("400");
                combo.bind('change', function () {
                    var selectedIndex = combo.select();
                    if (selectedIndex < 0) return;
                    var item = combo.dataItem(selectedIndex);
                    if (!item) return;
                    options.model.set("title_jpn", item.get("title_jpn"));
                    options.model.set("title_eng", item.get("title_eng"));
                    options.model.set("note_jpn", item.get("note_jpn"));
                    options.model.set("note_eng", item.get("note_eng"));
                });
            }

        });

        //schedulePhraseViewModel.bind("change", function (e) {
        //    //log("schedulePhraseViewModel: change: ", e, schedulePhraseViewModel);
        //});

        return schedulePhraseViewModel;
    }
);

