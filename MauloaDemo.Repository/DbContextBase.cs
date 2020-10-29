using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
//using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using CBAF;
using MauloaDemo.Models;


namespace MauloaDemo.Repository {

    public abstract class DbContextBase : DbContext {
        internal const string DBSERVER_PLACEHOLDER = "%DBSERVER%";
        internal const string DBNAME_PLACEHOLDER = "%DBNAME%";
        internal const string DBUSER_PLACEHOLDER = "%DBUSER%";
        internal const string DBPWD_PLACEHOLDER = "%DBPWD%";

        public DbContextBase()
            : base("CatWebConnection") {
            SetConfiguration();
        }

        public DbContextBase(string connectionStringName)
            : base(connectionStringName) {
            SetConfiguration();
        }

        public DbContextBase(DbConnection existingConnection)
            : base(existingConnection, false) {
            SetConfiguration();
        }

        private void SetConfiguration() {

            //動的プロキシの生成を抑止する。（これでWCFサービスでのSerialize時にエラーが出なくなる。但しLazy Loadingも出来なくなる。）
            this.Configuration.ProxyCreationEnabled = false;

            //変更追跡機能を使わない。--> ObjectReflectionHelperのTrimStringsメソッドで固定長文字列フィールドの空白除去を行う為。
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        protected static string GetConnectionStringForRegion(string connectionStringName, string region_cd) {
            var connectionString = TypeHelper.GetStr(ConfigurationManager.ConnectionStrings[connectionStringName]);

            //string dbserver = RegionConfig.GetRegionAttr(region_cd, "dbserver");
            //string dbname = RegionConfig.GetRegionAttr(region_cd, "dbname");
            //string dbuser = RegionConfig.GetRegionAttr(region_cd, "dbuser");
            //string dbpwd = RegionConfig.GetRegionAttr(region_cd, "dbpwd");
            //dbpwd = Encryption.Decrypt(dbpwd);

            //if (String.IsNullOrWhiteSpace(dbserver) || String.IsNullOrWhiteSpace(dbname)) {
            //    throw new InvalidOperationException(String.Format("Invalid region_cd. ({0})", region_cd));
            //}

            //connectionString = connectionString
            //                   .Replace(DBSERVER_PLACEHOLDER, dbserver)
            //                   .Replace(DBNAME_PLACEHOLDER, dbname)
            //                   .Replace(DBUSER_PLACEHOLDER, dbuser)
            //                   .Replace(DBPWD_PLACEHOLDER, dbpwd);

            return connectionString;
        }

        /// <summary>
        /// 地域に応じて接続するDBを切り替える。
        /// もし現在のサーバー環境で接続可能な状態にない(=region.configで設定されていない)地域コードが指定された場合は例外を発生する。
        /// </summary>
        /// <param name="region_cd">接続する地域のコード</param>
        protected void SetConnectionString(string connectionStringName, string region_cd) {
            string conn_str = GetConnectionStringForRegion(connectionStringName, region_cd);
            this.Database.Connection.ConnectionString = conn_str;
        }

        protected IDbConnection GetConnection(IDbTransaction tran) {

            //EntityFrameworkと同じConnectionを使う。
            IDbConnection cn = (tran != null) ? tran.Connection : ((IObjectContextAdapter)this).ObjectContext.Connection;

            if (cn.State != ConnectionState.Open) {
                cn.Open();
            }

            return cn;
        }

        /// <summary>
        /// SQLを実行してDataSetを返す。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="prms"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public DataSet OpenDataSet(string sql, IDataParameter[] prms, IDbTransaction tran = null, int timeout = 0) {
            GetConnection(tran);                                                        //Closed状態の時に自動的にOpenする。
            SqlConnection sqlConnection = (SqlConnection)this.Database.Connection;      

            //リフレクションを用いてEntityTransactionからSqlTransactionを取得。
            SqlTransaction sqlTran = null;
            if (tran != null) {
                sqlTran = (SqlTransaction)tran.GetType().InvokeMember("StoreTransaction",
                                                BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.InvokeMethod
                                                | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic, null, tran, new object[0]);
                sqlConnection = sqlTran.Connection;
            }

            var cmd = sqlConnection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = sqlTran;

            if (timeout <= 0) {
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultQueryTimeout"]);
            }
            cmd.CommandTimeout = timeout;

            //以下の2行はタイムアウトエラー対策。(設定が違うとクエリー実行プランが変わるらしい?)
            // 参考URL：  http://www.atmarkit.co.jp/bbs/phpBB/viewtopic.php?topic=38608&forum=7&start=8
            //           http://stackoverflow.com/questions/4539765/code-keeps-timing-out
            //           http://stackoverflow.com/questions/9974/query-times-out-from-web-app-but-runs-fine-from-management-studio
            //
            cmd.CommandText = "SET ARITHABORT ON";
            cmd.ExecuteNonQuery();

            cmd.CommandText = sql;

            if (prms != null) {
                foreach (var p in prms) {
                    cmd.Parameters.Add(p);
                }
            }

            var adapter = new SqlDataAdapter((SqlCommand)cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            return ds;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private int ExecuteSQLOrStoredProc( 
                        CommandType commandType,
                        string sql, 
                        IDataParameter[] prms, 
                        IDbTransaction tran = null, 
                        int timeout = 0) {
            GetConnection(tran);                                                        //Closed状態の時に自動的にOpenする。
            SqlConnection cn = (SqlConnection)this.Database.Connection;

            //Get sql transaction via reflection
            SqlTransaction sqlTran = null;
            if (tran != null) {
                sqlTran = (SqlTransaction)tran.GetType().InvokeMember("StoreTransaction",
                                                BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.InvokeMethod
                                                | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic, null, tran, new object[0]);
                cn = sqlTran.Connection;
            }
            var cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = sqlTran;

            if (timeout <= 0) {
                timeout = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultQueryTimeout"]);
            }
            cmd.CommandTimeout = timeout;

            //以下の2行はタイムアウトエラー対策。(設定が違うとクエリー実行プランが変わるらしい?)
            // 参考URL：  http://www.atmarkit.co.jp/bbs/phpBB/viewtopic.php?topic=38608&forum=7&start=8
            //           http://stackoverflow.com/questions/4539765/code-keeps-timing-out
            //           http://stackoverflow.com/questions/9974/query-times-out-from-web-app-but-runs-fine-from-management-studio
            //
            cmd.CommandText = "SET ARITHABORT ON";
            cmd.ExecuteNonQuery();

            cmd.CommandText = sql;
            cmd.CommandType = commandType;

            if (prms != null) {
                foreach (var p in prms) {
                    cmd.Parameters.Add(p);
                }
            }

            int resultCount = cmd.ExecuteNonQuery();
            return resultCount;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public int ExecuteSQL(
                        string sql, 
                        IDataParameter[] prms, 
                        IDbTransaction tran = null, 
                        int timeout = 0) {

            return ExecuteSQLOrStoredProc(CommandType.Text, sql, prms, tran, timeout);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public int ExecuteStoredProcedure(
                        string procName, 
                        IDataParameter[] prms, 
                        IDbTransaction tran = null, 
                        int timeout = 0) {

            return ExecuteSQLOrStoredProc(CommandType.StoredProcedure, procName, prms, tran, timeout);
        }

        public IDbTransaction BeginTrans(IsolationLevel iso = IsolationLevel.ReadCommitted) {
            IDbConnection cn = GetConnection(null);
            var tran = cn.BeginTransaction(iso);
            return tran;
        }

        public void AddSqlParam(
                        StringBuilder criteria_buffer,
                        string add_criteria) {
            if (criteria_buffer.Length > 0) criteria_buffer.Append(" AND ");
            criteria_buffer.Append(" (");
            criteria_buffer.Append(add_criteria);
            criteria_buffer.Append(") ");
        }

        public void AddSqlParamIfTrue(
                        bool condition,
                        StringBuilder criteria_buffer,
                        string add_criteria) {
            if (!condition) return;
            AddSqlParam(criteria_buffer, add_criteria);
        }

        public void AddSqlParam(
                        StringBuilder criteria_buffer,
                        string add_criteria,
                        List<SqlParameter> prms,
                        string paramName,
                        object value) {

            AddSqlParam(
                        criteria_buffer,
                        add_criteria,
                        prms,
                        paramName,
                        value, 
                        true);
        }

        public void AddSqlParam(
                        StringBuilder criteria_buffer,
                        string add_criteria,
                        List<SqlParameter> prms,
                        string paramName,
                        object value,
                        bool ignoreIfNullOrEmpty) {

            if (TypeHelper.IsNullOrEmptyOrMinValue(value)) {
                //NULLや空白値、最小値を無視する場合（デフォルト）は何もせずに戻る。
                if (ignoreIfNullOrEmpty) return;

                //無視しない場合はDBNull.Valueに置き換える。
                value = DBNull.Value;
            };

            if (criteria_buffer.Length > 0) criteria_buffer.Append(" AND ");
            criteria_buffer.Append(" (");
            criteria_buffer.Append(String.Format(add_criteria, paramName));
            criteria_buffer.Append(") ");

            prms.Add(new SqlParameter(paramName, value));
        }

        public void MergeCriteriaToSql(StringBuilder sql_buffer, StringBuilder criteria_buffer) {
            if (sql_buffer == null || criteria_buffer == null) {
                throw new ArgumentNullException();
            }

            if (criteria_buffer.Length > 0) {
                sql_buffer.Append(" WHERE ");
                sql_buffer.Append(criteria_buffer);
            }
        }

    
    }
}