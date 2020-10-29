using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Utilities {

    public static class Common {

        public static int GetDaysToWedDate(string wed_date) {
            DateTime wd = TypeHelper.GetDateTime(wed_date);
            return GetDaysToWedDate(wd);
        }

        public static int GetDaysToWedDate(DateTime? wd) {
            int return_value = int.MaxValue;
            if (!wd.HasValue) return return_value;
            if (wd.Equals(DateTime.MinValue)) return return_value;

            var wedDate = wd.GetValueOrDefault();
            if (wedDate == null) return return_value;

            return wedDate.Subtract(DateTime.Today).Days;
        }

        public static DateTime GetJapanNow() {
            return DateTime.UtcNow.AddHours(9);
        }

        public static DateTime GetJapanDate() {
            return GetJapanNow().Date;
        }

        //public static DateTime GetRegionNow(string region_cd) {
        //    return RegionConfig.GetRegionNow(region_cd);    //Alias to RegionConfig's same  method.
        //}

        //public static DateTime GetRegionToday(string region_cd) {
        //    return RegionConfig.GetRegionToday(region_cd);  //Alias to RegionConfig's same  method.
        //}

        public static string GetDayName(DateTime d, bool isJPN)
        {
            string[] dayNamesJA = { "日", "月", "火", "水", "木", "金", "土" };
            string[] dayNamesEN = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            int dayIndex = (int)d.DayOfWeek;
            return isJPN ? dayNamesJA[dayIndex]
                         : dayNamesEN[dayIndex];
        }


    }
}