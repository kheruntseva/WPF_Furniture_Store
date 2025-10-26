using System;
using System.Data.Entity;

namespace mywpf
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<RectangleItem> RectangleItems { get; }
        IRepository<User> Users { get; }
        IRepository<Review> Reviews { get; }
        IRepository<OrderHistory> OrderHistory { get; }
        DbContext Context { get; } 
        void Save();
    }
}