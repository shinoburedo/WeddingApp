using System.Web;
using System.Web.Mvc;

namespace MauloaDemo.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //全ての画面に認証を要求する。(APIについてはWebApiConfig.csにて設定。)
            filters.Add(new AuthorizeAttribute());

            //コントローラがJsonResultを返す場合に日付型のフィールドで独自のシリアライズを行う。（サーバーとクライアントでTimeZoneが異なると日時がずれる問題の対処。）
            filters.Add(new JsonDateFormatFilterAttribute());
        }
    }
}