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
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ShoppingItem> ShoppingItems { get; set; }
        public DbSet<HomeChoreTask> HomeChoreTasks { get; set; }
        public DbSet<PointsHistory> PointsHistory { get; set; }
        public DbSet<TaskApproval> TaskApprovals { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskSchedule> taskSchedules { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserHomes>()
        .HasKey(uh => new { uh.UserId, uh.HomeId });
        }

    }
}
