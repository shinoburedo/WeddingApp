using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CBAF;
using MauloaDemo.Models;


namespace MauloaDemo.Repository {
    public class RegionConfigRepository : IRepository<Region> {

        private Dictionary<string, Region> _regions;

        public RegionConfigRepository() {

            //region.configファイルの情報を読み込む。
            XmlNodeList list = RegionConfig.GetRegionNodeList();
            _regions = new Dictionary<string, Region>();

            foreach (XmlNode regionNode in list) {
                var region = new Region();
                region.region_cd = RegionConfig.GetAttrValue(regionNode, "region_cd");
                region.region_name = RegionConfig.GetAttrValue(regionNode, "region_name");
                region.region_name_jpn = RegionConfig.GetAttrValue(regionNode, "region_name_jpn");
                region.server_location = RegionConfig.GetAttrValue(regionNode, "server_location");

                string active = RegionConfig.GetAttrValue(regionNode, "active");
                region.active = String.IsNullOrWhiteSpace(active) ? true : TypeHelper.GetBool(active);

                region.region_group = RegionConfig.GetAttrValue(regionNode, "region_group");
                region.overseas = TypeHelper.GetBool(RegionConfig.GetAttrValue(regionNode, "overseas"));
                region.TimeDiffFromUTC = TypeHelper.GetShort(RegionConfig.GetAttrValue(regionNode, "TimeDiffFromUTC"));
                region.SvrTimeZone = RegionConfig.GetAttrValue(regionNode, "SvrTimeZone");
                region.base_url = RegionConfig.GetAttrValue(regionNode, "base_url");
                region.svc_url = RegionConfig.GetAttrValue(regionNode, "svc_url");
                region.tax_type = RegionConfig.GetAttrValue(regionNode, "tax_type");
                region.paper_size = RegionConfig.GetAttrValue(regionNode, "paper_size");

                region.currency_format = RegionConfig.GetAttrValue(regionNode, "currency_format");
                region.currency_symbol = RegionConfig.GetAttrValue(regionNode, "currency_symbol");
                region.currency_format_with_symbol = RegionConfig.GetAttrValue(regionNode, "currency_format_with_symbol");

                region.enable_OKA_CSC = TypeHelper.GetBool(RegionConfig.GetAttrValue(regionNode, "enable_OKA_CSC"));
                region.dbname = RegionConfig.GetAttrValue(regionNode, "dbname");
                region.dbserver = RegionConfig.GetAttrValue(regionNode, "dbserver");

                _regions.Add(region.region_cd, region);
            }
        }

        public string RegionCd {
            get { throw new NotImplementedException(); }
        }

        //public DbContextBase Context {
        //    get { throw new NotImplementedException(); }
        //}

        public Region Find(params object[] keyValues) {
            return this.Find((string)keyValues[0]);
        }

        public Region Find(string region_cd) {
            Region region = null;
            _regions.TryGetValue(region_cd, out region);
            return region;
        }

        public IQueryable<Region> GetList() {
            var list = _regions.Values.AsQueryable();
            list = list.Where(m => m.active == true);
            return list;
        }

        public IQueryable<Region> GetList(LoginUser loginUser,
                                    bool activeOnly,
                                    bool include_ALL,
                                    bool include_Overseas) {

            var list = _regions.Values.AsQueryable();

            if (activeOnly) {
                list = list.Where(m => m.active == true);
            }

            if (!include_ALL) {
                list = list.Where(m => m.region_cd != "ALL");
            }
            if (!include_Overseas) {
                list = list.Where(m => m.overseas == false);
            }

            //if (loginUser != null && loginUser.AccessLevels != null) {
            //    //アクセス権限のある地域のみ返す。
            //    var accessList = loginUser.AccessLevels;
            //    list = list.Where(m => accessList.Any(a => a.region_cd == m.region_cd && a.access_level > 0));
            //}

            return list;
        }

        public IQueryable<Region> GetListForiWink(LoginUser loginUser,
                                    bool activeOnly,
                                    bool include_ALL,
                                    bool include_Overseas) {

            var list = _regions.Values.AsQueryable();

            if (activeOnly) {
                list = list.Where(m => m.active == true);
            }

            if (!include_ALL) {
                list = list.Where(m => m.region_cd != "ALL");
            }
            if (!include_Overseas) {
                list = list.Where(m => m.overseas == false);
            }

            //if (loginUser != null && loginUser.iWinkAccessLevels != null) {
            //    //アクセス権限のある地域のみ返す。
            //    var accessList = loginUser.iWinkAccessLevels;
            //    list = list.Where(m => accessList.Any(a => a.region_cd == m.region_cd && a.access_level > 0));
            //}

            return list;
        }

        public bool Exists(string region_cd) {
            Region region = this.Find(region_cd);
            return (region != null);
        }

        public bool Exists(params object[] keyValues) {
            return this.Exists((string)keyValues[0]);
        }



        public void Add(Region entity) {
            throw new NotImplementedException();
        }

        public void Remove(Region entity) {
            throw new NotImplementedException();
        }

        public void SetModified(Region entity) {
            throw new NotImplementedException();
        }

        public void SaveChanges() {
            throw new NotImplementedException();
        }


        public void ApplyMappings(Region entity, bool convertBlankToNull = false) {
            throw new NotImplementedException();
        }

        public void ApplyMappings(IEnumerable<Region> list, bool convertBlankToNull = false) {
            throw new NotImplementedException();
        }
    }
}


