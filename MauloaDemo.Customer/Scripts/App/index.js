
(function ($) {

    //マウスオーバーで画像を変えるエフェクト。
    $.fn.ChangeImg = function (options) {
        var defaults = {
            delay: 700      //ms
        };
        var setting = $.extend(defaults, options);

        var busy_flag = false;
        var lastPos = { x: 0, y: 0 };

        this.on("mousemove", function (e) {
            if (busy_flag) return;
            if (e.pageX == lastPos.x && e.pageY == lastPos.y) return;
            lastPos.x = e.pageX;
            lastPos.y = e.pageY;

            var parent = $(this).parent();
            var imgs = parent.find("img");
            var index = imgs.index(this);

            $(this).removeClass("shown").addClass("hidden");

            index++;
            if (index > imgs.length - 1) index = 0;

            imgs.eq(index).removeClass("hidden").addClass("shown");
            busy_flag = true;
            console.log(setting);

            setTimeout(function () {
                busy_flag = false;
            }, setting.delay);

        });

        //mouseout時は最初の画像を表示する
        $(this).on("mouseout", function (e) {
            if (this.className === "hidden") return;
            $(this).removeClass("shown").addClass("hidden");
            $(this).parent().find("img").eq(0).removeClass("hidden").addClass("shown");
        });

        return this;
    };

    $(function () {
        var images = $("#list_content a img");
        images.ChangeImg({ delay: 700 });
    });

})(jQuery);


