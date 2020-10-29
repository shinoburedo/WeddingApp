using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;
using Newtonsoft.Json;

namespace MauloaDemo.Models {

    [Table("c_item")]
    public class CItem : IValidatableObject {

        [Key(), Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }
        [NotMapped]
        public string item_name_eng { get; set; }
        [NotMapped]
        public string item_name_jpn { get; set; }

        public string item_info_eng { get; set; }
        public string item_info_jpn { get; set; }

        public string price_note_eng { get; set; }
        public string price_note_jpn { get; set; }

        public string cxl_info_eng { get; set; }
        public string cxl_info_jpn { get; set; }

        public short? disp_order { get; set; }

        public bool prepare_to_cust { get; set; }
        public bool open_to_cust { get; set; }
        public string movie_url { get; set; }

        public string product_detail_eng { get; set; }
        public string product_detail_jpn { get; set; }

        public byte image_count { get; set; }
        public DateTime? image_upload_date { get; set; }

        [MaxLength(6), Required, Hankaku, JsonIgnore]
        public string last_person { get; set; }

        [JsonIgnore]
        public DateTime update_date { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
