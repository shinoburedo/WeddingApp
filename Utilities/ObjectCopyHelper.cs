using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;


namespace WatabeWedding.Utilities {
    public class ObjectCopyHelper {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Wink/iWink用のCBAF2008ライブラリから移植したコード。(参照用のTemporary)
        /// 
        /// オブジェクトのコピーはこれを使わずにAutoMapperを使う事。
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[vsmike]	4/5/2006	Created
        ///     [masa]      11/13/2008  modified to copy BaseChiledlen
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void CopyValues(object oFrom, object oTo) {
			if (oFrom == null)
				throw new ArgumentNullException("oFrom", "Source is not set.");
			if (oTo == null)
				throw new ArgumentNullException("oTo", "Destination is not set.");

			if (oFrom.GetType().IsArray && oTo.GetType().IsArray) {
				throw new Exception("Must use CopyArray method to copy an array.");
			}

            MemberInfo[] fldsFrom = oFrom.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            MemberInfo[] fldsTo = oTo.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);

			foreach (MemberInfo fFrom in fldsFrom) {

				if ((fFrom.MemberType == MemberTypes.Property | fFrom.MemberType == MemberTypes.Field)) {
                    foreach (MemberInfo fTo in fldsTo) {

						if ((fTo.MemberType == MemberTypes.Property | fTo.MemberType == MemberTypes.Field)) {
                            MyMemberClass myFrom = new MyMemberClass(oFrom, fFrom);
                            MyMemberClass myTo = new MyMemberClass(oTo, fTo);


							if (myFrom.Name.ToLower() == myTo.Name.ToLower() && (myTo.IsReadOnly == false)) {
								if (myFrom.FieldType.IsArray) {
									if (myTo.FieldType.IsArray) {
										object[] tmpToArr = (object[])myTo.Value;
										//<-- テンポラリ変数を使ってCopyArrayを呼ばないとByRefで上手く値が返って来ない！  (以前はCtype(myTo.value, Object()を２つ目の引数として渡していたが、これでは値が返って来ない。))
										CopyArray((object[])myFrom.Value, ref tmpToArr);
										myTo.Value = tmpToArr;
									} else if (myTo.FieldType.IsGenericType) {
										CopyArrayToGenericList(myFrom, myTo);
									}


								} else if ((!myFrom.FieldType.IsValueType) & ((!object.ReferenceEquals(myFrom.FieldType, typeof(string))))) {
									if (myFrom.Value == null) {
										myTo.Value = null;
									} else if (myFrom.FieldType.IsGenericType) {
										CopyGenericListToArray(myFrom, myTo);
									} else {
										if (myTo.Value == null) {
											myTo.Value = myTo.FieldType.Assembly.CreateInstance(myTo.FieldType.FullName);
										}
										CopyValues(myFrom.Value, myTo.Value);
									}

								} else {
									if ((!object.ReferenceEquals(myTo.FieldType, myFrom.FieldType))) {
										if (object.ReferenceEquals(myTo.FieldType, typeof(string))) {
											myTo.Value = Convert.ToString(myFrom.Value);
										} else if (object.ReferenceEquals(myTo.FieldType, typeof(System.DateTime))) {
											if (TypeHelper.IsDate(myFrom.Value as string)) {
												myTo.Value = Convert.ToDateTime(myFrom.Value);
											} else {
												myTo.Value = System.DateTime.MinValue;
											}
										} else if (object.ReferenceEquals(myTo.FieldType, typeof(int))) {
											myTo.Value = Convert.ToInt32(myFrom.Value);
										} else if (object.ReferenceEquals(myTo.FieldType, typeof(decimal))) {
											myTo.Value = Convert.ToDecimal(myFrom.Value);
										} else if (object.ReferenceEquals(myTo.FieldType, typeof(double))) {
											myTo.Value = Convert.ToDouble(myFrom.Value);
										} else if (object.ReferenceEquals(myTo.FieldType, typeof(float))) {
											myTo.Value = Convert.ToSingle(myFrom.Value);

										} else if (myTo.FieldType.IsEnum & myFrom.FieldType.IsEnum) {
											myTo.Value = Enum.Parse(myTo.FieldType, myFrom.Value.ToString());

										} else {
											myTo.Value = myFrom.Value;
										}
									} else {
										myTo.Value = myFrom.Value;
									}
								}

								break; // TODO: might not be correct. Was : Exit For
							}
						}
					}
				}
			}

		}


        public static void CopyArray(object[] arrFrom, ref object[] arrTo) {
            if (arrFrom == null) {
                arrTo = null;
                return;
            }

            Type fromType = arrFrom.GetType().GetElementType();
            Type toType = default(Type);

            if (arrTo == null) {
                toType = fromType;
            }
            else {
                toType = arrTo.GetType().GetElementType();
            }

            if (arrTo == null || arrTo.Length != arrFrom.Length) {
                //同じ要素数に合わせる
                arrTo = (object[])System.Array.CreateInstance(toType, arrFrom.GetLength(0));
            }

            for (int i = 0; i <= arrFrom.GetUpperBound(0); i++) {
                if (object.ReferenceEquals(fromType, typeof(string)) || fromType.IsValueType) {
                    arrTo[i] = arrFrom[i];
                }
                else {
                    if (arrTo[i] == null) {
                        arrTo[i] = toType.Assembly.CreateInstance(toType.FullName);
                    }
                    CopyValues(arrFrom[i], arrTo[i]);
                }
            }

        }


        private static void CopyArrayToGenericList(MyMemberClass myFrom, MyMemberClass myTo) {
            object[] arrFrom = (object[])myFrom.Value;
            if (arrFrom == null) {
                myTo.Value = null;
                return;
            }

            IList<object> objTo = (IList<object>)myTo.Value;
            object itm = null;
            for (int i = 0; i <= arrFrom.GetUpperBound(0); i++) {
                if ((arrFrom[i] != null)) {
                    itm = System.Activator.CreateInstance(myTo.FieldType.GetGenericArguments()[0]);
                    CopyValues(arrFrom[i], itm);
                    objTo.Add(itm);
                }
            }

            myTo.Value = objTo;
        }



        private static void CopyGenericListToArray(MyMemberClass myFrom, MyMemberClass myTo) {
            ICollection<object> objFrom = (ICollection<object>)myFrom.Value;
            if (objFrom == null) {
                myTo.Value = null;
                return;
            }

            object[] objTo = null;

            //同じ要素数で配列を生成
            objTo = (object[])System.Array.CreateInstance(myTo.FieldType.GetElementType(), objFrom.Count);

            int i = 0;
            foreach (object fromItem in objFrom) {
                objTo[i] = myTo.FieldType.GetElementType().Assembly.CreateInstance(myTo.FieldType.GetElementType().FullName);
                CopyValues(fromItem, objTo[i]);
                i += 1;
            }

            myTo.Value = objTo;
        }


    }
}


