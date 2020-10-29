using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using CBAF;

namespace MauloaDemo.Models.Combined {

    public class CustomerItem {

        public string area_cd { get; set; }
        public string item_type { get; set; }
        public string item_cd { get; set; }
        public string item_name_eng { get; set; }
        public string item_name_jpn { get; set; }

        public string item_name { get; set; }               //Language('J' or 'E')に対応した文字列。(DBには無い)

        public string item_info_eng { get; set; }
        public string item_info_jpn { get; set; }
        public string item_info { get; set; }
        public string price_note_eng { get; set; }
        public string price_note_jpn { get; set; }
        public string price_note { get; set; }
        public string cxl_info_eng { get; set; }
        public string cxl_info_jpn { get; set; }
        public string cxl_info { get; set; }
        public short? disp_order { get; set; }
        public bool open_to_cust { get; set; }
        public string movie_url { get; set; }
        public string product_detail { get; set; }
        public string product_detail_eng { get; set; }
        public string product_detail_jpn { get; set; }
        public string abbrev { get; set; }

        public decimal? price { get; set; }
        public string price_currency { get; set; }
        public short? price_type { get; set; }

        public decimal gross { get; set; }
        public decimal d_net { get; set; }
        public int y_net { get; set; }
        public bool tentative { get; set; }
        //[NotMapped]
        //public decimal? price { get; set; }
        //[NotMapped]
        //public string price_currency { get; set; }
        //[NotMapped]
        //public short? price_type { get; set; }

        public byte image_count { get; set; }
        public DateTime? image_upload_date { get; set; }

        public List<ItemGrouping> pkg_info { get; set; }
        public List<ChurchAvail> church_avail { get; set; }
        public string church_cd { get; set; }

        [NotMapped]
        public string item_information { get; set; }
        [NotMapped]
        public int cnt_picture_s { get; set; }
        [NotMapped]
        public string alb_mount { get; set; }
        [NotMapped]
        public string alb_cover { get; set; }
        [NotMapped]
        public string alb_type { get; set; }

        [NotMapped]
        public string image_upload_date_str
        {
            get
            {
                if (!this.image_upload_date.HasValue) return "";
                return this.image_upload_date.Value.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [NotMapped]
        public string image_upload_date_param
        {
            get
            {
                if (!this.image_upload_date.HasValue) return "";
                return this.image_upload_date.Value.ToString("yyyyMMddHHmmss");
            }
        }

        [NotMapped]
        public string PhotoPath
        {
            get
            {
                //return MyWeddingOrderItem.getPhotoPath(this.region_cd, this.type, this.item_cd, 1, "S", this.image_upload_date);
                return "";
            }
        }

        [NotMapped]
        public bool is_pkg
        {
            get
            {
                if (this.item_type == "PHP" || this.item_type == "PHS" || this.item_type == "PKG")
                {
                    return true;
                }
                return false;
            }
        }

    }

    public class ChurchAvail {
        public string church_cd { get; set; }
        public DateTime? block_date { get; set; }
        public DateTime? start_time { get; set; }
        public string rokki { get; set; }
        public string status { get; set; }
        public string day_name { get; set; }

        public string str_block_date
        {
            get
            {
                if (block_date == null) return "";
                return block_date.Value.ToString("MM/dd") + "（" + this.day_name + "）" + this.rokki;
            }
        }

        public string str_start_time
        {
            get
            {
                if (start_time == null) return "";
                return start_time.Value.ToString("HH:mm");
            }
        }
    }

    public class ItemPriceInfo {
        public short price_type { get; set; }
        public decimal? price { get; set; }
        public string price_cur { get; set; }
        public bool? tentative { get; set; }
        public decimal? gross { get; set; }
        public decimal? d_net { get; set; }
        public int? y_net { get; set; }
    }



}
