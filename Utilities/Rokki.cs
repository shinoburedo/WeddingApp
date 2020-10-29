using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;


namespace MauloaDemo.Utilities {

    /// <summary>
    /// iWinkから移植したクラス。
    /// 
    /// VB.NETからC#への移植には下記の自動化ツールを使用。
    /// http://converter.telerik.com/
    /// 
    /// 
    /// ●Rokkiクラスの使い方
    ///
    /// //Rokkiクラスのコンストラクタに最初の日付を渡します。
    /// var d = new DateTime(2016, 1, 1);
    /// var rokki = new Rokki(d);
    ///
    /// //これで1日目の六輝の文字列が帰ります。
    /// var result = rokki.GetCurValue();
    /// 
    /// for (var i = 0; i < 6; i++){
    ///     // ２日目以降はMoveNextした後に結果を取得します。
    ///     rokki.MoveNext();
    ///     result = rokki.GetCurValue();
    /// }
    /// </summary>
    public class Rokki {
        private int iRokki;
        private int iRokkiCur;
        private DateTime dRokkiCur;
        private List<string> aRokki;
        private List<string> aRokkiTbl;

        public Rokki(DateTime initialDate) {
            init();

            dRokkiCur = initialDate;        //.Subtract(new System.TimeSpan(1, 0, 0, 0));
            string cRokkiDt = dRokkiCur.Year.ToString() + Strings.Right("0" + dRokkiCur.Month, 2) + Strings.Right("0" + dRokkiCur.Day, 2);
            for (this.iRokki = 0; this.iRokki <= aRokkiTbl.Count - 1; this.iRokki++) {
                if (Strings.Left(Convert.ToString(aRokkiTbl[iRokki]), 8).CompareTo(cRokkiDt) > 0) {
                    break; 
                }
            }
            string sRokki = Convert.ToString(aRokkiTbl[iRokki - 1]);
            DateTime dRokkiDt = new DateTime(Convert.ToInt32(Strings.Left(sRokki, 4)), Convert.ToInt32(Strings.Mid(sRokki, 5, 2)), Convert.ToInt32(Strings.Mid(sRokki, 7, 2)));
            iRokkiCur = Convert.ToInt32(Strings.Mid(sRokki, 9, 1));
            int diff = (int)(Microsoft.VisualBasic.DateAndTime.DateDiff(DateInterval.Day, dRokkiDt, dRokkiCur));
            iRokkiCur = (iRokkiCur + diff) % 6;
        }

        public string GetCurValue() {
            return Convert.ToString(aRokki[iRokkiCur]);
        }


        public void MoveNext() {
            string cRokkiDt = null;

            dRokkiCur = dRokkiCur.AddDays(1);
            cRokkiDt = dRokkiCur.Year.ToString() + Strings.Right("0" + dRokkiCur.Month.ToString(), 2) + Strings.Right("0" + dRokkiCur.Day.ToString(), 2);
            if (Strings.Left(Convert.ToString(aRokkiTbl[iRokki]), 8) == cRokkiDt) {
                iRokkiCur = Convert.ToInt32(Strings.Mid(Convert.ToString(aRokkiTbl[iRokki]), 9, 1));
                iRokki = iRokki + 1;
            } else {
                iRokkiCur = iRokkiCur + 1;
                if (iRokkiCur > 5) {
                    iRokkiCur = 0;
                }
            }
        }

        private void init() {
            aRokki = new List<string>();
            var _with1 = aRokki;
            _with1.Add("先勝");
            _with1.Add("友引");
            _with1.Add("先負");
            _with1.Add("仏滅");
            _with1.Add("大安");
            _with1.Add("赤口");

            aRokkiTbl = new List<string>();
            var _with2 = aRokkiTbl;
            //_with2.Add("200101240");
            //_with2.Add("200102231");
            //_with2.Add("200103252");
            //_with2.Add("200104243");
            //_with2.Add("200105233");
            //_with2.Add("200106214");
            //_with2.Add("200107215");
            //_with2.Add("200108190");
            //_with2.Add("200109171");
            //_with2.Add("200110172");
            //_with2.Add("200111153");
            //_with2.Add("200112154");
            //_with2.Add("200201135");
            //_with2.Add("200202120");
            //_with2.Add("200203141");
            //_with2.Add("200204132");
            //_with2.Add("200205123");
            //_with2.Add("200206114");
            //_with2.Add("200207105");
            //_with2.Add("200208090");
            //_with2.Add("200209071");
            //_with2.Add("200210062");
            //_with2.Add("200211053");
            //_with2.Add("200212044");
            //_with2.Add("200301035");
            //_with2.Add("200302010");
            //_with2.Add("200303031");
            //_with2.Add("200304022");
            //_with2.Add("200305013");
            //_with2.Add("200305314");
            //_with2.Add("200306305");
            //_with2.Add("200307290");
            //_with2.Add("200308281");
            //_with2.Add("200309262");
            //_with2.Add("200310253");
            //_with2.Add("200311244");
            //_with2.Add("200312235");
            //_with2.Add("200401220");
            //_with2.Add("200402201");
            //_with2.Add("200403211");
            //_with2.Add("200404192");
            //_with2.Add("200405193");
            //_with2.Add("200406184");
            //_with2.Add("200407175");
            //_with2.Add("200408160");
            //_with2.Add("200409141");
            //_with2.Add("200410142");
            //_with2.Add("200411123");
            //_with2.Add("200412124");
            //_with2.Add("200501105");
            //_with2.Add("200502090");
            //_with2.Add("200503101");
            //_with2.Add("200504092");
            //_with2.Add("200505083");
            //_with2.Add("200506074");
            //_with2.Add("200507065");
            //_with2.Add("200508050");
            //_with2.Add("200509041");
            //_with2.Add("200510032");
            //_with2.Add("200511023");
            //_with2.Add("200512024");
            //_with2.Add("200512315");
            //_with2.Add("200601290");
            //_with2.Add("200602281");
            //_with2.Add("200603292");
            //_with2.Add("200604283");
            //_with2.Add("200605274");
            //_with2.Add("200606265");
            //_with2.Add("200607250");
            //_with2.Add("200608240");
            //_with2.Add("200609221");
            //_with2.Add("200610222");
            //_with2.Add("200611213");
            //_with2.Add("200612204");
            //_with2.Add("200701195");
            //_with2.Add("200702180");
            //_with2.Add("200703191");
            //_with2.Add("200704172");
            //_with2.Add("200705173");
            //_with2.Add("200706154");
            //_with2.Add("200707145");
            //_with2.Add("200708130");
            //_with2.Add("200709111");
            //_with2.Add("200710112");
            //_with2.Add("200711103");
            //_with2.Add("200712104");
            //_with2.Add("200801085");
            //_with2.Add("200802070");
            //_with2.Add("200803081");
            //_with2.Add("200804062");
            //_with2.Add("200805053");
            //_with2.Add("200806044");
            //_with2.Add("200807035");
            //_with2.Add("200808010");
            //_with2.Add("200808311");
            //_with2.Add("200809292");
            //_with2.Add("200810293");
            //_with2.Add("200811284");
            //_with2.Add("200812275");
            //_with2.Add("200901260");
            //_with2.Add("200902251");
            //_with2.Add("200903272");
            //_with2.Add("200904253");
            //_with2.Add("200905244");
            //_with2.Add("200906234");
            //_with2.Add("200907225");
            //_with2.Add("200908200");
            //_with2.Add("200909191");
            //_with2.Add("200910182");
            //_with2.Add("200911173");
            //_with2.Add("200912164");
            //_with2.Add("201001155");
            //_with2.Add("201002140");
            //_with2.Add("201003161");
            //_with2.Add("201004142");
            //_with2.Add("201005143");
            //_with2.Add("201006124");
            //_with2.Add("201007125");
            //_with2.Add("201008100");
            //_with2.Add("201009081");
            //_with2.Add("201010082");
            //_with2.Add("201011063");
            //_with2.Add("201012064");
            //_with2.Add("201101045");
            //_with2.Add("201102030");
            //_with2.Add("201103051");
            //_with2.Add("201104032");
            //_with2.Add("201105033");
            //_with2.Add("201106024");
            //_with2.Add("201107015");
            //_with2.Add("201107310");
            //_with2.Add("201108291");
            //_with2.Add("201109272");
            //_with2.Add("201110273");
            //_with2.Add("201111254");
            //_with2.Add("201112255");
            //_with2.Add("201201230");
            //_with2.Add("201202221");
            //_with2.Add("201203222");
            //_with2.Add("201204212");
            //_with2.Add("201205213");
            //_with2.Add("201206204");
            //_with2.Add("201207195");
            //_with2.Add("201208180");
            //_with2.Add("201209161");
            //_with2.Add("201210152");
            //_with2.Add("201211143");
            //_with2.Add("201212134");
            //_with2.Add("201301125");
            //_with2.Add("201302100");
            //_with2.Add("201303121");
            //_with2.Add("201304102");
            //_with2.Add("201305103");
            //_with2.Add("201306094");
            //_with2.Add("201307085");
            //_with2.Add("201308070");
            //_with2.Add("201309051");
            //_with2.Add("201310052");
            //_with2.Add("201311033");
            //_with2.Add("201312034");
            //_with2.Add("201401015");
            //_with2.Add("201401310");
            //_with2.Add("201403011");
            //_with2.Add("201403312");
            //_with2.Add("201404293");
            //_with2.Add("201405294");
            //_with2.Add("201406275");
            //_with2.Add("201407270");
            //_with2.Add("201408251");
            //_with2.Add("201409242");
            //_with2.Add("201410242");
            //_with2.Add("201411223");
            //_with2.Add("201412224");
            //_with2.Add("201501205");
            //_with2.Add("201502190");
            //_with2.Add("201503201");
            //_with2.Add("201504192");
            //_with2.Add("201505183");
            //_with2.Add("201506164");
            //_with2.Add("201507165");
            _with2.Add("201508140");
            _with2.Add("201509131");
            _with2.Add("201510132");
            _with2.Add("201511123");
            _with2.Add("201512114");
            _with2.Add("201601105");
            _with2.Add("201602080");
            _with2.Add("201603091");
            _with2.Add("201604072");
            _with2.Add("201605073");
            _with2.Add("201606054");
            _with2.Add("201607045");
            _with2.Add("201608030");
            _with2.Add("201609011");
            _with2.Add("201610012");
            _with2.Add("201610313");
            _with2.Add("201611294");
            _with2.Add("201612295");
            _with2.Add("201701280");
            _with2.Add("201702261");
            _with2.Add("201703282");
            _with2.Add("201704263");
            _with2.Add("201705264");
            _with2.Add("201706244");
            _with2.Add("201707235");
            _with2.Add("201708220");
            _with2.Add("201709201");
            _with2.Add("201710202");
            _with2.Add("201711183");
            _with2.Add("201712184");
            _with2.Add("201801175");
            _with2.Add("201802160");
            _with2.Add("201803171");
            _with2.Add("201804162");
            _with2.Add("201805153");
            _with2.Add("201806144");
            _with2.Add("201807135");
            _with2.Add("201808110");
            _with2.Add("201809101");
            _with2.Add("201810092");
            _with2.Add("201811083");
            _with2.Add("201812074");
            _with2.Add("201901065");
            _with2.Add("201902050");
            _with2.Add("201903071");
            _with2.Add("201904052");
            _with2.Add("201905053");
            _with2.Add("201906034");
            _with2.Add("201907035");
            _with2.Add("201908010");
            _with2.Add("201908301");
            _with2.Add("201909292");
            _with2.Add("201910283");
            _with2.Add("201911274");
            _with2.Add("201912265");
            _with2.Add("202001250");
            _with2.Add("202002241");
            _with2.Add("202003242");
            _with2.Add("202004233");
            _with2.Add("202005233");
            _with2.Add("202006214");
            _with2.Add("202007215");
            _with2.Add("202008190");
            _with2.Add("202009171");
            _with2.Add("202010172");
            _with2.Add("202011153");
            _with2.Add("202012154");
            _with2.Add("999999999");

        }
    }

}
