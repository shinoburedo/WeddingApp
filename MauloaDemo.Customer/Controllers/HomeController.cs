using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.SqlTypes;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Threading;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Utilities;
using PagedList;
//using PagedList;
//using MauloaDemo.Utilities;
//using MauloaDemo.Customer.ViewModels;
//using MauloaDemo.Models;
//using WtbApi.Data.Models.Destination;
//using WtbApi.Data.Repository;
//using WtbApi.Proxy.All;
//using WtbApi.Proxy.Destination;

namespace MauloaDemo.Customer.Controllers {
    public class HomeController : BaseController {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            ////SEO対策のため meta name='description'タグを入れる。
            //ViewBag.MetaDescription = TypeHelper.GetStr(ConfigurationManager.AppSettings["MetaDescription_Home"]);
            ////SEO対策のため meta name='keywords'タグを入れる。
            //ViewBag.MetaKeywords = TypeHelper.GetStr(ConfigurationManager.AppSettings["MetaKeywords_Home"]);

            return IndexLanguage(this.CurrentRegionInfo.CurrentLanguage);
        }

        [Route("{language}")]
        public ActionResult IndexLanguage(string language)
        {
            ////SEO対策のため meta name='description'タグを入れる。
            //ViewBag.MetaDescription = TypeHelper.GetStr(ConfigurationManager.AppSettings["MetaDescription_Home"]);
            ////SEO対策のため meta name='keywords'タグを入れる。
            //ViewBag.MetaKeywords = TypeHelper.GetStr(ConfigurationManager.AppSettings["MetaKeywords_Home"]);

            return View("Index");
        }

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult AreaMenu() {
        //    var model = new RegionConfigRepository(this.CurrentRegionInfo.CurrentLanguage).GetList(null, true, false, true)
        //                         .ToList();
        //    foreach (var region in model) {
        //        var area_repo = new AreaRepository(region.region_cd);
        //        foreach (var area in region.areas) {
        //            area.image_upload_date = area_repo.Find(area.area_cd).image_upload_date;
        //        }
        //    }
        //    return PartialView("_AreaMenu", model);
        //}

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult AreaSideMenu() {
        //    var model = new RegionConfigRepository(this.CurrentRegionInfo.CurrentLanguage).GetList(null, true, false, true)
        //                         .ToList();
        //    return PartialView("_AreaSideMenu", model);
        //}

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ItemMenu()
        {
            var repo = new ItemTypeRepository();
            var model = repo.GetListForCustomer(this.CurrentRegionInfo.CurrentLanguage);

            return PartialView("_ItemMenu", model);
        }

        [AllowAnonymous]
        public ActionResult ItemList(int? page)
        {
            var region_cd = this.CurrentRegionInfo.CurrentDestination;
            var area_cd = this.CurrentSelectItem.area_cd;
            var item_type = this.CurrentSelectItem.item_type;
            var lang = this.CurrentRegionInfo.CurrentLanguage;
            string church_cd = null;
            var agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["AgentCd"]);

            ////ログイン済みでかつ確定済のPKGがあったらそのchurch_cdで表示するオプション商品を絞り込む。
            //if (this.CurrentLoginUser != null)
            //{
            //    var wtBookingRepo = new WtbApi.Proxy.All.WtBookingRepository();
            //    var c_num = this.CurrentLoginUser.c_num;
            //    if (!string.IsNullOrEmpty(c_num))
            //    {
            //        church_cd = wtBookingRepo.GetConfirmedWeddingChurchCd(region_cd, c_num, area_cd);
            //    }
            //}

            //商品一覧を取得。
            var model = new CItemRepository()
                                .GetItemList(item_type, lang, church_cd, this.CurrentSelectItem.wed_date, agent_cd);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            //return PartialView("_ItemList", model);
            return PartialView("_ItemList", model.ToPagedList(pageNumber, pageSize));
        }

        //[AllowAnonymous]
        //[ChildActionOnly]
        //public ActionResult TypeList() {
        //    var model = new WtbApi.Proxy.Destination.TrfTypeRepository(this.CurrentRegionInfo.CurrentDestination)
        //                        .GetTrfTypes(area_cd: this.CurrentSelectItem.area_cd,
        //                                     period_id: this.CurrentSelectItem.period_id, 
        //                                     trf_kind: null,
        //                                     open_to_cust_only: true,
        //                                     lang: this.CurrentRegionInfo.CurrentLanguage);
        //    return PartialView("_TypeList", model);
        //}

        //[Route("Types/{region_cd}/{area_cd}")]
        //[Route("Types/{region_cd}/{area_cd}/{str_wed_date}")]
        //public ActionResult Types(string region_cd, string area_cd, string str_wed_date = null, string lang = null) {

        //    if (!string.IsNullOrEmpty(lang)) {
        //        this.CurrentRegionInfo.CurrentLanguage = lang;
        //    }
        //    if (this.CurrentSelectItem != null && this.CurrentSelectItem.session_prior) {
        //        //Login直後でPKGアイアムを購入済の場合は、wed_dateをPKGのwed_dateにする
        //        this.CurrentSelectItem.session_prior = false;
        //        SetInformation(this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, this.CurrentSelectItem.str_wed_date, null, null);
        //        return SelectWeddingDate(this.CurrentSelectItem.wed_date, this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, true, false);
        //    }
        //    if (string.IsNullOrEmpty(str_wed_date)) {
        //        DateTime wed_date = RegionConfig.GetRegionToday(region_cd).AddMonths(2);
        //        if (this.CurrentSelectItem != null && this.CurrentSelectItem.wed_date != new DateTime()) {
        //            wed_date = this.CurrentSelectItem.wed_date;
        //        }
        //        str_wed_date = wed_date.ToString("yyyyMMdd");
        //    }
        //    SetInformation(region_cd, area_cd, str_wed_date, null, null);

        //    ViewBag.RegionName = this.CurrentRegionInfo.GetRegionName(IsJPN());
        //    ViewBag.AreaName = IsJPN() ? RegionConfig.GetAreaNameJpn(region_cd, area_cd)
        //                           : RegionConfig.GetAreaName(region_cd, area_cd);
        //    var fb_url = "Types/" + region_cd + "/" + area_cd + "?lang=" + CurrentRegionInfo.CurrentLanguage;
        //    ViewBag.FbUrl = fb_url.Replace("//", "");

        //    return View();
        //}

        //[Route("{region_cd}/{area_cd}/{trf_kind}/{trf_type}")]
        //public ActionResult DefaultItems(string region_cd, string area_cd, string trf_kind, string trf_type, string lang = null) {
        //    DateTime default_date = RegionConfig.GetRegionToday(region_cd).AddMonths(2);
        //    if (!string.IsNullOrEmpty(lang)) {
        //        this.CurrentRegionInfo.CurrentLanguage = lang;
        //    }
        //    return Items(region_cd, area_cd, default_date.ToString("yyyyMMdd"), trf_kind, trf_type);
        //}

        [Route("{language}/items/{item_type}")]
        public ActionResult Items(string language, string item_type, string str_wed_date, int? page_number = 1)
        {
            //if (this.CurrentSelectItem != null && this.CurrentSelectItem.session_prior)
            //{
            //    //Login直後でPKGアイアムを購入済の場合は、wed_dateをPKGのwed_dateにする
            //    this.CurrentSelectItem.session_prior = false;
            //    SetInformation(this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, this.CurrentSelectItem.str_wed_date, item_type);
            //    return SelectWeddingDate(this.CurrentSelectItem.wed_date, this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, true, false);
            //}



            //SetInformation(this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, str_wed_date, item_type);
            //ViewBag.PageNum = page_number;




            //ViewBag.RegionName = this.CurrentRegionInfo.GetRegionName(IsJPN());
            //ViewBag.AreaName = IsJPN() ? RegionConfig.GetAreaNameJpn(region_cd, area_cd)
            //                       : RegionConfig.GetAreaName(region_cd, area_cd);
            //var fb_url = region_cd + "/" + area_cd + "/" + trf_kind + "/" + trf_type + "?lang=" + CurrentRegionInfo.CurrentLanguage;
            //ViewBag.FbUrl = fb_url.Replace("//", "");

            return View();

        }

        ///// <summary>
        ///// 言語が選択された時の処理。
        ///// </summary>
        ///// <param name="region_cd"></param>
        ///// <returns></returns>
        //public ActionResult SelectLanguage(string language, string ReturnUrl) {

        //    if (!string.IsNullOrWhiteSpace(language)) {
        //        this.CurrentRegionInfo.CurrentLanguage = language;
        //        Session[Constants.SSKEY_CURRENT_REGION] = this.CurrentRegionInfo;

        //        //Cookieに言語設定を保存。(LocalStorageだと１回めのリクエストと同時に送る事が出来ないので、この用途にはCookieの方が良い。)
        //        SetLanguageCookie(language);
        //    }

        //    if (string.IsNullOrWhiteSpace(ReturnUrl)) {
        //        //クロールエラー対策。検索エンジンロボットの場合はUrlReferrerはnullなのでその場合に500エラーにならない様に。
        //        if (Request.UrlReferrer != null) {
        //            ReturnUrl = Request.UrlReferrer.ToString();
        //        } else { 
        //            ReturnUrl = "";
        //        }
        //    }
        //    return RedirectToLocal(ReturnUrl);
        //}

        ///// <summary>
        ///// 希望挙式日が選択された時の処理。
        ///// </summary>
        ///// <param name="region_cd"></param>
        ///// <returns></returns>
        //public ActionResult SelectWeddingDate(DateTime? wed_date = null, string region_cd = null, string area_cd = null, bool item_list_display = false, bool type_list_display = false) {

        //    region_cd = string.IsNullOrEmpty(region_cd) ? this.CurrentRegionInfo.CurrentDestination : region_cd;
        //    area_cd = string.IsNullOrEmpty(area_cd) ? this.CurrentSelectItem.area_cd : area_cd;

        //    if (string.IsNullOrEmpty(region_cd)) {
        //        region_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultRegion"]);
        //    }
        //    if (string.IsNullOrEmpty(area_cd)) {
        //        area_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultArea"]);
        //    }
        //    if (wed_date == null || wed_date.Value < SqlDateTime.MinValue.Value) {
        //        var default_date = RegionConfig.GetRegionToday(region_cd).AddMonths(2);
        //        wed_date = (this.CurrentSelectItem.wed_date == DateTime.MinValue) ? default_date : this.CurrentSelectItem.wed_date;
        //    }

        //    //URLを編集
        //    var current_url = TypeHelper.GetStr(ConfigurationManager.AppSettings["BaseURL"]);
        //    if (type_list_display) {
        //        current_url += "/Types";
        //    } else if (item_list_display) {
        //        if (String.IsNullOrEmpty(this.CurrentSelectItem.trf_type)) {
        //            current_url += "/Types";
        //        //} else {
        //        //    current_url += "/Items";
        //        }
        //    } else if (String.IsNullOrEmpty(this.CurrentSelectItem.trf_type)) {
        //        current_url += "/Types";
        //    } else if (!String.IsNullOrEmpty(this.CurrentSelectItem.trf_item_cd)) {
        //        current_url += "/Detail";
        //    //} else {
        //    //    current_url += "/Items";
        //    }
        //    current_url += "/" + region_cd;
        //    current_url += "/" + area_cd;
        //    var str_wed_date = wed_date.Value.ToString("yyyyMMdd");
        //    current_url += "/" + str_wed_date;

        //    if (!type_list_display && !String.IsNullOrEmpty(this.CurrentSelectItem.trf_type)) {
        //        current_url += "/" + this.CurrentSelectItem.trf_kind;
        //        current_url += "/" + this.CurrentSelectItem.trf_type;
        //        if (!item_list_display && !String.IsNullOrEmpty(this.CurrentSelectItem.trf_item_cd)) {
        //            current_url += "/" + this.CurrentSelectItem.trf_cat;
        //            current_url += "/" + this.CurrentSelectItem.trf_item_cd;
        //        }
        //    }
        //    return RedirectToLocal(current_url);
        //}

        //[Route("Detail/{region_cd}/{area_cd}/{trf_kind}/{trf_type}/{trf_cat}/{trf_item_cd}")]
        //public ActionResult DefaultDetail(string region_cd, string area_cd, string trf_kind, string trf_type, string trf_cat, string trf_item_cd, string lang = null) {
        //    DateTime default_date = RegionConfig.GetRegionToday(region_cd).AddMonths(2);
        //    if (!string.IsNullOrEmpty(lang)) {
        //        this.CurrentRegionInfo.CurrentLanguage = lang;
        //    }
        //    return Detail(region_cd, area_cd, default_date.ToString("yyyyMMdd"), trf_kind, trf_type, trf_cat, trf_item_cd);
        //}

        //[Route("Detail/{region_cd}/{area_cd}/{str_wed_date}/{trf_kind}/{trf_type}/{trf_cat}/{trf_item_cd}/{page_number:int?}")]
        //public ActionResult Detail(string region_cd, string area_cd, string str_wed_date, string trf_kind, string trf_type, string trf_cat, string trf_item_cd, short? page_number = 1) {
        //    if (this.CurrentSelectItem.session_prior) {
        //        //Login直後でPKGアイアムを購入済の場合は、wed_dateをPKGのwed_dateにする
        //        this.CurrentSelectItem.session_prior = false;
        //        SetInformation(this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, this.CurrentSelectItem.str_wed_date, trf_kind, trf_type, trf_cat, trf_item_cd);
        //        return SelectWeddingDate(this.CurrentSelectItem.wed_date, this.CurrentRegionInfo.CurrentDestination, this.CurrentSelectItem.area_cd, false, false);
        //    }
        //    SetInformation(region_cd, area_cd, str_wed_date, trf_kind, trf_type, trf_cat, trf_item_cd);

        //    //表示通貨判断
        //    var is_japan = this.IsJpnClientOrJpnAccount();

        //    var lang = this.CurrentRegionInfo.CurrentLanguage;

        //    //Item詳細取得
        //    var model = new WtbApi.Proxy.Destination.TrfItemRepository(region_cd)
        //                    .FindItem(
        //                        area_cd,
        //                        this.CurrentSelectItem.period_id,
        //                        trf_kind,
        //                        trf_type,
        //                        trf_cat,
        //                        trf_item_cd, 
        //                        lang, 
        //                        is_japan, 
        //                        CurrentSelectItem.wed_date);

        //    ViewBag.PageNum = page_number;
        //    ViewBag.FbUrl = "Detail/" + region_cd + "/" + area_cd + "/" + trf_kind + "/" + trf_type + "/" + trf_cat + "/" + trf_item_cd + "?lang=" + CurrentRegionInfo.CurrentLanguage;
        //    return View("Detail", model);
        //}

        private void SetInformation(string region_cd, string area_cd, string str_wed_date, string item_type, string item_cd = null)
        {

            region_cd = TypeHelper.GetStr(region_cd).ToUpper();
            area_cd = TypeHelper.GetStr(area_cd).ToUpper();
            str_wed_date = TypeHelper.GetStr(str_wed_date).ToUpper();
            item_type = TypeHelper.GetStr(item_type).ToUpper();
            item_cd = TypeHelper.GetStr(item_cd).ToUpper();

            //CurrentRegionInfo
            //this.CurrentRegionInfo.CurrentLocation = region_cd;
            this.CurrentRegionInfo.CurrentDestination = region_cd;
            SetCurrentRegionInfo();

            DateTime wed_date;
            if (!DateTime.TryParseExact(str_wed_date, "yyyyMMdd", null,
                                        DateTimeStyles.None, out wed_date))
            {
                wed_date = RegionConfig.GetRegionToday(region_cd).AddMonths(2);
            }

            //現在日(現地時間)から3日後より前の日付は指定不可とする。
            var min_date = RegionConfig.GetRegionToday(region_cd).AddDays(3);
            if (wed_date < min_date)
            {
                wed_date = min_date;
            }

            //CurrentSelectItem
            this.CurrentSelectItem.area_cd = area_cd;
            this.CurrentSelectItem.wed_date = wed_date;
            this.CurrentSelectItem.min_date = min_date;
            this.CurrentSelectItem.item_type = item_type;
            this.CurrentSelectItem.item_cd = item_cd;
            Session[Constants.SSKEY_SELECT_ITEM] = this.CurrentSelectItem;
        }

        //public ActionResult Modal(string region_cd, string trf_type, string trf_item_cd, int selected_photo_no, DateTime? image_upload_date, int cnt_pic) {
        //    ViewBag.TrfItemCd = trf_item_cd;
        //    ViewBag.PhotoNo = selected_photo_no;
        //    ViewBag.image_upload_date = image_upload_date;
        //    ViewBag.CntPicture = cnt_pic;

        //    //try {
        //    //    var count = new WtbApi.Proxy.Destination.TrfItemRepository(region_cd)
        //    //                                .GetCountPictures(region_cd, trf_type, trf_item_cd);
        //    //} catch (Exception) {
        //    //    //クロールエラー対策。万が一API呼び出しに失敗しても500エラーにしない。
        //    //    ViewBag.CntPicture = selected_photo_no;
        //    //}
        //    return View();
        //}


        //public ActionResult Types() {
        //    return View();
        //}

        //public ActionResult Items() {
        //    return View();
        //}

        //public ActionResult Help() {
        //    return View();
        //}

        //public ActionResult About() {
        //    return View();
        //}

        //public ActionResult Guide() {
        //    return View();
        //}

        //public ActionResult Data() {
        //    return View();
        //}

        //public ActionResult Term() {
        //    return View();
        //}

        //public ActionResult Privacy() {
        //    return View();
        //}

        //public ActionResult CxlPolicy() {
        //    return View();
        //}

        //public ActionResult Faq() {
        //    if (IsJPN()) {
        //        return View();
        //    } else {
        //        return View("FaqEng");
        //    }
        //}

        //public ActionResult Precaution() {
        //    return View();
        //}

        //public ActionResult Contact() {
        //    var model = new MauloaDemo.Customer.ViewModels.ContactMessage();

        //    var regions = new RegionConfigRepository(this.CurrentRegionInfo.CurrentLanguage)
        //                         .GetList(null, true, false, true)
        //                         .ToList();

        //    List<RegionAreaViewModel> areas = regions.SelectMany(r => r.areas,
        //                                (r, a) => new RegionAreaViewModel() {
        //                                    region_cd = r.region_cd,
        //                                    area_cd = a.area_cd,
        //                                    area_name = r.region_name_dis + " - " + a.area_name_dis
        //                                })
        //                       .Where(a => {
        //                           //web_emailが入っているエリアだけを抽出。
        //                           var areaRepo = new WtbApi.Proxy.Destination.AreaRepository(a.region_cd);
        //                           var area = areaRepo.Find(a.area_cd);
        //                           if (area == null) return false;
        //                           return !string.IsNullOrEmpty(area.web_email);
        //                       }).ToList();
        //    ViewBag.Areas = areas;

        //    if (this.CurrentLoginUser != null) {
        //        var user = this.CurrentLoginUser;
        //        model.Name = user.CustomerName;
        //        model.Email = user.email;

        //        if (!string.IsNullOrEmpty(user.c_num)) {
        //            var wtBookingRepo = new WtbApi.Proxy.All.WtBookingRepository();

        //            //wt_bookingの一覧を取得。(PKGのみ)
        //            var pkg_item = wtBookingRepo.GetPkgBooking(user.account_id);
        //            if (pkg_item != null) { 
        //                model.area_cd = pkg_item.area_cd;
        //            }
        //        }
        //    }

        //    return View(model);
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public JsonResult PostContactMessage(MauloaDemo.Customer.ViewModels.ContactMessage contactMessage) {
        //    var result = new Dictionary<string, object>();

        //    if (contactMessage == null || !ModelState.IsValid) {
        //        result.Add("result", "invalid");
        //        var list = new List<KeyValuePair<string, string>>();
        //        foreach (var state in ModelState) {
        //            if (state.Value.Errors.Count > 0) {
        //                for (var i = 0; i < state.Value.Errors.Count; i++) {
        //                    var e = state.Value.Errors[i];
        //                    list.Add(new KeyValuePair<string, string>(state.Key + i.ToString(), e.ErrorMessage));
        //                }
        //            }
        //        }
        //        result.Add("errors", list);
        //        return Json(result);
        //    } 

        //    try {
        //        var region_cd = RegionConfig.GetRegionCdFromAreaCd(contactMessage.area_cd);
        //        var areaRepo = new WtbApi.Proxy.Destination.AreaRepository(region_cd);
        //        var area = areaRepo.Find(contactMessage.area_cd);
        //        if (area == null) {
        //            throw new Exception(IsJPN() ? "エリアが正しくありません。" : "Invalid area.");
        //        }
        //        if (string.IsNullOrEmpty(area.web_email)) {
        //            throw new Exception(IsJPN() ? "このエリアには宛先のメールアドレスが設定されていません。" : "This area has no email address.");
        //        }
        //        area.description = RegionConfig.GetAreaName(region_cd, area.area_cd);
        //        area.area_name_jpn = RegionConfig.GetAreaNameJpn(region_cd, area.area_cd);
        //        area.area_name_dis = IsJPN() ? area.area_name_jpn : area.description;

        //        var repo = new WtbApi.Proxy.All.WtEmailQueueRepository();
        //        var clientIp = Request.UserHostAddress;

        //        //現地店舗に送るメッセージを送信キューに追加。
        //        string message = (this.CurrentLoginUser != null ? "Customer#: " + this.CurrentLoginUser.c_num + "\r\n" : "");
        //        message += "Name: " + contactMessage.Name + "\r\n";
        //        message += "Email: " + contactMessage.Email + "\r\n";
        //        message += "Area: " + contactMessage.area_cd + "\r\n\r\n";
        //        message += contactMessage.Message;
        //        var msg = new WtEmailQueue() { 
        //            from_addr = TypeHelper.GetStr(ConfigurationManager.AppSettings["ContactEmailFrom"]),
        //            to_addr = area.web_email,
        //            replyto_addr = contactMessage.Email,
        //            subject = "[WATABE.COM] " + contactMessage.Subject,
        //            body = message
        //        };
        //        repo.Insert(msg, null);

        //        //カスタマーに送るメッセージを送信キューに追加。
        //        message = (IsJPN() ? "下記のお問合せ内容を" + area.area_name_jpn + "エリア担当の現地店舗に送信しました。" 
        //                           : "The following message has been sent to our branch office in " + area.description + ".")  + "\r\n\r\n"
        //                    + "Name: " + contactMessage.Name + "\r\n"
        //                    + "Email: " + contactMessage.Email + "\r\n\r\n"
        //                    + contactMessage.Message;
        //        var msg_cust = new WtEmailQueue() {
        //            from_addr = TypeHelper.GetStr(ConfigurationManager.AppSettings["ContactEmailFrom"]),
        //            to_addr = contactMessage.Email,
        //            replyto_addr = area.web_email,
        //            subject = "[WATABE.COM] " + (IsJPN() ? "お問合せ有り難うございます。" : "Thank you for contacting us."),
        //            body = message
        //        };
        //        repo.Insert(msg_cust, clientIp);

        //        Thread.Sleep(3000);     //自動POSTを抑止するために少し間を空ける。

        //        result.Add("result", "ok");
        //    } catch (Exception ex) {
        //        result.Add("result", "ng");
        //        result.Add("error", ex.Message);
        //    }

        //    return Json(result);
        //}

        //public ActionResult GetChurchAvailList(DateTime? wed_date, string trf_item_cd, string area_cd, string trf_kind, string trf_type, string trf_cat) {
        //    var list = new List<WtbApi.Data.DTO.Destination.ChurchAvail>();

        //    //クロールエラー対策
        //    if (!wed_date.HasValue || string.IsNullOrEmpty(trf_item_cd) || string.IsNullOrEmpty(area_cd)
        //        || string.IsNullOrEmpty(trf_kind) || string.IsNullOrEmpty(trf_type) || string.IsNullOrEmpty(trf_cat)) {
        //        //下で500エラーになるのを避けるため0件のリストをJSONで返す。
        //        return Json(list, JsonRequestBehavior.AllowGet);        
        //    }

        //    var repository = new WtbApi.Proxy.Destination.TrfItemRepository(this.CurrentRegionInfo.CurrentDestination);
        //    list = repository.GetChurchAvailList(
        //                        wed_date.Value, 
        //                        null, 
        //                        trf_kind, 
        //                        trf_type, 
        //                        trf_cat, 
        //                        area_cd, 
        //                        trf_item_cd)
        //                            .ToList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetReviewList(string region_cd, string area_cd, string trf_kind, string trf_type, string trf_cat, string trf_item_cd) {
        //    var list = new List<WtReview>();

        //    //クロールエラー対策
        //    if (string.IsNullOrEmpty(region_cd) || string.IsNullOrEmpty(area_cd)
        //            || string.IsNullOrEmpty(trf_kind) || string.IsNullOrEmpty(trf_type) || string.IsNullOrEmpty(trf_cat)
        //            || string.IsNullOrEmpty(trf_item_cd)) {
        //        //下で500エラーになるのを避けるため0件のリストをJSONで返す。
        //        return Json(list, JsonRequestBehavior.AllowGet);        
        //    } 

        //    var repository = new WtbApi.Proxy.All.WtReviewRepository();
        //    list = repository.GetReviewList(
        //                        trf_kind,
        //                        trf_type,
        //                        trf_cat,
        //                        trf_item_cd,
        //                        region_cd,
        //                        area_cd,
        //                        this.CurrentSelectItem.period_id)
        //                            .ToList();

        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

    }
}