﻿using PlayGen.SUGAR.Server.EntityFramework.Tests;
using Xunit;

namespace PlayGen.SUGAR.Client.Tests
{
	[CollectionDefinition(nameof(ClearDatabaseFixture))]
	// Note: This class must be in the same assemblt as the tests in order for xUnit to detect it
	public class ClearDatabaseFixtureCollection : ICollectionFixture<ClearDatabaseFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
