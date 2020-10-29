using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Text;
using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Repository;
using MauloaDemo.Models;


namespace MauloaDemo.Web.Controllers {

    [NoClientCache]                                 //IEだとAjaxでGETするとブラウザのキャッシュが使われてしまう問題の対処。
    public class CommonController : BaseController {

        //[ChildActionOnly]
        //public ActionResult AgentPopup(string inputId, string inputValue, string inputClass, string inputStyle, string linkId, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return PartialView("_AgentPopup");
        //}

        //public ActionResult SearchAgent(string inputId, string inputValue, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return View();
        //}

        //public ActionResult GetAgentList(string agentCd = "", string agentName = "", bool showDiscon = false) {
        //    AgentRepository repository = new AgentRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var agents = repository.GetList(agentCd, agentName, showDiscon).ToList();
        //    repository.ApplyMappings(agents);                 //末尾の空白を除去。
        //    return Json(agents, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult AgentAutoComplete(string text, string isUseDiscon, bool isCheckMode = false) {
        //    AgentRepository repository = new AgentRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var agents = repository.GetList(text, isUseDiscon == "1", isCheckMode)
        //                    .Select(m => new { 
        //                                agent_cd = m.agent_cd, 
        //                                agent_name = m.agent_name })
        //                    .ToList();
        //    ObjectReflectionHelper.TrimStrings(agents);     //末尾の空白を除去。
        //    return Json(agents, JsonRequestBehavior.AllowGet);
        //}

        //[ChildActionOnly]
        //public ActionResult ChurchPopup(string inputId, string inputValue, string inputClass, string inputStyle, string linkId, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return PartialView("_ChurchPopup");
        //}

        //public ActionResult SearchChurch(string inputId, string inputValue, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return View();
        //}

        //public ActionResult GetChurchList(string churchCd = "", string churchName = "", bool showDiscon = false) {
        //    ChurchRepository repository = new ChurchRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list= repository.GetList(churchCd, churchName, showDiscon)
        //                            .Select(m => new { church_cd = m.church_cd, church_name = m.church_name })
        //                            .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ChurchAutoComplete(string text, string isUseDiscon, bool isCheckMode = false) {
        //    ChurchRepository repository = new ChurchRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(text, isUseDiscon == "1", isCheckMode)
        //                            .Select(m => new { church_cd = m.church_cd, church_name = m.church_name })
        //                            .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[ChildActionOnly]
        //public ActionResult HotelPopup(string inputId, string inputValue, string inputClass, string inputStyle, string linkId) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    return PartialView("_HotelPopup");
        //}

        //public ActionResult SearchHotel(string inputId, string inputValue) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    return View();
        //}

        //public ActionResult GetHotelList(string hotelCd = "", string hotelName = "") {
        //    HotelRepository repository = new HotelRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(hotelCd, hotelName)
        //                                .Select(m => new {
        //                                    hotel_cd = m.hotel_cd,
        //                                    hotel_name = m.hotel_name
        //                                })
        //                                .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult HotelAutoComplete(string text, bool isCheckMode = false) {
        //    HotelRepository repository = new HotelRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(text, isCheckMode)
        //                                .Select(m => new { 
        //                                            hotel_cd = m.hotel_cd, 
        //                                            hotel_name = m.hotel_name })
        //                                .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[ChildActionOnly]
        //public ActionResult VendorPopup(string inputId, string inputValue, string inputClass, string inputStyle, string linkId, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return PartialView("_VendorPopup");
        //}

        //public ActionResult SearchVendor(string inputId, string inputValue, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return View();
        //}

        //public ActionResult GetVendorList(string vendorCd = "", string vendorName = "", bool showDiscon = false) {
        //    VendorRepository repository = new VendorRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(vendorCd, vendorName, showDiscon)
        //                            .Select(m => new {
        //                                    vendor_cd = m.vendor_cd,
        //                                    vendor_name = m.vendor_name
        //                                })
        //                            .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult VendorAutoComplete(string text, string isUseDiscon, bool isCheckMode = false) {
        //    VendorRepository repository = new VendorRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(text, isUseDiscon == "1", isCheckMode)
        //                                .Select(m => new { 
        //                                                vendor_cd = m.vendor_cd, 
        //                                                vendor_name = m.vendor_name })
        //                                .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[ChildActionOnly]
        //public ActionResult ItemPopup(
        //                        string inputId,
        //                        string inputValue,
        //                        string inputClass,
        //                        string inputStyle,
        //                        string linkId,
        //                        string itemTypeId,
        //                        string isUseDiscon,
        //                        string cNum = null,
        //                        string areaCd = null) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    ViewBag.ItemTypeId = itemTypeId;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    ViewBag.CNum = cNum;
        //    ViewBag.AreaCd = areaCd;

        //    return PartialView("_ItemPopup");
        //}

        //public ActionResult SearchItem(string inputId, string inputValue, string itemTypeId, string itemTypeValue, string isUseDiscon, string cNum = null, string areaCd = null) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.ItemTypeId = itemTypeId;
        //    ViewBag.ItemTypeValue = itemTypeValue;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    ViewBag.CNum = cNum;
        //    ViewBag.AreaCd = areaCd;
        //    return View();
        //}

        //public ActionResult GetItemList(string itemCd = "", string itemName = "", string itemType = "", bool showDiscon = false, string cNum = null, string areaCd = null) {
        //    ItemRepository repository = new ItemRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(itemType, cNum, showDiscon, itemCd, 0, itemName).ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ItemAutoComplete(string text, string itemType, string isUseDiscon, bool isCheckMode = false, string areaCd = null) {
        //    ItemRepository repository = new ItemRepository(this.CurrentRegionInfo.CurrentDestination);
            
        //    var list = repository.GetList(text, itemType, areaCd, isUseDiscon == "1", isCheckMode)
        //                            .Select(m => new { 
        //                                item_cd = m.item_cd, 
        //                                item_name = m.item_name 
        //                            })
        //                            .ToList();

        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAreaList(
        //                        bool addBlankRow = false, 
        //                        string blankRowValue="", 
        //                        string blankRowText="") {

        //    AreaRepository repository = new AreaRepository();
        //    var list = repository
        //                .GetList()
        //                .Select(a => new { area_cd= a.area_cd, description= a.desc_eng})
        //                .ToList();

        //    if (addBlankRow) {
        //        list.Insert(0, new { area_cd = blankRowValue, description = blankRowText });
        //    }

        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult FindUserAreaCd(string areaCd) {
        //    AreaRepository repository = new AreaRepository();
        //    var areas = repository.Find(areaCd);
        //    return Json((areas == null ? false : true), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetItemTypeList() {
        //    ItemTypeRepository repository = new ItemTypeRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList().ToList();
        //    repository.ApplyMappings(list);             //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAllItemTypeList() {
        //    ItemTypeRepository repository = new ItemTypeRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetAllList().ToList();
        //    repository.ApplyMappings(list);             //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[ChildActionOnly]
        //public ActionResult SubAgentPopup(string inputId, string inputValue, string inputAgentCd, string inputClass, string inputStyle, string linkId, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputAgentCd = inputAgentCd;    // 入力されたAgentCD
        //    ViewBag.InputClass = inputClass;
        //    ViewBag.InputStyle = inputStyle;
        //    ViewBag.LinkId = linkId;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return PartialView("_SubAgentPopup");
        //}

        //public ActionResult SearchSubAgent(string inputId, string inputValue, string inputAgentCd, string isUseDiscon) {
        //    ViewBag.InputId = inputId;
        //    ViewBag.InputValue = inputValue;
        //    ViewBag.InputAgentCd = inputAgentCd;
        //    ViewBag.IsUseDiscon = isUseDiscon;
        //    return View();
        //}

        //public ActionResult GetSubAgentList(string childCd, string name, string parentCd,int type=0, bool showDiscon = false) {
        //    AgentParentRepository repository = new AgentParentRepository(this.CurrentRegionInfo.CurrentDestination);

        //    var list = repository.GetList(childCd, name, parentCd,(AgentParentDao.Category)type, false).ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult SubAgentAutoComplete(
        //                        string text = "", 
        //                        string agentCd = "", 
        //                        string isUseDiscon = "", 
        //                        bool isCheckMode = false, 
        //                        Dictionary<string, List<Dictionary<string, string>>> filter = null) {

        //    //KendoDropDownから送られてくるfilterパラメータを解析してchild_cdを取得。
        //    if (filter != null && filter["filters"] != null && string.IsNullOrEmpty(text)) {
        //        foreach (var filterItem in filter["filters"]) {
        //            if ("child_cd".Equals(filterItem["field"])) {
        //                text = TypeHelper.GetStrTrim(filterItem["value"]);
        //                break;
        //            }
        //        } 
        //    }

        //    AgentParentRepository repository = new AgentParentRepository(this.CurrentRegionInfo.CurrentDestination);
        //    var list = repository.GetList(text, agentCd, isCheckMode)
        //                         .ToList();
        //    ObjectReflectionHelper.TrimStrings(list);     //末尾の空白を除去。
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
      

        /// <summary>
        /// 郵便番号から住所を検索して返す。
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns>
        /// zipcloud APIのドキュメントはこちら: http://zipcloud.ibsnet.co.jp/doc/api 
        /// </returns>
        public ActionResult GetAddressFromZipCode(string zipcode) {
            WebClient wc = new WebClient();
            //wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows XP)");

            var url = "http://zipcloud.ibsnet.co.jp/api/search?zipcode=" + zipcode;
            byte[] data = wc.DownloadData(url);

            Encoding enc = Encoding.UTF8;
            string html = enc.GetString(data);
            return Content( html, "application/json", Encoding.UTF8);
        }

    }
}
