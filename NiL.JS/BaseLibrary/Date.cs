using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using NiL.JS.Core;
using NiL.JS.Core.Interop;
using NiL.JS.Expressions;
using NiL.JS.Statements;

namespace NiL.JS.BaseLibrary
{
#if !(PORTABLE || NETCORE)
    [Serializable]
#endif
    public sealed class Date
    {
        /// <summary>
        /// Allows for the fact javascript allows a weird number for any parameter.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month">The month number, zero indexed!</param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="millisecond"></param>
        /// <param name="isUTC"></param>
        /// <returns></returns>
        private static DateTime createDate(int year, int month, int day, int hour, int minute, int second, int millisecond, bool isUTC = false)
        {
            var dt = new DateTime(1, 1, 1, 0,0,0,0, isUTC ? DateTimeKind.Utc : DateTimeKind.Local);
            return dt.AddYears(year-1).AddMonths(month).AddDays(day-1).AddHours(hour).AddMinutes(minute).AddSeconds(second).AddMilliseconds(millisecond);
            //return new DateTime(year,month,day, hour,minute,second, millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local);
        }

/*        private static IEnumerable<string> tokensOf(string source)
        {
            int position = 0;
            int prevPos = 0;
            while (position < source.Length)
            {
                if (source[position] == '(' && (prevPos == position || source.IndexOf(':', prevPos, position - prevPos) == -1))
                {
                    if (prevPos != position)
                    {
                        yield return source.Substring(prevPos, position - prevPos);
                        prevPos = position;
                    }
                    int depth = 1;
                    position++;
                    while (depth > 0 && position < source.Length)
                    {
                        switch (source[position++])
                        {
                            case '(':
                                depth++;
                                break;
                            case ')':
                                depth--;
                                break;
                        }
                    }
                    prevPos = position;
                    continue;
                }
                if (!Tools.IsWhiteSpace(source[position]))
                {
                    position++;
                    continue;
                }
                if (prevPos != position)
                {
                    yield return source.Substring(prevPos, position - prevPos);
                    prevPos = position;
                }
                else
                    prevPos = ++position;
            }
            if (prevPos != position)
                yield return source.Substring(prevPos);
        }

        private static int indexOf(IList<string> list, string value, bool ignoreCase)
        {
            for (var i = 0; i < list.Count; i++)
                if (string.Compare(list[i], value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                    return i;
            return -1;
        }*/

        
        /*
        private static bool parseSelf(string timeStr, out long time, out long timeZoneOffset)
        {
            timeStr = timeStr.Trim(Tools.TrimChars);
            if (string.IsNullOrEmpty(timeStr))
            {
                time = 0;
                timeZoneOffset = 0;
                return false;
            }
            time = 0;
            timeZoneOffset = 0;
            bool wasForceMonth = false;
            bool wasMonth = false;
            bool wasDay = false;
            bool wasYear = false;
            bool wasTZ = false;
            bool wasTzo = false;
            int month = 0;
            int year = 0;
            int day = 1;
            string[] timeTokens = null;
            int tzoH = 0;
            int tzoM = 0;
            int temp = 0;
            bool pm = false;
            foreach (var token in tokensOf(timeStr))
            {
                if (indexOf(daysOfWeek, token, true) != -1)
                    continue;
                var index = indexOf(months, token, true);
                if (index != -1)
                {
                    if (wasMonth)
                    {
                        if (wasForceMonth
                            || (wasDay && wasYear))
                            return false;
                        if (!wasDay)
                        {
                            day = month;
                            wasDay = true;
                        }
                        else if (!wasYear)
                        {
                            year = month;
                            wasYear = true;
                        }
                        else
                            return false;
                    }
                    wasForceMonth = true;
                    wasMonth = true;
                    month = index + 1;
                    continue;
                }
                if (int.TryParse(token, out index))
                {
                    if (!wasMonth && index <= 12 && index > 0)
                    {
                        month = index;
                        wasMonth = true;
                        continue;
                    }
                    if (!wasDay && index > 0 && index <= 31)
                    {
                        day = index;
                        wasDay = true;
                        continue;
                    }
                    if (!wasYear)
                    {
                        if ((wasDay || wasMonth)
                            && (!wasDay || !wasMonth))
                            return false;
                        year = index;
                        wasYear = true;
                        continue;
                    }
                    return false;
                }
                if (token.IndexOf(':') != -1)
                {
                    if (timeTokens != null)
                        return false;
                    timeTokens = token.Split(':');
                    continue;
                }
                if (token.StartsWith("gmt", StringComparison.OrdinalIgnoreCase)
                    || (token.StartsWith("ut", StringComparison.OrdinalIgnoreCase) && (token.Length == 2 || token[2] == 'c' || token[2] == 'C'))
                    || token.StartsWith("pst", StringComparison.OrdinalIgnoreCase)
                    || token.StartsWith("pdt", StringComparison.OrdinalIgnoreCase))
                {
                    if (wasTZ)
                        return false;
                    if (token.Length <= 3)
                    {
                        //var tzo = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                        //tzoH += tzo.Hours;
                        //tzoM += tzo.Minutes;
                    }
                    else
                    {
                        if (wasTzo)
                            return false;
                        if (!int.TryParse(token.Substring(3), out temp))
                            return false;
                        tzoM += temp % 100;
                        tzoH += temp / 100;
                    }
                    if (token.StartsWith("pst", StringComparison.OrdinalIgnoreCase))
                        tzoH -= 8;
                    if (token.StartsWith("pdt", StringComparison.OrdinalIgnoreCase))
                        tzoH -= 7;
                    wasTZ = true;
                    continue;
                }
                if (!wasTzo && (token[0] == '+' || token[0] == '-') && int.TryParse(token.Substring(3), out temp))
                {
                    tzoM += temp % 100;
                    tzoH += temp / 100;
                    wasTzo = true;
                    wasTZ = true;
                    continue;
                }
                if (string.Compare("am", token, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }
                if (string.Compare("pm", token, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pm = true;
                    continue;
                }
                if (wasDay)
                    return false;
            }
            try
            {
                if ((wasDay || wasMonth || wasYear)
                    && (!wasDay || !wasMonth || !wasYear))
                    return false;
                if (year < 100)
                    year += (DateTime.Now.Year / 100) * 100;
                time = dateToMilliseconds(year, month - 1, day,
                    timeTokens != null && timeTokens.Length > 0 ? (long)double.Parse(timeTokens[0]) - tzoH : -tzoH,
                    timeTokens != null && timeTokens.Length > 1 ? (long)double.Parse(timeTokens[1]) - tzoM : -tzoM,
                    timeTokens != null && timeTokens.Length > 2 ? (long)double.Parse(timeTokens[2]) : 0,
                    timeTokens != null && timeTokens.Length > 3 ? (long)double.Parse(timeTokens[3]) : 0
                    );
                if (pm)
                    time += _hourMilliseconds * 12;
                timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Ticks / 10000;
                if (wasTZ)
                {
                    time += timeZoneOffset;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        */

/*        private static DateTime parseIso8601(string timeStr)
        {
            return DateTime.Parse(timeStr);

            const string format = "YYYY|-MM|-DD|THH|:MM|:SS|.SSS";

            var year = 0;
            var month = int.MinValue;
            var day = int.MinValue;
            var hour = 0;
            var minutes = 0;
            var seconds = 0;
            var milliseconds = 0;
            time = 0;
            timeZoneOffset = 0;
            int part = 0; // 0 - дата, 1 - время, 2 - миллисекунды
            int i = 0, j = 0;
            for (; i < format.Length; i++, j++)
            {
                if (timeStr.Length <= j)
                {
                    if (format[i] == '|')
                        break;
                    else
                        return false;
                }

                switch (char.ToLowerInvariant(format[i]))
                {
                    case 'y':
                        {
                            if (part != 0)
                                return false;
                            if (!Tools.IsDigit(timeStr[j]))
                                return false;
                            year = year * 10 + timeStr[j] - '0';
                            break;
                        }
                    case 'm':
                        {
                            if (!Tools.IsDigit(timeStr[j]))
                                return false;
                            switch (part)
                            {
                                case 0:
                                    {
                                        if (month == int.MinValue)
                                            month = 0;
                                        month = month * 10 + timeStr[j] - '0';
                                        break;
                                    }
                                case 1:
                                    {
                                        minutes = minutes * 10 + timeStr[j] - '0';
                                        break;
                                    }
                                default:
                                    return false;
                            }
                            break;
                        }
                    case 'd':
                        {
                            if (part != 0)
                                return false;
                            if (!Tools.IsDigit(timeStr[j]))
                                return false;
                            if (day == int.MinValue)
                                day = 0;
                            day = day * 10 + timeStr[j] - '0';
                            break;
                        }
                    case 'h':
                        {
                            if (part != 1)
                                return false;
                            if (!Tools.IsDigit(timeStr[j]))
                                return false;
                            hour = hour * 10 + timeStr[j] - '0';
                            break;
                        }
                    case 's':
                        {
                            if (part < 1)
                                return false;
                            if (!Tools.IsDigit(timeStr[j]))
                                return false;
                            if (part == 1)
                                seconds = seconds * 10 + timeStr[j] - '0';
                            else
                                milliseconds = milliseconds * 10 + timeStr[j] - '0';
                            break;
                        }
                    case ':':
                        {
                            if (part != 1)
                                return false;
                            if (format[i] != timeStr[j])
                                return false;
                            break;
                        }
                    case '/':
                        {
                            if (part != 0)
                                return false;
                            if (format[i] != timeStr[j])
                                return false;
                            break;
                        }
                    case ' ':
                        {
                            if (format[i] != timeStr[j])
                                return false;
                            while (j < timeStr.Length && Tools.IsWhiteSpace(timeStr[j]))
                                j++;
                            j--;
                            break;
                        }
                    case '-':
                        {
                            if (format[i] != timeStr[j])
                                return false;
                            break;
                        }
                    case 't':
                        {
                            if ('t' != char.ToLowerInvariant(timeStr[j]))
                                return false;
                            if (part == 0)
                                part++;
                            else
                                return false;
                            break;
                        }
                    case '.':
                        {
                            if ('.' != timeStr[j])
                            {
                                if (char.ToLowerInvariant(timeStr[j]) == 'z')
                                {
                                    j = timeStr.Length;
                                    i = format.Length;
                                    break;
                                }
                                return false;
                            }
                            if (part != 1)
                                return false;
                            else
                            {
                                part++;
                                break;
                            }
                        }
                    case '|':
                        {
                            j--;
                            break;
                        }
                    default:
                        return false;
                }
            }
            if (j < timeStr.Length && char.ToLowerInvariant(timeStr[j]) != 'z')
                return false;
            if (month == int.MinValue)
                month = 1;
            if (day == int.MinValue)
                day = 1;
            if (year < 100)
                year += (DateTime.Now.Year / 100) * 100;
            time = dateToMilliseconds(year, month - 1, day, hour, minutes, seconds, milliseconds);
            timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(time * 10000)).Ticks / 10000;
            time += timeZoneOffset;
            return true;
        }*/

        private static DateTime tryParse(string timeString)
        {
            var guess = Parse(timeString);
            if (guess == _error)
                guess = ParseRelaxed(timeString);
            return guess;
        }

        private DateTime value;
        private static readonly DateTime _error = DateTime.MinValue;
        private static readonly DateTime _mindate = new DateTime(1970,1,1,0,0,0,0);
        private bool isError => (value == DateTime.MinValue);

        [DoNotEnumerate]
        public Date()
        {
            value = DateTime.Now;
        }

        [Hidden]
        public Date(long ticks)
        {
            value = msToDateTime(ticks);
        }

        [Hidden]
        public Date(DateTime dt)
        {
            value = dt;
        }

        [Hidden] public DateTime Value => value;

        private static DateTime msToDateTime(double milliseconds)
        {
            if (double.IsNaN(milliseconds) || double.IsInfinity(milliseconds))
                return _error;
            try
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(System.Math.Truncate(milliseconds));
            }
            catch (ArgumentOutOfRangeException)
            {
                return _error;
            }
        }

        private static double DateTimeToMs(DateTime dt)
        {
            return System.Math.Floor(dt.Subtract(_mindate).TotalMilliseconds);
        }


        [DoNotEnumerate]
        [ArgumentsCount(7)]
        public Date(Arguments args)
        {
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
                                value = _error;
                                break;
                            }
                            value = msToDateTime((long)timeValue);
                            break;
                        }
                    case JSValueType.String:
                    {
                        value = tryParse(arg.ToString());
                        break;
                    }
                }
            }
            else
            {
                bool error = false;
                for (var i = 0; i < 9 && !error; i++)
                {
                    if (args[i].Exists && !args[i].Defined)
                    {
                        error = true;
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
                    value = _error;
                    return;
                }
                if (y > 9999999
                    || y < -9999999)
                {
                    value = _error;
                    return;
                }
                if (m == int.MaxValue
                    || m == int.MinValue)
                {
                    value = _error;
                    return;
                }
                if (d == int.MaxValue
                    || d == int.MinValue)
                {
                    value = _error;
                    return;
                }
                if (h == int.MaxValue
                    || h == int.MinValue)
                {
                    value = _error;
                    return;
                }
                if (n == int.MaxValue
                    || n == int.MinValue)
                {
                    value = _error;
                    return;
                }
                if (s == int.MaxValue
                    || s == int.MinValue)
                {
                    value = _error;
                    return;
                }
                if (ms == int.MaxValue
                    || ms == int.MinValue)
                {
                    value = _error;
                    return;
                }
                for (var i = 7; i < System.Math.Min(8, args.length); i++)
                {
                    var t = Tools.JSObjectToInt64(args[i], 0, true);
                    if (t == int.MaxValue
                    || t == int.MinValue)
                    {
                        value = _error;
                        return;
                    }
                }
                if (y < 100)
                    y += 1900;
                value =  createDate(y, m, d, h, n, s, ms);
            }
        }

        [DoNotEnumerate]
        public JSValue valueOf()
        {
            return getTime();
        }

        [DoNotEnumerate]
        public JSValue getTime()
        {
            if (isError)
                return double.NaN;
            return valueInTicks(value);
        }

        private static double valueInTicks(DateTime t)
        {
            return System.Math.Floor(t.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        [DoNotEnumerate]
        public static JSValue now()
        {
            return valueInTicks(DateTime.Now);
        }

        [DoNotEnumerate]
        public JSValue getTimezoneOffset()
        {
            if (isError)
                return Number.NaN;
            return -(int)TimeZoneInfo.Local.GetUtcOffset(value).TotalMinutes;
        }

        [DoNotEnumerate]
        public JSValue getYear()
        {
            return getPart('Y', false)-1900;
        }

        [DoNotEnumerate]
        public JSValue getFullYear()
        {
            return getPart('Y', false);
        }

        private int getPart(char part, bool isUTC)
        {
            var from = isUTC ? value.ToUniversalTime() : value;
            switch (part)
            {
                case 'Y': return from.Year;
                case 'M': return from.Month-1;
                case 'D': return from.Day;
                case 'h': return from.Hour;
                case 'm': return from.Minute;
                case 's': return from.Second;
                case 'n': return from.Millisecond;
                case 'd': return (int)from.DayOfWeek;
                default: throw new Exception("Invalid date part requested");
            }
        }

        private void setPart(char part, int amount, bool isUTC)
        {
            var from = isUTC ? value.ToUniversalTime() : value;
            switch (part)
            {
                case 'Y': value = new DateTime(amount, from.Month,from.Day,from.Hour,from.Minute,from.Second,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 'M': value = new DateTime(from.Year, amount+1,from.Day,from.Hour,from.Minute,from.Second,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 'D': value = new DateTime(from.Year, from.Month,amount,from.Hour,from.Minute,from.Second,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 'h': value = new DateTime(from.Year, from.Month,from.Day,amount,from.Minute,from.Second,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 'm': value = new DateTime(from.Year, from.Month,from.Day,from.Hour,amount,from.Second,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 's': value = new DateTime(from.Year, from.Month,from.Day,from.Hour,from.Minute,amount,from.Millisecond, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                case 'n': value = new DateTime(from.Year, from.Month,from.Day,from.Hour,from.Minute,from.Second,amount, isUTC ? DateTimeKind.Utc : DateTimeKind.Local); break;
                default: throw new Exception("Invalid date part specified");
            }
        }

        [DoNotEnumerate]
        public JSValue getUTCFullYear()
        {
            return getPart('Y', true);
        }

        [DoNotEnumerate]
        public JSValue getMonth()
        {
            return getPart('M', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCMonth()
        {
            return getPart('M', true)-1;
        }

        [DoNotEnumerate]
        public JSValue getDate()
        {
            return getPart('D', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCDate()
        {
            return getPart('D', true);
        }

        [DoNotEnumerate]
        public JSValue getDay()
        {
            return getPart('d', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCDay()
        {
            return getPart('d',true);
        }

        [DoNotEnumerate]
        public JSValue getHours()
        {
            return getPart('h', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCHours()
        {
            return getPart('h', true);
        }

        [DoNotEnumerate]
        public JSValue getMinutes()
        {
            return getPart('m', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCMinutes()
        {
            return getPart('m', true);
        }

        [DoNotEnumerate]
        public JSValue getSeconds()
        {
            return getPart('s', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCSeconds()
        {
            return getPart('s', true);
        }

        [DoNotEnumerate]
        public JSValue getMilliseconds()
        {
            return getPart('n', false);
        }

        [DoNotEnumerate]
        public JSValue getUTCMilliseconds()
        {
            return getPart('n', true);
        }

        [DoNotEnumerate]
        public JSValue setTime(JSValue time)
        {
            if (time == null
                || !time.Defined
                || (time._valueType == JSValueType.Double && (double.IsNaN(time._dValue) || double.IsInfinity(time._dValue))))
            {
                value = _error;
            }
            else
            {
                this.value = msToDateTime(Tools.JSObjectToInt64(time));
            }
            return getTime();
        }

        [DoNotEnumerate]
        public JSValue setMilliseconds(JSValue milliseconds)
        {
            setPart('n', Tools.JSObjectToInt32(milliseconds), false);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setUTCMilliseconds(JSValue milliseconds)
        {
            setPart('n', Tools.JSObjectToInt32(milliseconds), true);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setSeconds(JSValue seconds, JSValue milliseconds)
        {
            if (seconds != null && seconds.Exists)
                setPart('s', Tools.JSObjectToInt32(seconds), false);
            if (!isError && milliseconds != null && milliseconds.Exists)
                setPart('n', Tools.JSObjectToInt32(milliseconds), false);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setUTCSeconds(JSValue seconds, JSValue milliseconds)
        {
            if (seconds != null && seconds.Exists)
                setPart('s', Tools.JSObjectToInt32(seconds), true);
            if (!isError && milliseconds != null && milliseconds.Exists)
                setPart('n', Tools.JSObjectToInt32(milliseconds), true);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setMinutes(JSValue minutes, JSValue seconds, JSValue milliseconds)
        {
            if (minutes != null && minutes.Exists)
                setPart('m', Tools.JSObjectToInt32(minutes), false);
            return setSeconds(seconds, milliseconds);
        }

        [DoNotEnumerate]
        public JSValue setUTCMinutes(JSValue minutes, JSValue seconds, JSValue milliseconds)
        {
            if (minutes != null && minutes.Exists)
                setPart('m', Tools.JSObjectToInt32(minutes), true);
            return setUTCSeconds(seconds, milliseconds);
        }

        [DoNotEnumerate]
        public JSValue setHours(JSValue hours, JSValue minutes, JSValue seconds, JSValue milliseconds)
        {
            if (hours != null && hours.Exists)
                setPart('h', Tools.JSObjectToInt32(hours), false);
            return setMinutes(minutes, seconds, milliseconds);
        }

        [DoNotEnumerate]
        public JSValue setUTCHours(JSValue hours, JSValue minutes, JSValue seconds, JSValue milliseconds)
        {
            if (hours != null && hours.Exists)
                setPart('h', Tools.JSObjectToInt32(hours), true);
            return setUTCMinutes(minutes, seconds, milliseconds);
        }

        [DoNotEnumerate]
        public JSValue setDate(JSValue days)
        {
            if (days != null && days.Exists)
                setPart('D', Tools.JSObjectToInt32(days), false);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setUTCDate(JSValue days)
        {
            if (days != null && days.Exists)
                setPart('D', Tools.JSObjectToInt32(days), true);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setMonth(JSValue month, JSValue days)
        {
            if (month != null && month.Exists)
                setPart('M', Tools.JSObjectToInt32(month), false);
            return setDate(days);
        }

        [DoNotEnumerate]
        public JSValue setUTCMonth(JSValue month, JSValue days)
        {
            if (month != null && month.Exists)
                setPart('M', Tools.JSObjectToInt32(month), true);
            return setUTCDate(days);
        }

        [DoNotEnumerate]
        public JSValue setYear(JSValue year)
        {
            if (year != null && year.Exists)
                setPart('Y', Tools.JSObjectToInt32(year), false);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setUTCYear(JSValue year)
        {
            if (year != null && year.Exists)
                setPart('Y', Tools.JSObjectToInt32(year), true);
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setFullYear(JSValue year, JSValue month, JSValue day)
        {
            if (year != null && year.Exists)
            {
                setPart('Y', Tools.JSObjectToInt32(year), true);
                return setMonth(month, day);
            }
            value = _error;
            return DateTimeToMs(value);
        }

        [DoNotEnumerate]
        public JSValue setUTCFullYear(JSValue year, JSValue month, JSValue day)
        {
            return setFullYear(year, month, day);
        }

        [DoNotEnumerate]
        [CLSCompliant(false)]
        public JSValue toString()
        {
            return ToString();
        }
/*
        [Hidden]
        public DateTime ToDateTime()
        {
            var y = getYearImpl();
            while (y > 2800)
                y -= 2800;
            while (y < 0)
                y += 2800;
            var dt = new DateTime(0, DateTimeKind.Local);
            dt = dt.AddDays(getDateImpl() - 1);
            dt = dt.AddMonths(getMonthImpl());
            dt = dt.AddYears(y - 1);
            dt = dt.AddHours(getHoursImpl());
            dt = dt.AddMinutes(getMinutesImpl());
            dt = dt.AddSeconds(getSecondsImpl());
            dt = dt.AddMilliseconds(getMillisecondsImpl());
            return dt;
        }*/

        [DoNotEnumerate]
        public JSValue toLocaleString()
        {
            return value.ToShortDateString() + ", " + value.ToString("HH:mm:ss");
        }

        [DoNotEnumerate]
        public JSValue toLocaleTimeString()
        {
            return value.ToString("HH:mm:ss");
        }

        [DoNotEnumerate]
        public JSValue toISOString()
        {
            return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        [DoNotEnumerate]
        public JSValue toJSON(JSValue obj)
        {
            return toISOString();
        }

        [DoNotEnumerate]
        public JSValue toUTCString()
        {
            return value.ToUniversalTime().ToString(@"ddd, d MMM yyyy HH:mm:ss \G\M\T");
        }

        [DoNotEnumerate]
        public JSValue toGMTString()
        {
            return value.ToUniversalTime().ToString(@"ddd, d MMM yyyy HH:mm:ss \G\M\T");
        }

        [DoNotEnumerate]
        public JSValue toTimeString()
        {
            var dateTime = value.ToLocalTime();
            return dateTime.ToString("HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) 
                   + FindTimezone(dateTime);
        }

        [DoNotEnumerate]
        public JSValue toDateString()
        {
            return value.ToString("ddd MMM d yyyy");
        }

        [DoNotEnumerate]
        public JSValue toLocaleDateString()
        {
            return value.ToShortDateString();
        }

        [Hidden]
        public override string ToString()
        {
            if (isError)
                return "Invalid date";

            var dateTime = value.ToLocalTime();
            return dateTime.ToString("ddd MMM dd yyyy HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                   FindTimezone(dateTime);
        }

        [DoNotEnumerate]
        public static JSValue parse(string dateTime)
        {
            var guess = tryParse(dateTime);
            return guess==_error ? Number.NaN : DateTimeToMs(guess);
        }

        [DoNotEnumerate]
        [ArgumentsCount(7)]
        public static JSValue UTC(Arguments dateTime)
        {
            try
            {
                var d = new Date(dateTime);
                return DateTimeToMs(d.value);
            }
            catch
            {
                return double.NaN;
            }
        }


        private static string FindTimezone(DateTime dateTime)
        {
            var timeZone = TimeZoneInfo.Local;
            int offsetInMinutes = (int)timeZone.GetUtcOffset(dateTime).TotalMinutes;
            int hhmm = offsetInMinutes / 60 * 100 + offsetInMinutes % 60;
            var zoneName = timeZone.IsDaylightSavingTime(dateTime) ? timeZone.DaylightName : timeZone.StandardName;
            return string.Format(hhmm < 0 ? "GMT{0:d4} ({1})" : "GMT+{0:d4} ({1})", hhmm, zoneName);
        }


        static Regex regex = new Regex(
            @"^((?<Y>[0-9]{4}) (-(?<M>[0-9]{2}) (-(?<D>[0-9]{2}))?)?)
               (T (?<H>[0-9]{2}) : (?<m>[0-9]{2}) (:(?<s>[0-9]{2}) (\.(?<n>[0-9]{1,3})[0-9]*)?)?
               (?<z>Z|(?<zh>[+-][0-9]{2}) : (?<zm>[0-9]{2}))?)?$",
               RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        private static DateTime Parse(string input)
        {
            var match = regex.Match(input);
            if (match.Success)
            {
                int Y = regValue(match, "Y", 1970);
                int M = regValue(match, "M", 1);
                int D = regValue(match, "D", 1);
                int h = regValue(match, "h", 0);
                int m = regValue(match, "m", 0);
                int s = regValue(match, "s", 0);
                int n = regValue(match, "n", 0);
                int multiplier = match.Groups["n"].Value.Length;
                n = n * (1000 - (int)System.Math.Pow(10,3-multiplier));

                if ((M<1 || M>12) ||
                    (D < 1 || D > 31) ||
                    (h > 23 || h < 0) ||
                    (m > 59 || m < 0) ||
                    (s > 59 || s < 0) ||
                    (n > 999 || s < 0))
                    return _error;

                var zone = match.Groups["z"].Value;
                var zonediff = 0;
                if (!System.String.IsNullOrWhiteSpace(zone) && zone != "Z" )
                {
                    var zm = regValue(match, "zm", 0);
                    var zh = regValue(match, "zh", 0);
                    if (zh > 23)
                        return _error;
                    if (zh > 59)
                        return _error;

                    zonediff = zh*60 + ((zh < 0) ? -zm : zm);
                }
                try
                {
                    return new DateTime(Y, M,D, h,m,s,n, DateTimeKind.Utc).AddMinutes(zonediff);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return _error;
                }

            }
            return _error;
        }

        private static DateTime ParseRelaxed(string input)
        {
            var parts = input.Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new DateTime().ToUniversalTime();
            int offset = 0;
            bool isLocalTz = true;

            bool haveday = false;
            bool havemonth = false;
            bool haveyear = false;
            List<int> freeform = new List<int>();
            foreach (var part in parts)
            {
                if (LocalNames.Timezones.ContainsKey(part))
                {
                    offset = LocalNames.Timezones[part];
                    isLocalTz = false;
                }
                else if (part.StartsWith("GMT", StringComparison.OrdinalIgnoreCase) ||
                    part.StartsWith("UTC", StringComparison.OrdinalIgnoreCase) ||
                    part.StartsWith("+", StringComparison.OrdinalIgnoreCase) ||
                    part.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                {
                    var subpart = part.StartsWith("GMT") || part.StartsWith("UTC") ? part.Substring(3) : part;
                    var tzamount = int.Parse(subpart);
                    if (System.Math.Abs(tzamount) < 100)
                        offset = tzamount * 60;
                    else
                        offset = (tzamount / 100) * 60 + (tzamount % 100);
                    isLocalTz = false;
                }
                else if (part.IndexOfAny(new [] {'/', '-'}) != -1)
                {
                    var datepart = DateTime.Parse(part);
                    result = datepart.Add(result.TimeOfDay);
                    haveday = havemonth = haveyear = true;
                }
                else if (part.Equals("PM", StringComparison.OrdinalIgnoreCase))
                {
                    if (result.Hour < 12)
                        result = result.AddHours(12);
                }
                else if (part.Equals("AM", StringComparison.OrdinalIgnoreCase))
                { } //Ignore
                else if (part.IndexOf(':') >= 0)
                {
                    var timepart = DateTime.Parse(part);
                    result = result.Date.Add(timepart.TimeOfDay);
                }
                else if (LocalNames.Months.ContainsKey(part))
                {
                    var month = LocalNames.Months[part];
                    result = new DateTime(result.Year, month,result.Day,result.Hour,result.Minute,result.Second,result.Millisecond, DateTimeKind.Utc);
                    havemonth = true;
                }
                else if (LocalNames.Weekdays.ContainsKey(part))
                {
                    // Ignore days
                }
                else if (part.All(Char.IsNumber))
                {
                    var val = int.Parse(part);
                    freeform.Add(val);
                }
                else if (part.StartsWith("Z", StringComparison.OrdinalIgnoreCase))
                {
                    //Ignore Z (UTC)
                }
                else if (part.StartsWith("("))
                {
                    break; // Anything in parenthesis is classed as the end of the date part
                }
                else
                {
                    Console.WriteLine($"Not sure how to handle part called '{part}'");
                }
            }

            // Check numbers scattered around to identify possible years/month/day
            foreach (var val in freeform)
            {
                if (val < 13 && !havemonth)
                    result = new DateTime(result.Year, val, result.Day, result.Hour, result.Minute, result.Second,
                        result.Millisecond, DateTimeKind.Utc);
                else if (val < 32 && !haveday)
                {
                    result = new DateTime(result.Year, result.Month, val, result.Hour, result.Minute, result.Second,
                        result.Millisecond, DateTimeKind.Utc);
                    haveday = true;
                }
                else if (val > 1900)
                    result = new DateTime(val, result.Month, result.Day, result.Hour, result.Minute, result.Second,
                        result.Millisecond, DateTimeKind.Utc);
                else if (val >= 50 && !haveyear)
                    result = new DateTime(val + 1900, result.Month, result.Day, result.Hour, result.Minute,
                        result.Second, result.Millisecond, DateTimeKind.Utc);
                else if (val < 50 && !haveyear)
                    result = new DateTime(val + 2000, result.Month, result.Day, result.Hour, result.Minute,
                        result.Second, result.Millisecond, DateTimeKind.Utc);
            }

            return isLocalTz ? result.Add(result.Subtract(result.ToLocalTime())) : result.AddMinutes(-offset);
        }



        private static int regValue(Match match, string field, int def)
        {
            return int.TryParse(match.Groups[field].Value, out int value) ? value : def;
        }

        private class LocalNames
        {
            public static readonly Dictionary<string, int> Weekdays = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public static readonly Dictionary<string, int> Months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            public static readonly Dictionary<string, int> Timezones;

            static LocalNames()
            {
                var sname = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames;
                var lname = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames;
                for (int i = 0; i < 12; i++)
                {
                    Months[sname[i]] = i+1;
                    Months[lname[i]] = i+1;
                }

                var wnames = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedDayNames;
                var wnamel = CultureInfo.InvariantCulture.DateTimeFormat.DayNames;
                for (int i = 0; i < 7; i++)
                {
                    Weekdays[wnames[i]] = i;
                    Weekdays[wnamel[i]] = i;
                }

                Timezones = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    {"M", -12*60},
                    {"PST", -8*60},
                    {"PDT", -7*60},
                    {"MST", -7*60},
                    {"MDT", -6*60},
                    {"CST", -6*60},
                    {"EST", -5*60},
                    {"CDT", -5*60},
                    {"EDT", -4*60},
                    {"A", -1*60},
                    {"UT", 0},
                    {"UTC", 0},
                    {"GMT", 0},
                    {"Z", 0},
                    {"N", 1*60},
                    {"Y", 12*60}
                };


            }
        }


        #region Do not remove

        [Hidden]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [Hidden]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}