require([], function (
    ){
    //*-------------------------------- View Model　--------------------------------*//
    var viewModel = <any>kendo.observable({
        listFiles: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "CustomerFolder/GetGridData",
                    type: "POST",
                    dataType: "json",
                    serverFiltering: true,
                    data: function () {
                        return {
                            c_num: $("#hdnCNum").val()
                        };
                    }
                },
            },
            //schema: {
            //    model: CustomerListItem
            //},
            error: App.Utils.HandleServerError
        }),

        onUploadComplete: function (e) {
            viewModel.refreshGrid();
        },

        onUploadError: function (e) {
            if (e.XMLHttpRequest && e.XMLHttpRequest.responseText) {
                App.Utils.ShowAlert(e.XMLHttpRequest.responseText, true);
            } else {
                App.Utils.ShowAlert("An error occured while uploading the file.", true);
            }
        },

        error_handler: function (e) {
            if (e.errors) {
                var message = "Errors:\n";
                $.each(e.errors, function (key, value) {
                    if ('errors' in value) {
                        $.each(value.errors, function () {
                            message += this + "\n";
                        });
                    }
                });
                alert(message);
            }
        },

        onDownloadFile: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = <any>grid.dataItem(tr);
            var newUrl = App.Config.BaseUrl + "customers/customerFolder/Download?c_num=" + $("#hdnCNum").val() + "&filename=" + selected.FilenameForUrl;
            window.location.href = newUrl;
        },

        onDeleteFile: function (e) {
            if (e) e.preventDefault();
            var tr = $(e.target).closest("tr");
            var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
            var selected = <any>grid.dataItem(tr);
            if (!confirm("Are you sure to DELETE the file '" + selected.Filename + "'?")) return;

            var strData = JSON.stringify({
                c_num: $("#hdnCNum").val(),
                fileNames: [selected.Filename]
            });

            App.Utils.ShowLoading();

            var request = $.ajax({
                url: "customerFolder/Delete",
                type: "POST",
                data: strData,
                processData: false,
                contentType: "application/json; charset=utf-8"
            })
            .done(function (data) {
                if (data.Result == "success") {
                    App.Utils.ShowAlert("File has been deleted successfully.");
                    viewModel.refreshGrid();
                } else {
                    alert(data.Message);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert("Unexpected error:\n\n" + errorThrown);
            })
            .always(function () {
                App.Utils.HideLoading();
            });

        },

        refreshGrid: function (e) {
            if (e) e.preventDefault();
            var grid = <any>$("#grd").data("kendoGrid");
            grid.dataSource.transport.options.read.data = { "c_num": $("#hdnCNum").val() };
            grid.dataSource.read();
        }

});

    //全体のViewModelをバインド。
    kendo.bind($("#app"), viewModel);
    App.ViewModel = viewModel;

});

