using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.IntegrationTests.Shared
{
	[CollectionDefinition("Sequential")]
	public class DatabaseCollection : ICollectionFixture<WebApplicationFactory<Program>>
	{
	}
}
