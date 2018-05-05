using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;

namespace IntegrationTests.BaseLibrary
{
    [TestClass]
    public class DateTests
    {

        [TestMethod]
        public void ShouldAllowNullDateInternallyAsArgument()
        {
            var date = new Date(new Arguments { JSValue.Null });
            Assert.AreEqual("1970-01-01T00:00:00.000Z",date.toISOString());
        }

        [TestMethod]
        public void ShouldInterpretNullDate()
        {
            var ctx = new Context();
            var result = ctx.Eval("new Date(null).toISOString()").Value;
            Assert.AreEqual("1970-01-01T00:00:00.000Z",result);
        }

        [TestMethod]
        public void ShouldAssumeUTC()
        {
            var date = new Date(new Arguments { "1971-04-01T00:00:00Z" });
            Assert.AreEqual("1971-04-01T00:00:00.000Z",date.toISOString());
        }

        [TestMethod]
        public void ShouldAssumeNotUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01T00:00:00" });
            Assert.AreNotEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString());
        }

        [TestMethod]
        public void ShouldAssumeNoTimeIsUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01" });
            Assert.AreEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString());
        }

        [TestMethod]
        public void ShouldAssumeWithTimeIsNotUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01T00:00" });
            Assert.AreNotEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString());
        }

        [TestMethod]
        public void DateAndTimeIsNotUTC()
        {
            var datewithtime = new Date(new Arguments { 1970,2,3,10,0 });
            Assert.AreNotEqual("1971-03-02T10:00:00.000Z",datewithtime.toISOString());
        }

        [TestMethod]
        public void ShouldParseUSformat()
        {
            var date = new Date(new Arguments { "10/31/2010 08:00" });
            Assert.AreEqual(date.ToDateTime(), DateTime.Parse("2010-10-31 08:00"));
        }

        [TestMethod]
        public void ShouldGiveISOString()
        {
            var expected = "1970-01-01T00:00:00.000Z";
            var date = new Date(new Arguments { "1970"});
            Assert.AreEqual(date.toISOString(), expected);
        }

        [TestMethod]
        public void NewDateShouldContainCurrentTime()
        {
            var dateTime = ConvertTimezone(DateTime.Now);
            var date = new Date();

            Assert.AreEqual(date.getDate(), dateTime.Day);
            Assert.AreEqual((int)date.getMonth().Value + 1, dateTime.Month);
            Assert.AreEqual(date.getYear(), dateTime.Year-1900);

            Assert.AreEqual(date.getHours(), dateTime.Hour);
            Assert.AreEqual(date.getMinutes(), dateTime.Minute);
            Assert.AreEqual(date.getSeconds(), dateTime.Second);
        }

        [TestMethod]
        public void ShouldCorrectHandleSwitchFromDstToStandard_SydneyTimeZone()
        {
            var timezone = TimeZoneInfo.GetSystemTimeZones()
                .First(x => x.Id.Contains("AUS Eastern Standard Time"));
            Date.CurrentTimeZone = timezone;

            var d1 = new Date(new Arguments { 953996400000 });
            var d2 = new Date(new Arguments { 954000000000 });
            var d3 = new Date(new Arguments { 954003600000 });

            Assert.IsTrue(d1.ToString().StartsWith("Sun Mar 26 2000 02:00:00 GMT+1100"));
            Assert.IsTrue(d2.ToString().StartsWith("Sun Mar 26 2000 02:00:00 GMT+1000"));
            Assert.IsTrue(d3.ToString().StartsWith("Sun Mar 26 2000 03:00:00 GMT+1000"));
        }

        [TestMethod]
        public void ShouldCorrectHandleSwitchFromDstToStandard_UKTimeZone()
        {
            var timezone = TimeZoneInfo.GetSystemTimeZones()
                .First(x => x.Id.Contains("GMT Standard Time"));
            Date.CurrentTimeZone = timezone;

            var d1 = new Date(new Arguments { 972779400000 });
            var d2 = new Date(new Arguments { 972781200000 });
            var d3 = new Date(new Arguments { 972784800000 });

            Assert.IsTrue(d1.ToString().StartsWith("Sun Oct 29 2000 01:30:00 GMT+0100"));
            Assert.IsTrue(d2.ToString().StartsWith("Sun Oct 29 2000 01:00:00 GMT+0000"));
            Assert.IsTrue(d3.ToString().StartsWith("Sun Oct 29 2000 02:00:00 GMT+0000"));
        }

        
        
        private DateTime ConvertTimezone(DateTime local)
        {
            return TimeZoneInfo.ConvertTime(local, TimeZoneInfo.Local, Date.CurrentTimeZone);
        }
        
    }
}
