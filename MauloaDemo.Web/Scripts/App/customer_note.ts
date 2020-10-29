define(['models/customer' ],
    function (Customer) {

        var noteViewModel = <any>kendo.observable({

        note: null,
        note_org: null,
        add_note: null,
        customer: null,
        dirty: false,
        saved_callback: null,
        isEditCancelVisible: false,
        isEditPastVisible: false,
        isEditPastDisable: true,

        openWindow: function (note, customer, saved_callback) {
            var customer = new Customer(customer);
            this.set("customer", customer);
            this.set("note", note);
            this.set("note_org", note);
            this.set("add_note", "");
            this.set("saved_callback", saved_callback);
            this.set("dirty", false);
            $("#add_note").show();
            if (this.isEditPastEnable()) {
                this.set("isEditPastVisible", true);
            } else {
                this.set("isEditPastVisible", false);
            }
            this.set("isEditCancelVisible", false);
            this.set("isEditPastDisable", true);

            var win = $("#NoteDialog");
            var kendoWindow = <any>win.data("kendoWindow");
            kendoWindow.setOptions({
                title: "Edit Note",
                width: "70%",
                minWidth: 460,
                minHeight: 560,
            });
            kendoWindow.center().open();

            //初期フォーカスをセット。
            var txt = win.find("#add_note textarea");
            kendoWindow.unbind();
            kendoWindow.bind("activate", function () {
                setTimeout(function () {
                    if (txt.length) txt.focus();
                }, 100);
            });
        },

        closeDialog: function (e) {
            if (e) e.preventDefault();
            $(".note-dialog").parent().data("kendoWindow").close();
        },

        onOkClick: function (e) {
            if (e) e.preventDefault();
            noteViewModel.saveNote();
        },

        onEditClick: function (e) {
            if (e) e.preventDefault();
            this.set("isEditPastVisible", false);
            this.set("isEditCancelVisible", true);
            this.set("isEditPastDisable", false);
            $("#add_note").hide();
        },

        onEditCancelClick: function (e) {
            if (e) e.preventDefault();
            this.set("note", this.get("note_org"));
            this.set("isEditPastVisible", true);
            this.set("isEditCancelVisible", false);
            this.set("isEditPastDisable", true);
            $("#add_note").show();
        },

        isEditPastEnable: function () {
            return App.User.IsStaff && App.User.AccessLevel >= 4;
        },

        saveNote: function () {
            var edit_all = noteViewModel.get("isEditCancelVisible");
            var add_note = "";
            if (edit_all) {
                add_note = noteViewModel.get("note");
            } else {
                add_note = noteViewModel.get("add_note");
            }
            if (!add_note) {
                alert(App.L("Noteを入力して下さい。", "Note is required."))
                return false;
            }
            App.Utils.ShowLoading(true);

            var customer = noteViewModel.get("customer");

            Customer.saveNote(customer.c_num, add_note, edit_all, {
                success: function (result) {
                    if (result && result.message === "ok") {
                        //log("Costume Info saved. info_id=" + result.info_id);
                        App.Utils.HideLoading(true);
                        noteViewModel.closeDialog();
                        var callback = noteViewModel.get("saved_callback");
                        if (callback) callback();

                    } else {
                        App.Utils.HideLoading(true);
                        App.Utils.ShowAlert(result.message, true);
                    }
                },
                fail: App.Utils.ShowAlertAjaxErr
            });
        }

    });

    noteViewModel.bind("change", function (e) {
        if (e.field.indexOf("note") >= 0) {
            noteViewModel.set("dirty", true);
        }
    });

    return noteViewModel;

});
