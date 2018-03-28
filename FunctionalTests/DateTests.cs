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
            Assert.AreEqual(snapshot.ToString("d/MM/yyyy, HH:mm:ss"), stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTC()
        {
            var context = new Context();
            var snapshot = DateTime.Now;
            var code = $"(new Date(2018, 1,1,15,30,25,500)).toLocaleString()";
            var stringValue = context.Eval(code);
            Assert.AreEqual("1/02/2018, 15:30:25", stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTCHours() // 11 hours difference
        {
            var context = new Context();
            var code = $"(new Date(2018,1,1,23,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Feb 2018, 11:30:25 PM").ToUniversalTime().Hour, stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTCHours2() // 10 hours difference
        {
            var context = new Context();
            var code = $"(new Date(2018, 7,1,23,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Aug 2018, 11:30:25 PM").ToUniversalTime().Hour, stringValue.Value);

        }

        [TestMethod]
        public void TestParse1() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 2018 3:17:45 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:45 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestParse2() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 15:17:45')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:45 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestParse3() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 15:17')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:00 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestParse4() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 04:17 GMT')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:00 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestParse5() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 12 January 3:17 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 12 2018 15:17:00 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestParse6() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 3:17 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 12 2018 15:17:00 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

        [TestMethod]
        public void TestTimezone1() // 10 hours difference
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 13:00 +0100')).toJSON()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 12 2018 15:17:00 GMT+1100 (AUS Eastern Daylight Time)", val);
        }

    }
}
