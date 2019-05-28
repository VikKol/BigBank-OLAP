using Xunit;

namespace BigBank.IntegrationTests.Core
{
    [CollectionDefinition(nameof(TestCollection), DisableParallelization = true)]
    public class TestCollection : ICollectionFixture<TestFixture>
    {
    }
}