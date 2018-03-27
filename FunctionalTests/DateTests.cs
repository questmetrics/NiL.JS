using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestClass]
    public sealed class DateTimeUTCTests
    {
        [TestMethod]
        public void DateMatchesLocal()
        {
            var context = new Context();
            var snapshot = DateTime.Now;
            var code = $"(new Date({snapshot.Year}, {snapshot.Month-1},{snapshot.Day},{snapshot.Hour},{snapshot.Minute},{snapshot.Second},{snapshot.Millisecond})).toLocaleString()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(snapshot.ToString("d/MM/yyyy, h:mm:ss tt"), stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTC()
        {
            var context = new Context();
            var snapshot = DateTime.Now;
            var code = $"(new Date(2018, 1,1,15,30,25,500)).toLocaleString()";
            var stringValue = context.Eval(code);
            Assert.AreEqual("1/02/2018, 3:30:25 PM", stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTCHours() // 11 hours difference
        {
            var context = new Context();
            var code = $"(new Date(2018, 1,1,11,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Feb 2018, 0:30:25 PM").ToUniversalTime().Hour.ToString(), stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTCHours2() // 10 hours difference
        {
            var context = new Context();
            var code = $"(new Date(2018, 7,1,10,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Aug 2018, 0:30:25 PM").ToUniversalTime().Hour.ToString(), stringValue.Value);

        }

        [TestMethod]
        public void TestUTCOffset() // 10 hours difference
        {
            var context = new Context();
            var code = $"(new Date(2018, 7,1,10,30,25,500)).getUTCOffset()";
            var val1 = context.Eval(code);
            var code2 = $"(new Date(2018, 1,1,10,30,25,500)).getUTCOffset()";
            var val2 = context.Eval(code);
            Assert.AreNotEqual(val1, val2);
        }

    }
}
