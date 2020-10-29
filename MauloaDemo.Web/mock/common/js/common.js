$(function(){
    // #で始まるアンカーをクリックした場合に処理
   $('a[href^=#]').click(function() {
      // スクロールの速度
      var speed = 400; // ミリ秒
      // アンカーの値取得
      var href= $(this).attr("href");
      // 移動先を取得
      var target = $(href == "#" || href == "" ? 'html' : href);
      // 移動先を数値で取得
      var position = target.offset().top;
      // スムーススクロール
      $('body,html').animate({scrollTop:position}, speed, 'swing');
      return false;
   });

    $("#datepicker").datepicker();
    $("#datepicker").datepicker("option", "showOn", 'both');
    $("#datepicker").datepicker("option", "buttonImageOnly", true);
    $("#datepicker").datepicker("option", "buttonImage", 'common/img/icon_calendar.png');

    $('.btn_Menu').on('click', function() {
        if(!$('.sidebarMenu').hasClass('is-slideOn')){
            $('.bg_overlay').addClass('is-visible');
            $('.sidebarMenu').addClass('is-slideOn');
        }else{
            $('.bg_overlay').removeClass('is-visible');
            $('.sidebarMenu').removeClass('is-slideOn');
        }
    });

    //全体クリックで閉じる処理
    $('.bg_overlay').on('click', function() {
        $('.bg_overlay').removeClass('is-visible');
        $('.sidebarMenu').removeClass('is-slideOn');
    });

});


custom_selectbox = function(select, obj){
    var set_selectbox = function(){
        var value = jQuery(this).find('option:selected').html();
        $(this).parent().find(obj).find('span').html(value);
    }
    jQuery(select).find('select').each(set_selectbox).on('change', set_selectbox);
}
 
jQuery(function(){
    custom_selectbox('.custom-selectbox', '.custom-inner');
});
