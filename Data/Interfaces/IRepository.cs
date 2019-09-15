using System.Collections.Generic;
using System.Linq;

namespace SimpleSprint.Data.Interfaces
{
    public interface IRepository<TModel> where TModel : class
    {
          #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TModel GetById(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(TModel entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<TModel> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TModel entity);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<TModel> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(TModel entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<TModel> entities);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TModel> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<TModel> TableNoTracking { get; }

        #endregion
    }
}