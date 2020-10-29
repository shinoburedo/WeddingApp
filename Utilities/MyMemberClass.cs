using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MauloaDemo.Utilities {

    /// <summary>
    /// プロパティとメンバ変数を共通に扱うためのラッパークラス。
    /// </summary>
    public class MyMemberClass {

        public System.Type FieldType;
        private object _DataObject;
        private MemberInfo _fld;
        public string Name = "";

        public MyMemberClass(object oDataObject, MemberInfo fld) {
            if (fld.MemberType != MemberTypes.Property & fld.MemberType != MemberTypes.Field) {
                throw new Exception("MyMemberClass accepts only Property or Field.");
            }

            _DataObject = oDataObject;
            _fld = fld;

            this.Name = fld.Name;

            if (_fld.MemberType == MemberTypes.Field) {
                this.FieldType = ((FieldInfo)_fld).FieldType;
            }
            else if (_fld.MemberType == MemberTypes.Property) {
                this.FieldType = ((PropertyInfo)_fld).PropertyType;
            }
        }

        public object Value {
            get {
                object v = null;
                if (_fld.MemberType == MemberTypes.Field) {
                    v = ((FieldInfo)_fld).GetValue(_DataObject);
                }
                else if (_fld.MemberType == MemberTypes.Property) {
                    v = ((PropertyInfo)_fld).GetValue(_DataObject, null);
                }
                return v;
            }
            set {
                if (_fld.MemberType == MemberTypes.Field) {
                    ((FieldInfo)_fld).SetValue(_DataObject, value);
                }
                else if (_fld.MemberType == MemberTypes.Property) {
                    ((PropertyInfo)_fld).SetValue(_DataObject, value, null);
                }
            }
        }

        public bool HasValue {
            get {
                return (Value != null);
            }
        } 

        public bool IsReadOnly {
            get {
                if (_fld.MemberType == MemberTypes.Field) {
                    return false;
                }
                else {
                    return !(((PropertyInfo)_fld).CanWrite);
                }
            }
        }

        public bool IsKey{
            get {
                return _fld.IsDefined(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true);
            }
        }

        public Type GetArrayListElementType() {
            Type rtn = null;
            object[] arr = _fld.GetCustomAttributes(typeof(System.Xml.Serialization.XmlArrayItemAttribute), true);

            if ((arr != null)) {
                if (arr.Length > 0) {
                    rtn = ((System.Xml.Serialization.XmlArrayItemAttribute)arr[0]).Type;
                }
            }
            return rtn;
        }

    }

}
