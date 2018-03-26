using System;
using NiL.JS.Core;
using NiL.JS.Core.Interop;

namespace NiL.JS.BaseLibrary
{
#if !(PORTABLE || NETCORE)
    [Serializable]
#endif

    /// <summary>
    /// The prototype for the Date object.
    /// </summary>
    public sealed class Date
    {
        /// <summary>
        /// The underlying DateTime value.
        /// </summary>
        private DateTime value;

        /// <summary>
        /// A DateTime that represents an invalid date.
        /// </summary>
        private static readonly DateTime InvalidDate = DateTime.MinValue;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Date instance and initializes it to the current time.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public Date()
            : this(getNow())
        {
        }

        /// <summary>
        /// Creates a new Date instance from the given date value.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        public Date(double value)
            : this(toDateTime(value))
        {
        }

        /// <summary>
        /// Creates a new Date instance from the given date string.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="dateStr"> A string representing a date, expressed in RFC 1123 format. </param>
        public Date(string dateStr)
            : this(DateParser.Parse(dateStr))
        {
        }


        [DoNotEnumerate]
        [ArgumentsCount(7)]
        public Date(Arguments args)
        {
            bool _error = false;

            if (args.length == 1)
            {
                var arg = args[0];
                if (arg._valueType >= JSValueType.Object)
                    arg = arg.ToPrimitiveValue_Value_String();

                switch (arg._valueType)
                {
                    case JSValueType.Integer:
                    case JSValueType.Boolean:
                    case JSValueType.Double:
                        {
                            var timeValue = Tools.JSObjectToDouble(arg);
                            if (double.IsNaN(timeValue) || double.IsInfinity(timeValue))
                            {
                                _error = true;
                                break;
                            }
                            value = new Date(timeValue).Value;
                            break;
                        }
                    case JSValueType.String:
                        {
                            value = new Date(arg.ToString()).Value;
                            //_error = !tryParse(arg.ToString(), out _time, out _timeZoneOffset);
                            break;
                        }
                }
            }
            else
            {
                for (var i = 0; i < 9 && !_error; i++)
                {
                    if (args[i].Exists && !args[i].Defined)
                    {
                        _error = true;
                        return;
                    }
                }
                int y = Tools.JSObjectToInt32(args[0], 1, true);
                int m = Tools.JSObjectToInt32(args[1], 0, true);
                int d = Tools.JSObjectToInt32(args[2], 1, true);
                int h = Tools.JSObjectToInt32(args[3], 0, true);
                int n = Tools.JSObjectToInt32(args[4], 0, true);
                int s = Tools.JSObjectToInt32(args[5], 0, true);
                int ms = Tools.JSObjectToInt32(args[6], 0, true);
                if (y == int.MaxValue
                    || y == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (y > 9999999
                    || y < -9999999)
                {
                    _error = true;
                    return;
                }
                if (m == int.MaxValue
                    || m == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (d == int.MaxValue
                    || d == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (h == int.MaxValue
                    || h == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (n == int.MaxValue
                    || n == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (s == int.MaxValue
                    || s == int.MinValue)
                {
                    _error = true;
                    return;
                }
                if (ms == int.MaxValue
                    || ms == int.MinValue)
                {
                    _error = true;
                    return;
                }
                for (var i = 7; i < System.Math.Min(8, args.length); i++)
                {
                    var t = Tools.JSObjectToInt64(args[i], 0, true);
                    if (t == long.MaxValue
                    || t == long.MinValue)
                    {
                        _error = true;
                        return;
                    }
                }
                if (y < 100)
                    y += 1900;
                value = new Date(y, m, d, h, n, s, ms).Value;
            }
        }



        /// <summary>
        /// Creates a new Date instance from various date components, expressed in local time.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <remarks>
        /// If any of the parameters are out of range, then the other values are modified accordingly.
        /// </remarks>
        public Date(int year, int month, int day = 1, int hour = 0,
            int minute = 0, int second = 0, int millisecond = 0)
            : this(toDateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local))
        {
        }


        [Hidden]
        public Date(long ticks, long timeZoneOffset) : this(ticks)
        {
            // ?
        }

        [Hidden]
        public DateTime ToDateTime()
        {
            return value;
        }


        /// <summary>
        /// Creates a new Date instance from the given date.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="dateTime"> The date to set the instance value to. </param>
        public Date(DateTime dateTime)
        {
            this.value = dateTime;
        }

 /*       /// <summary>
        /// Creates the Date prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, DateConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = getDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }
*/


        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// gets the date represented by this object in standard .NET DateTime format.
        /// </summary>
        public DateTime Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// gets the date represented by this object as the number of milliseconds elapsed since
        /// January 1, 1970, 00:00:00 UTC.
        /// </summary>
        public double ValueInMilliseconds
        {
            get { return toJSDate(this.value); }
        }

        /// <summary>
        /// gets a value indicating whether the date instance is valid.  A date can be invalid if
        /// NaN is passed to any of the constructor parameters.
        /// </summary>
        public bool isValid
        {
            get { return this.value != InvalidDate; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Returns the year component of this date, according to local time.
        /// </summary>
        /// <returns> The year component of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getFullYear()
        {
            return getDateComponent(DateComponent.Year, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the year component of this date as an offset from 1900, according to local time.
        /// </summary>
        /// <returns> The year component of this date as an offset from 1900, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getYear()
        {
            return getDateComponent(DateComponent.Year, DateTimeKind.Local) - 1900;
        }

        /// <summary>
        /// Returns the month component of this date, according to local time.
        /// </summary>
        /// <returns> The month component (0-11) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getMonth()
        {
            return getDateComponent(DateComponent.Month, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the day of the month component of this date, according to local time.
        /// </summary>
        /// <returns> The day of the month component (1-31) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getDate()
        {
            return getDateComponent(DateComponent.Day, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the day of the week component of this date, according to local time.
        /// </summary>
        /// <returns> The day of the week component (0-6) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getDay()
        {
            return getDateComponent(DateComponent.DayOfWeek, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the hour component of this date, according to local time.
        /// </summary>
        /// <returns> The hour component (0-23) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getHours()
        {
            return getDateComponent(DateComponent.Hour, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the minute component of this date, according to local time.
        /// </summary>
        /// <returns> The minute component (0-59) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getMinutes()
        {
            return getDateComponent(DateComponent.Minute, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the seconds component of this date, according to local time.
        /// </summary>
        /// <returns> The seconds component (0-59) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getSeconds()
        {
            return getDateComponent(DateComponent.Second, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the millisecond component of this date, according to local time.
        /// </summary>
        /// <returns> The millisecond component (0-999) of this date, according to local time. </returns>
        [DoNotEnumerate]
        public JSValue getMilliseconds()
        {
            return getDateComponent(DateComponent.Millisecond, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the number of milliseconds since January 1, 1970, 00:00:00 UTC.
        /// </summary>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </returns>
        [DoNotEnumerate]
        public JSValue getTime()
        {
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Returns the time-zone offset in minutes for the current locale.
        /// </summary>
        /// <returns> The time-zone offset in minutes for the current locale. </returns>
        [DoNotEnumerate]
        public JSValue getTimezoneOffset()
        {
            if (this.value == InvalidDate)
                return double.NaN;
            return -(int)TimeZoneInfo.Local.GetUtcOffset(this.Value).TotalMinutes;
        }

        /// <summary>
        /// Returns the year component of this date, according to universal time.
        /// </summary>
        /// <returns> The year component of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCFullYear()
        {
            return getDateComponent(DateComponent.Year, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the month component of this date, according to universal time.
        /// </summary>
        /// <returns> The month component (0-11) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCMonth()
        {
            return getDateComponent(DateComponent.Month, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the day of the month component of this date, according to universal time.
        /// </summary>
        /// <returns> The day of the month component (1-31) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCDate()
        {
            return getDateComponent(DateComponent.Day, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the day of the week component of this date, according to universal time.
        /// </summary>
        /// <returns> The day of the week component (0-6) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCDay()
        {
            return getDateComponent(DateComponent.DayOfWeek, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the hour component of this date, according to universal time.
        /// </summary>
        /// <returns> The hour component (0-23) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCHours()
        {
            return getDateComponent(DateComponent.Hour, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the minute component of this date, according to universal time.
        /// </summary>
        /// <returns> The minute component (0-59) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCMinutes()
        {
            return getDateComponent(DateComponent.Minute, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the seconds component of this date, according to universal time.
        /// </summary>
        /// <returns> The seconds component (0-59) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCSeconds()
        {
            return getDateComponent(DateComponent.Second, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the millisecond component of this date, according to universal time.
        /// </summary>
        /// <returns> The millisecond component (0-999) of this date, according to universal time. </returns>
        [DoNotEnumerate]
        public JSValue getUTCMilliseconds()
        {
            return getDateComponent(DateComponent.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setFullYear(double year)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Local, year);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setFullYear(double year, double month)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Local, year, month);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setFullYear(double year, double month, double day)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Local, year, month, day);
        }

        /// <summary>
        /// sets the year of this date, according to local time.
        /// </summary>
        /// <param name="year"> The year.  Numbers less than 100 will be assumed to be  </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setYear(double year)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Local, year >= 0 && year < 100 ? year + 1900 : year);
        }

        /// <summary>
        /// sets the month of this date, according to local time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMonth(double month)
        {
            return setDateComponents(DateComponent.Month, DateTimeKind.Local, month);
        }

        /// <summary>
        /// sets the month of this date, according to local time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMonth(double month, double day)
        {
            return setDateComponents(DateComponent.Month, DateTimeKind.Local, month, day);
        }

        /// <summary>
        /// sets the day of this date, according to local time.
        /// </summary>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setDate(double day)
        {
            return setDateComponents(DateComponent.Day, DateTimeKind.Local, day);
        }

        /// <summary>
        /// sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setHours(double hour)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Local, hour);
        }

        /// <summary>
        /// sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setHours(double hour, double minute)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute);
        }

        /// <summary>
        /// sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setHours(double hour, double minute, double second)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute, second);
        }

        /// <summary>
        /// sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setHours(double hour, double minute, double second, double millisecond)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute, second, millisecond);
        }

        /// <summary>
        /// sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMinutes(double minute)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Local, minute);
        }

        /// <summary>
        /// sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMinutes(double minute, double second)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Local, minute, second);
        }

        /// <summary>
        /// sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMinutes(double minute, double second, double millisecond)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Local, minute, second, millisecond);
        }

        /// <summary>
        /// sets the seconds component of this date, according to local time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setSeconds(double second)
        {
            return setDateComponents(DateComponent.Second, DateTimeKind.Local, second);
        }

        /// <summary>
        /// sets the seconds component of this date, according to local time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setSeconds(double second, double millisecond)
        {
            return setDateComponents(DateComponent.Second, DateTimeKind.Local, second, millisecond);
        }

        /// <summary>
        /// sets the milliseconds component of this date, according to local time.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setMilliseconds(double millisecond)
        {
            return setDateComponents(DateComponent.Millisecond, DateTimeKind.Local, millisecond);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCFullYear(double year)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Utc, year);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCFullYear(double year, double month)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Utc, year, month);
        }

        /// <summary>
        /// sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCFullYear(double year, double month, double day)
        {
            return setDateComponents(DateComponent.Year, DateTimeKind.Utc, year, month, day);
        }

        /// <summary>
        /// sets the month of this date, according to universal time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMonth(double month)
        {
            return setDateComponents(DateComponent.Month, DateTimeKind.Utc, month);
        }

        /// <summary>
        /// sets the month of this date, according to universal time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMonth(double month, double day)
        {
            return setDateComponents(DateComponent.Month, DateTimeKind.Utc, month, day);
        }

        /// <summary>
        /// sets the day of this date, according to universal time.
        /// </summary>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCDate(double day)
        {
            return setDateComponents(DateComponent.Day, DateTimeKind.Utc, day);
        }

        /// <summary>
        /// sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCHours(double hour)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour);
        }

        /// <summary>
        /// sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCHours(double hour, double minute)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute);
        }

        /// <summary>
        /// sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCHours(double hour, double minute, double second)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute, second);
        }

        /// <summary>
        /// sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCHours(double hour, double minute, double second, double millisecond)
        {
            return setDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute, second, millisecond);
        }

        /// <summary>
        /// sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMinutes(double minute)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute);
        }

        /// <summary>
        /// sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMinutes(double minute, double second)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute, second);
        }

        /// <summary>
        /// sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMinutes(double minute, double second, double millisecond)
        {
            return setDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute, second, millisecond);
        }

        /// <summary>
        /// sets the seconds component of this date, according to universal time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCSeconds(double second)
        {
            return setDateComponents(DateComponent.Second, DateTimeKind.Utc, second);
        }

        /// <summary>
        /// sets the seconds component of this date, according to universal time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCSeconds(double second, double millisecond)
        {
            return setDateComponents(DateComponent.Second, DateTimeKind.Utc, second, millisecond);
        }

        /// <summary>
        /// sets the milliseconds component of this date, according to universal time.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setUTCMilliseconds(double millisecond)
        {
            return setDateComponents(DateComponent.Millisecond, DateTimeKind.Utc, millisecond);
        }

        /// <summary>
        /// sets the date and time value of ths date.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [DoNotEnumerate]
        public JSValue setTime(double millisecond)
        {
            this.value = toDateTime(millisecond);
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Returns the date as a string.
        /// </summary>
        /// <returns> The date as a string. </returns>
        [DoNotEnumerate]
        public string toDateString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToLocalTime().ToString("ddd MMM dd yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the date as a string using GMT (Greenwich Mean Time).
        /// </summary>
        /// <returns> The date as a string. </returns>
        [DoNotEnumerate]
        public string toGMTString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the date as a string using GMT (Greenwich Mean Time).
        /// </summary>
        /// <returns> The date as a string. </returns>
        [DoNotEnumerate]
        public string toISOString()
        {
            if (this.value == InvalidDate)
                ExceptionHelper.Throw(new RangeError("The date is invalid"));
            return this.value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Used by the JSON.stringify to transform objects prior to serialization.
        /// </summary>
        /// <param name="thisObject"> The object that is being operated on. </param>
        /// <param name="key"> Unused. </param>
        /// <returns> The date as a serializable string. </returns>
        [DoNotEnumerate]
        public JSValue toJSON(JSValue obj)
        {
            var number = obj.Value;
            if (number is double && (double.IsInfinity((double)number) || double.IsNaN((double)number)))
                return null;
            return toISOString();
        }
/*
        public static object toJSON(ObjectInstance thisObject, string key)
        {
            return thisObject.CallMemberFunction("toISOString");
        }
*/

        /// <summary>
        /// Returns the date as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [DoNotEnumerate]
        public string toLocaleDateString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToLocalTime().ToString("D", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns the date and time as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [DoNotEnumerate]
        public string toLocaleString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToLocalTime().ToString("F", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns the time as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [DoNotEnumerate]
        public string toLocaleTimeString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToLocalTime().ToString("T", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representing the date and time.
        /// </summary>
        /// <param name="thisRef"> The object that is being operated on. </param>
        /// <returns> A string representing the date and time. </returns>
        [DoNotEnumerate]
        [CLSCompliant(false)]
        public static string toString(object thisRef)
        {
            // As of ES6, this method is generic.
            if ((thisRef is Date) == false)
                return "Invalid Date";

            var instance = (Date)thisRef;
            if (instance.value == InvalidDate)
                return "Invalid Date";

            var dateTime = instance.value.ToLocalTime();
            return dateTime.ToString("ddd MMM dd yyyy HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                toTimeZoneString(dateTime);
        }

        /// <summary>
        /// Returns the time as a string.
        /// </summary>
        /// <returns></returns>
        [DoNotEnumerate]
        public string toTimeString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";

            var dateTime = this.value.ToLocalTime();
            return dateTime.ToString("HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                toTimeZoneString(dateTime);
        }

        /// <summary>
        /// Returns the date as a string using UTC (universal time).
        /// </summary>
        /// <returns></returns>
        [DoNotEnumerate]
        public string toUTCString()
        {
            if (this.value == InvalidDate)
                return "Invalid Date";
            return this.value.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /*
                /// <summary>
                /// Returns the primitive value of this object.
                /// </summary>
                /// <returns> The primitive value of this object. </returns>
                [DoNotEnumerate]
                public double valueOf()
                {
                    return this.ValueInMilliseconds;
                }
        */
        [DoNotEnumerate]
        public JSValue valueOf()
        {
            return getTime();
        }

        /*

                /// <summary>
                /// Returns a primitive value that represents the current object.  Used by the addition and
                /// equality operators.
                /// </summary>
                /// <param name="engine"> The current script environment. </param>
                /// <param name="thisObj"> The object to operate on. </param>
                /// <param name="hint"> Specifies the conversion behaviour.  Must be "default", "string" or "number". </param>
                /// <returns></returns>
                [DoNotEnumerate]
                private static object toPrimitive(ScriptEngine engine, ObjectInstance thisObj, string hint)
                {
                    // This behaviour differs from the standard behaviour only in that the "default" hint
                    // results in a conversion to a string, not a number.
                    if (hint == "default" || hint == "string")
                        return thisObj.GetPrimitiveValuePreES6(PrimitiveTypeHint.String);
                    if (hint == "number")
                        return thisObj.GetPrimitiveValuePreES6(PrimitiveTypeHint.Number);
                    throw new JavaScriptException(engine, ErrorType.TypeError, "Invalid type hint.");
                }
        */



        //     STATIC JAVASCRIPT METHODS (FROM DATECONSTRUCTOR)
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the current date and time as the number of milliseconds elapsed since January 1,
        /// 1970, 00:00:00 UTC.
        /// </summary>
        /// <returns> The current date and time as the number of milliseconds elapsed since January 1,
        /// 1970, 00:00:00 UTC. </returns>
        public static JSValue now()
        {
            return toJSDate(getNow());
        }

        /// <summary>
        /// Given the components of a UTC date, returns the number of milliseconds since January 1,
        /// 1970, 00:00:00 UTC to that date.
        /// </summary>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC to the given
        /// date. </returns>
        /// <remarks>
        /// This method differs from the Date constructor in two ways:
        /// 1. The date components are specified in UTC time rather than local time.
        /// 2. A number is returned instead of a Date instance.
        /// 
        /// If any of the parameters are out of range, then the other values are modified accordingly.
        /// </remarks>
        public static double UTC(int year, int month, int day = 1, int hour = 0,
            int minute = 0, int second = 0, int millisecond = 0)
        {
            return toJSDate(toDateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));
        }

        /// <summary>
        /// Parses a string representation of a date, and returns the number of milliseconds since
        /// January 1, 1970, 00:00:00 UTC.
        /// </summary>
        /// <param name="dateStr"> A string representing a date, expressed in RFC 1123 format. </param>
        public static double parse(string dateStr)
        {
            return toJSDate(DateParser.Parse(dateStr));
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________


        private enum DateComponent
        {
            Year = 0,
            Month = 1,
            Day = 2,
            Hour = 3,
            Minute = 4,
            Second = 5,
            Millisecond = 6,
            DayOfWeek,
        }

        /// <summary>
        /// gets a single component of this date.
        /// </summary>
        /// <param name="component"> The date component to extract. </param>
        /// <param name="localOrUniversal"> Indicates whether to retrieve the component in local
        /// or universal time. </param>
        /// <returns> The date component value, or <c>NaN</c> if the date is invalid. </returns>
        private double getDateComponent(DateComponent component, DateTimeKind localOrUniversal)
        {
            if (this.value == InvalidDate)
                return double.NaN;

            // Convert the date to local or universal time.
            switch (localOrUniversal)
            {
                case DateTimeKind.Local:
                    this.value = this.Value.ToLocalTime();
                    break;
                case DateTimeKind.Utc:
                    this.value = this.Value.ToUniversalTime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("localOrUniversal");
            }

            // Extract the requested component.
            switch (component)
            {
                case DateComponent.Year:
                    return this.value.Year;
                case DateComponent.Month:
                    return this.value.Month - 1;    // Javascript month is 0-11.
                case DateComponent.Day:
                    return this.value.Day;
                case DateComponent.DayOfWeek:
                    return (double)this.value.DayOfWeek;
                case DateComponent.Hour:
                    return this.value.Hour;
                case DateComponent.Minute:
                    return this.value.Minute;
                case DateComponent.Second:
                    return this.value.Second;
                case DateComponent.Millisecond:
                    return this.value.Millisecond;
                default:
                    throw new ArgumentOutOfRangeException("component");
            }
        }

        /// <summary>
        /// sets one or more components of this date.
        /// </summary>
        /// <param name="firstComponent"> The first date component to set. </param>
        /// <param name="localOrUniversal"> Indicates whether to set the component(s) in local
        /// or universal time. </param>
        /// <param name="componentValues"> One or more date component values. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        private double setDateComponents(DateComponent firstComponent, DateTimeKind localOrUniversal, params double[] componentValues)
        {
            // Convert the date to local or universal time.
            switch (localOrUniversal)
            {
                case DateTimeKind.Local:
                    this.value = this.Value.ToLocalTime();
                    break;
                case DateTimeKind.Utc:
                    this.value = this.Value.ToUniversalTime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("localOrUniversal");
            }

            // get the current component values of the date.
            int[] allComponentValues = new int[7];
            allComponentValues[0] = this.value.Year;
            allComponentValues[1] = this.value.Month - 1;   // Javascript month is 0-11.
            allComponentValues[2] = this.value.Day;
            allComponentValues[3] = this.value.Hour;
            allComponentValues[4] = this.value.Minute;
            allComponentValues[5] = this.value.Second;
            allComponentValues[6] = this.value.Millisecond;

            // Overwrite the component values with the new ones that were passed in.
            for (int i = 0; i < componentValues.Length; i++)
            {
                double componentValue = componentValues[i];
                if (double.IsNaN(componentValue) || double.IsInfinity(componentValue))
                {
                    this.value = InvalidDate;
                    return this.ValueInMilliseconds;
                }
                allComponentValues[(int)firstComponent + i] = (int)componentValue;
            }

            // Construct a new date.
            this.value = toDateTime(allComponentValues[0], allComponentValues[1], allComponentValues[2],
                allComponentValues[3], allComponentValues[4], allComponentValues[5], allComponentValues[6],
                localOrUniversal);

            // Return the date value.
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Converts a .NET date into a javascript date.
        /// </summary>
        /// <param name="dateTime"> The .NET date. </param>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC </returns>
        private static double toJSDate(DateTime dateTime)
        {
            if (dateTime == InvalidDate)
                return double.NaN;
            // The spec requires that the time value is an integer.
            // We could round to nearest, but then date.toUTCString() would be different from Date(date.getTime()).toUTCString().
            return System.Math.Floor(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        /// <summary>
        /// Converts a javascript date into a .NET date.
        /// </summary>
        /// <param name="milliseconds"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        /// <returns> The equivalent .NET date. </returns>
        private static DateTime toDateTime(double milliseconds)
        {
            // Check if the milliseconds value is out of range.
            if (double.IsNaN(milliseconds))
                return InvalidDate;

            try
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(System.Math.Truncate(milliseconds));
            }
            catch (ArgumentOutOfRangeException)
            {
                return InvalidDate;
            }
        }

        /// <summary>
        /// Given the components of a date, returns the equivalent .NET date.
        /// </summary>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <param name="kind"> Indicates whether the components are in UTC or local time. </param>
        /// <returns> The equivalent .NET date. </returns>
        private static DateTime toDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            if (month >= 0 && month < 12 &&
                day >= 1 && day <= DateTime.DaysInMonth(year, month + 1) &&
                hour >= 0 && hour < 24 &&
                minute >= 0 && minute < 60 &&
                second >= 0 && second < 60 &&
                millisecond >= 0 && millisecond < 1000)
            {
                // All parameters are in range.
                return new DateTime(year, month + 1, day, hour, minute, second, millisecond, kind);
            }
            else
            {
                // One or more parameters are out of range.
                try
                {
                    DateTime value = new DateTime(year, 1, 1, 0, 0, 0, kind);
                    value = value.AddMonths(month);
                    if (day != 1)
                        value = value.AddDays(day - 1);
                    if (hour != 0)
                        value = value.AddHours(hour);
                    if (minute != 0)
                        value = value.AddMinutes(minute);
                    if (second != 0)
                        value = value.AddSeconds(second);
                    if (millisecond != 0)
                        value = value.AddMilliseconds(millisecond);
                    return value;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // One or more of the parameters was NaN or way too big or way too small.
                    // Return a sentinel invalid date.
                    return InvalidDate;
                }
            }
        }

        /// <summary>
        /// gets the current time and date.
        /// </summary>
        /// <returns> The current time and date. </returns>
        private static DateTime getNow()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Returns a string of the form "GMT+1200 (New Zealand Standard Time)".
        /// </summary>
        /// <param name="dateTime"> The date to get the time zone information from. </param>
        /// <returns> A string of the form "GMT+1200 (New Zealand Standard Time)". </returns>
        private static string toTimeZoneString(DateTime dateTime)
        {
            var timeZone = TimeZoneInfo.Local;

            // Compute the time zone offset in hours-minutes.
            int offsetInMinutes = (int)timeZone.GetUtcOffset(dateTime).TotalMinutes;
            int hhmm = offsetInMinutes / 60 * 100 + offsetInMinutes % 60;

            // get the time zone name.
            string zoneName;
            if (timeZone.IsDaylightSavingTime(dateTime))
                zoneName = timeZone.DaylightName;
            else
                zoneName = timeZone.StandardName;

            if (hhmm < 0)
                return string.Format("GMT{0:d4} ({1})", hhmm, zoneName);
            else
                return string.Format("GMT+{0:d4} ({1})", hhmm, zoneName);
        }

    }
}
 