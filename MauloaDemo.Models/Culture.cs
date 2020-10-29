using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models {

    public class Culture {

        public static Dictionary<string, string> GetCultureList() {
            var cultures = new Dictionary<string, string>();
            cultures.Add("ja-JP", "Japanese");
            cultures.Add("en-US", "English");
            return cultures;
        }

        public static Dictionary<string, string> GetDateFormats() {
            var dateFormats = new Dictionary<string, string>();
            dateFormats.Add("MM/dd/yyyy", "MM/dd/yyyy (e.g. 12/31/2015)");
            dateFormats.Add("yyyy/MM/dd", "yyyy/MM/dd (e.g. 2015/12/31)");
            return dateFormats;
        }

        public static Dictionary<string, string> GetPrintDateFormats(string culture) {
            var dateFormats = new Dictionary<string, string>();

            switch (culture){
                case "ja-JP":
                    dateFormats.Add("yyyy/MM/dd", "yyyy/MM/dd (e.g. 2015/12/31)");
                    break;
                //case "en-AU":
                //    dateFormats.Add("dd/MM/yyyy", "dd/MM/yyyy (e.g. 31/12/2015)");
                //    break;
                //case "fr-FR":
                //case "it-IT":
                //    dateFormats.Add("dd MMM,yy", "dd MMM,yy (e.g. 31 DEC,15)");
                //    break;
                default:
                    dateFormats.Add("MM/dd/yyyy", "MM/dd/yyyy (e.g. 12/31/2015)");
                    break;
            }

            return dateFormats;
        }

        public static Dictionary<string, string> GetTimeFormats() {
            var timeFormats = new Dictionary<string, string>();
            //timeFormats.Add("hh:mm tt", "12 hours (e.g. 03:45 PM)");
            timeFormats.Add("HH:mm", "24 hours (e.g. 15:45)");
            return timeFormats;
        }

    }
}
