using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace MauloaDemo.Utilities {

    /// <summary>
    /// オブジェクトに対してリフレクションを使って全プロパティ(およびフィールド)に一括処理を行うユーティリティクラス。
    /// </summary>
    public class ObjectReflectionHelper {

        /// <summary>
        /// 文字列型プロパティ・フィールドの末尾の空白を除去。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="convertBlankToNull">空白文字のみの場合にnullに変換するかどうか。</param>
        public static void TrimStrings(object target, bool convertBlankToNull = false) {
            if (target == null) return;

            if (target is IEnumerable<object>) {
                TrimStrings((IEnumerable<object>)target, convertBlankToNull);
                return;
            }

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = target.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo f in flds) {
                if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                    var m = new MyMemberClass(target, f);

                    if (m.Value is IEnumerable<object>) {
                        //IEnumerableのフィールドがあれば全ての子要素に対して同じ処理を行う。
                        TrimStrings((IEnumerable<object>)m.Value, convertBlankToNull);

                    } else if (m.Value is string) {
                        //ReadOnlyのプロパティは除外。
                        if (!m.IsReadOnly && m.HasValue) {
                            string s = ((string)m.Value).TrimEnd();
                            if (convertBlankToNull && string.IsNullOrWhiteSpace(s)) {
                                s = null;
                            }
                            m.Value = s;
                        }
                    }

                } 
            }
        }

        //対象が配列やList<T>などIEnumerableの場合。
        public static void TrimStrings(IEnumerable<object> list, bool convertBlankToNull = false) {
            foreach (var item in list) {
                TrimStrings(item, convertBlankToNull);
            }
        }

        //対象が配列やList<string>の場合。
        public static void TrimStrings(List<string> list, bool convertBlankToNull = false) {
            var len = list.Count;
            for (int i = 0; i < len; i++) {
                list[i] = list[i].TrimEnd();
            }
        }


        /// <summary>
        /// 日付型プロパティ・フィールドのKindにLocalをセット。(JSON変換時にUTCとの時差が出る問題の対応)
        /// </summary>
        /// <param name="target"></param>
        public static void SetDateTimeKind(object target, DateTimeKind kind) {
            if (target == null) return;

            if (!target.GetType().IsClass || target.GetType().IsValueType || target.GetType().IsPrimitive || target is string) {
                return;
            }

            if (target is IEnumerable<object>) {
                SetDateTimeKind((IEnumerable<object>)target, kind);
                return;
            }

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = target.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo f in flds) {
                if (f.MemberType == MemberTypes.Field /*|| f.MemberType == MemberTypes.Property*/) {
                    var m = new MyMemberClass(target, f);

                    if (m.Value is IEnumerable<object>) {
                        //IEnumerableのフィールドがあれば全ての子要素に対して同じ処理を行う。
                        SetDateTimeKind((IEnumerable<object>)m.Value, kind);
                    }
                    else if (m.Value is DateTime) {
                        //ReadOnlyのプロパティは除外。
                        if (!m.IsReadOnly && m.HasValue) {
                            m.Value= DateTime.SpecifyKind((DateTime)m.Value, kind);
                        }
                    } else if (m.Value is DateTime?) {
                        //ReadOnlyのプロパティは除外。
                        if (!m.IsReadOnly && m.HasValue && ((DateTime?)m.Value).HasValue) {
                            m.Value = DateTime.SpecifyKind(((DateTime?)m.Value).GetValueOrDefault(), kind);
                        }
                    } 

                }
            }
        }

        //対象が配列やList<T>などIEnumerableの場合。
        public static void SetDateTimeKind(IEnumerable<object> list, DateTimeKind kind) {
            for(int i=0; i< list.Count();i++) {
                var item = list.ElementAt(i);
                SetDateTimeKind(item, kind);
            }
        }

        public static void SetDateTimeKind(List<DateTime> list, DateTimeKind kind) {
            var len = list.Count;
            for (int i = 0; i < len; i++) {
                list[i] = DateTime.SpecifyKind(list[i], kind);
            }
        }

        public static void SetDateTimeKind(List<DateTime?> list, DateTimeKind kind) {
            var len = list.Count;
            for (int i = 0; i < len; i++) {
                if (list[i].HasValue) {
                    list[i] = DateTime.SpecifyKind( list[i].GetValueOrDefault(), kind);
                }
            }
        }

        public static void SetDateTimeKind(ref DateTime d, DateTimeKind kind) {
            d = DateTime.SpecifyKind(d, kind);
        }

        public static void SetDateTimeKind(ref DateTime? d, DateTimeKind kind) {
            if (d.HasValue) {
                d = DateTime.SpecifyKind(d.GetValueOrDefault(), kind);
            }
        }



        public static string BuildCSVHeaderStringFromObject(object obj, string[] columns, string[] titles) {
            var sOut = new StringBuilder();

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            foreach (MemberInfo f in flds) {
                string title;

                if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                    var m = new MyMemberClass(obj, f);
                    title = m.Name;

                    if (columns != null) {
                        title = null;
                        for (int ix = 0; ix < columns.Length; ix++) {
                            if (string.Equals(columns[ix], m.Name)) {
                                title = m.Name;
                                if (titles != null && !string.IsNullOrWhiteSpace(titles[ix])) {
                                    title = titles[ix];
                                }
                            }
                        }
                    }

                    if (title != null) {
                        if (i > 0) sOut.Append(",");
                        sOut.Append(title);
                        i++;
                    }
                }
            }

            return sOut.ToString();
        }

        public static string BuildCSVHeaderStringFromObjectSortByColumns(object obj, string[] columns, string[] titles) {
            var sOut = new StringBuilder();

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            for (int ix = 0; ix < columns.Length; ix++) {
                string column = columns[ix];

                foreach (MemberInfo f in flds) {
                    string title = null;

                    if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                        var m = new MyMemberClass(obj, f);
                        if (string.Equals(column, m.Name)) {
                            title = m.Name;
                            if (titles != null && !string.IsNullOrWhiteSpace(titles[ix])) {
                                title = titles[ix];
                            }
                        }

                        if (title != null) {
                            if (i > 0) sOut.Append(",");
                            sOut.Append(title);
                            i++;
                            break;
                        }
                    }
                }
            }

            return sOut.ToString();
        }

        public static string BuildCSVStringFromObject(object obj, string[] columns) {
            var sOut = new StringBuilder();

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            foreach (MemberInfo f in flds) {

                if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                    var m = new MyMemberClass(obj, f);
                    string str;

                    if (columns == null || columns.Contains(m.Name)) {
                        str = null;

                        if (m.FieldType.Equals(typeof(DateTime)) || m.FieldType.Equals(typeof(DateTime?))) {
                            str = GetDateStringForCSV(m.Value, m.Name);
                        } else if (m.FieldType.Equals(typeof(string)) || m.FieldType.IsValueType) {
                            str = TypeHelper.GetStrTrim(m.Value);
                        }

                        if (str != null) {
                            //半角カンマは全角カンマに変換する。
                            str = str.Replace(",", "，");

                            if (i > 0) sOut.Append(",");
                            sOut.Append(str);
                            i++;
                        }
                    }
                }
            }

            return sOut.ToString();
        }

        public static string BuildCSVStringFromObjectSortByColumns(object obj, string[] columns) {
            var sOut = new StringBuilder();

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            int i = 0;
            foreach (string column in columns) {

                foreach (MemberInfo f in flds) {

                    if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                        var m = new MyMemberClass(obj, f);

                        if (string.Equals(column, m.Name)) {
                            string str = null;

                            if (m.FieldType.Equals(typeof(DateTime)) || m.FieldType.Equals(typeof(DateTime?))) {
                                str = GetDateStringForCSV(m.Value, m.Name);
                            } else if (m.FieldType.Equals(typeof(string)) || m.FieldType.IsValueType) {
                                str = TypeHelper.GetStrTrim(m.Value);
                            }

                            if (str != null) {
                                //半角カンマは全角カンマに変換する。
                                str = str.Replace(",", "，");

                                if (i > 0) sOut.Append(",");
                                sOut.Append(str);
                                i++;
                                break;
                            }
                        }
                    }
                }
            }

            return sOut.ToString();
        }

        public static string GetDateStringForCSV(object v, string colname) {
            string s = "";
            if (v == null || DBNull.Value.Equals(v)) return s;

            System.DateTime d = TypeHelper.GetDateTime(v);
            if (d.Hour != 0 || d.Minute != 0 || d.Second != 0) {
                if (colname.EndsWith("_time")) {
                    s = d.ToString("HH:mm");
                } else {
                    s = d.ToString("yyyy/MM/dd HH:mm:ss");
                }
            } else {
                s = d.ToString("yyyy/MM/dd");
            }
            return s;
        }




    }
}
