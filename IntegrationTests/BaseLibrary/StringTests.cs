using NiL.JS.Core;
using NUnit.Framework;

namespace IntegrationTests.BaseLibrary
{
    [TestFixture]
    public class StringTests
    {
        [SetUp]
        public void TestInitialize()
        {
            new GlobalContext().ActivateInCurrentThread();
        }

        [TearDown]
        public void MyTestMethod()
        {
            Context.CurrentContext.GlobalContext.Deactivate();
        }

        [Test]
        public void ReplaceWithRegexpAndReplacer()
        {
            var script = @"
function foo() { 
    var e = 'ab'; 
    var i = e.replace(/[a-z]/g, f => String.fromCharCode((f <= 'Z' ? 90 : 122) >= (f = (f.charCodeAt(0)) % 26 + (f <= 'Z' ? 90 : 122) - 26) ? f : f - 26)); 
    return i;
}

foo();";
            var context = new Context();
            var result = context.Eval(script);

            Assert.AreEqual(JSValueType.String, result.ValueType);
            Assert.AreEqual("st", result.Value);
        }
    }
}
