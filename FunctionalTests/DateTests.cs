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
        public void DateMatchesLocalUTCHours()
        {
            var context = new Context();
            var code = $"(new Date(2018,1,1,23,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Feb 2018, 11:30:25 PM").ToUniversalTime().Hour, stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalUTCHours2()
        {
            var context = new Context();
            var code = $"(new Date(2018, 7,1,23,30,25,500)).getUTCHours()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(DateTime.Parse("1 Aug 2018, 11:30:25 PM").ToUniversalTime().Hour, stringValue.Value);

        }

        [TestMethod]
        public void TestParse1()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 2018 3:17:45 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:45 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestParse2()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 15:17:45')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:45 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestParse3()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 15:17')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:00 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestParse4()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('1 Jan 18 04:17 GMT')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Mon Jan 01 2018 15:17:00 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestParse5()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 12 January 3:17 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Fri Jan 12 2018 15:17:00 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestParse6()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 3:17 pm')).toString()";
            var val = context.Eval(code);
            Assert.AreEqual("Fri Jan 12 2018 15:17:00 GMT+1100 (AUS Eastern Summer Time)", val);
        }

        [TestMethod]
        public void TestTimezoneUtc()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 13:00 UTC')).toJSON()";
            var val = context.Eval(code);
            Assert.AreEqual("2018-01-12T13:00:00.000Z", val);
        }

        [TestMethod]
        public void TestTimezone11()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 13:00 +1100')).toJSON()";
            var val = context.Eval(code);
            Assert.AreEqual("2018-01-12T02:00:00.000Z", val);
        }

        [TestMethod]
        public void TestTimezoneEDT()
        {
            var context = new Context();
            var code = $"new Date(Date.parse('2018 January 12 13:00 EDT')).toJSON()";
            var val = context.Eval(code);
            Assert.AreEqual("2018-01-12T17:00:00.000Z", val);
        }

        [TestMethod]
        public void DateMatchesSpecifiedParse()
        {
            var context = new Context();
            var snapshot = DateTime.Now;
            var code = $"new Date(Date.parse('{snapshot:yyyy MMMM d HH:mm zz}')).toJSON()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(snapshot.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00.000Z"), stringValue.Value);
        }

        [TestMethod]
        public void DateMatchesLocalParse()
        {
            var context = new Context();
            var snapshot = DateTime.Now;
            var code = $"new Date(Date.parse('{snapshot:yyyy MMMM d HH:mm}')).toJSON()";
            var stringValue = context.Eval(code);
            Assert.AreEqual(snapshot.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:00.000Z"), stringValue.Value);
        }

    }
}
