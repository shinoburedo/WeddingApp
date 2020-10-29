using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Utilities {

    public class HankakuZenkaku {
        public const int LOCALEID_JAPAN = 1041;
        private static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        private const string AllowedChars = " -,\\^~`._*&%+#@!$=()[]{}?<>:;\"'|/";

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 文字のバイト長を返します。Encoding指定なし。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int LenB(string str) {
            return LenB(str, sjisEnc);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 文字のバイト長を返します。Encoding指定なし。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int LenB(string str, Encoding enc) {
            if (string.IsNullOrEmpty(str)) return 0;

            int num = enc.GetByteCount(str);
            return num;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 全角文字(2byte)の文字列かどうかをチェック。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool isZenkaku(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int num = sjisEnc.GetByteCount(str);
            return (num == str.Length * 2);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 半角文字(1byte)の文字列かどうかをチェック。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool isHankaku(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int num = sjisEnc.GetByteCount(str);
            return (num == str.Length);
        }

        //使い方の例です。。。
        //  public static void Main() {
        //    string str = "全角文字だけ";
        //    Console.WriteLine(isZenkaku(str)); // 出力：True
        //    Console.WriteLine(isHankaku(str)); // 出力：False

        //    str = "ﾊﾝｶｸonly";
        //    Console.WriteLine(isZenkaku(str)); // 出力：False
        //    Console.WriteLine(isHankaku(str)); // 出力：True

        //    str = "全角and半角";
        //    Console.WriteLine(isZenkaku(str)); // 出力：False
        //    Console.WriteLine(isHankaku(str)); // 出力：False
        //  }

        //================================================================
        public static bool isZenkakuOrAlphaNumOrSymbol(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int i = 0;
            string t = null;

            for (i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1);
                if (isHankaku(t)) {
                    if ((!TypeHelper.IsNumeric(t)) && (!IsAlpha(t)) && (AllowedChars.IndexOf(t) < 0)) {
                        return false;
                    }
                }
            }

            return true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// アルファベット(a-z, A-Z)から成る文字列かどうかをチェック。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool IsAlpha(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int i = 0;
            string t = null;
            int a = 0;

            for (i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1).ToUpper();

                a = t.ToCharArray()[0];
                if (a < "A".ToCharArray()[0] | a > "Z".ToCharArray()[0]) {
                    return false;
                }
            }

            return true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 半角数字(0-9)から成る文字列かどうかをチェック。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[masa]	2/15/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static bool isHankakuNumbers(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int i = 0;
            string t = null;
            int a = 0;

            for (i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1);
                if (!isHankaku(t))
                    return false;

                a = t.ToCharArray()[0];
                if (a < "0".ToCharArray()[0] | a > "9".ToCharArray()[0]) {
                    return false;
                }
            }

            return true;
        }

        //================================================================
        public static bool isHankakuRomaji(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            int i = 0;
            string t = null;

            for (i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1);
                if (!isHankaku(t))
                    return false;
                if ((!IsAlpha(t)) && (AllowedChars.IndexOf(t) < 0))
                    return false;
            }

            return true;
        }

        public static bool isHankakuAlphaNumeric(string str) {
            if (string.IsNullOrEmpty(str)) return false;

            string t = null;

            for (int i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1);

                if (!isHankaku(t))
                    return false;

                if ((!IsAlpha(t)) && (!TypeHelper.IsNumeric(t)))
                    return false;
            }

            return true;
        }

        public static bool isHankakuAlphaNumericOrSymbol(string str, bool allowBlank = true) {
            if (string.IsNullOrEmpty(str)) return false;
            if (!allowBlank && str.Contains(" ")) return false;

            string t = null;

            for (int i = 0; i <= str.Length - 1; i++) {
                t = str.Substring(i, 1);

                if (!isHankaku(t))
                    return false;

                if ((!IsAlpha(t)) && (!TypeHelper.IsNumeric(t)) && (AllowedChars.IndexOf(t) < 0))
                    return false;
            }

            return true;
        }

        //================================================================
        public static string toHankaku(string str) {
            if (string.IsNullOrEmpty(str)) return str;

            return Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Narrow, LOCALEID_JAPAN);
        }

        //================================================================
        public static string toZenkaku(string str) {
            if (string.IsNullOrEmpty(str)) return str;

            return Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.Wide, LOCALEID_JAPAN);
        }


        public static void ValidateHankakuAlphaNumeric(string s, string caption, string field_name) {
            if (string.IsNullOrEmpty(s)) return;

            //半角英数字と一部の記号以外は例外を発生。
            if (!isHankakuAlphaNumericOrSymbol(s)) {
                string msg = caption + " can include only alphabets and numbers.";
                throw new ArgumentException(msg, field_name);
            }
        }

    }
}