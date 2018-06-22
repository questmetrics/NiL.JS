using System;
using System.Threading;
using NiL.JS.Core;
using NUnit.Framework;

namespace IntegrationTests.BaseLibrary
{
    [TestFixture]
    public sealed class JsonTests
    {
        [TestCase]
        [Timeout(1000)]
        public void IncorrectJsonShouldBringToError()
        {
            string json = "\"a\":0";
            Assert.Throws<JSException>(() => NiL.JS.BaseLibrary.JSON.parse(json));
        }
    }
}
