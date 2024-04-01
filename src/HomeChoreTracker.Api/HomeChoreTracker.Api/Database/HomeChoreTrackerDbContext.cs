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
		public DbSet<FinancialRecord> FinancialRecords { get; set; }
		public DbSet<FinancialCategory> FinancialCategories { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ShoppingItem> ShoppingItems { get; set; }
        public DbSet<HomeChoreTask> HomeChoreTasks { get; set; }
        public DbSet<PointsHistory> PointsHistory { get; set; }
        public DbSet<TaskApproval> TaskApprovals { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskSchedule> TaskSchedules { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TaskVote> TaskVotes { get; set; }
        public DbSet<Advice> Advices { get; set; }
        public DbSet<BusyInterval> BusyIntervals { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserHomes>()
                .HasKey(uh => new { uh.UserId, uh.HomeId });

            modelBuilder.Entity<Home>()
                .HasMany(h => h.Purchases)
                .WithOne(p => p.Home)
                .HasForeignKey(p => p.HomeId);

            modelBuilder.Entity<Home>()
                .HasMany(h => h.Tasks)
                .WithOne(t => t.Home)
                .HasForeignKey(t => t.HomeId);

            modelBuilder.Entity<HomeChoreTask>()
                .HasMany(t => t.TaskAssignments)
                .WithOne(ta => ta.Task)
                .HasForeignKey(ta => ta.TaskId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CalendarEvents)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FinancialRecords)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
               .HasMany(u => u.BusyIntervals)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserHomes)
                .WithOne(uh => uh.User)
                .HasForeignKey(uh => uh.UserId);

            modelBuilder.Entity<Home>()
                .HasMany(h => h.UserHomes)
                .WithOne(uh => uh.Home)
                .HasForeignKey(uh => uh.HomeId);

            modelBuilder.Entity<Home>()
                .HasMany(h => h.Tasks)
                .WithOne(t => t.Home)
                .HasForeignKey(t => t.HomeId);

            modelBuilder.Entity<TaskAssignment>()
                .HasMany(ta => ta.TaskVotes)
                .WithOne(tv => tv.TaskAssignment)
                .HasForeignKey(tv => tv.TaskAssignmentId);
        }
    }
}
