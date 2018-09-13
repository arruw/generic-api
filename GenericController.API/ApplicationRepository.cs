using GenericController.API;
using GenericController.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericController.API
{
    public interface IApplicationRepository<T> : IDisposable
    {
        IQueryable<T> Get();

        T Get(Guid id);

        Task<T> GetAsync(Guid id);

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
            return _context.Set<T>();
        }

        public T Get(Guid id)
        {
            return Get().SingleOrDefault(e => e.Id == id);
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await Get().SingleOrDefaultAsync(e => e.Id == id);
        }

        public void Create(T record)
        {
            var now = DateTime.UtcNow;
            _context.Add(record);
            _context.Entry(record).Property<DateTime>("CreatedOn").CurrentValue = now;
            _context.Entry(record).Property<DateTime>("ModifiedOn").CurrentValue = now;
        }

        public void Update(T record)
        {          
            _context.Set<T>().Attach(record);
            _context.Entry(record).Property<DateTime>("ModifiedOn").CurrentValue = DateTime.UtcNow;
            _context.Entry(record).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            var record = Get(id);

            if (record != null)
            {
                _context.Remove(record);
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
