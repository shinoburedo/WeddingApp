jQuery(function($) {

//タブ切替
	$(".tabArea li").click(function() {
		$(".tabArea li").removeClass('current');
		var num = $(".tabArea li").index(this);
		$(".tabContens").addClass('disnon');
		$(".tabContens").eq(num).removeClass('disnon');
        $(this).addClass('current');
	});

});