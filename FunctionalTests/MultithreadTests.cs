using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS;
using NiL.JS.Core;
using NiL.JS.Extensions;

namespace FunctionalTests
{
    [TestFixture]
    public sealed class MultiThreadTests
    {

        GlobalContext gctx;

        [Test]
        public void TestParallelThreads()
        {
            gctx = new GlobalContext("Shared");
            gctx.DefineVariable("fred").Assign(12345);
            
            List<Task> alltasks = new List<Task>();
            for (int t = 0; t < 1000; t++)
            {
                var newtask = Task.Run(() =>
                {
                    var xx = t;
                    var result = RunAsync(xx);
                    Assert.AreEqual(xx + 1000, result, $"Result should have been {xx + 1000} in thread {xx}, but was {result} ({Thread.CurrentThread.ManagedThreadId})");
                });
                alltasks.Add(newtask);
            }
            Console.WriteLine($"Created all threads...");

            Task.WaitAll(alltasks.ToArray());
            Console.WriteLine($"All completed!");
        }

        private int RunAsync(int t)
        {
            var ctx = new Context(gctx);
            ctx.Eval("var temp = this", JSValue.Marshal(t));
            Thread.Sleep(1);
            ctx.Eval("temp+=500;", JSValue.Marshal(t));
            Thread.Sleep(1);
            var result = ctx.Eval("temp+=500;", JSValue.Marshal(t));
            return result.As<int>();
        }
    }
}
