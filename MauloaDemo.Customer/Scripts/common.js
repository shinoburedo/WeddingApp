jQuery(function($) {

    //プラットフォーム振り分け
	if (navigator.userAgent.indexOf('iPhone') > 0){
		$("html").addClass("iphone");
	}
	if (navigator.userAgent.indexOf('iPod') > 0){
		$("html").addClass("iPod");
	}
	if (navigator.userAgent.indexOf('Android') > 0){
		$("html").addClass("Android");
	}

    //ページ内リンク
	$("a[href^=#]"+".aScroll").click(function(){
		var Hash = $(this.hash);
		var HashOffset = $(Hash).offset().top;
		$("html,body").animate({
			scrollTop: HashOffset
		}, 100);
		return false;
	});

    //ページトップ非表示
    $('#btnPagetop').css('display','none');
	$(window).scroll(function () {
		if ($(this).scrollTop() > 100) {
			$('#btnPagetop').fadeIn();
		} else {
			$('#btnPagetop').fadeOut();
		}
	});
	
});

