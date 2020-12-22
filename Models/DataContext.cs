using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Readible.Models
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration config;

        public DataContext(IConfiguration config)
        {
            this.config = config;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookComment> BookComments { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var server = config["Db:Server"];
            var port = config["Db:Port"];
            var database = config["Db:Database"];
            var user = config["Db:User"];
            var password = config["Db:Password"];

            optionsBuilder.UseNpgsql(
                $"Server={server};Port={port};Database={database};User Id={user};Password={password}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>().HasMany(e => e.ConfirmedOrders).WithOne(f => f.ConfirmedManager)
                .HasForeignKey(f => f.ConfirmerId);
            modelBuilder.Entity<Manager>().HasMany(e => e.CompletedOrders).WithOne(f => f.CompletedManager)
                .HasForeignKey(f => f.CompleterId);
            modelBuilder.Entity<Order>().HasOne(e => e.ConfirmedManager).WithMany(f => f.ConfirmedOrders)
                .HasForeignKey(f => f.ConfirmerId);
            modelBuilder.Entity<Order>().HasOne(e => e.CompletedManager).WithMany(f => f.CompletedOrders)
                .HasForeignKey(f => f.CompleterId);
        }
    }
}