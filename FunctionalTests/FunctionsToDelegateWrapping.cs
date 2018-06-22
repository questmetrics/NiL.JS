using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestFixture]
    public class FunctionsToDelegateWrapping
    {
        [Test]
        public void TryToAddFunctionIntoListOfDelegates_Marshal()
        {
            var context = new Context();
            var list = new List<Func<string, string>>();
            context.DefineVariable("list").Assign(JSValue.Marshal(list));

            context.Eval("list.push(x => 'hi ' + x)"); // IList marshaled as NativeList with array-like interface

            Assert.AreEqual("hi Test", list[0]("Test"));
        }

        [Test]
        public void TryToAddFunctionIntoListOfDelegates_Wrap()
        {
            var context = new Context();
            var list = new List<Func<string, string>>();
            context.DefineVariable("list").Assign(JSValue.Wrap(list));

            context.Eval("list.Add(x => 'hi ' + x)");

            Assert.AreEqual("hi Test", list[0]("Test"));
        }
    }
}
