using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceResults.Data.Core;

namespace Internal.RaceResults.Data.Core
{
    [TestClass]
    public class ContainerConstantsTests
    {
        [TestMethod]
        public void ContainerNameTest()
        {
            Assert.AreEqual("MemberContainer", ContainerConstants.MemberContainerName);
            Assert.AreEqual("OrganizationContainer", ContainerConstants.OrganizationContainerName);
            Assert.AreEqual("RaceContainer", ContainerConstants.RaceContainerName);
            Assert.AreEqual("RaceResultContainer", ContainerConstants.RaceResultContainerName);
            Assert.AreEqual("MemberMatchRecordContainer", ContainerConstants.MemberMatchRecordContainerName);
        }
    }
}
