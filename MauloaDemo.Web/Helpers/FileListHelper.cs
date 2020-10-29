using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CBAF;
using MauloaDemo.Models;


namespace MauloaDemo.Web {

    public class FileList {

	    private string _Path;

	    public FileList(string sPath) {
		    _Path = sPath;
	    }

	    public string Path {
		    get { return _Path; }
	    }

	    public FileListItem CreateNewItem(string sUploadUser)
	    {
		    var itm = new FileListItem(this);
		    itm.UploadUser = sUploadUser;
		    return itm;
	    }

	    public FileListItem GetItem(string fname, LoginUser loginUser)
	    {
		    fname = _Path + "\\" + fname;
		    var fi = new FileInfo(fname);
            if (fi.Exists) {
                return GetItemProps(fi, loginUser);
            } else {
                return null;
            }
	    }

	    public FileListItem GetItemProps(FileInfo fi, LoginUser loginUser)
	    {
		    var itm = new FileListItem(this);
			itm.Filename = fi.Name;
			itm.Size = fi.Length;
			itm.UploadDate = fi.LastWriteTime;
            //itm.UploadDateStr = itm.UploadDate.ToString(loginUser.date_format + " " + loginUser.time_format);
            itm.UploadDateStr = itm.UploadDate.ToString("yyyy/MM/dd HH:mm:ss");

		    var fname = itm.FullpathInfo();
		    var fi2 = new FileInfo(fname);
		    if (fi2.Exists) {
                var sr = new StreamReader(fname, System.Text.Encoding.Default); //Encodingを指定しないと日本語が化ける。
			    itm.Description = sr.ReadLine();
			    if (sr.Peek() > -1) {
				    itm.UploadUser = sr.ReadLine();
			    }
			    if (sr.Peek() > -1) {
				    itm.DispOrder = TypeHelper.GetInt(sr.ReadLine());
			    }
			    sr.Close();
		    }

		    return itm;
	    }

	    public List<FileListItem> GetFileList(LoginUser loginUser)
	    {
		    DirectoryInfo d = new DirectoryInfo(_Path);
		    System.IO.FileInfo[] arrfi = null;
		    FileListItem itm = null;
		    var arrSort = new List<FileListItem>();
		    int ix = 0;

		    if (d.Exists) {
			    //フォルダ内のファイル一覧を取得
			    arrfi = d.GetFiles();
                foreach (FileInfo fi in arrfi){
				    //*.ifo, *.scc, "Thumbs.db"は除外
				    if (fi.Extension.ToLower() != ".ifo" 
                        && fi.Extension.ToLower() != ".scc" 
                        && fi.Name.ToLower() != "thumbs.db") {

                            itm = GetItemProps(fi, loginUser);
					    arrSort.Insert(ix, itm);
					    ix += 1;
				    }
	            }

			    //ソート順に従ってソートする。
			    //arrSort.TrimToSize();
			    arrSort.Sort();
		    }

		    return arrSort;
	    }

	    public void DeleteFile(string Filename)
	    {
            var fname = System.IO.Path.Combine(_Path, Filename);
		    var fi = new FileInfo(fname);
		    if (fi.Exists) {
			    fi.Delete();
		    }

		    fname = System.IO.Path.GetDirectoryName(fname) + "\\" + System.IO.Path.GetFileNameWithoutExtension(fname) + ".ifo";
            fi = new FileInfo(fname);
		    if (fi.Exists) {
			    fi.Delete();
		    }
	    }

	    public void SaveDescription(FileListItem itm)
	    {
		    //一旦ifoファイルを削除する。
		    var fname = itm.FullpathInfo();
		    var fi = new FileInfo(fname);
		    if (fi.Exists) {
			    fi.Delete();
		    }

		    //新しいDescriptionで再度作成する。
		    var sw = new StreamWriter(fname, false, System.Text.Encoding.Default);
		    sw.WriteLine(itm.Description);
		    sw.WriteLine(itm.UploadUser);
		    sw.WriteLine(itm.DispOrder);
		    sw.Close();
		    sw = null;
	    }
    }

    public class FileListItem : IComparable
    {

	    private FileList _parent;
	    public string Filename;
	    public string Description;
	    public DateTime UploadDate;
        public string UploadDateStr;
        public string UploadUser;
	    public long Size;

	    public int DispOrder;

        public FileListItem(FileList parent) {
		    _parent = parent;
        }

        public string FilenameForUrl { 
            get{
                return System.Web.HttpUtility.UrlEncode(this.Filename);
            }
        }

        public string SizeInKB {
            get {
                var kb = Math.Round((double)(this.Size / 1024.0), 0);
                return kb.ToString("#,0");
            }
        }

        public string Fullpath() {
		    string s = null;
		    s = _parent.Path + "\\" + Filename;
		    return s;
	    }

	    public string FullpathInfo() {
		    string s = null;
		    s = _parent.Path + "\\" + Filename;
		    s = Path.GetDirectoryName(s) + "\\" + Path.GetFileNameWithoutExtension(s) + ".ifo";
		    return s;
	    }

	    //DispOrder, UpdateDate, Filenameの順にソートする。
	    public int CompareTo(object obj) {
		    FileListItem itm = default(FileListItem);

		    if (object.ReferenceEquals(obj.GetType(), typeof(FileListItem))) {
			    itm = (FileListItem)obj;

			    if (this.DispOrder == itm.DispOrder) {
				    //if (this.UploadDate == itm.UploadDate) {
					    return this.Filename.CompareTo(itm.Filename);
				    //} else {
					//    return this.UploadDate.CompareTo(itm.UploadDate);
				    //}
			    } else {
				    return this.DispOrder.CompareTo(itm.DispOrder);
			    }

		    } else {
			    return 0;
		    }
	    }
    }

}

