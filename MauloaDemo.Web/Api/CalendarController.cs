using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Repository;


namespace MauloaDemo.Web.Api {

    public class CalendarController : BaseApiController {

        private CustomerRepository db = new CustomerRepository();

        // GET api/Calendar
        public async Task<Calendar> Get(
                        DateTime start_date, 
                        string church_cd, 
                        string sub_agent_cd, 
                        int weeks = 4, 
                        string location = null) {

            var agent_cd = "";
            IEnumerable<AgentParent> agentList = null;
            var subAgentRepository = new SubAgentRepository(db);

            //エージェントユーザーの場合はsub_agent_cdを適切に絞り込む。
            if (_loginUser.IsAgent()) {
                //PickUp時間表示のために親AgentCdを取得。
                agent_cd = subAgentRepository.GetAgentCd(_loginUser.sub_agent_cd);

                if (_loginUser.HasChildAgents) {
                    if (string.IsNullOrEmpty(sub_agent_cd)) {
                        sub_agent_cd = _loginUser.sub_agent_cd;
                    }
                    agentList = subAgentRepository.GetChildList(sub_agent_cd);
                } else {
                    //末端のエージェントの場合は自分自身のみ。
                    sub_agent_cd = _loginUser.sub_agent_cd;
                }
            } else if (!string.IsNullOrEmpty(sub_agent_cd)) {
                //PickUp時間表示のために親AgentCdを取得。
                agent_cd = subAgentRepository.GetAgentCd(sub_agent_cd);
                if (string.IsNullOrEmpty(agent_cd)) {
                    throw new InvalidOperationException(string.Format("Invalid SubAgentCd. ({0})", sub_agent_cd));
                }

                if (sub_agent_cd != AgentParent.GetOwnSubAgentCd()) {
                    agentList = subAgentRepository.GetChildList(sub_agent_cd);
                }
            }

            //church_cdが「PHOTO」の場合以外はlocationパラメータは無視する。
            if (church_cd != ChurchBlock.CHURCH_CD_PHOTO) {
                location = null;
            }

            var calendar = new Calendar(start_date, weeks);
            var sunset_time = TypeHelper.GetStr(ConfigurationManager.AppSettings["SunsetBlockTime"]);

            IEnumerable<Models.Combined.ChurchBlockInfo> blocks = null;
            if (!string.IsNullOrEmpty(church_cd)) {
                var available_only = (_loginUser.access_level <= 1);

                //church_block, church_timeテーブルから時間枠とブロック情報を取得。
                var blockRepository = new ChurchBlockRepository(db);
                blocks = await blockRepository.GetBlocks(
                                    church_cd, 
                                    calendar.StartDate, 
                                    calendar.EndDate,
                                    string.Equals(church_cd, ChurchBlock.CHURCH_CD_PHOTO) ? "P" : "", 
                                    location, 
                                    agent_cd,
                                    _loginUser);

                foreach (var block in blocks) {
                    bool isSameAgent = block.sub_agent_cd == sub_agent_cd || string.IsNullOrEmpty(sub_agent_cd);
                    if (agentList != null && !string.IsNullOrEmpty(block.sub_agent_cd)) {
                        isSameAgent = agentList.Any(a => a.child_cd == block.sub_agent_cd);
                    } 
                    if (!isSameAgent) {
                        if (!string.IsNullOrEmpty(block.block_status)) {
                            block.block_status = "X";
                        }
                        block.c_num = "";
                        block.agent_cd = "";
                        block.sub_agent_cd = "";
                        block.g_last = "";
                        block.g_last_kanji = "";

                        if (block.is_irregular_time) {
                            //イレギュラー時間での申し込みは別のエージェントには見せなくて良い。
                            continue;
                        }
                    }

                    var item = new CalendarItem() {
                        Type = "BLOCK",
                        Date = block.block_date.Date,
                        StartTime = block.start_time,
                        EndTime = null,
                        PickUpTime = block.pickup_time,
                        block_status = block.block_status ?? "",
                        book_status = block.book_status ?? "",
                        c_num = block.c_num ?? "",
                        agent_cd = block.agent_cd ?? "",
                        sub_agent_cd = block.sub_agent_cd ?? "",
                        g_last = block.g_last ??  "",
                        g_last_kanji = block.g_last_kanji ?? "",
                        church_cd = block.church_cd,
                        is_sunset = block.is_sunset,
                        is_irregular_time = block.is_irregular_time,
                        is_finalized = block.is_finalized
                    };
                    calendar.AddCalendarItem(item);
                }

            } else {
                //customerテーブルから挙式日が期間内のカスタマー一覧を取得。
                var prms = new CustomerRepository.SearchParams();
                prms.srch_type = "customer";
                prms.date_from = calendar.StartDate;
                prms.date_to = calendar.EndDate;
                prms.church_cd = church_cd;
                prms.sub_agent_cd = sub_agent_cd;
                prms.book_status = "";
                prms.date_type = "W";
                prms.skip = 0;
                prms.take = 1000;
                prms.pageSize = 1000;
                var result = await Task.Run(() => db.Search(prms, this._loginUser));

                //カスタマーをCalendarItemとして追加。
                foreach (var customer in result.data) {
                    var item = new CalendarItem() {
                        Type = "CUST",
                        Date = customer.wed_date.Value.Date,
                        StartTime = customer.wed_time,
                        EndTime = null,
                        PickUpTime = null,
                        block_status = "*BKD*",
                        book_status = customer.book_status,
                        c_num = customer.c_num,
                        agent_cd = customer.agent_cd,
                        sub_agent_cd = customer.sub_agent_cd,
                        g_last = customer.g_last,
                        g_last_kanji = "",  //customer.g_last_kanji ?? "",
                        church_cd = customer.church_cd ?? "",
                        is_irregular_time = customer.is_irregular_time,
                        is_finalized = customer.final_date.HasValue
                    };
                    calendar.AddCalendarItem(item);
                }
            }


            foreach (var day in calendar.Days) {
                foreach (var item in day.Items) {
                    var s = "";
                    if (item.Type == "CUST") {
                        s = string.Format("{0} {1} ({2})", item.StartTimeStr, item.church_cd, item.agent_cd);
                    } else {
                        if (!string.IsNullOrEmpty(item.g_last)) {
                            if (string.IsNullOrEmpty(sub_agent_cd)) {
                                s = string.Format("{0} {1} ({2})", item.StartTimeStr, item.church_cd, item.agent_cd);
                            } else {
                                s = string.Format("{0} {1} {2}", item.StartTimeStr, item.church_cd, item.g_last);
                            }
                        } else {
                            s = string.Format("{0} {1}", item.StartTimeStr, item.block_status);
                            if (!string.IsNullOrEmpty(item.PickUpTimeStr)) {
                                s += string.Format(" (P/U {0})", item.PickUpTimeStr);
                            }
                        }
                    }
                    item._description = s;
                }
        	}       

            return calendar;
        }

    }
}