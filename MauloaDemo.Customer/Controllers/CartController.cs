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
using MauloaDemo.Models.Combined;
//using PagedList;
//using MauloaDemo.Utilities;
//using MauloaDemo.Customer.ViewModels;
//using MauloaDemo.Models;
//using WtbApi.Data.Models.Destination;
//using WtbApi.Data.Repository;
//using WtbApi.Proxy.All;
//using WtbApi.Proxy.Destination;

namespace MauloaDemo.Customer.Controllers {
    public class CartController : BaseController {
        public ActionResult GetCartCount()
        {
            var hash = new Dictionary<string, object>();
            string failureType = "error";

            try
            {
                var count = 0;
                if (this.CurrentLoginUser == null)
                {
                    //未ログインだったらセッションから取得
                    var cartList = Session[Constants.SSKEY_CART] as List<CBooking>;
                    if (cartList != null)
                    {
                        count = cartList.Count();
                    }
                }
                else
                {
                    //ログイン済だったらDBから取得
                    var wtBookingRepo = new CBookingRepository();
                    count = wtBookingRepo.GetCountByStatus(CBooking.PAYSTATUS_INIT, this.CurrentLoginUser.account_id);
                }
                hash.Add("Count", count);
                hash.Add("Result", "success");
            }
            catch (Exception ex)
            {
                hash.Add("Result", failureType);
                hash.Add("Message", ex.Message);
            }
            return Json(hash);
        }

        public ActionResult AddCart(CustomerItem currentItem, short quantity, short? fixed_qty, DateTime wed_date, string wed_time, bool is_reserve)
        {
            var hash = new Dictionary<string, object>();
            string failureType = "error";

            try
            {
                var cartList = Session[Constants.SSKEY_CART] as List<CBooking>;
                if (cartList != null)
                {
                    //PKGの場合、既に同じアイテムがカートにあったらNG(異なるperiodでもNG)
                    if (currentItem.is_pkg)
                    {
                        var exists = cartList.Any(m => m.item_cd == currentItem.item_cd);
                        if (exists)
                        {
                            hash.Add("Result", "error");
                            hash.Add("Message", L("You've already selected same item.", "既に同じ商品がカートに入っています。"));
                            return Json(hash);
                        }
                    }
                }

                //表示通貨判断
                var is_japan = this.IsJpnClientOrJpnAccount();

                var lang = this.CurrentRegionInfo.CurrentLanguage;
                var region_cd = this.CurrentRegionInfo.CurrentDestination;

                //Item詳細取得
                var item = new CItemRepository()
                                .FindItem(
                                    currentItem.item_cd,
                                    lang,
                                    wed_date);

                int account_id = this.CurrentLoginUser == null ? 0 : this.CurrentLoginUser.account_id;
                string c_num = this.CurrentLoginUser == null ? null : this.CurrentLoginUser.c_num;

                DateTime? wed_time_d = null;
                if (!String.IsNullOrEmpty(wed_time))
                {
                    wed_time_d = System.DateTime.ParseExact(wed_time, "HH:mm", null);
                }
                var booking = EditCBooking(item, account_id, c_num, quantity, wed_date, wed_time_d, is_reserve, currentItem);

                if (this.CurrentLoginUser != null)
                {
                    //ログイン済の場合DB保存
                    var cBookingRepo = new CBookingRepository();
                    cBookingRepo.AddCart(booking);
                    cartList = cBookingRepo.GetBookingList(CBooking.PAYSTATUS_INIT, this.CurrentLoginUser.account_id, IsJPN());
                }
                else
                {
                    //未ログインの場合、セッションの情報を更新
                    if (cartList == null)
                    {
                        cartList = new List<CBooking>();
                    }
                    var exist_booking = cartList.Where(m => m.account_id == booking.account_id
                        && m.area_cd == booking.area_cd
                        && m.item_type == booking.item_type
                        && m.alb_cover == booking.alb_cover && m.alb_mount == booking.alb_mount && m.alb_type == booking.alb_type && m.dvd_menucolor == booking.dvd_menucolor
                        && m.item_cd == booking.item_cd && m.payment_status == CBooking.PAYSTATUS_INIT).FirstOrDefault();

                    if (exist_booking == null || (exist_booking.fixed_qty >= 1 || exist_booking.fixed_qty == null))
                    {
                        booking.booking_id_for_session = cartList.Count() + 1;
                        cartList.Add(booking);
                    }
                    else
                    {
                        //同じItemがカートに入っていたらquantityを更新(ただしfixed_qtyが数量固定のものは別レコードとしてカートに追加)
                        var total = exist_booking.quantity + booking.quantity;
                        exist_booking.quantity = (short)total;
                    }
                }
                Session[Constants.SSKEY_CART] = cartList;
                var total_quantity = (short)cartList.Count();
                this.CurrentSelectItem.wed_date = wed_date;

                hash.Add("Result", "success");
                hash.Add("Count", total_quantity);
            }
            catch (Exception ex)
            {
                //ex = Common.GetMostInnerException(ex);
                hash.Add("Result", failureType);
                hash.Add("Message", ex.Message);
            }
            return Json(hash);
        }

        private CBooking EditCBooking(
                            CustomerItem item,
                            int account_id,
                            string c_num,
                            short quantity,
                            DateTime wed_date,
                            DateTime? wed_time,
                            bool is_reserve,
                            CustomerItem currentItem)
        {

            //TrfItemDao.ItemInfo型からCBooking型にコピー
            var booking = new CBooking();
            booking.item_cd = item.item_cd;
            booking.item_name = item.item_name;
            booking.account_id = account_id;
            booking.c_num = c_num;
            booking.area_cd = item.area_cd;
            //booking.area_name = this.CurrentRegionInfo.CurrentLanguage == "J" ? RegionConfig.GetAreaNameJpn(this.CurrentRegionInfo.CurrentDestination, item.area_cd) : RegionConfig.GetAreaName(this.CurrentRegionInfo.CurrentDestination, item.area_cd);
            string agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BookingAgentCd"]);
            string sub_agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BookingSubAgentCd"]);
            booking.agent_cd = agent_cd;
            booking.sub_agent_cd = sub_agent_cd;
            booking.item_type = item.item_type;
            booking.wed_date = wed_date;
            booking.quantity = quantity;
            booking.price = item.price.HasValue ? item.price.Value : (Decimal)0;
            booking.price_new = booking.price;
            booking.price_type = item.price_type.HasValue ? item.price_type.Value : (short)0;
            booking.price_cur = item.price_currency;
            booking.price_cur_new = booking.price_cur;
            //通貨表記
            booking.curfmt_withsymbol = RegionConfig.GetCurrencyFormatWithSymbol(this.CurrentRegionInfo.CurrentDestination);
            if (String.IsNullOrEmpty(booking.curfmt_withsymbol))
            {
                booking.curfmt_withsymbol = booking.price_cur_new + RegionConfig.GetCurrencyFormat(this.CurrentRegionInfo.CurrentDestination);
            }
            booking.cnt_picture_s = item.cnt_picture_s;
            booking.image_upload_date = item.image_upload_date;
            booking.service_date = wed_date;
            booking.service_time = wed_time;
            booking.rcp_private_room = false;
            booking.rcp_room_id = 0;
            booking.payment_status = CBooking.PAYSTATUS_INIT;
            //booking.fixed_qty = item.fixed_qty;
            booking.church_cd = item.church_cd;
            booking.reserve_pkg = is_reserve;

            if (booking.item_type == "DVD")
            {
                booking.dvd_menucolor = currentItem.alb_cover;
            }
            else
            {
                booking.alb_cover = currentItem.alb_cover;
            }
            booking.alb_mount = currentItem.alb_mount;
            booking.alb_type = currentItem.alb_type;
            booking.last_person = Constants.LAST_PERSON;

            booking.app_cd = CBooking.APPCD_WATABECOM_PC;  //watabe.comのPC向けサイト

            return booking;
        }

        //public ActionResult Index(string error, bool displayNavigation = false)
        //{
        //    if (!string.IsNullOrEmpty(error))
        //    {
        //        ViewBag.Message = error;
        //    }
        //    SetPaymentMaintInf();
        //    if (displayNavigation)
        //    {
        //        ViewBag.DisplayNavigation = true;
        //    }
        //    else
        //    {
        //        ViewBag.DisplayNavigation = false;
        //    }

        //    return View();
        //}

        //public JsonResult GetBookingList()
        //{
        //    var list = new List<CBooking>();
        //    var repository = new WtbApi.Proxy.All.CBookingRepository();
        //    if (this.CurrentLoginUser != null)
        //    {
        //        list = repository.GetBookingListByStatus(CBooking.PAYSTATUS_INIT, this.CurrentLoginUser.account_id, IsJPN());
        //    }
        //    else
        //    {
        //        list = Session[Constants.SSKEY_CART] as List<CBooking>;
        //        if (list != null)
        //        {
        //            list = repository.GetBookingDetails(list, IsJPN());
        //        }
        //        else
        //        {
        //            list = new List<CBooking>();
        //        }
        //    }
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult UpdateQuantity(int booking_id_for_session, int wt_id, short quantity)
        //{
        //    var hash = new Dictionary<string, object>();
        //    string failureType = "error";

        //    try
        //    {
        //        //セッションのリストを更新
        //        var list = Session[Constants.SSKEY_CART] as List<CBooking>;
        //        if (this.CurrentLoginUser != null)
        //        {
        //            var wtBookingRepo = new WtbApi.Proxy.All.CBookingRepository();
        //            wtBookingRepo.UpdateQuantity(this.CurrentLoginUser.account_id, wt_id, quantity, Constants.LAST_PERSON);

        //            list = wtBookingRepo.GetBookingListByStatus(CBooking.PAYSTATUS_INIT, this.CurrentLoginUser.account_id, IsJPN());
        //        }
        //        else
        //        {
        //            var del_item = list.Where(m => m.booking_id_for_session == booking_id_for_session).FirstOrDefault();
        //            del_item.quantity = quantity;
        //        }
        //        Session[Constants.SSKEY_CART] = list;
        //        var total_quantity = (short)list.Count();

        //        hash.Add("Result", "success");
        //        hash.Add("Count", total_quantity);
        //    }
        //    catch (Exception ex)
        //    {
        //        hash.Add("Result", failureType);
        //        hash.Add("Message", ex.Message);
        //    }
        //    return Json(hash);
        //}

        //public JsonResult Delete(int booking_id_for_session, int wt_id)
        //{
        //    var hash = new Dictionary<string, object>();
        //    string failureType = "error";

        //    try
        //    {
        //        //セッションのリストから削除
        //        var list = Session[Constants.SSKEY_CART] as List<CBooking>;
        //        if (this.CurrentLoginUser != null)
        //        {
        //            int account_id = this.CurrentLoginUser.account_id;
        //            //テーブルから削除
        //            var wtBookingRepo = new WtbApi.Proxy.All.CBookingRepository();
        //            wtBookingRepo.Delete(account_id, wt_id);

        //            //DBからカート内容の一覧を再取得
        //            list = wtBookingRepo.GetBookingListByStatus(CBooking.PAYSTATUS_INIT, account_id, IsJPN());
        //        }
        //        else
        //        {
        //            var del_item = list.Where(m => m.booking_id_for_session == booking_id_for_session).FirstOrDefault();
        //            list.Remove(del_item);
        //        }
        //        Session[Constants.SSKEY_CART] = list;
        //        var total_quantity = (short)list.Count();

        //        hash.Add("Result", "success");
        //        hash.Add("Count", total_quantity);
        //    }
        //    catch (Exception ex)
        //    {
        //        hash.Add("Result", failureType);
        //        hash.Add("Message", ex.Message);
        //    }
        //    return Json(hash);
        //}

        ////public ActionResult GetChurchAvailList(DateTime wed_date, string trf_item_cd, string region_cd) {
        ////    var repository = new WtbApi.Proxy.Destination.TrfItemRepository(region_cd);
        ////    var list = repository.GetChurchAvailList(wed_date, null, trf_item_cd);
        ////    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        ////    return Json(list, JsonRequestBehavior.AllowGet);
        ////}

        //private WtAccountInfo GetWtAccountInfo()
        //{
        //    var accountInfo = new WtAccountInfo();
        //    if (this.CurrentLoginUser != null)
        //    {
        //        accountInfo.c_num = this.CurrentLoginUser.c_num;
        //        accountInfo.last = this.CurrentLoginUser.e_last_name;
        //        accountInfo.first = this.CurrentLoginUser.e_first_name;
        //        accountInfo.c_num = this.CurrentLoginUser.c_num;
        //        accountInfo.email = this.CurrentLoginUser.email;
        //        accountInfo.account_id = this.CurrentLoginUser.account_id;
        //        accountInfo.is_groom = this.CurrentLoginUser.is_groom;

        //        if (!string.IsNullOrEmpty(accountInfo.c_num))
        //        {

        //            //JpnInfoを取得するため、どのエリアで申込み済かBookingのAreaで判定
        //            var repo_booking = new WtbApi.Proxy.All.CBookingRepository();
        //            var last_booking = repo_booking.GetLastBooking(accountInfo.c_num);
        //            if (last_booking != null)
        //            {
        //                var last_region_cd = last_booking.region_cd;
        //                var last_area_cd = last_booking.area_cd;

        //                //相手の氏名を取得
        //                var repo = new CustomerRepository(last_region_cd);
        //                var customer = repo.Find(accountInfo.c_num);
        //                if (accountInfo.is_groom)
        //                {
        //                    accountInfo.p_last = customer.b_last;
        //                    accountInfo.p_first = customer.b_first;
        //                }
        //                else
        //                {
        //                    accountInfo.p_last = customer.g_last;
        //                    accountInfo.p_first = customer.g_first;
        //                }

        //                var repo_customer = new JpnInfoRepository(last_region_cd);
        //                var myWedInfo = repo_customer.GetMyWeddingInfo(
        //                                                accountInfo.account_id,
        //                                                accountInfo.c_num,
        //                                                this.CurrentRegionInfo.CurrentLanguage == "J");
        //                if (myWedInfo != null)
        //                {
        //                    var jpn_info = myWedInfo.jpnInfo;
        //                    if (jpn_info != null)
        //                    {
        //                        if (this.CurrentLoginUser.is_groom)
        //                        {
        //                            accountInfo.last_kana = jpn_info.g_last_kana;
        //                            accountInfo.first_kana = jpn_info.g_first_kana;
        //                            accountInfo.last_kanji = jpn_info.g_last_kanji;
        //                            accountInfo.first_kanji = jpn_info.g_first_kanji;
        //                            accountInfo.p_last_kana = jpn_info.b_last_kana;
        //                            accountInfo.p_first_kana = jpn_info.b_first_kana;
        //                            accountInfo.p_last_kanji = jpn_info.b_last_kanji;
        //                            accountInfo.p_first_kanji = jpn_info.b_first_kanji;
        //                        }
        //                        else
        //                        {
        //                            accountInfo.last_kana = jpn_info.b_last_kana;
        //                            accountInfo.first_kana = jpn_info.b_first_kana;
        //                            accountInfo.last_kanji = jpn_info.b_last_kanji;
        //                            accountInfo.first_kanji = jpn_info.b_first_kanji;
        //                            accountInfo.p_last_kana = jpn_info.g_last_kana;
        //                            accountInfo.p_first_kana = jpn_info.g_first_kana;
        //                            accountInfo.p_last_kanji = jpn_info.g_last_kanji;
        //                            accountInfo.p_first_kanji = jpn_info.g_first_kanji;
        //                        }
        //                        accountInfo.zip_code = jpn_info.jpn_zip;
        //                        accountInfo.addr_kanji = jpn_info.addr_kanji1 + jpn_info.addr_kanji2 + jpn_info.addr_kanji3;
        //                        accountInfo.addr_kana = jpn_info.addr_kana1 + jpn_info.addr_kana2 + jpn_info.addr_kana3;
        //                        accountInfo.address = jpn_info.addr_kana1 + jpn_info.addr_kana2 + jpn_info.addr_kana3;
        //                        accountInfo.home_tel = jpn_info.home_tel;
        //                        accountInfo.work_tel = jpn_info.work_tel;   //プロパティ名・カラム名はwork_telだが内容は携帯番号なので注意。
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return accountInfo;
        //}

        //public ActionResult Checkout(List<CBooking> purchase_list)
        //{
        //    var hash = new Dictionary<string, string>();

        //    //ログイン済か判定
        //    if (this.CurrentLoginUser == null)
        //    {
        //        //ログイン画面へ遷移
        //        hash.Add("Result", "error");
        //        hash.Add("Message", "購入手続きの前にログインしてください。");
        //        return Json(hash);
        //    }

        //    var purchase = new PurchaseInfo();
        //    purchase.region_cd = purchase_list[0].region_cd;
        //    purchase.area_cd = purchase_list[0].area_cd;
        //    purchase.area_name = purchase_list[0].area_name;
        //    purchase.total_quantity = TypeHelper.GetShort(purchase_list.Count());
        //    purchase.total_amount = TypeHelper.GetDecimal(purchase_list.Sum(x => x.total));
        //    purchase.accountInfo = GetWtAccountInfo();
        //    purchase.itemList = purchase_list;
        //    try
        //    {
        //        ValidateCheckout(this.CurrentRegionInfo.CurrentLanguage, purchase);
        //        ValidateCheckoutDate(this.CurrentRegionInfo.CurrentLanguage, purchase);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        hash.Add("Result", "error");
        //        hash.Add("Message", ex.Message);
        //        return Json(hash);
        //    }

        //    if (purchase_list.Any(m => m.is_pkg_item))
        //    {
        //        //Cart内にPKGアイテムがあったらPKGの日付を取得
        //        purchase.wed_date = purchase_list.Where(m => m.is_pkg_item)
        //                         .Select(m => m.wed_date)
        //                         .FirstOrDefault();
        //    }
        //    else
        //    {
        //        //Cart内にPKGアイテムがなければBookingを検索し、そのPKGの日付を取得
        //        var wtBookingRepo = new WtbApi.Proxy.All.CBookingRepository();
        //        var myWedOrders = wtBookingRepo.GetMyWeddingOrderList(purchase.region_cd, purchase.accountInfo.c_num, IsJPN());
        //        purchase.wed_date = myWedOrders.Where(i => i.isPkg && i.wed_date.HasValue && !"XCBN".Contains(i.wink_status))
        //                              .Select(i => i.wed_date.Value)
        //                              .FirstOrDefault();
        //    }

        //    //日付を合わせて確認画面を表示
        //    var changed = false;
        //    foreach (var booking in purchase_list)
        //    {
        //        if (booking.wed_date != purchase.wed_date)
        //        {
        //            changed = true;
        //            booking.wed_date = purchase.wed_date;
        //            booking.service_date = purchase.wed_date;

        //            ////period_idが変わったら価格を取得し直す
        //            //if (pkg_booking.period_id != booking.period_id) {
        //            booking.period_id = new WtbApi.Proxy.Destination.TrfPeriodRepository(booking.region_cd)
        //                                .GetPeriodIdByWedDate(purchase.wed_date);
        //            var priceInfo = new WtbApi.Proxy.Destination.TrfItemRepository(booking.region_cd)
        //                            .GetItemPrice(booking.period_id, booking.trf_item_cd, booking.area_cd, booking.price_type, booking.wed_date);
        //            if (priceInfo != null)
        //            {
        //                booking.price_new = priceInfo.price ?? 0;
        //                booking.price_cur_new = priceInfo.price_cur;
        //            }
        //            //}
        //        }
        //    }
        //    purchase.itemList = purchase_list;

        //    //購入リストをセッションに保存
        //    Session[Constants.SSKEY_PURCHASE_INFO] = purchase;

        //    //次の画面を表示。
        //    if (changed)
        //    {
        //        hash.Add("Result", "confirm");
        //    }
        //    else
        //    {
        //        hash.Add("Result", "success");
        //    }
        //    return Json(hash);
        //}

        //private void ValidateCheckout(string language, PurchaseInfo purchase)
        //{
        //    var count_area = purchase.itemList
        //                             .Select(a => a.area_cd)
        //                             .Distinct()
        //                             .Count();
        //    if (count_area > 1)
        //    {
        //        //複数Areaを同時に申し込むのは不可
        //        throw new ArgumentNullException(String.Empty, language == "J" ? "異なる地域の商品を同時に購入することはできません。"
        //                                                                      : "You can't select any items in different areas at the same time.");
        //    }

        //    var count_pkg = purchase.itemList.Count(m => m.is_pkg_item);
        //    if (count_pkg > 1)
        //    {
        //        //複数PKGを同時に申し込むのは不可
        //        throw new ArgumentNullException(String.Empty, language == "J" ? "ウェディングプラン/フォトプラン商品を複数購入することはできません。"
        //                                                                      : "You can't select multiple wedding plan items.");
        //    }

        //    if (string.IsNullOrEmpty(purchase.accountInfo.c_num))
        //    {
        //        if (count_pkg == 0)
        //        {
        //            //PKGアイテムがカートに含まれず、確定PKGが購入履歴にもなければエラー
        //            throw new ArgumentNullException(String.Empty, language == "J" ? "まだウェディングプランまたはフォトプランが確定していないため、オプション商品のみを購入することはできません。"
        //                                                                          : "You have no wedding plan confirmed. Please purchase an wedding plan first in order to purchase any optional items.");
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }

        //    //現地側Winkステータスも含めてオーダー一覧を取得。
        //    var wtBookingRepo = new CBookingRepository();
        //    var orders = wtBookingRepo.GetMyWeddingOrderList(
        //                                purchase.region_cd,
        //                                purchase.accountInfo.c_num,
        //                                (language == "J"));

        //    if (count_pkg == 1)
        //    {
        //        var pkg_ordered = orders.Any(m => m.item_type == "PKG" && "DP".Contains(m.wink_status));
        //        if (pkg_ordered)
        //        {
        //            //すでにOK(wink_status='D')またはRQ(wink_status='P')のPKGがあればエラー
        //            throw new ArgumentNullException(String.Empty, language == "J" ? "すでにウェディングプランまたはフォトプラン商品をお申込み済みです。"
        //                                                                          : "You've already purchased another wedding/photo plan.");
        //        }
        //    }
        //    else
        //    {
        //        var pkg_cart = wtBookingRepo
        //                            .GetBookingListByStatus(null, purchase.accountInfo.account_id, language == "J")
        //                            .FirstOrDefault(m =>
        //                                           (m.region_cd == purchase.region_cd)
        //                                        && (m.payment_status == CBooking.PAYSTATUS_PAID || m.payment_status == CBooking.PAYSTATUS_AUTH)
        //                                        && (m.item_type == "PKG")
        //                                        && (m.land_wb_id == null));

        //        var pkg_ok = orders.FirstOrDefault(m => m.item_type == "PKG" && m.wink_status == "D");

        //        if (pkg_cart == null && pkg_ok == null)
        //        {
        //            //PKGアイテムがカートに含まれず、確定PKGが購入履歴にもなければエラー
        //            throw new ArgumentNullException(String.Empty, language == "J" ? "まだウェディングプランまたはフォトプランが確定していないため、オプション商品のみを購入することはできません。"
        //                                                                          : "You have no wedding plan confirmed. Please purchase an wedding plan first in order to purchase any optional items.");
        //        }
        //        else if (pkg_cart == null && pkg_ok != null)
        //        {
        //            //PKGアイテムがカートに含まれず、確定PKGの挙式日が過去だったらエラー
        //            var today = RegionConfig.GetRegionToday(purchase.region_cd);
        //            if (pkg_ok.wed_date <= today)
        //            {
        //                throw new ArgumentNullException(String.Empty, language == "J" ? "すでに挙式日が過ぎています。オプション商品のみを購入することはできません。"
        //                                                                              : "Wedding date has already passed. You can't select any optional items.");
        //            }
        //        }

        //        var exists_RQ = orders.Any(m => m.item_type == "PKG" && m.wink_status == "P");
        //        if (exists_RQ)
        //        {
        //            //購入履歴内のPKGが手配中（RQ）の時はエラー
        //            throw new ArgumentNullException(String.Empty, language == "J" ? "手配中のウェディングプランまたはフォトプラン商品があります。お申込みが確定してから他の商品を購入してください。"
        //                                                                          : "You have a wedding plan that is not confirmed yet. Please proceed after the wedding/photo plan is confirmed.");
        //        }
        //    }

        //}

        //private void ValidateCheckoutDate(string language, PurchaseInfo purchase)
        //{
        //    foreach (var booking in purchase.itemList)
        //    {
        //        //日付チェック
        //        var today = RegionConfig.GetRegionToday(booking.region_cd);
        //        if (booking.is_pkg_item)
        //        {
        //            if (booking.wed_date < today.AddDays(5))
        //            {
        //                throw new ArgumentNullException(String.Empty, language == "J" ? "ウェディングプランおよびフォトプラン商品は挙式日より5日前までしかご購入いただけません。"
        //                                                                              : "Wedding plan items are available until 5 days before the wedding date.");
        //            }
        //        }
        //        else
        //        {
        //            if (booking.wed_date < today.AddDays(3))
        //            {
        //                throw new ArgumentNullException(String.Empty, language == "J" ? "オプション商品は挙式日より3日前までしかご購入いただけません。"
        //                                                                              : "Optional items are available until 5 days before the wedding date.");
        //            }
        //        }
        //    }
        //}

        //public JsonResult GetConfirmBookingList()
        //{
        //    var purchase = Session[Constants.SSKEY_PURCHASE_INFO] as PurchaseInfo;
        //    return Json(purchase.itemList, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Confirmation()
        //{
        //    var purchase = Session[Constants.SSKEY_PURCHASE_INFO] as PurchaseInfo;
        //    if (purchase == null)
        //    {
        //        //セッションが無ければカートに戻る
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //public ActionResult CheckoutAfterConfirm()
        //{
        //    var purchase = Session[Constants.SSKEY_PURCHASE_INFO] as PurchaseInfo;
        //    var hash = new Dictionary<string, string>();
        //    try
        //    {
        //        ValidateCheckoutDate(this.CurrentRegionInfo.CurrentLanguage, purchase);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        hash.Add("Result", "error");
        //        hash.Add("Message", ex.Message);
        //        return Json(hash);
        //    }

        //    //CBookingの価格を更新
        //    new WtbApi.Proxy.All.CBookingRepository()
        //                .UpdatePrices(purchase.accountInfo.account_id, purchase.itemList);

        //    hash.Add("Result", "success");
        //    return Json(hash);
        //}


    }
}