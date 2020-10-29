using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using System.Data;
using System.Data.SqlClient;
using AutoMapper;

namespace MauloaDemo.Repository {

    public class ChurchBlockRepository : BaseRepository<ChurchBlock> {

        public class SearchParams {
            public string church_cd { get; set; }
            public DateTime year { get; set; }
            public DateTime month { get; set; }
            public bool fdHol { get; set; }
            public bool stHol { get; set; }
            public bool sun { get; set; }
        }

        public class SaveParams {
            public string church_cd { get; set; }
            public List<List<ChurchAvailSaveInfo>> avails { get; set; }
        }

        public ChurchBlockRepository() {
        }

        public ChurchBlockRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ChurchBlock, ChurchBlock>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public new async Task<IEnumerable<ChurchBlock>> GetList() {
            var list = await Task.Run(() => 
                            Context.ChurchBlocks
                                  .OrderBy(o => o.church_cd)
                                  .ThenBy(o => o.block_date)
                                  .ThenBy(o => o.start_time)
                                  .Take(2000)
                                  .ToList()
                       );
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ChurchBlockInfo>> GetBlocksForDay(
                                                            string church_cd, 
                                                            DateTime block_date, 
                                                            string plan_kind, 
                                                            string agent_cd, 
                                                            LoginUser user) {
            return await this.GetBlocks(church_cd, block_date, block_date, plan_kind, church_cd, agent_cd, user);
        }

        public async Task<IEnumerable<ChurchBlockInfo>> GetBlocks( 
                                                            string church_cd, 
                                                            DateTime start_date, 
                                                            DateTime end_date, 
                                                            string plan_kind,
                                                            string location, 
                                                            string agent_cd,
                                                            LoginUser user) {

            var sql = "EXEC usp_get_church_blocks @church_cd, @start_date, @end_date, @location, @agent_cd ";

            var prms = new SqlParamSet();
            prms.AddChar("@church_cd", 5, church_cd);
            prms.AddDateTime("@start_date", start_date);
            prms.AddDateTime("@end_date", end_date);
            prms.AddChar("@location", 5, location);
            prms.AddChar("@agent_cd", 6, agent_cd);

            var list = await Task.Run(() =>
                            Context.Database.SqlQuery<ChurchBlockInfo>(sql, prms.ToArray())
                                            .ToList()
                       );

            //Access Levelが1以下の場合は空き枠のみを返す。
            if (user.access_level <= 1) {
                var hiddenBlocks = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["HiddenBlocksForAccessLevel_1"]);
                list = list.Where(i => string.IsNullOrEmpty(i.block_status))
                            .Where(i => !hiddenBlocks.Contains("[" + i.start_time_s + "]"))
                            .ToList();
            }

            ObjectReflectionHelper.TrimStrings(list);


            if (plan_kind == "P") {
                var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
                foreach (var item in list) {
                    if (item.start_time_s == SunsetBlockTime) {
                        item.is_sunset = true;
                    }
                }
            }

            return list;
        }

        public List<ChurchBlockGridResult> GetList(string church_cd, DateTime year, DateTime month, bool fdHol, bool stHol, bool sun, string time_format) {
            var startDate = new DateTime(year.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var maxRow = (endDate - startDate).Days + 1;

            var headers = GetHeaderList(church_cd, time_format);
            var list = new List<ChurchBlockGridResult>();
            var status = "";
            var date = startDate;
            for (int i = 0; i < maxRow; i++) {
                var rowData = new ChurchBlockGridResult();
                rowData.blocks = new List<ChurchBlockResult>();
                rowData.is_holiday = false;
                rowData.block_date = date;

                for (int j = 0; j < headers.Count; j++) {
                    if (j == 0) {
                        rowData.date = date.ToString("MM/dd");
                    } else if (j == 1) {
                        switch (date.DayOfWeek) {
                            case DayOfWeek.Sunday:
                                rowData.day = "SUN";
                                status = sun ? "X" : "";
                                break;
                            case DayOfWeek.Monday:
                                rowData.day = "MON";
                                status = "";
                                break;
                            case DayOfWeek.Tuesday:
                                rowData.day = "TUE";
                                status = "";
                                break;
                            case DayOfWeek.Wednesday:
                                rowData.day = "WED";
                                status = "";
                                break;
                            case DayOfWeek.Thursday:
                                rowData.day = "THU";
                                status = "";
                                break;
                            case DayOfWeek.Friday:
                                rowData.day = "FRI";
                                status = "";
                                break;
                            case DayOfWeek.Saturday:
                                rowData.day = "SAT";
                                status = "";
                                break;
                            default:
                                break;
                        }
                    } else {
                        var data = new ChurchBlockResult();
                        var headerData = headers[j];
                        data.status = status;
                        data.start_time = headerData.start_time;
                        rowData.blocks.Add(data);
                    }
                }

                date = date.AddDays(1);
                list.Add(rowData);
            }

            // Holiday
            if (stHol || fdHol) {
                var holidays = Context.Holidays.Where(m => m.holiday >= startDate && m.holiday <= endDate);

                if (stHol && !fdHol) {
                    holidays = holidays.Where(m => m.st_flag == true);
                }
                if (!stHol && fdHol) {
                    holidays = holidays.Where(m => m.st_flag == false);
                }

                holidays.ToList().ForEach(h => {
                    var rowData = list.Where(l => l.block_date == h.holiday).FirstOrDefault();
                    if (rowData != null) {
                        rowData.is_holiday = true;
                        rowData.blocks.ForEach(b => b.status = "Closed");
                    }
                });
            }

            // ChurchBlock
            var churchBlocks = Context.ChurchBlocks.Where(m => m.church_cd == church_cd
                                                            && m.block_date >= startDate
                                                            && m.block_date <= endDate)
                                                   .OrderBy(m => m.block_date)
                                                   .ThenBy(m => m.start_time)
                                                   .ToList();

            churchBlocks.ForEach(m => {
                var rowData = list.Where(l => l.block_date == m.block_date).FirstOrDefault();
                if (rowData != null) {
                    rowData.blocks.ForEach(b => {
                        if (b.start_time == m.start_time) {
                            b.status = m.block_status;
                        }
                    });
                }
            });

            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public List<ChurchBlockHeader> GetHeaderList(string church_cd, string time_format) {
            var list = new List<ChurchBlockHeader>();

            var data = Context.ChurchTimes.Where(m => m.church_cd == church_cd)
                                         .OrderBy(m => m.start_time)
                                         .ToList();

            list.Add(new ChurchBlockHeader {
                header_name = "Date"
            });

            list.Add(new ChurchBlockHeader {
                header_name = "Day"
            });

            data.ForEach(m => {
                var header = new ChurchBlockHeader();
                header.start_time = m.start_time;
                header.header_name = m.start_time.ToString(time_format);
                list.Add(header);
            });

            return list;
        }

        public List<ChurchBlock> GetList(string church_cd, DateTime year, DateTime month) {
            var startDate = new DateTime(year.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var churchBlocks = Context.ChurchBlocks.Where(m => m.church_cd == church_cd
                                                            && m.block_date >= startDate
                                                            && m.block_date <= endDate)
                                                   .OrderBy(m => m.block_date)
                                                   .ThenBy(m => m.start_time)
                                                   .ToList();
            ObjectReflectionHelper.TrimStrings(churchBlocks);
            return churchBlocks;
        }

        public async Task SaveChurchAvailListAsync(string church_cd, List<List<ChurchAvailSaveInfo>> list, LoginUser user) {
            await Task.Run(() => this.SaveChurchAvailList(church_cd, list, user));
        }

        public void SaveChurchAvailList(string church_cd, List<List<ChurchAvailSaveInfo>> list, LoginUser user) {

            using (var tran = Context.BeginTrans()) {
                try {
                    list.ForEach(m => {
                        SaveChurchAvail(church_cd, m, user, tran);
                    });
                    tran.Commit();
                } catch (Exception ex) {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        public void SaveChurchAvail(string church_cd, List<ChurchAvailSaveInfo> avails, LoginUser user, IDbTransaction tran) {

            var procname = "usp_save_church_avail";

            avails.ForEach(m => {
                ChurchBlock oldModel = null;
                ChurchBlock item_db = null;
                item_db = this.Context.ChurchBlocks.Where(i => i.church_cd == church_cd && i.block_date == m.block_date && i.start_time == m.start_time).FirstOrDefault();
                oldModel = Mapper.Map(item_db, typeof(ChurchBlock), typeof(ChurchBlock)) as ChurchBlock;

                var prmReturn = new SqlParameter("@rtn", SqlDbType.Int);
                prmReturn.Direction = ParameterDirection.ReturnValue;

                var prms = new SqlParamSet();
                prms.AddChar("@church_cd", 5, church_cd);
                prms.AddDateTime("@block_date", m.block_date);
                prms.AddDateTime("@start_time", m.start_time);
                prms.AddChar("@block", 6, m.status);
                prms.AddChar("@last_person", 15, user.last_person);

                Context.ExecuteStoredProcedure(procname, prms.ToArray(), tran);

                var rtn = TypeHelper.GetInt(prmReturn.Value);
                if (rtn != 0) {
                    throw new Exception();
                }

                //変更ログを保存。
                var log = new LogChangeRepository(this);
                if (oldModel != null && string.IsNullOrEmpty(m.status)) {
                    log.InsertLogForDelete(user.login_id, null, oldModel);
                } else {
                    if (item_db == null) item_db = new ChurchBlock();
                    item_db.block_status = m.status;
                    log.InsertLog(user.login_id, null, oldModel, item_db);
                }

            });
        }
    }
}
