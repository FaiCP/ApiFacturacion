using GestorAdmi.Tests.Integration.Infrastructure;

namespace GestorAdmi.Tests.Integration.Tests;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}