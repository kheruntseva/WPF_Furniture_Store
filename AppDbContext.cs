using mywpf;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Threading.Tasks;
namespace mywpf
{
    public class AppDbContext : DbContext
{
    public DbSet<RectangleItem> RectangleItems { get; set; }
    public DbSet<ItemTranslation> ItemTranslations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public AppDbContext() : base("name=AppDbConnection")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemTranslation>()
            .HasRequired(t => t.RectangleItem)
            .WithMany(r => r.ItemTranslations)
            .HasForeignKey(t => t.RectangleItemId)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PurchasedProducts)
            .WithMany()
            .Map(m =>
            {
                m.ToTable("PurchasedProducts");
                m.MapLeftKey("UserId");
                m.MapRightKey("RectangleItemId");
            });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<OrderHistory>()
            .HasRequired(o => o.User)
            .WithMany(u => u.OrderHistory)
            .HasForeignKey(o => o.UserId)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<OrderHistory>()
            .HasRequired(o => o.RectangleItem)
            .WithMany()
            .HasForeignKey(o => o.RectangleItemId)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<OrderHistory>().ToTable("OrderHistory");

        modelBuilder.Entity<Review>()
            .HasRequired(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<Review>()
            .HasRequired(r => r.RectangleItem)
            .WithMany()
            .HasForeignKey(r => r.RectangleItemId)
            .WillCascadeOnDelete(false);

        modelBuilder.Entity<Review>().ToTable("Reviews");
    }

    // Async  методы
    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    // транкзации
    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        using (var transaction = Database.BeginTransaction())
        {
            try
            {
                await action();
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

public class OrderHistory
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RectangleItemId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Color { get; set; }
    public string Category { get; set; }
    public string Availability { get; set; }

    public virtual User User { get; set; }
    public virtual RectangleItem RectangleItem { get; set; }
}

public class Review
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    [NotMapped]
    public Guid ItemId { get; set; }
    public Guid RectangleItemId { get; set; }
    public string ReviewText { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }

    public virtual User User { get; set; }
    public virtual RectangleItem RectangleItem { get; set; }
}
}