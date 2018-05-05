using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.ClearScript.V8;
using NiL.JS.Core;
using NiL.JS.Extensions;

namespace Comparisons
{
    internal class Program
    {
        private const char TICK = '✓';
        private const char CROSS = '×';
        static Context nil = new Context();
        static V8ScriptEngine v8 = new V8ScriptEngine();
        
        public static void Main(string[] args)
        {

            // Warm up engines so times are synced, and create a single sample UTC date for base tests
            nil.Eval("var d = new Date(2000,1,1)");
            v8.Evaluate("var d = new Date(2000,1,1)");
            
            var dates = File.ReadAllLines("dates.js");
            var tests = File.ReadAllLines("scripts.js");

            foreach (var date in dates.Where(d => !string.IsNullOrWhiteSpace(d) && !d.StartsWith("//")))
            {
               
                foreach (var line in tests)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                    {
                        string r1, r2;
                        var datepart = $"({date})";
                        var withdate = line.Replace("[DATE]", datepart);
                        var cmd = $"({withdate}).toString()";
                        try
                        {
                            r1 = nil.Eval(cmd).As<string>();
                        }
                        catch (Exception e)
                        {
                            r1 = $"Exception: {e.Message}";
                        }

                        try
                        {
                            r2 = v8.Evaluate(cmd).ToString();
                        }
                        catch (Exception e)
                        {
                            r2 = $"Exception: {e.Message}";
                        }

                        if (String.Compare(r1, r2) != 0)
                        {
                            BAD($"Difference: {withdate}, \n  NiL={r1} {ifdate(r1)}\n   v8={r2} {ifdate(r2)}");
                        }
                        else
                        {
                            GOOD($"{withdate}");
                        }
                    }
                }
            }

            Console.WriteLine("Finished");
        }
        
        private static string ifdate(string di)
        {
            if (long.TryParse(di, out long val))
            {
                return nil.Eval($"new Date({val})").As<string>();
            }
            return "";
        }

        private static void GOOD(string s)
        {
            var orig = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //Console.WriteLine(TICK + "   " + s);
            Console.ForegroundColor = orig;
        }

        private static void BAD(string s)
        {
            var orig = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(CROSS + "   " + s);
            Console.ForegroundColor = orig;
        }
    }
}