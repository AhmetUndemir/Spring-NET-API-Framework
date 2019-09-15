using System;
using System.Threading.Tasks;
using SimpleSprint.Data.Interfaces;
using SimpleSprint.Models;
using SimpleSprint.Models.Auth;

namespace SimpleSprint.Data {
    public class UnitOfWork : IUnitOfWork {
        private readonly SsDbContext _dbContext;
        public UnitOfWork (SsDbContext dbContext) {
            if (dbContext == null)
                throw new ArgumentNullException ("Db Context Can Not Be Null");
            _dbContext = dbContext;

            _user = CreateRepo<User> ();
        }

        private readonly Lazy<IRepository<User>> _user;

        #region TableProperties
        public IRepository<User> User => _user.Value;
        #endregion

        public void Dispose () {
            _dbContext.Dispose ();
            GC.SuppressFinalize (this);
        }

        public int SaveChanges () {
            try {
                return _dbContext.SaveChanges ();
            } catch {
                throw;
            }
        }

        private Lazy<IRepository<TModel>> CreateRepo<TModel> () where TModel : class {
            return new Lazy<IRepository<TModel>> (() => new Repository<TModel> (_dbContext));
        }

        public async Task<int> SaveChangesAsync () {
            try {
                return await _dbContext.SaveChangesAsync ();
            } catch {
                throw;
            }
        }
    }
}