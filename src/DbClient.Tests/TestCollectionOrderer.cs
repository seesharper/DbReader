using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
[assembly: TestCollectionOrderer("DbClient.Tests.TestCollectionOrderer", "DbClient.Tests")]
namespace DbClient.Tests
{
    public class TestCollectionOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            var instanceReaderTestsCollections = testCollections.Where(tc => tc.DisplayName.Contains("InstanceReaderTests"));

            if (instanceReaderTestsCollections.Count() > 0)
            {
                return instanceReaderTestsCollections.Concat(testCollections.Except(instanceReaderTestsCollections));
            }

            return testCollections;
        }
    }
}