using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using MauloaDemo.Models;


namespace MauloaDemo.Repository {

    public class ProjMEntities : DbContextBase {

        //引数無しでnewした場合は開発用DBに接続されるので注意。
        //これは単体テストやVS2012内でControllerを自動生成させる時等に使う為のもの。
        private ProjMEntities()
            : base("DefaultConnection") {
            Database.SetInitializer<ProjMEntities>(null);

            //this.SetConnectionString("TestHNL", "HWI");
        }

        private ProjMEntities(string connectionStringName, string region_cd)
            : base(connectionStringName) {
            Database.SetInitializer<ProjMEntities>(null);

            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd");
            }

            this.SetConnectionString(connectionStringName, region_cd);
        }

        private ProjMEntities(DbConnection existingConnection)
            : base(existingConnection) {
                Database.SetInitializer<ProjMEntities>(null);
        }

        //通常はこちらでインスタンスを生成する事。
        public static ProjMEntities Create() {
            return Create("DefaultConnection", "");
        }

        //例：　var context = DestinationDbContext.Create("CatWebConnection");
        //
        public static ProjMEntities Create(string connectionStringName, string region_cd) {

            //最初にSqlConnectionを作成。
            var connectionString = GetConnectionStringForRegion(connectionStringName, region_cd);
            var sqlConnection = new SqlConnection(connectionString);

            //作成済みのSqlConnectionを渡してDbContextを作成。
            //これによってDbContextが勝手にConnectionをCloseする事が無くなる。
            //独自のSQL文やストアドを呼ぶロジックとDbContext経由のデータ操作を同一のTransaction内で実行したい場合に必要。
            //参考URL：
            //      This SqlTransaction has completed; it is no longer usable. Entity Framework Code First - Stack Overflow
            //      http://stackoverflow.com/questions/9843449/this-sqltransaction-has-completed-it-is-no-longer-usable-entity-framework-code
            //      task parallel library - Entity Framework, SqlCommand and TransactionScope - Stack Overflow
            //      http://stackoverflow.com/questions/12725805/entity-framework-sqlcommand-and-transactionscope
            //
            //      http://blogs.msdn.com/b/diego/archive/2012/01/26/exception-from-dbcontext-api-entityconnection-can-only-be-constructed-with-a-closed-dbconnection.aspx
            //      http://social.msdn.microsoft.com/Forums/en-US/adodotnetentityframework/thread/12e883cb-c12f-4116-8e2f-f44a19cf2f9b
            //
            //
            //別の選択肢としては、System.TransactionのTransactionScopeを使う方法もある。(今回は採用しない)
            //      http://sakapon.wordpress.com/2011/07/
            //      http://stackoverflow.com/questions/10915400/frequent-saves-with-entity-framework
            //      http://stackoverflow.com/questions/6028626/ef-code-first-dbcontext-and-transactions
            //
            var context = new ProjMEntities(sqlConnection);
            return context;
        }


        public DbSet<AddressInfo> AddressInfos { get; set; }

        public DbSet<Agent> Agents { get; set; }

        public DbSet<AgentParent> AgentParents { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<Arrangement> Arrangements { get; set; }

        //public DbSet<CalInfo> CalInfos { get; set; }

        //public DbSet<CalType> CalTypes { get; set; }

        public DbSet<CBooking> CBookings { get; set; }

        public DbSet<Church> Churches { get; set; }

        public DbSet<ChurchBlock> ChurchBlocks { get; set; }

        public DbSet<ChurchTime> ChurchTimes { get; set; }

        public DbSet<CItem> CItems { get; set; }

        public DbSet<CosInfo> CosInfos { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<DeliveryInfo> DeliveryInfos { get; set; }

        public DbSet<Holiday> Holidays { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ItemCost> ItemCosts { get; set; }

        public DbSet<ItemOption> ItemOptions { get; set; }

        public DbSet<ItemPrice> ItemPrices { get; set; }

        public DbSet<ItemType> ItemTypes { get; set; }

        public DbSet<ItemVendor> ItemVendors { get; set; }

        public DbSet<LogAction> LogActions { get; set; }

        public DbSet<LogChange> LogChanges { get; set; }

        public DbSet<LogChangeArchive> LogChangeArchives { get; set; }

        public DbSet<LoginUser> LoginUsers { get; set; }

        public DbSet<LoginUserToken> LoginUserTokens { get; set; }

        public DbSet<MakeInfo> MakeInfos { get; set; }

        public DbSet<MgmReport> MgmReports { get; set; }
        public DbSet<MgmReportParam> MgmReportParams { get; set; }
        public DbSet<MgmReportComboList> MgmReportComboLists { get; set; }

        public DbSet<Notice> Notices { get; set; }        

        public DbSet<PickupPlace> PickupPlaces { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<RcpInfo> RcpInfos { get; set; }

        public DbSet<Sales> Sales { get; set; }

        public DbSet<ScheduleNote> ScheduleNotes { get; set; }
        public DbSet<ScheduleNoteTemplate> ScheduleNoteTemplates { get; set; }
        public DbSet<SchedulePattern> SchedulePatterns { get; set; }
        public DbSet<SchedulePatternItem> SchedulePatternItems { get; set; }
        public DbSet<SchedulePatternLine> SchedulePatternLines { get; set; }
        public DbSet<SchedulePatternNote> SchedulePatternNotes { get; set; }
        public DbSet<SchedulePhrase> SchedulePhrases { get; set; }

        public DbSet<ShootInfo> ShootInfos { get; set; }

        public DbSet<TransInfo> TransInfos { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<WedInfo> WedInfos { get; set; }

        public DbSet<CAccount> WtAccounts { get; set; }        

    }
}
