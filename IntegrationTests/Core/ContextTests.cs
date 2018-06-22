using System;
using NUnit.Framework;
using NiL.JS.Core;

namespace IntegrationTests.Core
{
    [TestFixture]
    public class ContextTests
    {
        [Test]
        public void Eval_ThisBindShouldBeOverridable()
        {
            var context = new Context();
            var oldThis = context.ThisBind;
            var myThis = JSObject.CreateObject();

            var value0 = context.Eval("this", myThis);
            var value1 = context.Eval("this");

            Assert.AreEqual(myThis, value0);
            Assert.AreNotEqual(null, value1);
            Assert.AreEqual(oldThis, value1);
        }

        [Test]
        public void ContextInNonStrictModeByDefault()
        {
            var context = new Context();

            var isStrict = context.Eval(@"
try { 
    (function(){ return arguments.callee; })();
    true;
}
catch(e) {
    false;
}");

            Assert.AreEqual(true, (bool)isStrict);
        }

        [Test]
        public void ContextCanBeInStrictMode()
        {
            var context = new Context(true);

            var isStrict = context.Eval(@"
try { 
    (function(){ return arguments.callee; })();
    false;
}
catch(e) {
    true;
}");

            Assert.AreEqual(true, (bool)isStrict);
        }
    }
}
