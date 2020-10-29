using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using CBAF;

namespace MauloaDemo.Models {

    public class Calendar {

        public int MinYear = 2014;        //開始日の最小値の年。
        public const int max_weeks = 20;        //週数の最大値。

        private DateTime _MinWedDate;
        public DateTime MinWedDate { get { return _MinWedDate; } }

        private DateTime _SearchDate;
        public DateTime SearchDate { get { return _SearchDate; } }

        private DateTime _StartDate;
        public DateTime StartDate { get { return _StartDate; } }

        private DateTime _EndDate;
        public DateTime EndDate { get { return _EndDate; } }

        private int _Weeks;
        public int Weeks { get{ return _Weeks; } }

        private List<CalendarDay> _Headers;
        public List<CalendarDay> Headers { get { return _Headers; } }

        private List<CalendarDay> _Days;
        public List<CalendarDay> Days { get { return _Days; } }

        //コンストラクタ
        public Calendar(DateTime searchDate, int weeks) {

            _MinWedDate = GetMinWedDate();

            if (searchDate < _MinWedDate) searchDate = _MinWedDate;
            if (weeks < 1) weeks = 1;
            if (weeks > max_weeks) weeks = max_weeks;

            _SearchDate = searchDate;
            _Weeks = weeks;

            //開始日を直前の日曜日に合わせる。
            _StartDate = searchDate;
            for (DateTime d = _StartDate; d >= _StartDate.AddDays(-7); d = d.AddDays(-1)) {
                if (d.DayOfWeek == 0) {
                    _StartDate = d;
                    break;
                }
            }

            _EndDate = _StartDate.AddDays(7 * weeks).AddDays(-1);

            _Headers = new List<CalendarDay>();
            _Days = new List<CalendarDay>();

            createDays();
        }

        public static DateTime GetMinWedDate() {
            var min_wed_date_str = ConfigurationManager.AppSettings["MinWedDate"];
            if (string.IsNullOrEmpty(min_wed_date_str)) min_wed_date_str = "2014/01/01";
            var d = TypeHelper.GetDateTime(min_wed_date_str);
            return d;
        }

        public string StartDateStr {
            get {
                return this.StartDate.ToString("yyyy/MM/dd");
            }
        }
        public string EndDateStr {
            get {
                return this.EndDate.ToString("yyyy/MM/dd");
            }
        }

        public void AddCalendarItem(CalendarItem item) {
            var calDay = this.Days.Where(d => d.Date == item.Date).SingleOrDefault();
            if (calDay == null) {
                throw new Exception("Date is out of range.");
            }

            item.is_past = calDay.is_past;
            calDay.Items.Add(item);
        }

        public int DayCount { 
            get {
                return this.EndDate.Subtract(this.StartDate).Days;
            }
        }


        private void createDays() {
            DateTime hwi_today = DateTime.UtcNow.AddHours(-10).Date;

            int n = 0;
            for (var d = this.StartDate; d <= this.StartDate.AddDays(6); d = d.AddDays(1)) {
                this.Headers.Add(new CalendarDay(d, n, false, false));
                n++;
            }

            n = 0;
            for (var d = this.StartDate; d <= this.EndDate; d = d.AddDays(1)) {
                var is_past = (d.CompareTo(hwi_today) < 0) || (d.CompareTo(_MinWedDate) < 0);
                this.Days.Add(new CalendarDay(d, n, d.Equals(this.SearchDate), is_past));
                n++;
            }
        }
    }

    public class CalendarDay {

        public int DayIndex { get; set; }
        public DateTime Date { get; set; }
        public List<CalendarItem> Items { get; set; }
        public bool isSearchDate { get; set; }
        public bool is_past { get; set; }


        //コンストラクタ
        public CalendarDay(DateTime date, int dayIndex, bool is_search_date, bool is_past) {
            this.Date = date;
            this.DayIndex = dayIndex;
            this.isSearchDate = is_search_date;
            this.is_past = is_past;

            this.Items = new List<CalendarItem>();
        }

        public string  DateStr{
            get{
                return this.Date.ToString("MM/dd");
            }
        }
        public string DayName {
            get {
                return this.Date.ToString("ddd");
            }
        }
        public int DayNum {
            get {
                return (int)this.Date.DayOfWeek;
            }
        }

    }

    public class CalendarItem {

        public string Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PickUpTime { get; set; }

        public string block_status { get; set; }
        public string book_status { get; set; }
        public string c_num { get; set; }
        public string agent_cd { get; set; }
        public string sub_agent_cd { get; set; }
        public string g_last { get; set; }
        public string g_last_kanji { get; set; }
        public string staff_cd { get; set; }
        public string staff_name { get; set; }
        public string church_cd { get; set; }
        public bool is_sunset { get; set; }
        public bool is_past { get; set; }
        public bool is_irregular_time { get; set; }
        public string _description { get; set; }
        public bool is_finalized { get; set; }

        public CalendarItem() {
        }

        public string DateStr {
            get {
                return this.Date.ToString("MM/dd/yyyy");
            }
        }

        public string StartTimeStr {
            get {
                if (!this.StartTime.HasValue) return string.Empty;
                if (this.is_sunset) return "Sunset";
                return this.StartTime.Value.ToString("HH:mm");
            }
        }

        public string EndTimeStr {
            get {
                if (!this.EndTime.HasValue) return string.Empty;
                return this.EndTime.Value.ToString("HH:mm");
            }
        }

        public string PickUpTimeStr {
            get {
                if (!this.PickUpTime.HasValue) return string.Empty;
                if (this.is_sunset) return string.Empty;
                return this.PickUpTime.Value.ToString("HH:mm");
            }
        }


        public string Description {
            get {
                return this._description;
            }
        }
    }

}
