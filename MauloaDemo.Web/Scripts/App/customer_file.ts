//TypeScriptコンパイラ用型定義
interface ICustomerFile extends kendo.data.ObservableObject {
    openWindow(c_num: string): void;
    onClose(e?: any): void;
    refreshGrid(e?: any): void;
    
}

define([],
    function () {

        var file = <any>kendo.data.Model.define({
            id: "Filename",
            fields: {
                "Filename": { type: "string", validation: { required: true } },
                "Size": { type: "string" }
            }
        });

        var customerFile: ICustomerFile = <any>kendo.observable({
            c_num: "",
            status: "",
            error: "",
            listFiles: new kendo.data.DataSource({
                transport: {
                    read: {
                        url: App.Config.BaseUrl + "CustomerFolder/GetGridData",
                        type: "POST",
                        dataType: "json",
                        serverFiltering: true,
                        data: function () {
                            return {
                                c_num: customerFile.get("c_num")
                            };
                        }
                    },
                },
                schema: {
                    model: file
                },
                error: App.Utils.HandleServerError
            }),

            openWindow: function (c_num) {
                var win = $("#editWindow").data("kendoWindow");
                win.title("Folder");
                win.center().open();

                $(".k-upload-files.k-reset").find("li").parent().remove(); //Upload済ファイルの進捗Listをクリアする

                var div = $("#editDiv");
                kendo.unbind(div);
                kendo.bind(div, customerFile);
                customerFile.set("c_num", c_num);
                var list = customerFile.get("listFiles");
                list.read();
                //customerFile.init(church_cd, is_copied);
            },

            onUploadComplete: function (e) {
                customerFile.trigger("changed");
                customerFile.refreshGrid();
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
                var selected = new file(grid.dataItem(tr));
                var newUrl = App.Config.BaseUrl + "customers/customerFolder/Download?c_num=" + $("#hdnCNum").val() + "&filename=" + selected.FilenameForUrl;
                //window.location.href = newUrl;
                window.open(newUrl, "_blank");

            },

            onDeleteFile: function (e) {
                if (e) e.preventDefault();
                var tr = $(e.target).closest("tr");
                var grid = tr.closest("div[data-role='grid']").data("kendoGrid");
                var selected = new file(grid.dataItem(tr));
                if (!confirm("Are you sure to DELETE the file '" + selected.Filename + "'?")) return;

                var strData = JSON.stringify({
                    c_num: $("#hdnCNum").val(),
                    fileNames: [selected.Filename]
                });

                App.Utils.ShowLoading();

                var request = $.ajax({
                    url: App.Config.BaseUrl + "customerFolder/Delete",
                    type: "POST",
                    data: strData,
                    processData: false,
                    contentType: "application/json; charset=utf-8"
                })
                    .done(function (data) {
                        if (data.Result == "success") {
                            customerFile.trigger("changed");
                            App.Utils.ShowAlert("File has been deleted successfully.");
                            customerFile.refreshGrid();
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
                var grid = $("#grd").data("kendoGrid");
                //grid.dataSource.transport.options.read.data = { "c_num": $("#hdnCNum").val() };
                grid.dataSource.read();
            },

            onClose: function (e) {
                if (e) e.preventDefault();
                $("#editWindow").data("kendoWindow").close();
            }

        });

        return customerFile;
    }
);




