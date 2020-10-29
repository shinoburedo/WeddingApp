using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using CBAF;
using System.Configuration;

namespace MauloaDemo.Models.Combined {

    public class CustomerListItem {

        // Customer
        public string c_num { get; set; }
        public string cust_name {get; set;}
        public string cust_name_kanji { get; set; }
        public string g_last { get; set; }
        public string agent_cd { get; set; }
        public string sub_agent_cd { get; set; }
        public string area_cd { get; set; }
        public string church_cd { get; set; }
        public DateTime? wed_date { get; set; }
        public DateTime? wed_time { get; set; }
        public DateTime? cxl_date { get; set; }
        public DateTime? final_date { get; set; }
        public DateTime create_date { get; set; }
        public DateTime update_date { get; set; }

        // Sales 
        public int? op_seq { get; set; }
        public int? parent_op_seq { get; set; }
        public string book_status { get; set; }         // K(=OK), Q(=RQ), N(=NG), X(=CXLRQ), C(=CXL)
        public string item_type { get; set; }

        // WedInfo
        public bool is_irregular_time { get; set; }

        // Item 
        public string item_cd { get; set; }
        public string item_name { get; set; }
        public string item_name_jpn { get; set; }

        // log_change
        public int? log_id { get; set; }
        public DateTime? log_datetime { get; set; }
        public string log_by { get; set; }
        public string log_by_sub_agent_cd { get; set; }
        public string table_name { get; set; }
        public string action { get; set; }

        [NotMapped]     //ObjectReflectionHelper.TrimStrings()の対象にならない様に。
        public string changes { get; set; }

        public bool archived { get; set; }
        public string archive_by { get; set; }
        public DateTime? archive_datetime { get; set; }
        
        [NotMapped]
        private List<ChangeDiff> _changeDiffs;

        [NotMapped]
        public string wed_date_s {
            get {
                var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
                var wed_time_s = this.wed_time.HasValue ? this.wed_time.Value.ToString("HH:mm") : "";
                if (string.Equals(wed_time_s, SunsetBlockTime)) {
                    wed_time_s = "Sunset";
                }
                return wed_time_s;
            }
        }

        // Dupcheck
        public bool dup_check_done { get; set; }

        public string BookStatus {
            get {
                return BookingStatus.GetTextForValue(this.book_status);
            }
        }

        public string ChangesStrForStaff {
            get {
                return GetChangesStr(true);
            }
        }

        public string ChangesStrForAgent {
            get {
                return GetChangesStr(false);
            }
        }

        private string GetChangesStr(bool is_staff) {
            var s = this.changes;

            try {
                if (this.action == "U") {
                    var str = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(this.item_name_jpn)) {
                        str.AppendFormat("「{0}」", this.item_name_jpn);
                    }

                    foreach (var item in this.ChangeDiffs) {
                        if (str.Length > 0) str.AppendLine("<br />");

                        var old_value = TypeHelper.GetStr(item.oldValue);
                        var new_value = TypeHelper.GetStr(item.newValue);
                        old_value = string.IsNullOrWhiteSpace(old_value) ? string.Format("'{0}'", old_value) : old_value;
                        new_value = string.IsNullOrWhiteSpace(new_value) ? string.Format("'{0}'", new_value) : new_value;

                        if (item.key == "note") {
                            var new_note = StringHelper.CutByWithEllipsis(TypeHelper.GetStr(item.newValue), 50);   //Noteは最大50文字まで。
                            str.AppendFormat("{0}: {1}", item.key, new_note);
                        } else if (item.key == "staff_note") {
                            //staff_noteの変更はエージェントには見せない。
                            if (is_staff) {
                                var new_staffnote = StringHelper.CutByWithEllipsis(TypeHelper.GetStr(item.newValue), 50);  //Staff Noteは最大50文字まで。
                                str.AppendFormat("{0}: {1}", item.key, new_staffnote);
                            }
                        } else if (item.key == "disp_seq") {
                            //disp_seqの変更は見せない。
                        } else {
                            str.AppendFormat("{0}: {1} → {2}", item.key, old_value, new_value);
                        }
                    }
                    s = str.ToString();
                } else if (this.action == "I") {
                    s = "(New) " + this.item_name_jpn;
                    if (this.table_name == "file") {
                        s = getFileChangeStr();
                    }
                } else if (this.action == "D") {
                    s = "(Deleted) " + this.item_name_jpn;
                    if (this.table_name == "file") {
                        s = getFileChangeStr();
                    }
                }

            } catch (Exception ex) {
                //Ignore exceptions here, especially for JsonConvert.DeserializeObject().
            }
            return s;
        }

        private string getFileChangeStr() {
            string s = this.action == "I" ? "(Uploaded) " : "(Deleted) ";
            FileInfo fileListItem = null;
            try {
                fileListItem = (FileInfo)JsonConvert.DeserializeObject<FileInfo>(this.changes);
            } catch (Exception) {
                //Ignore exceptions.
            }
            if (fileListItem != null) {
                s += fileListItem.filename;
            } else {
                s += this.changes;
            }
            return s;
        }

        private List<ChangeDiff> ChangeDiffs{
            get {
                if (_changeDiffs != null) {
                    return _changeDiffs;
                }

                if (this.action != "U" || string.IsNullOrWhiteSpace(this.changes)) {
                    _changeDiffs = new List<ChangeDiff>();
                    return _changeDiffs;
                }
                    
                _changeDiffs = (List<ChangeDiff>)JsonConvert.DeserializeObject<ICollection<ChangeDiff>>(this.changes);
                return _changeDiffs;
            }
        }

        public bool IsStatusChanged {
            get {
                if (this.action != "U") return false;
                return this.ChangeDiffs.Any(m => m.key == "book_status");
                //return this.changes.Contains("\"key\":\"book_status\"");
            }
        }

        public bool IsConfirmed {
            get {
                if (this.action != "U") return false;
                return this.ChangeDiffs.Any(m => m.key == "book_status"
                                                && BookingStatus.TEXT_OK.Equals(TypeHelper.GetStr(m.newValue)));
            }
        }

        public bool IsDropped {
            get {
                if (this.action != "U") return false;
                return this.ChangeDiffs.Any(m => m.key == "book_status"
                                                && BookingStatus.IsDropped(TypeHelper.GetStr(m.newValue)));
            }
        }

    }
}
