using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models.Helpers {

    public class UpperCaseHelper {

        /// <summary>
        /// 文字列型プロパティ・フィールドで[UpperCase]属性が付いているものの値を大文字に変換する。
        /// </summary>
        /// <param name="target"></param>
        public static void Apply(object target) {
            if (target == null) return;

            if (target is IEnumerable<object>) {
                Apply((IEnumerable<object>)target);
                return;
            }

            //リフレクションで全てのフィールドとプロパティをループ処理。
            MemberInfo[] flds = target.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo f in flds) {
                if (f.MemberType == MemberTypes.Field || f.MemberType == MemberTypes.Property) {
                    var m = new MyMemberClass(target, f);

                    if (m.Value is IEnumerable<object>) {
                        //IEnumerableのフィールドがあれば全ての子要素に対して同じ処理を行う。
                        Apply((IEnumerable<object>)m.Value);

                    } else if (m.Value is string && f.GetCustomAttribute(typeof(UpperCaseAttribute)) != null) {
                        //ReadOnlyのプロパティは除外。
                        if (!m.IsReadOnly && m.HasValue) {
                            string s = (string)m.Value;
                            if (!string.IsNullOrEmpty(s)) {
                                m.Value = s.ToUpper();
                            }
                        }
                    }

                }
            }
        }

        //対象が配列やList<T>などIEnumerableの場合。
        public static void Apply(IEnumerable<object> list) {
            foreach (var item in list) {
                Apply(item);
            }
        }
    }

}
