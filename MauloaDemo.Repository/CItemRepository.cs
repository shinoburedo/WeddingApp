using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;
using System.Configuration;
using System.Data;

namespace MauloaDemo.Repository {
    public class CItemRepository : BaseRepository<CItem> {

        public class SearchParams {
            public string plan_type { get; set; }           //W=Wedding Plan, P=Photo Plan, O=Options
            public string item_type { get; set; }
            public string church_cd { get; set; }
            public string item_cd { get; set; }
            public string item_name { get; set; }

            public DateTime? wed_date { get; set; }         //discon_dateとの比較のために必要。
            public string c_num { get; set; }               //オプション検索の場合の価格取得のために必要。(c_numからwed_date, plan_kindを判断する)
            public string sub_agent_cd { get; set; }        //プラン検索の場合の価格取得のために必要。
            public bool open_to_japan_only { get; set; }
        }


        public CItemRepository() {
        }

        public CItemRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            //AutoMapper.Mapper.CreateMap<Item, Item>();
            //AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Gets the TRF item list.
        /// </summary>
        /// <param name="item_type">The item_type.</param>
        /// <param name="item_cd">The item_cd.</param>
        /// <param name="item_name">The item_name.</param>
        /// <param name="lang">The language.</param>
        /// <param name="prepare_to_cust">if set to <c>true</c> [prepare_to_cust].</param>
        /// <param name="open_to_cust">if set to <c>true</c> [open_to_cust].</param>
        /// <param name="church_cd">The church_cd if the customer has confirmed package order.</param>
        /// <returns>IEnumerable&lt;PriceListItem&gt;.</returns>
        public IEnumerable<CustomerItem> GetTrfItemList(
                                            string item_type,
                                            string item_cd,
                                            string item_name,
                                            string lang,
                                            DateTime? wed_date = null,
                                            bool prepare_to_cust = false,
                                            bool open_to_cust = true,
                                            string agent_cd = null,
                                            string church_cd = null
                                            )
        {

            var sql = @" EXEC usp_c_get_item_list
                                @language, 
                                @item_type, 
                                @item_cd, 
                                @item_name, 
                                @prepare_to_cust, 
                                @open_to_cust,
                                @agent_cd,
                                @church_cd,
                                @wed_date ";

            var prms = new SqlParamSet();

            prms.AddChar("@language", 5, lang);
            prms.AddVarChar("@item_type", 3, item_type);
            prms.AddVarChar("@item_cd", 15, item_cd);       //varcharにしないとLIKE検索が出来ない。
            prms.AddNVarChar("@item_name", 200, item_name);
            prms.AddBit("@prepare_to_cust", prepare_to_cust);
            prms.AddBit("@open_to_cust", open_to_cust);
            prms.AddChar("@agent_cd", 6, agent_cd);
            prms.AddChar("@church_cd", 5, church_cd);
            prms.AddDateTime("@wed_date", wed_date);

            var list = Context.Database.SqlQuery<CustomerItem>(sql, prms.ToArray())
                                .Take(1000)
                                .ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }


        /// <summary>
        /// Gets the item list.
        /// </summary>
        /// <returns>IEnumerable&lt;PriceListItem&gt;.</returns>
        public IEnumerable<CustomerItem> GetItemList(
                                string item_type,
                                string lang,
                                string church_cd,
                                DateTime wed_date,
                                string agent_cd)
        {

            //同じストアドの呼び出しを一箇所に集約。
            var list = this.GetTrfItemList(
                                item_type,
                                null /*item_cd*/,
                                null /*item_name*/,
                                lang,
                                wed_date,
                                false,
                                true,
                                agent_cd,
                                church_cd
                                );

            foreach (var item in list)
            {
                //item_infの編集
                //item.item_information = EditItemInfo(item.item_info);
                ////価格取得
                ////　日本から見ている場合とタヒチ、バリなど現地通貨での決済が不可能な地域からのオーダーの場合は必ず円表示
                //short price_type = WtBooking.PRICE_TYPE_DESTINATION;
                //var paymentJPY = RegionConfig.GetPaymentCurrencyCd(this.RegionCd) == "JPY";
                //if (is_jpn || paymentJPY)
                //{
                //    price_type = WtBooking.PRICE_TYPE_JAPAN;
                //}
                //var priceInfo = this.GetItemPrice(period_id, item.item_cd, area_cd, price_type, wed_date);
                //if (priceInfo != null)
                //{
                //    item.price = priceInfo.price ?? 0;
                //    item.price_currency = priceInfo.price_cur;
                //    item.price_type = priceInfo.price_type;
                //}
                //画像ファイル数
                item.cnt_picture_s = item.image_count; //GetCountPictures(this.RegionCd, item.type, item.item_cd);
                //Pkg情報取得
                if (item.item_type == "PKG")
                {
                    GetPkgInfoSub(item, lang);
                }


            }

            return list;
        }

        /// <summary>
        /// Finds the item.
        /// </summary>
        /// <param name="trfItem">The TRF item.</param>
        /// <param name="lang">The language. 'J' or 'E'.</param>
        /// <param name="region_cd">The region_cd.</param>
        /// <param name="user_location_is_japan">'true' if the user is located in Japan.</param>
        /// <param name="wed_date">The wed_date.</param>
        /// <returns>ItemInfo.</returns>
        public CustomerItem FindItem(
                                    string item_cd,
                                    string lang,
                                    DateTime wed_date)
        {

            var item_repository = new ItemRepository(this);
            var item = item_repository.Find(item_cd);

            if (item == null) return null;
            ObjectReflectionHelper.TrimStrings(item);

            var c_item = new CustomerItem();
            c_item.item_cd = item.item_cd;
            c_item.item_type = item.item_type;
            c_item.item_name = lang == "J" ? item.item_name_jpn : item.item_name;
            c_item.abbrev = item.abbrev;

            //Pkg情報取得
            if (c_item.is_pkg)
            {
                GetChurchAvailSub(c_item, wed_date, lang);
            }

            return c_item;
        }



        /// <summary>
        /// Finds the item.
        /// </summary>
        /// <param name="trfItem">The TRF item.</param>
        /// <param name="lang">The language. 'J' or 'E'.</param>
        /// <param name="region_cd">The region_cd.</param>
        /// <param name="user_location_is_japan">'true' if the user is located in Japan.</param>
        /// <param name="wed_date">The wed_date.</param>
        /// <returns>ItemInfo.</returns>
        public async Task<CustomerItem> FindItemAsyn(
                                    string item_cd,
                                    string lang,
                                    DateTime wed_date)
        {

            var item_repository = new ItemRepository(this);
            var item = await item_repository.FindAsync(item_cd);

            if (item == null) return null;
            ObjectReflectionHelper.TrimStrings(item);

            var c_item = new CustomerItem();
            c_item.item_cd = item.item_cd;
            c_item.item_type = item.item_type;
            c_item.item_name = lang == "J" ? item.item_name_jpn : item.item_name;
            c_item.abbrev = item.abbrev;

            //var sql =
            //    @"EXEC usp_c_item 
            //        @language, 
            //        @trf_kind, 
            //        @trf_type, 
            //        @area_cd, 
            //        @trf_item_cd, 
            //        @trf_cat,
            //        @period_id, 
            //        @wed_date,
            //        @is_jpn ";

            //var prms = new SqlParamSet();
            //prms.AddChar("@language", 1, lang);
            //prms.AddChar("@trf_kind", 5, trf_kind);
            //prms.AddChar("@trf_type", 5, trf_type);
            //prms.AddChar("@area_cd", 3, area_cd);
            //prms.AddChar("@trf_item_cd", 15, trf_item_cd);
            //prms.AddChar("@trf_cat", 6, trf_cat);
            //prms.AddBit("@is_jpn", user_location_is_japan);
            //prms.AddInt("@period_id", period_id);
            //prms.AddDateTime("@wed_date", wed_date);

            //var item = Context.Database.SqlQuery<TrfItemInfo>(sql, prms.ToArray())
            //                           .SingleOrDefault();

            ////Media情報取得
            //if (item.item_type == "ALB" || item.item_type == "VID")
            //{
            //    GetMediaInfoSub(item, wed_date, lang);
            //}

            //Pkg情報取得
            if (c_item.is_pkg)
            {
                GetChurchAvailSub(c_item, wed_date, lang);
            }

            return c_item;
        }


        private void GetPkgInfoSub(CustomerItem item, string lang)
        {
            var param = new ItemOptionRepository.SearchParams
            {
                item_cd = item.item_cd,
                item_type = item.item_type
            };
            var child_list = new ItemOptionRepository().SearchOptions(param).ToList();
            item.pkg_info = child_list;
        }


        private void GetChurchAvailSub(CustomerItem item, DateTime wed_date, string lang)
        {
            //ChurchAvail情報取得 wed_dateの前後1週間分
            var sql = @" EXEC usp_c_get_church_avail @date_from_p, @date_to_p, @item_cd, @church_cd, @agent_cd, @rtn_mode ";
            var prms = new SqlParamSet();
            var min_date = RegionConfig.GetRegionNow(this.RegionCd);
            min_date = min_date.AddDays(5); //最速でも5日後以降から表示
            var from = min_date > wed_date.AddDays(-3) ? TypeHelper.DateStrMDY(min_date) : TypeHelper.DateStrMDY(wed_date.AddDays(-3));
            var to = min_date > wed_date.AddDays(-3) ? TypeHelper.DateStrMDY(min_date.AddDays(6)) : TypeHelper.DateStrMDY(wed_date.AddDays(3));
            var agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["AgentCd"]);

            prms.AddChar("@date_from_p", 8, from);
            prms.AddChar("@date_to_p", 8, to);
            prms.AddChar("@item_cd", 15, item.item_cd);
            prms.AddChar("@church_cd", 5, null);
            prms.AddChar("@agent_cd", 6, agent_cd);
            prms.AddSmallInt("@rtn_mode", 0);

            var church_avail_list = Context.Database.SqlQuery<ChurchAvail>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(church_avail_list);

            if (church_avail_list != null && church_avail_list.Count > 0)
            {

                //言語に応じて曜日をセット
                var isJapan = lang == "J";
                church_avail_list.ForEach(ch =>
                            ch.day_name = ch.block_date == null
                                            ? ""
                                            : Utilities.Common.GetDayName(ch.block_date.Value, isJapan));

                //六輝取得
                //Rokkiクラスのコンストラクタに最初の日付を渡します。
                var first_day = church_avail_list[0].block_date.Value;
                var d = new DateTime(first_day.Year, first_day.Month, first_day.Day);
                var rokki = new Utilities.Rokki(d);

                //これで1日目の六輝の文字列が帰ります。
                church_avail_list[0].rokki = rokki.GetCurValue();

                for (var i = 1; i < church_avail_list.Count(); i++)
                {
                    // ２日目以降はMoveNextした後に結果を取得します。
                    rokki.MoveNext();
                    church_avail_list[i].rokki = rokki.GetCurValue();
                }

                item.church_avail = church_avail_list;
                item.church_cd = church_avail_list[0].church_cd;
            }
            else
            {
                item.church_avail = church_avail_list;
                item.church_cd = "";
            }
        }

        public IEnumerable<ChurchAvail> GetChurchAvail(string item_cd, DateTime wed_date)
        {
            //ChurchAvail情報取得 wed_dateの各church_time
            var sql = @" EXEC usp_c_get_church_avail @date_from_p, @date_to_p, @item_cd, @church_cd, @agent_cd, @rtn_mode ";
            var prms = new SqlParamSet();
            var agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["AgentCd"]);

            prms.AddChar("@date_from_p", 8, TypeHelper.DateStrMDY(wed_date));
            prms.AddChar("@date_to_p", 8, TypeHelper.DateStrMDY(wed_date));
            prms.AddChar("@item_cd", 15, item_cd);
            prms.AddChar("@church_cd", 5, null);
            prms.AddChar("@agent_cd", 6, agent_cd);
            prms.AddSmallInt("@rtn_mode", 1);

            var church_avail_list = Context.Database.SqlQuery<ChurchAvail>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(church_avail_list);
            return church_avail_list;

        }

        /// <summary>
        /// Gets the item price.
        /// </summary>
        /// <param name="period_id">The period_id.</param>
        /// <param name="item_cd">The trf_item_cd.</param>
        /// <param name="agent_cd">The area_cd.</param>
        /// <param name="price_type">The price_type.</param>
        /// <param name="wed_date">The wed_date.</param>
        /// <returns>DataSet.</returns>
        public ItemPriceInfo GetItemPrice(
                            string item_cd,
                            DateTime wed_date)
        {

            var agent_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["AgentCd"]);

            var sql = @" EXEC usp_find_price @item_cd, @wed_date, @agent_cd, @plan_kind,
			            @gross output, 
			            @d_net output, 
			            @y_net output, 
			            @tentative output ";

            var prms = new SqlParamSet();
            prms.AddChar("@item_cd", 15, item_cd);
            prms.AddDateTime("@wed_date", wed_date);
            prms.AddChar("@agent_cd", 6, agent_cd);
            prms.AddChar("@plan_kind", 1, null);
            var ret_gross= prms.AddDecimal("@gross", 0, 10, 2, ParameterDirection.Output);
            var ret_d_net = prms.AddDecimal("@d_net", 0, 10, 2, ParameterDirection.Output);
            var ret_y_net = prms.AddInt("@y_net", 0, ParameterDirection.Output);
            var ret_tentative = prms.AddBit("@tentative", false, ParameterDirection.Output);

            ////タヒチ、バリなど現地通貨での決済が不可能な地域からのオーダーの場合は必ず円表示
            //var paymentCurrencyCd = RegionConfig.GetPaymentCurrencyCd(this.RegionCd);
            //if (paymentCurrencyCd == "JPY")
            //{
            //    price_type = WtBooking.PRICE_TYPE_JAPAN;
            //}

            Context.ExecuteStoredProcedure(sql, prms.ToArray());

            var item_price_info = new ItemPriceInfo();

            item_price_info.gross = TypeHelper.GetDecimal(ret_gross.Value);
            item_price_info.d_net = TypeHelper.GetDecimal(ret_d_net.Value);
            item_price_info.y_net= TypeHelper.GetInt(ret_y_net.Value);
            item_price_info.tentative= TypeHelper.GetBool(ret_tentative.Value);

            return item_price_info;
        }

    }
}
