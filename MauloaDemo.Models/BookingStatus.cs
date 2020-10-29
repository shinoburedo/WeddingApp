using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models {

    public class BookingStatus {

        public const string RQ = "Q";
        public const string OK = "K";
        public const string NG = "N";
        public const string CXLRQ = "X";
        public const string CXL = "C";

        public const string DROPPED = "CXN";

        public const string TEXT_RQ = "RQ";
        public const string TEXT_OK = "OK";
        public const string TEXT_NG = "NG";
        public const string TEXT_CXLRQ = "CXLRQ";
        public const string TEXT_CXL = "CXL";

        private string _value;
        private string _text;
        private static Dictionary<string, string> _dic = new Dictionary<string, string> { 
            { RQ, TEXT_RQ },          // リクエスト中
            { OK, TEXT_OK },          // 確定
            { NG, TEXT_NG },          // 却下
            { CXLRQ, TEXT_CXLRQ },    // キャンセルリクエスト中
            { CXL, TEXT_CXL }         // キャンセル済
        };

        public BookingStatus(string value) {
            this.value = value;
        }

        public string value {
            get {
                return _value;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new Exception("value for BookingStatus cannot be null or blank.");
                }
                _value = value.ToUpper().Trim();
                _text = GetTextForValue(_value);
            }
        }

        public string text {
            get {
                return _text;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    throw new Exception("text for BookingStatus cannot be null or blank.");
                }
                _text = value.ToUpper().Trim();
                _value = GetValueForText(_text);
            }
        }

        public static string GetValueForText(string text) {
            var s = "";
            if (string.IsNullOrEmpty(text)) return s;
            if (_dic.ContainsValue(text)) s = _dic.SingleOrDefault(i => i.Value == text).Key;
            return s;
        }

        public static string GetTextForValue(string value) {
            var s = "";
            if (string.IsNullOrEmpty(value)) return s;
            if (_dic.ContainsKey(value)) s = _dic[value];
            return s;
        }

        /// <summary>
        /// 現在のステータスを受け取って次に選択可能なステータスのリストを返す。
        /// </summary>
        /// <param name="cur_status"></param>
        /// <param name="is_staff"></param>
        /// <returns></returns>
        public static List<BookingStatus> GetAvailableStatusList(string cur_status, bool is_staff) {
            var list = new List<BookingStatus>();

            if (is_staff) {
                switch (cur_status) {
                    case RQ:
                        list = new List<BookingStatus>{ 
                                            new BookingStatus(RQ), 
                                            new BookingStatus(OK), 
                                            new BookingStatus(CXLRQ), 
                                            new BookingStatus(NG), 
                                            new BookingStatus(CXL) };
                        break;
                    case OK:
                        list = new List<BookingStatus>{ 
                                            new BookingStatus(OK), 
                                            new BookingStatus(CXLRQ), 
                                            new BookingStatus(CXL) };
                        break;
                    case CXLRQ:
                        list = new List<BookingStatus>{ 
                                            new BookingStatus(CXLRQ), 
                                            new BookingStatus(CXL) };
                        break;
                    case NG:
                        list = new List<BookingStatus> { new BookingStatus(NG) };
                        break;
                    case CXL:
                        list = new List<BookingStatus> { new BookingStatus(CXL) };
                        break;
                }
            } else {
                switch (cur_status) {
                    case RQ:
                        list = new List<BookingStatus>{ 
                                            new BookingStatus(RQ), 
                                            new BookingStatus(CXLRQ) };
                        break;
                    case OK:
                        list = new List<BookingStatus>{ 
                                            new BookingStatus(OK), 
                                            new BookingStatus(CXLRQ) };
                        break;
                    case CXLRQ:
                        list = new List<BookingStatus> { new BookingStatus(CXLRQ) };
                        break;
                    case NG:
                        list = new List<BookingStatus> { new BookingStatus(NG) };
                        break;
                    case CXL:
                        list = new List<BookingStatus> { new BookingStatus(CXL) };
                        break;
                }
            }
            return list;
        }

        public static bool IsDropped(string book_status) {
            return book_status == TEXT_CXL
                 || book_status == TEXT_CXLRQ
                 || book_status == TEXT_NG;
        }

    }

}
