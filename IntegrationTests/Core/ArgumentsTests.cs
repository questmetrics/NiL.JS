using System.Linq;
using NUnit.Framework;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;

namespace IntegrationTests.Core
{
    [TestFixture]
    public class ArgumentsTests
    {
        [Test]
        public void ValuesEnumeration()
        {
            Assert.AreEqual(1, new Arguments { Number.POSITIVE_INFINITY }.Count());
            Assert.AreEqual(0, new Arguments { }.Count());
        }
    }
}
