using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MauloaDemo.Utilities {

    public class JsonHelper {
        public const string DATE_FORMAT = "ddd MMM dd yyyy HH:mm:ss";       //これ以外のフォーマットだとカルチャーが変わると上手く行かないケースがある。
        public const string DATE_FORMAT_JPN = "yyyy/MM/dd HH:mm:ss";        //カルチャーが日本語(ja-JP)の場合はこれでないと上手く行かない！

        /// <summary>
        /// DateTime型をカルチャーに関わらず、かつサーバー間の時差にも影響されない文字列で変換するためのヘルパー。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MySerializeObject(object data) {
            var isoConvert = new IsoDateTimeConverter();

            isoConvert.Culture = System.Globalization.CultureInfo.InvariantCulture;

            if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("ja")) {
                isoConvert.DateTimeFormat = DATE_FORMAT_JPN;
            } else {
                isoConvert.DateTimeFormat = DATE_FORMAT;
            }
            return JsonConvert.SerializeObject(data, isoConvert);
        }

    }
}
