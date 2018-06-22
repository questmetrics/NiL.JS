using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS.Core;

namespace IntegrationTests.Core.Functions
{
    [TestFixture]
    public class MethodProxyTests
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

        public class MyObject
        {
            public string Text { get; set; }
        }

        [Test]
        public void UndefinedToStringPropertyShouldConvertToNull()
        {
            var context = new Context();
            var obj = new MyObject();
            context.DefineVariable("obj").Assign(JSValue.Wrap(obj));

            context.Eval("obj.Text = undefined");

            Assert.IsNull(obj.Text);
        }
    }
}
