using System;
using System.Collections.Generic;
using System.Linq;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using NiL.JS.Extensions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace IntegrationTests.BaseLibrary
{
    [TestFixture]
    public class DateTests
    {

        [TestCase]
        public void DateStringify() {
            var dt  = new DateTime(2018, 05, 15, 10, 05, 02);
            var dt_j = JSObject.Marshal(dt);
            var json = JSON.stringify(dt_j);
            var j_s = JSON.parse(json);
            var j_dt = new Date(new Arguments() { j_s });
            var dt_o = j_dt.ToDateTime();
            Assert.AreEqual(dt, dt_o);
        }
        
        [TestCase]
        public void ShouldAllowNullDateInternallyAsArgument()
        {
            var date = new Date(new Arguments { JSValue.Null });
            Assert.That(date.toISOString().As<string>(),Is.EqualTo("1970-01-01T00:00:00.000Z"));
        }

        [TestCase]
        public void ShouldInterpretNullDate()
        {
            var ctx = new Context();
            var result = ctx.Eval("new Date(null).toISOString()").As<string>();
            Assert.AreEqual("1970-01-01T00:00:00.000Z",result);
        }

        [TestCase]
        public void ShouldAssumeUTC()
        {
            var date = new Date(new Arguments { "1971-04-01T00:00:00Z" });
            Assert.AreEqual("1971-04-01T00:00:00.000Z",date.toISOString().As<string>());
        }

        [TestCase]
        public void ShouldAssumeNotUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01T00:00:00" });
            Assert.AreNotEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString().As<string>());
        }

        [TestCase]
        public void ShouldAssumeNoTimeIsUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01" });
            Assert.AreEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString().As<string>());
        }

        [TestCase]
        public void ShouldAssumeWithTimeIsNotUTC()
        {
            var datelocal = new Date(new Arguments { "1971-04-01T00:00" });
            Assert.AreNotEqual("1971-04-01T00:00:00.000Z",datelocal.toISOString().As<string>());
        }

        [TestCase]
        public void DateAndTimeIsNotUTC()
        {
            var datewithtime = new Date(new Arguments { 1970,2,3,10,0 });
            Assert.AreNotEqual("1971-03-02T10:00:00.000Z",datewithtime.toISOString().As<string>());
        }

        [TestCase]
        public void ShouldParseUSformat()
        {
            var date = new Date(new Arguments { "10/31/2010 08:00" });
            Assert.AreEqual(date.ToDateTime(), DateTime.Parse("2010-10-31 08:00"));
        }

        [TestCase]
        public void ShouldGiveISOString()
        {
            var expected = "1970-01-01T00:00:00.000Z";
            var date = new Date(new Arguments { "1970"});
            Assert.AreEqual(date.toISOString().As<string>(), expected);
        }

        [TestCase]
        public void NewDateShouldContainCurrentTime()
        {
            var dateTime = ConvertTimezone(DateTime.Now);
            var date = new Date();

            Assert.AreEqual(date.getDate().As<int>(), dateTime.Day, "Day mismatch");
            Assert.AreEqual((int)date.getMonth().Value + 1, dateTime.Month, "Month mismatch");
            Assert.AreEqual(date.getYear().As<int>(), dateTime.Year-1900, "Year mismatch");

            Assert.AreEqual(date.getHours().As<int>(), dateTime.Hour, "Hours mismatch");
            Assert.AreEqual(date.getMinutes().As<int>(), dateTime.Minute, "Minutes mismatch");
            Assert.AreEqual(date.getSeconds().As<int>(), dateTime.Second, "Seconds mismatch");
        }

        [TestCase]
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

            Date.CurrentTimeZone = TimeZoneInfo.Local;
        }

        [TestCase]
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
            
            Date.CurrentTimeZone = TimeZoneInfo.Local;
        }

        
        
        private DateTime ConvertTimezone(DateTime local)
        {
            return TimeZoneInfo.ConvertTime(local, TimeZoneInfo.Local, Date.CurrentTimeZone);
        }
        
    }
}
