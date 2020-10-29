using System;
using System.Linq;
using System.Collections.Generic;

namespace MauloaDemo.Repository {

    public interface IRepository<T> 
        where T : class {

        T Find(params object[] keyValues);

        //bool Exists(params  object[] keyValues);

        IQueryable<T> GetList();

        void Add(T entity);

        void Remove(T entity);

        void SetModified(T entity);

        void SaveChanges();

        void ApplyMappings(T entity, bool convertBlankToNull = false);

        void ApplyMappings(IEnumerable<T> list, bool convertBlankToNull = false);
    }
}

