using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;


namespace MauloaDemo.Repository {

    public abstract class BaseRepository<TEntity> : IRepository<TEntity>, IDisposable, IContextHolder
        where TEntity : class {

        protected ProjMEntities _context;
        protected string _region_cd;

        internal BaseRepository(){
            //init(RegionConfig.REGION_ALL);
            init(RegionConfig.DEFAULT_REGION);
        }

        internal BaseRepository(string region_cd) {
            if (String.IsNullOrWhiteSpace(region_cd)) {
                throw new ArgumentNullException("region_cd");
            }

            init(region_cd);
        }

        //既に存在するリポジトリと共通のDbContextを使いたい場合はこのコンストラクタを使う。
        internal BaseRepository(IContextHolder sourceRepository) {
            if (sourceRepository == null) {
                throw new ArgumentNullException("sourceRepository");
            }

            _context = sourceRepository.Context;
            init(sourceRepository.RegionCd);
        }

        internal virtual void init(string region_cd) {
            _region_cd = region_cd;

            if (_context == null) { 
                _context = ProjMEntities.Create("DefaultConnection", region_cd);
            }
        }


        public string RegionCd {
            get {
                return _region_cd;
            }
        }

        public ProjMEntities Context {
            get {
                return _context;
            }
        }

        public TEntity Find(params object[] keyValues) {
            var set = _context.Set<TEntity>();

            var entity = (TEntity)set.Find(keyValues);

            ApplyMappings(entity);

            return entity;
        }

        public async Task<TEntity> FindAsync(params object[] keyValues) {
            var set = _context.Set<TEntity>();

            TEntity entity = await set.FindAsync(keyValues);

            ApplyMappings(entity);

            return entity;
        }

        public virtual void ApplyMappings(TEntity entity, bool convertBlankToNull = false) {
            if (entity == null) return;

            ObjectReflectionHelper.TrimStrings(entity, convertBlankToNull);
        }

        public void ApplyMappings(IEnumerable<TEntity> list, bool convertBlankToNull = false) {
            foreach (var item in list) {
                ApplyMappings(item, convertBlankToNull);
            }
        }

        public IQueryable<TEntity> GetList() {
            return _context.Set<TEntity>();
        }

        public bool Exists(params object[] keyValues) {
            return (this.Find(keyValues) != null);
        }

        public void Add(TEntity entity) {
            _context.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity) {
            _context.Set<TEntity>().Remove(entity);
        }

        public void SetModified(TEntity entity) {
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public void SaveChanges() {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync() {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// コードとして使用を許可しない文字をチェックする。
        /// 許可しない文字が含まれている場合は例外を発生する。
        /// </summary>
        /// <param name="cd"></param>
        protected void CheckInvalidCharsForCode(string cd) {
            const string INVALID_CHARS = "\\*/%\'\":;?,<>";

            if (string.IsNullOrEmpty(cd)) return;

            if (cd.IndexOfAny(INVALID_CHARS.ToCharArray()) >= 0) {
                throw new Exception("The code cannot contain these characters. '" + INVALID_CHARS + "' ");
            }
        }


        /// <summary>
        /// IDisposableの正しい実装方法は下記を参照。
        /// http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k%28CA1063%29;k%28TargetFrameworkMoniker-.NETFramework
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposableの正しい実装方法は下記を参照。
        /// http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k%28CA1063%29;k%28TargetFrameworkMoniker-.NETFramework
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // ここにManaged resourceの解放処理を書く。
                if (_context != null) {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    
    }
}