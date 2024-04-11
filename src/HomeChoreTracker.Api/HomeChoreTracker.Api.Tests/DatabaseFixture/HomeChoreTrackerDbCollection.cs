using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.Tests.DatabaseFixture
{
	[CollectionDefinition("Sequential")]
	public class HomeChoreTrackerDbCollection : ICollectionFixture<HomeChoreTrackerDbFixture>
	{
	}
}
