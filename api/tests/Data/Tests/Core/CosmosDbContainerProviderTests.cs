using System.Collections.Generic;
using Internal.RaceResults.Data.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Common.Models;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Core
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
