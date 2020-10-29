using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Serializable]
    [Table("c_booking")]
    public class CBooking {

        //決済状況
        public const string PAYSTATUS_INIT = "0";           //初期状態
        public const string PAYSTATUS_PAID = "1";           //売上済み
        public const string PAYSTATUS_AUTH = "2";           //オーソリ済み
        public const string PAYSTATUS_PAID_CAN = "3";       //売上キャンセル
        public const string PAYSTATUS_AUTH_CAN = "4";       //オーソリキャンセル
        public const string PAYSTATUS_PAID_RVS = "5";       //売上の補正
        public const string PAYSTATUS_AUTH_RVS = "6";       //オーソリの補正

        //RQ扱いアイテム
        public const string RESERVE_YES = "Y";                  //手配要フラグ。

        //PriceType
        public const short PRICE_TYPE_JAPAN = 3;                //円 Gross
        public const short PRICE_TYPE_DESTINATION = 1;          //ドル(現地通貨) Gross

        //App Cd
        public const string APPCD_WATABECOM_PC = "WATABECOM_PC";    //PC向けWebサイト
        public const string APPCD_WATABECOM_SP = "WATABECOM_SP";    //スマートフォン向けWebサイト
        public const string APPCD_WATABECOM_APP = "WATABECOM_APP";  //モバイルアプリ


        [Key]
        public int booking_id { get; set; }
        public int account_id { get; set; }
        public string c_num { get; set; }
        public string area_cd { get; set; }
        public string agent_cd { get; set; }
        public string sub_agent_cd { get; set; }
        public int? op_seq { get; set; }
        public string item_cd { get; set; }
        public string item_type { get; set; }
        public DateTime wed_date { get; set; }
        public string str_wed_date
        {
            get
            {
                return wed_date == null ? null : wed_date.ToString("yyyyMMdd");
            }
        }
        public DateTime create_date { get; set; }
        public short quantity { get; set; }
        public Decimal price { get; set; }
        public short price_type { get; set; }
        public string price_cur { get; set; }
        public DateTime? service_date { get; set; }
        public DateTime? service_time { get; set; }
        public string str_service_time
        {
            get
            {
                return service_time == null ? null : service_time.Value.ToString("yyyyMMddHHmm");
            }
        }

        [NotMapped]
        public string abbrev { get; set; }
        public bool rcp_private_room { get; set; }
        public int rcp_room_id { get; set; }
        public byte? rcp_seat_only { get; set; }
        public short? rcp_time_flag { get; set; }
        public short? mks_time_flag { get; set; }
        public string bga { get; set; }
        public string payment_status { get; set; }
        public DateTime? del_date { get; set; }
        public string payment_id { get; set; }
        public short? wed_h_seq { get; set; }
        public int? land_wb_id { get; set; }
        public string alb_mount { get; set; }
        public string alb_cover { get; set; }
        public string alb_type { get; set; }
        public string dvd_menucolor { get; set; }
        public int? order_num { get; set; }
        public Decimal? price_charge { get; set; }
        public string price_cur_charge { get; set; }
        public bool reserve_pkg { get; set; }
        public DateTime? confirm_date_jpn { get; set; }          //現地側でOKになった日時。（日本時間）
        public Decimal? cxl_charge { get; set; }
        public bool cxl_changed { get; set; }
        public string cxl_change_reason { get; set; }
        public DateTime? reviewreq_mail_sent_date { get; set; }

        [Required, StringLength(50), Hankaku, UpperCase]
        public string app_cd { get; set; }                      //申込み時に使用されたアプリケーション varchar(50)

        [Required, StringLength(15), Hankaku]
        public string last_person { get; set; }
        public DateTime update_date { get; set; }


        [NotMapped]
        public string area_name { get; set; }

        [NotMapped]
        public Decimal total
        {
            get
            {
                decimal p = (this.price_charge.HasValue && this.price_charge.Value != 0)
                                ? this.price_charge.Value
                                : this.price_new;
                return this.quantity * p;
            }
        }

        /// <summary>
        /// 決済通貨が「日本円」か否か。(沖縄、国内、およびタヒチ、バリは true を返す。)
        /// これが true の場合は海外からのオーダーであっても日本円での決済となる。
        /// false の場合は日本国内からのオーダーは日本円、海外からのオーダーは現地通貨での決済となる。
        /// </summary>
        /// <returns></returns>
        //[NotMapped]
        //public bool IsPaymentJPY
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(this.region_cd)) return false;
        //        return RegionConfig.GetPaymentCurrencyCd(this.region_cd) == "JPY";
        //    }
        //}


        //[NotMapped]
        //public decimal? cxl_charge { get; set; }

        [NotMapped]
        public string cursymbol_for_dis { get; set; }

        [NotMapped]
        public string curfmt_withsymbol { get; set; }

        [NotMapped]
        public string alb_mount_name { get; set; }

        [NotMapped]
        public string alb_cover_name { get; set; }

        [NotMapped]
        public string alb_type_name { get; set; }

        [NotMapped]
        public string dvd_menucolor_name { get; set; }

        [NotMapped]
        public int cnt_picture_s { get; set; }

        [NotMapped]
        public string item_name { get; set; }

        [NotMapped]
        public DateTime? image_upload_date { get; set; }

        [NotMapped]
        public bool delete { get; set; }

        [NotMapped]
        public int booking_id_for_session { get; set; } //セッションに保存されたBookingを識別するキー

        [NotMapped]
        public short? fixed_qty { get; set; }

        [NotMapped]
        public string church_cd { get; set; }

        [NotMapped]
        public string reserv_1 { get; set; }

        [NotMapped]
        public Decimal price_new { get; set; }

        [NotMapped]
        public string price_cur_new { get; set; }

        [NotMapped]
        public DateTime? discon_date { get; set; }
        [NotMapped]
        public string str_discon_date
        {
            get
            {
                return discon_date == null ? null : discon_date.Value.ToString("yyyyMMdd");
            }
        }

        [NotMapped]
        public bool is_pkg_item
        {
            get
            {
                return this.item_type == "PKG";
            }
        }



        //public static List<WtBooking> FromMyWeddingOrderItemList(List<MyWeddingOrderItem> items, bool is_jpn)
        //{
        //    var bookings = items
        //                    .Select(i => WtBooking.FromMyWeddingOrderItem(i, is_jpn))
        //                    .ToList();
        //    return bookings;
        //}

        //public static WtBooking FromMyWeddingOrderItem(MyWeddingOrderItem item, bool is_jpn)
        //{
        //    var booking = new WtBooking()
        //    {
        //        wt_id = item.wt_id,
        //        account_id = item.account_id,
        //        c_num = item.c_num,
        //        region_cd = item.region_cd,
        //        area_cd = item.area_cd,
        //        agent_cd = "",
        //        sub_agent_cd = "",
        //        op_seq = null,
        //        item_cd = "",
        //        trf_item_cd = item.trf_item_cd,
        //        item_type = item.item_type,
        //        wed_date = TypeHelper.GetDateTime(item.wed_date),
        //        create_date = TypeHelper.GetDateTime(item.order_date_jpn.Value),
        //        quantity = item.quantity,
        //        price = item.init_price,
        //        price_charge = item.price_charge,
        //        price_cur = item.price_type == 1 || item.price_type == 2 ? "$" : "\\",
        //        price_cur_charge = item.price_type == 1 || item.price_type == 2 ? "$" : "\\",
        //        price_type = item.price_type,
        //        curfmt_withsymbol = item.price_type == 1 || item.price_type == 2
        //                                ? RegionConfig.GetCurrencyFormatWithSymbol(item.region_cd)
        //                                : RegionConfig.GetCurrencyFormatWithSymbol("JPN"),
        //        service_date = item.service_date,
        //        service_time = item.service_time,
        //        abbrev = item.abbrev,
        //        payment_status = item.payment_status,
        //        payment_id = item.payment_id,
        //        land_wb_id = item.land_wb_id,
        //        alb_mount = item.alb_mount,
        //        alb_mount_name = item.alb_mount_name,
        //        alb_cover = item.alb_cover,
        //        alb_cover_name = item.alb_cover_name,
        //        alb_type = item.alb_type,
        //        alb_type_name = item.alb_type_name,
        //        dvd_menucolor = item.dvd_menucolor,
        //        dvd_menucolor_name = item.dvd_menucolor_name,
        //        period_id = item.period_id.HasValue ? item.period_id.Value : 0,
        //        trf_kind = item.trf_kind,
        //        trf_type = item.trf_type,
        //        trf_cat = item.trf_cat,
        //        trf_item_name = item.trf_item_name,
        //        order_num = item.order_num,
        //        image_upload_date = item.image_upload_date
        //    };
        //    booking.price_new = booking.price_charge.HasValue ? booking.price_charge.Value : 0;
        //    booking.price_cur_new = booking.price_cur_charge;
        //    booking.area_name = is_jpn ? RegionConfig.GetAreaNameJpn(item.region_cd, item.area_cd) : RegionConfig.GetAreaName(item.region_cd, item.region_cd);
        //    return booking;
        //}

        //public string PhotoPath
        //{
        //    get
        //    {
        //        return MyWeddingOrderItem.getPhotoPath(this.region_cd, this.trf_type, this.trf_item_cd, 1, "S", this.image_upload_date);
        //    }
        //}

        /// <summary>
        /// オプションの日付を挙式日に合わせる。
        /// 但しエステやリハーサルメイク等特定の商品には挙式日の前日をセットする。
        /// </summary>
        /// <param name="pkg_wed_date"></param>
        public void AdjustServiceDate(DateTime? pkg_wed_date)
        {
            if (!pkg_wed_date.HasValue) return;
            if (this.is_pkg_item) return;

            var correct_service_date = pkg_wed_date.Value;
            if (this.item_type == "AES" || this.abbrev == "REMK")
            {
                //エステおよびリハーサルメイクは挙式の前日。
                correct_service_date = pkg_wed_date.Value.AddDays(-1);
            }

            //正しい日付に合わせる。
            this.service_date = correct_service_date;
        }

    }
}