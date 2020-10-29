using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using CBAF;

namespace MauloaDemo.Repository {
    public static class RegionConfig {
        public const string DEFAULT_REGION = "HWI";
        //public const string REGION_ALL = "ALL";

        private static string GetRegionConfigPath() {
            string path = ConfigurationManager.AppSettings["RegionConfigPath"];
            path = String.IsNullOrWhiteSpace(path) ? String.Empty : path;

            if (path.Contains("%ROOT%")) {
                path = path.Replace("%ROOT%", RegionConfig.GetApplicationRootPath());
            }

            if (String.Empty.Equals(path)) {
                path = RegionConfig.GetApplicationRootPath();
            }

            if (!path.EndsWith("\\")) {
                path += "\\";
            }
            path += "region.config";
            return path;
        }

        public static string GetApplicationRootPath() {
            string path = string.Empty;

            //if (HttpContext.Current != null) {
            //    if (HttpContext.Current.Server != null) {
            //        path = HttpContext.Current.Server.MapPath("/");
            //    }
            //}

            if (String.Empty.Equals(path)) {
                path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("/", "\\");
            }

            if (path.EndsWith("\\")) {
                path = path.TrimEnd("\\".ToCharArray());
            }

            return path.ToLower();
        }

        private static XmlDocument LoadXmlDocument() {
            XmlDocument xml = new XmlDocument();
            string path = RegionConfig.GetRegionConfigPath();
            xml.Load(path);
            return xml;
        }

        private static XmlNode GetRegionNode(string region_cd) {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNode node = xml.SelectSingleNode("//region[@region_cd='" + region_cd + "']");
            return node;
        }

        private static XmlNode GetRegionNodeFromAreaCd(string area_cd) {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNode node = xml.SelectSingleNode("//area[@area_cd='" + area_cd + "']");
            node = node.ParentNode;
            return node;
        }

        public static string GetRegionCdFromAreaCd(string area_cd) {
            XmlNode node = GetRegionNodeFromAreaCd(area_cd);
            string region_cd = GetAttrValue(node, "region_cd");
            return region_cd;
        }


        public static string GetDefaultAreaCd(string region_cd) {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNode region_node = null;
            XmlNode area_node = null;
            string area_cd = null;

            if (!String.IsNullOrWhiteSpace(region_cd)) {
                region_node = xml.SelectSingleNode("//region[@region_cd='" + region_cd + "']");
                if (region_node != null) {
                    area_node = region_node.SelectSingleNode("./area[@default='True']");
                    if (area_node == null) {
                        area_node = area_node.SelectSingleNode("./area[0]");
                    }
                }
            }
            if (area_node != null) {
                area_cd = GetAttrValue(area_node, "area_cd");
            }
            return area_cd;
        }

        private static XmlNode GetAreaNode(string region_cd, string area_cd) {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNode node = null;

            if (!String.IsNullOrWhiteSpace(region_cd)) {
                node = xml.SelectSingleNode("//region[@region_cd='" + region_cd + "']");
                if (node != null) {
                    node = node.SelectSingleNode("./area[@area_cd='" + area_cd + "']");
                }
            } else {
                node = xml.SelectSingleNode("//area[@area_cd='" + area_cd + "']");
            }

            return node;
        }

        private static XmlNode GetBranchNode(string region_cd, string area_cd, string branch_cd) {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNode areaNode = GetAreaNode(region_cd, area_cd);
            XmlNode branchNode = null;

            if (areaNode != null) {
                branchNode = areaNode.SelectSingleNode("//branch[@branch_cd='" + branch_cd + "']");
            }

            return branchNode;
        }

        public static XmlNodeList GetRegionNodeList() {
            XmlDocument xml = RegionConfig.LoadXmlDocument();
            XmlNodeList list;

            //if (ActiveOnly) {
            //    list = xml.SelectNodes("//region[not(@active='false')]");
            //}
            //else {
            list = xml.SelectNodes("//region");

            //}

            return list;
        }

        public static string GetRegionAttr(string region_cd, string attr_name) {
            XmlNode node = RegionConfig.GetRegionNode(region_cd);
            if (node != null) {
                XmlAttribute att = node.Attributes[attr_name];
                if (att != null) {
                    return att.Value;
                }
            }
            return String.Empty;
        }

        public static string GetAreaAttr(string region_cd, string area_cd, string attr_name) {
            XmlNode node = RegionConfig.GetAreaNode(region_cd, area_cd);
            if (node != null) {
                XmlAttribute att = node.Attributes[attr_name];
                if (att != null) {
                    return att.Value;
                }
            }
            return String.Empty;
        }

        public static string GetRegionOrAreaAttr(
                                string region_cd,
                                string area_cd,
                                string attr_name,
                                string default_region_cd,
                                string default_area_cd,
                                string default_value) {

            var s = RegionConfig.GetAreaAttr(region_cd, area_cd, attr_name);
            if (string.IsNullOrEmpty(s)) {
                s = RegionConfig.GetRegionAttr(region_cd, attr_name);
                if (string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(default_region_cd)) {
                    s = GetRegionOrAreaAttr(default_region_cd, default_area_cd, attr_name, null, null, default_value);
                }
            }
            s = string.IsNullOrEmpty(s) ? default_value : s;
            s = s ?? string.Empty;
            return s;
        }

        public static string GetAttrValue(XmlNode node, string attr_name) {
            if (node != null) {
                XmlAttribute att = node.Attributes[attr_name];
                if (att != null) {
                    return att.Value;
                }
            }
            return String.Empty;
        }

        public static string GetServerLocation() {
            return TypeHelper.GetStr(ConfigurationManager.AppSettings["ServerLocation"]);
        }

        public static string GetServerLocationForRegionCd(string region_cd) {
            return GetRegionAttr(region_cd, "server_location");
        }

        public static bool IsServiceEnabled(string region_cd) {
            string svc_url = GetRegionAttr(region_cd, "svc_url");
            return !String.IsNullOrWhiteSpace(svc_url);
        }

        public static bool IsDBConnected(string region_cd) {
            string dbname = GetRegionAttr(region_cd, "dbname");
            string dbserver = GetRegionAttr(region_cd, "dbserver");
            return (!String.IsNullOrWhiteSpace(dbname) && !String.IsNullOrWhiteSpace(dbserver));
        }

        //アプリケーションのデフォルトURLを返す。
        public static string GetBaseUrl(string region_cd) {
            string base_url = GetRegionAttr(region_cd, "base_url");
            return base_url;
        }

        //Base URLからホスト名・ドメイン名までを除いてパスだけを返す。
        public static string GetBasePath(string region_cd) {
            string url = GetBaseUrl(region_cd);
            string path = GetPathFromURL(url);
            return path;
        }

        //iWinkのデフォルトURLを返す。
        public static string GetiWinkUrl(string region_cd) {
            string url = GetRegionAttr(region_cd, "iwink_url");
            return url;
        }

        //iWink URLからホスト名・ドメイン名までを除いてパスだけを返す。
        public static string GetiWinkPath(string region_cd) {
            string url = GetiWinkUrl(region_cd);
            string path = GetPathFromURL(url);
            return path;
        }

        //URLからホスト名・ドメイン名までを除いてパスだけを返す。
        public static string GetPathFromURL(string url) {
            string path = "";
            int p = url.IndexOf("://");
            if (p > 0) {
                int q = url.IndexOf("/", p + 3);
                if (q > 0) {
                    path = url.Substring(q);
                }
            }

            return path;
        }


        public static short GetRegionTimeDiffFromUTC(string region_cd) {
            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd", "region_cd is required for RegionConfig.GetRegionTimeDiffFromUTC().");
            }

            short diff = TypeHelper.GetShort(GetRegionAttr(region_cd, "TimeDiffFromUTC"));
            return diff;
        }

        public static DateTime GetRegionNow(string region_cd) {
            short diff = GetRegionTimeDiffFromUTC(region_cd);
            DateTime now = DateTime.UtcNow.AddHours(diff);
            return now;
        }

        public static DateTime GetRegionToday(string region_cd) {
            return GetRegionNow(region_cd).Date;
        }

        public static string GetCurrencyFormat(string region_cd) {
            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd", "region_cd is required for RegionConfig.GetCurrencyFormat().");
            }

            string format = TypeHelper.GetStr(GetRegionAttr(region_cd, "currency_format"));
            return format;
        }

        public static string GetCurrencySymbol(string region_cd) {
            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd", "region_cd is required for RegionConfig.GetCurrencySymbol().");
            }

            string s = TypeHelper.GetStr(GetRegionAttr(region_cd, "currency_symbol"));
            return s;
        }

        public static string GetCurrencyFormatWithSymbol(string region_cd) {
            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd", "region_cd is required for RegionConfig.GetCurrencyFormatWithSymbol().");
            }

            string s = TypeHelper.GetStr(GetRegionAttr(region_cd, "currency_format_with_symbol"));
            return s;
        }

        public static string GetRsAssignDefaults(string region_cd, string area_cd) {
            var json_str = RegionConfig.GetAreaAttr(region_cd, area_cd, "rs_assign_defaults");
            if (string.IsNullOrEmpty(json_str)) {
                var def_area_cd = RegionConfig.GetDefaultAreaCd(region_cd);
                json_str = RegionConfig.GetAreaAttr(region_cd, def_area_cd, "rs_assign_defaults");
                if (string.IsNullOrEmpty(json_str)) {
                    json_str = RegionConfig.GetAreaAttr("HWI", "HNL", "rs_assign_defaults");
                }
            }
            return json_str;
        }

        public static string GetOpenHours(string region_cd, string area_cd, string default_value) {
            return GetRegionOrAreaAttr(region_cd, area_cd, "open_hours", "HWI", "HNL", default_value);
        }

        public static string GetWedScheduleNote(string region_cd, string area_cd) {
            return GetRegionOrAreaAttr(region_cd, area_cd, "wedschedule_note", null, null, string.Empty);
        }

        public static bool IsRegionActive(string region_cd) {
            var attr_active = GetRegionAttr(region_cd, "active");
            if (string.IsNullOrEmpty(attr_active)) {
                attr_active = "true";
            }
            return TypeHelper.GetBool(attr_active);
        }
    }
}

