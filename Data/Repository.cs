using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleSprint.Data.Interfaces;

namespace SimpleSprint.Data
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TModel> _dbSet;

        public Repository(SsDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TModel>();
        }
        ~Repository()
        {
            this._context.Dispose();
        }


        public void Delete(TModel entity)
        {
            _dbSet.Remove(entity);
            //_context.SaveChanges();
        }

        public void Delete(IEnumerable<TModel> entities)
        {
            _dbSet.RemoveRange(entities);
            //_context.SaveChanges();
        }

        public TModel GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public void Insert(TModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Add(entity);
            //_context.SaveChanges();
        }

        public void Insert(IEnumerable<TModel> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.AddRange(entities);
            //_context.SaveChanges();
        }

        public void Update(TModel entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            //_context.SaveChanges();
        }

        public void Update(IEnumerable<TModel> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
            //_context.SaveChanges();
        }


        #region Properties


        public virtual IQueryable<TModel> Table => _dbSet;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<TModel> TableNoTracking => _dbSet.AsNoTracking();



        #endregion


    }
}