using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestClass]
    public sealed class MathTests
    {
        [TestMethod]
        public void RoundTest()
        {
            var context = new Context();
            var code = $"Math.round(-1672567140000)";
            var val = context.Eval(code);
            Assert.AreEqual(-1672567140000, val);
        }

        [TestMethod]
        public void RoundTest1()
        {
            var context = new Context();
            var code = $"Math.round(-0.2)";
            var val = context.Eval(code);
            Assert.AreEqual(0, val);
        }

        [TestMethod]
        public void RoundTest2()
        {
            var context = new Context();
            var code = $"Math.round(-0.8)";
            var val = context.Eval(code);
            Assert.AreEqual(-1, val);
        }


        [TestMethod]
        public void RoundTest2b()
        {
            var context = new Context();
            var code = $"Math.round(-0.5)";
            var val = context.Eval(code);
            Assert.AreEqual(0, val);
        }


        [TestMethod]
        public void RoundTest3()
        {
            var context = new Context();
            var code = $"Math.round(0.8)";
            var val = context.Eval(code);
            Assert.AreEqual(1, val);
        }


        [TestMethod]
        public void RoundTest3a()
        {
            var context = new Context();
            var code = $"Math.round(0.5)";
            var val = context.Eval(code);
            Assert.AreEqual(1, val);
        }


        [TestMethod]
        public void RoundTest4()
        {
            var context = new Context();
            var code = $"Math.round(0.2)";
            var val = context.Eval(code);
            Assert.AreEqual(0, val);
        }


    }
}
