using HomeChoreTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeChoreTracker.Api.Database
{
    public class HomeChoreTrackerDbContext : DbContext
    {
        public HomeChoreTrackerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<HomeChoreBase> HomeChoresBases { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<UserHomes> UserHomes { get; set; }
        public DbSet<HomeInvitation> HomeInvitations { get; set; }
		public DbSet<Income> Incomes { get; set; }
		public DbSet<Expense> Expenses { get; set; }
        public DbSet<Advice> Advices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ShoppingItem> ShoppingItems { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserHomes>()
        .HasKey(uh => new { uh.UserId, uh.HomeId });
        }

    }
}
