using System;
using System.Data.Entity;

namespace mywpf
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed;

        public IRepository<RectangleItem> RectangleItems { get; }
        public IRepository<User> Users { get; }
        public IRepository<Review> Reviews { get; }
        public IRepository<OrderHistory> OrderHistory { get; }
        public DbContext Context { get { return _context; } }

        public UnitOfWork()
        {
            _context = new AppDbContext();
            RectangleItems = new Repository<RectangleItem>(_context);
            Users = new Repository<User>(_context);
            Reviews = new Repository<Review>(_context);
            OrderHistory = new Repository<OrderHistory>(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}