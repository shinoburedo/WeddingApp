using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Models.Combined {

    public class ChurchBlockInfo {

        public int church_block_id { get; set; }

        public string church_cd { get; set; }

        public DateTime block_date { get; set; }

        public DateTime start_time { get; set; }

        public DateTime? pickup_time { get; set; }
        public string pickup_time_s { get; set; }

        public string block_status { get; set; }        //'*BKD*'=Booked,'X'=Closed or NULL
        public string book_status { get; set; }         //'K'=OK, 'X'=CXLRQ, or NULL   ('*BKD*'の場合のみ'K'または'X'が入る。)

        public string c_num { get; set; }
        public string agent_cd { get; set; }
        public string sub_agent_cd { get; set; }
        public string g_last { get; set; }
        public string g_last_kanji { get; set; }
        public bool is_finalized { get; set; }          //true=Finalize済

        public bool is_sunset { get; set; }
        public bool is_irregular_time { get; set; }

        public string start_time_s { 
            get { 
                return is_sunset ? "Sunset" : start_time.ToString("HH:mm");
            } 
        }

    }

    public class ChurchBlockResult {
        public short? range_seq { get; set; }
        public DateTime? start_time { get; set; }
        public string status { get; set; }
        public DateTime block_date { get; set; }
    }

    public class ChurchBlockGridResult {
        public DateTime block_date { get; set; }
        public string date { get; set; }
        public string day { get; set; }
        public bool is_holiday { get; set; }
        public bool is_edited { get; set; }
        public List<ChurchBlockResult> blocks { get; set; }
    }

    public class ChurchBlockHeader {
        public string header_name { get; set; }
        public DateTime? start_time { get; set; }
    }

    public class ChurchAvailSaveInfo {
        public DateTime? start_time { get; set; }
        public string status { get; set; }
        public DateTime block_date { get; set; }
    }

}
