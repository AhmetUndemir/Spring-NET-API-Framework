using System;
using System.Threading.Tasks;
using SimpleSprint.Models;
using SimpleSprint.Models.Auth;

namespace SimpleSprint.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> User { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();

    }
}