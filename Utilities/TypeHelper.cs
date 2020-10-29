using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MauloaDemo.Utilities {

	public static class TypeHelper {

		/// -----------------------------------------------------------------------------
		/// <summary>
		///     文字列が日付型に変換可能かどうかを返します。</summary>
		/// <param name="stTarget">
		///     検査対象となる文字列。<param>
		/// <returns>
		///     指定した文字列が日付型に変換可能であれば true。それ以外は false。</returns>
		/// -----------------------------------------------------------------------------
		public static bool IsDate(string strDate) {
			DateTime dt;
			return DateTime.TryParse(strDate, out dt);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		///     文字列が数値であるかどうかを返します。</summary>
		/// <param name="stTarget">
		///     検査対象となる文字列。<param>
		/// <returns>
		///     指定した文字列が数値であれば true。それ以外は false。</returns>
		/// -----------------------------------------------------------------------------
		public static bool IsNumeric(string strNum) {
			double dNullable;

			return double.TryParse(
				strNum,
				System.Globalization.NumberStyles.Any,
				null,
				out dNullable
			);
		}

		/// -----------------------------------------------------------------------------
		/// <summary>
		///     オブジェクトが数値であるかどうかを返します。</summary>
		/// <param name="oTarget">
		///     検査対象となるオブジェクト。<param>
		/// <returns>
		///     指定したオブジェクトが数値であれば true。それ以外は false。</returns>
		/// -----------------------------------------------------------------------------
		public static bool IsNumeric(object oTarget) {
			return IsNumeric(oTarget.ToString());
		}

		public static string GetStr(Object value) {
			string s = (value == null) ? "" : value.ToString();
			return string.IsNullOrEmpty(s) ? "" : s;
		}

        public static string GetStrTrim(Object value) {
			return GetStr(value).Trim();
		}

		public static byte GetByte(object obj) {
			byte v = 0;
			if (obj == null) return v;
			Byte.TryParse(obj.ToString(), out v);
			return v;
		}

		public static short GetShort(object obj) {
			short v = 0;
			if (obj == null) return v;
			Int16.TryParse(obj.ToString(), out v);
			return v;
		}

		public static Int32 GetInt(object obj) {
			int v = 0;
			if (obj == null) return v;
			Int32.TryParse(obj.ToString(), out v);
			return v;
		}

		public static long GetLong(object obj) {
			long v = 0;
			if (obj == null) return v;
			Int64.TryParse(obj.ToString(), out v);
			return v;
		}

		public static bool GetBool(object obj) {
			bool v = false;
			if (obj == null) return v;
			Boolean.TryParse(obj.ToString(), out v);
			return v;
		}

		public static decimal GetDecimal(object obj) {
			decimal v = 0;
			if (obj == null) return v;
			Decimal.TryParse(obj.ToString(), out v);
			return v;
		}

		public static DateTime GetDateTime(object obj) {
			DateTime v = DateTime.MinValue;
			if (obj == null) return v;
			DateTime.TryParse(obj.ToString(), out v);
			return v;
		}

		public static bool IsNullOrEmptyOrMinValue(object value) {
			if (value == null) return true;

            if (value is DateTime? || value is DateTime) {
                var date_value = value as DateTime?;
                if (date_value != null) {
                    return (!date_value.HasValue || DateTime.MinValue.Equals(date_value));
                }
            }

            if (value is Int64? || value is Int64) {
                var int_value = value as Int64?;
                if (int_value != null) {
                    return (!int_value.HasValue || Int64.MinValue.Equals(int_value));
                }
            }

            if (value is Int32? || value is Int32) {
                var int_value = value as Int32?;
                if (int_value != null) {
                    return (!int_value.HasValue || Int32.MinValue.Equals(int_value));
                }
            }

            if (value is Int16? || value is Int16) {
                var int_value = value as Int16?;
                if (int_value != null) {
                    return (!int_value.HasValue || Int16.MinValue.Equals(int_value));
                }
            }

            if (value is Decimal? || value is Decimal) {
                var dec_value = value as Decimal?;
                if (dec_value != null) {
                    return (!dec_value.HasValue || Decimal.MinValue.Equals(dec_value));
                }
            }

            if (value is Double? || value is Double) {
                var dbl_value = value as Double?;
                if (dbl_value != null) {
                    return (!dbl_value.HasValue || Double.MinValue.Equals(dbl_value));
                }
            }

            if (value is Single? || value is Single) {
                var dbl_value = value as Single?;
                if (dbl_value != null) {
                    return (!dbl_value.HasValue || Single.MinValue.Equals(dbl_value));
                }
            }

            if (value is Boolean? || value is Boolean) {
                var bool_value = value as Boolean?;
                if (bool_value != null) {
                    return (!bool_value.HasValue);
                }
            }

            if (value is string) {
                var str_value = value as string;
                if (str_value != null) {
                    return (String.Empty.Equals(str_value));
                }
            }

            return false;
		}

		public static Object EmptyToDBNull(object value) {
			if (IsNullOrEmptyOrMinValue(value)) {
				return DBNull.Value;
			}

			return value;
		}

		public static Object EmptyToDBNull(string value) {
			if (string.IsNullOrEmpty(value)) {
				return DBNull.Value;
			}
			return value;
		}

		public static Object EmptyToDBNull(DateTime value) {
			if (DateTime.MinValue.Equals(value)) {
				return DBNull.Value;
			}
			return value;
		}

		public static Object EmptyToDBNull(DateTime? value) {
			if (value == null || !value.HasValue || DateTime.MinValue.Equals(value.Value)) {
				return DBNull.Value;
			}
			return value;
		}

		public static Object EmptyToDBNull(int value) {
			if (int.MinValue.Equals(value)) {
				return DBNull.Value;
			}
			return value;
		}

		public static Object EmptyToDBNull(int? value) {
			if (value == null || !value.HasValue || int.MinValue.Equals(value)) {
				return DBNull.Value;
			}
			return value;
		}

        /// <summary>
        /// VB6で保存された文字列の改行が'\n'（LF）だけの場合があるので必ず'\r\n'(CRLF)になる様に補正する。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertLf(string value) {
            if (value != null) {
                // \r\nを一度\nに置き換え、その後'\n'を'\r\n'に変換する。
                value = value.Replace("\r\n", "\n").Replace("\n", "\r\n");
            }
            return value;
        }

		public static string MakeSureLenB(string s, int iLen) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

			while (HankakuZenkaku.LenB(s) > iLen) {
				s = s.Substring(0, s.Length - 1);
			}
			return s;
		}

		public static string ListOfStringToString(ICollection<string> sList) {
			if (sList == null) return string.Empty;
			if (sList.Count == 0) return string.Empty;

			var rtn = new StringBuilder();

			foreach (string s in sList) {
				rtn.Append("'" + s + "',");
			}

			if (rtn.Length > 0) {
				rtn.Remove(rtn.Length - 1, 1);
			}

			return rtn.ToString();
		}


        // --> Common.GetJapanDate()に移動。
        //public static DateTime JapanDate() {
        //    return JapanNow().Date;
        //}

        // --> Common.GetJapanNow()に移動。
        //public static DateTime JapanNow() {
        //    return DateTime.UtcNow.AddHours(9);
        //}

        /// <summary>
		/// 100ミリ秒以下を無視して日付を比較する。
		/// DBに保存してから再度読み込んだ日付と保存前の日付を普通に比較すると異なる場合がある。
		/// 原因は、DBに保存する際に10ミリ秒以下の値が四捨五入される為。
		/// このメソッドではその対策として日付の違いが100ミリ秒以下の場合は同じと見なして０を返す。
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		public static int CompareDateMSec(DateTime d1, DateTime d2) {
			double diff = Math.Abs(d2.Subtract(d1).TotalMilliseconds);
			if (diff <= 100) return 0;

			return d1.CompareTo(d2);
		}

        /// <summary>
        /// 日付型に格納されている時刻を「HH:mm」形式の文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string TimeHHmm(DateTime? d) {
            return TimeStr(d, false, false);
        }

        /// <summary>
        /// 日付型に格納されている時刻を「HH:mm」形式の文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string TimeHHmm(DateTime d) {
            return TimeStr(d, false, false);
        }

        /// <summary>
        /// 日付型に格納されている時刻を「HH:mm:ss」形式の文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string TimeHHmmss(DateTime? d) {
            return TimeStr(d, true, false);
        }

        /// <summary>
        /// 日付型に格納されている時刻を「HH:mm:ss」形式の文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string TimeHHmmss(DateTime d) {
            return TimeStr(d, true, false);
        }

        /// <summary>
        /// 日付型に格納されている時刻を文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="Sec">true:秒を含める、false:秒を含めない</param>
        /// <param name="MSec">true: ミリ秒を含める、false： ミリ秒を含めない</param>
        /// <returns></returns>
        public static string TimeStr(DateTime? d, bool Sec, bool MSec) {
            if (d == null) return String.Empty;
            if (!d.HasValue) return String.Empty;

            return TimeStr(d.Value, Sec, MSec);
        }

        /// <summary>
        /// 日付型に格納されている時刻を文字列に変換する。
        /// </summary>
        /// <param name="d"></param>
        /// <param name="Sec">true:秒を含める、false:秒を含めない</param>
        /// <param name="MSec">true: ミリ秒を含める、false： ミリ秒を含めない</param>
        /// <returns></returns>
        public static string TimeStr(DateTime d, bool Sec, bool MSec) {
            string s = d.Hour.ToString("00") + ":" + d.Minute.ToString("00");

            if (Sec)
                s += ":" + d.Second.ToString("00");
            if (MSec)
                s += "." + d.Millisecond;

            return s;
        }

        /// <summary>
        /// Culture設定に影響されずに確実に日付をMM/dd/yyの文字列に変換する。
        /// ストアドのパラメータがchar(8)になっている場合にこれを使う。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string DateStrMDY(DateTime d) {
            
            string s = d.Month.ToString("00")
                     + "/" + d.Day.ToString("00")
                     + "/" + d.Year.ToString("0000").Substring(2);
            return s;
        }

        /// <summary>
        /// Culture設定に影響されずに確実に日付をMM/dd/yyの文字列に変換する。
        /// ストアドのパラメータがchar(8)になっている場合にこれを使う。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string DateStrMDY(DateTime? d) {
            if (d == null || !d.HasValue) return String.Empty;

            return DateStrMDY(d.Value);
        }

        /// <summary>
        /// 任意の型を受け取って日付型に変換し、さらに指定のフォーマットの文字列に変換して返す。
        /// 値がnullの場合は空文字列を返す。
        /// 
        /// フォーマットに「MMM」が含まれている場合は言語設定に関わらず英語の月名（3文字）に変換する。
        /// (d=12/25/2013, dateFormat="dd MMM,yy"の場合 --> 「25 DEC,13」を返す。)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static string DateStr(Object obj, string dateFormat) {
            if (obj == null) return string.Empty;
            var d = obj as DateTime?;
            if (d == null || !d.HasValue) return String.Empty;
            var s = d.Value.ToString(dateFormat);

            if (dateFormat.Contains("MMM") && !dateFormat.Contains("MMMM")) {
                //CultureがFrance/Italianなどの場合でもMMMが英語の月名（3文字）になる様に変換。
                var tmpMonth = d.Value.ToString("MMM");
                var month = GetMonthNameEng(d);         //言語設定に関わらず必ず英語の月名を返す。(5/22/2013 EUR要望)
                s = s.Replace(tmpMonth, month);
                s = s.ToUpper();
            }

            return s;
        }

        /// <summary>
        /// 日付から英語の月名（3文字）を返す。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetMonthNameEng(DateTime? d) {
            if (!d.HasValue) return string.Empty;
            return GetMonthNameEng(d.Value.Month);
        }

        public static string GetMonthNameEng(int month) {
            string[] monthNames = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};
            if (month < 1 || month > monthNames.Length) {
                throw new ArgumentOutOfRangeException("month", month, "Value must be between 1 and 12.");
            }
            return monthNames[month-1];
        }

        /// <summary>
        /// 日付から英語の曜日名を返す。
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetDayNameEng(DateTime? d) {
            if (!d.HasValue) return string.Empty;
            int n = (int)System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(d.Value);
            return GetDayNameEng(n);
        }

        public static string GetDayNameEng(int day) {
            string[] dayNames = {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            if (day < 0 || day > dayNames.Length-1) {
                throw new ArgumentOutOfRangeException("day", day, "Value must be between 0 and 6.");
            }
            return dayNames[day];
        }

        /// <summary>
        /// 任意の型を受け取って日付型に変換し、さらに指定のフォーマットの文字列に変換して返す。
        /// 値がnullの場合は空文字列を返す。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static string TimeStr(Object obj, string timeFormat) {
            return DateStr(obj, timeFormat);
        }

        /// <summary>
        /// 日付から日本語の曜日を返す。
        /// </summary>
        public static string GetWeekString(object date) {
            var dt = GetDateTime(date);

            // カルチャの「言語-国/地域」を「日本語-日本」に設定します。
            var culture = new System.Globalization.CultureInfo("ja-JP");
            // 和暦を表すクラスです。
            var jp = new System.Globalization.JapaneseCalendar();

            // 現在のカルチャで使用する暦を、和暦に設定します。
            culture.DateTimeFormat.Calendar = jp;
            return dt.ToString("dddd", culture);
        }

        /// <summary>
        /// 通貨を文字列にフォーマットする。
        /// </summary>
        /// <param name="amount">金額</param>
        /// <param name="format">通貨記号および書式</param>
        /// <returns>フォーマットされた文字列</returns>
        public static string CurStr(decimal amount, string format) {

            //円記号が特殊文字として扱われない様にエスケープ。(「\」から「\\」に)
            format = format.Replace("\\", "\\\\");      

            return amount.ToString(format);
        }


    }
}