/*
    Script for Contact Form.
*/
$(function () {
    var form = $(".contact-form");
    var parent = form.parent();
    var btn = form.find(".contact-button");
    btn.on("click", function (e) {
        e.preventDefault();
        form.submit();
    });

    form.on("submit", function (e) {
        e.preventDefault();
        if (!form.valid()) return;

        var email = $("#Email").val();
        var email_conf = $("#EmailConfirm").val();
        if (email != email_conf) {
            var msg = App.L("Emailアドレスを正しく入力してください。", "Enter a correct email adress. ");
            alert(msg);
            return false;
        }

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            beforeSend: beginSend
        })
        .done(function (e) {
            if (e.result === "ok") {
                showThanks();
            } else {
                showErr(null, null, e.error);
            }
        })
        .fail(showErr)
        .always(endSend);

        function beginSend() {
            parent.find(".error").html("");
            form.find(".spnWait").addClass("waiting");
            btn.addClass("waiting");
        }

        function endSend() {
            form.find(".spnWait").removeClass("waiting");
        }

        function showThanks() {
            form.fadeOut(300, function () {
                parent.find(".thanks").fadeIn(300, null);
            });
        }

        function showErr(jqXHR, textStatus, errorThrown) {
            var msg = App.L("エラーが発生しました。", "An error occurred. ") + "<br />" + errorThrown;
            parent.find(".error").html(msg);

            btn.removeClass("waiting");
        }
    });

});

