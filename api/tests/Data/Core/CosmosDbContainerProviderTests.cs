using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace RaceResultsTests.Data.Core
{
    [TestClass]
    public class CosmosDbContainerProviderTests
    {
        [TestMethod]
        public void ContructorTest()
        {
            List<IModel> includedData = new List<IModel>();
            ICosmosDbClient cosmosDbClient = Utils<IModel>.GetMockCosmosClient(includedData);
            RaceResultContainerClient containerClient = new RaceResultContainerClient(cosmosDbClient);
        }
    }
}
