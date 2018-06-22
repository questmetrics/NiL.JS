using System.Linq;
using NUnit.Framework;
using NiL.JS.Core;

namespace IntegrationTests.Core
{
    [TestFixture]
    public sealed class JSObjectTests
    {
        [Test]
        public void EnumerationShouldNotIncludePropertiesFromPrototype()
        {
            var proto = JSObject.CreateObject();
            proto["B"] = 1;
            var obj = JSObject.CreateObject();
            obj["A"] = 2;
            obj.__proto__ = proto;

            var L = obj.ToArray();

            Assert.AreEqual(1, L.Length);
            Assert.AreEqual(L[0].Key, "A");
            Assert.AreEqual(L[0].Value.Value, 2);
        }
    }
}
