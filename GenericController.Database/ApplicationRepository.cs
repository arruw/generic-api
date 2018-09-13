using GenericController.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericController.Database
{
    public interface IApplicationRepository<T> : IDisposable
    {
        IQueryable<T> Get();

        T Get(Guid id);

        void Create(T record);

        void Update(T record);

        void Delete(Guid id);

        int Save();

        Task<int> SaveAsync();
    }

    public class ApplicationRepository<T> : IApplicationRepository<T> where T : class, IApplicationEntity
    {
        private DbContext _context;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>().Where(e => !EF.Property<bool>(e, "IsDeleted"));
        }

        public T Get(Guid id)
        {
            return Get().SingleOrDefault(e => e.Id == id);
        }

        public void Create(T record)
        {
            //if (record.Id == default(Guid))
            //    record.Id = Guid.NewGuid();

            //record.CreatedOn = DateTime.UtcNow;
            //record.ModifiedOn = record.CreatedOn;
            //record.IsDeleted = false;
            _context.Add(record);
        }

        public void Update(T record)
        {
            //record.ModifiedOn = DateTime.UtcNow;
            //record.IsDeleted = false;
            _context.Set<T>().Attach(record);
            _context.Entry(record).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            var record = Get(id);

            if (record != null)
            {
                //record.ModifiedOn = DateTime.UtcNow;
                //record.IsDeleted = true;
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        #endregion
    }
}
