define(['models/customer']
        , function (Customer) {

    var dupListViewModel = App.DupListViewModel = <any>kendo.observable({

        customer: null,
        dupList: [],
        selectedCustomer: null,
        ignore_callback: null,
        select_callback: null,

        openWindow: function (customer, dup_list, ignore_callback, select_callback) {
            dupListViewModel.set("customer", customer);

            //log("dup_list before = ", dup_list);
            dup_list = dup_list.map(function (c) {
                var customer = $.extend(new Customer(), c);
                customer.parseJSON();
                return customer;
            });
            //log("dup_list after = ", dup_list);

            dupListViewModel.set("dupList", dup_list);
            dupListViewModel.set("ignore_callback", ignore_callback);
            dupListViewModel.set("select_callback", select_callback);

            var win = $("#DupListDialog");
            var kendoWindow = win.data("kendoWindow");
            kendoWindow.setOptions({
                title: "Duplicate Names",
                width: "70%",
                minWidth: 400,
                minHeight: 460,
            });
            kendoWindow.center().open();
        },

        isJPN: function () {
            return App.isJPN();
        },

        getSelectedRow: function () {
            var grid = $(".duplist-dialog").find("#grdDupList").data("kendoGrid");
            var selectedRows = grid.select();
            if (selectedRows && selectedRows.length == 1) {
                return selectedRows[0];
            } else {
                return null;
            }
        },
        getSelectedCustomer: function () {
            var row = this.getSelectedRow();
            if (!row) return null;
            var grid = $(".duplist-dialog").find("#grdDupList").data("kendoGrid");
            return grid.dataItem(row);
        },

        selectionChanged: function (e) {
            dupListViewModel.set("selectedCustomer", this.getSelectedCustomer());
        },

        selectCustomer: function(e){
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr")
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = <any>grid.dataItem(tr);

            dupListViewModel.set("selectedCustomer", selected);

            var select_callback = dupListViewModel.get("select_callback");
            if ($.isFunction(select_callback)) {
                select_callback(selected.c_num);
            }
            //dupListViewModel.closeDialog();
        },

        onOkClick: function (e) {
            if (e) e.preventDefault();
            if (!confirm(App.L("新たに同じ氏名で別のカスタマーが作成されますが、よろしいですか?",
                               "Are you sure to create another new customer with the same name?"))) return;

            var ignore_callback = dupListViewModel.get("ignore_callback");
            if ($.isFunction(ignore_callback)) {
                ignore_callback();
            }

            dupListViewModel.closeDialog();
        },

        closeDialog: function (e) {
            if (e) e.preventDefault();
            $(".duplist-dialog").parent().data("kendoWindow").close();
        }

    });

    //dupListViewModel.bind("change", function (e) {
    //    //console.log("dupListViewModel.change", e.field, dupListViewModel.get(e.field));
    //});

    return dupListViewModel;
});

