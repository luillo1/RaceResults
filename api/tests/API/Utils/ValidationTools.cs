using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Internal.Api.Utils
{
    public static class ValidationTools
    {
        public static void AssertFoundItem<T>(T expected, IActionResult result)
        {
            AssertFoundItems(new HashSet<T>(new T[] { expected }), result);
        }

        public static void AssertFoundItems<T>(HashSet<T> expected, IActionResult result)
        {
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            OkObjectResult actual = result as OkObjectResult;

            HashSet<T> actualItemsFound = null;
            switch (actual.Value)
            {
                case T foundItem:
                    actualItemsFound = new HashSet<T>(new T[] { foundItem });
                    break;
                case IEnumerable<T> foundItems:
                    actualItemsFound = new HashSet<T>(foundItems);
                    break;
                default:
                    Assert.Fail();
                    break;
            }

            Assert.AreEqual(expected.Count, actualItemsFound.Count);
            foreach (T expectedItem in expected)
            {
                Assert.IsTrue(actualItemsFound.Contains(expectedItem), $"Failed to find {expectedItem.ToString()}");
            }
        }
    }
}
