using DocumentFormat.OpenXml.Drawing.Diagrams;
using HomeChoreTracker.Api.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.DatabaseFixture
{
	public class HomeChoreTrackerDbFixture
	{
		private readonly DbContextOptionsBuilder<HomeChoreTrackerDbContext> contextOptionsBuilder = new DbContextOptionsBuilder<HomeChoreTrackerDbContext>();

		public DbContextOptions<HomeChoreTrackerDbContext> Options { get; private set; }
		public HomeChoreTrackerDbFixture()
		{
			contextOptionsBuilder.UseInMemoryDatabase(databaseName: "HomeChoreTrackerDb");
			Options = contextOptionsBuilder.Options;

			using var dbContext = new HomeChoreTrackerDbContext(Options);
			dbContext.Users.Add(new Models.User
			{
				Id = 1,
				StartDayTime = default,
				EndDayTime = default,
			});

			dbContext.SaveChanges();
		}
	}
}
