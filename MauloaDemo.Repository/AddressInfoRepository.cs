using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository.Exceptions;

namespace MauloaDemo.Repository {

    public class AddressInfoRepository : BaseRepository<AddressInfo> {

        public AddressInfoRepository() {
        }

        public AddressInfoRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        public class AddressInfoResult {
            public int info_id { get; set; }
            public string message { get; set; }
        }



        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            Mapper.CreateMap<AddressInfo, AddressInfo>();
            Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<AddressInfo>> GetList(string c_num) {
            var list = await Task.Run(() =>
                          Context.AddressInfos
                                      .Where(o => o.c_num == c_num)
                                      .OrderBy(o => o.info_id)
                                      .ToList()
                      );
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void Save(AddressInfo addressInfo, LoginUser user) {
            AddressInfo oldModel = null;
            AddressInfo existing = null;
            addressInfo.last_person = user.login_id;

            if (addressInfo.info_id == 0) {
                existing = addressInfo;
                existing.create_by = user.login_id;
                existing.create_date = DateTime.UtcNow;
                existing.last_person = user.login_id;
                existing.update_date = existing.create_date;
                this.Add(existing);
            } else {
                existing = this.Find(addressInfo.info_id);
                if (existing == null) {
                    throw new Exception(string.Format("The key value '{0}' was not found.", addressInfo.info_id));
                }
                oldModel = Mapper.Map<AddressInfo>(existing);

                //楽観的排他制御 (JSを経由するとミリ秒の情報が欠落するため秒単位で比較する。)
                if (TypeHelper.CompareDateBySecond(addressInfo.update_date, existing.update_date) != 0) {
                    throw new OptimisticLockException("Address", user.IsStaff() ? existing.last_person : "");
                }
                existing.pax_type = addressInfo.pax_type;
                existing.pax_name = addressInfo.pax_name;
                existing.jpn_zip = addressInfo.jpn_zip;
                existing.addr_kana1 = addressInfo.addr_kana1;
                existing.addr_kana2 = addressInfo.addr_kana2;
                existing.addr_kana3 = addressInfo.addr_kana3;
                existing.addr_kanji1 = addressInfo.addr_kanji1;
                existing.addr_kanji2 = addressInfo.addr_kanji2;
                existing.addr_kanji3 = addressInfo.addr_kanji3;
                existing.home_tel = addressInfo.home_tel;
                existing.work_tel = addressInfo.work_tel;
                existing.cell_tel = addressInfo.cell_tel;
                existing.e_mail = addressInfo.e_mail;
                existing.last_person = user.login_id;
                existing.update_date = DateTime.UtcNow;
                this.SetModified(existing);
            }
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLog(user.login_id, addressInfo.c_num, oldModel, existing);
        }

        public async Task SaveAsync(AddressInfo addressInfo, LoginUser user) {
            await Task.Run(() => this.Save(addressInfo, user));
        }


        public void Delete(int info_id, LoginUser user) {
            var item = this.Find(info_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", info_id));
            }
            Context.Set<AddressInfo>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, item.c_num, item);
        }

        public async Task DeleteAsync(int info_id, LoginUser user) {
            await Task.Run(() => this.Delete(info_id, user));
        }

    }
}
