using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MauloaDemo.Web {

    public static class ResourceExtensions {
        private static readonly string RootNamespace = typeof(MvcApplication).Namespace;
        private static readonly Dictionary<string, ResourceManager> ResourceManagers = new Dictionary<string, ResourceManager>();
        private static readonly string ResourcesRootFolderName = "Resources";
        private static readonly string SharedResourceFileName = "Shared"; //Shared.resxのこと

        public static string Resource(this TemplateFileInfo info, string name, params object[] args) {
            return Resource(info.VirtualPath, name, args);
        }

        public static string Resource(this Controller controller, string name, params object[] args) {

            //コントローラーの名前からリソースのパスを生成する
            var fname = controller.GetType().FullName;
            fname = controller.GetType().FullName.Replace(RootNamespace + ".", "");
            var cname = fname.Split('.').ToList();

            var resourcePath = string.Format("{0}.{1}.{2}", RootNamespace, ResourcesRootFolderName, string.Join(".", cname));
            return GetString(resourcePath, name, args);
        }

        public static string Resource(this System.Web.Http.ApiController controller, string name, params object[] args) {

            //コントローラーの名前からリソースのパスを生成する
            var fname = controller.GetType().FullName;
            fname = controller.GetType().FullName.Replace(RootNamespace + ".", "");
            var cname = fname.Split('.').ToList();

            var resourcePath = string.Format("{0}.{1}.{2}", RootNamespace, ResourcesRootFolderName, string.Join(".", cname));
            return GetString(resourcePath, name, args);
        }

        private static string Resource(string virtualPath, string name, params object[] args) {
            // ビューの仮想パスから resx のパスを作成する
            var resourcePath = GetResourcePath(virtualPath);
            if (resourcePath == null) return null;
            return GetString(resourcePath, name, args);
        }

        private static string GetString(string resourcePath, string name, params object[] args) {
            var resourceManager = GetResouceManagers(resourcePath);
            string value = "";
            try {
                value = string.Format(resourceManager.GetString(name, Thread.CurrentThread.CurrentUICulture), args);
            }catch (Exception) {
                //Viewのリソースに指定したnameの定義がない場合、Sharedリソースから取得を試みる
                var sharedPath = string.Format("{0}.{1}.{2}", RootNamespace, ResourcesRootFolderName, SharedResourceFileName);
                resourceManager = GetResouceManagers(sharedPath);
                value = string.Format(resourceManager.GetString(name, Thread.CurrentThread.CurrentUICulture), args);
            }
            return value;
        }

        private static ResourceManager GetResouceManagers(string resourcePath) {
            ResourceManager resourceManager;
            if (!ResourceManagers.TryGetValue(resourcePath, out resourceManager)) {
                resourceManager = new ResourceManager(resourcePath, Assembly.GetExecutingAssembly());
                ResourceManagers.Add(resourcePath, resourceManager);
            }
            return resourceManager;
        }

        private static string GetResourcePath(string virtualPath) {
            if (virtualPath == null) return null;

            // ~/Views/Shared/_Layout.cshtml のような文字列から ~/ と .cshtml を削除する
            var tokens = virtualPath.Substring(2, virtualPath.Length - 9).Split('/');
            if (tokens.Length == 0) {
                return null;
            }

            // 先頭文字を大文字にしつつ、リソースのパスを作成
            var textInfo = CultureInfo.InvariantCulture.TextInfo;
            return string.Format("{0}.{1}.{2}", RootNamespace, ResourcesRootFolderName, string.Join(".", tokens.Select(textInfo.ToTitleCase)));
        }
    }
}