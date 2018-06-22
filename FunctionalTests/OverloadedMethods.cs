﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestFixture]
    public sealed class OverloadedMethods
    {
        private class Class
        {
            public int Method()
            {
                return 0;
            }

            public int Method(int a, int b)
            {
                return 2;
            }

            public int Method(int a)
            {
                return 1;
            }
        }

        [Test]
        public void OverloadedMethods_0()
        {
            var context = new Context();
            var instance = new Class();

            context.DefineVariable($"{nameof(instance)}").Assign(JSValue.Marshal(instance));
            var result = context.Eval($"{nameof(instance)}.{nameof(instance.Method)}()");

            Assert.AreEqual(0, result.Value);
        }

        [Test]
        public void OverloadedMethods_1()
        {
            var context = new Context();
            var instance = new Class();

            context.DefineVariable($"{nameof(instance)}").Assign(JSValue.Marshal(instance));
            var result = context.Eval($"{nameof(instance)}.{nameof(instance.Method)}(1)");

            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void OverloadedMethods_2()
        {
            var context = new Context();
            var instance = new Class();

            context.DefineVariable($"{nameof(instance)}").Assign(JSValue.Marshal(instance));
            var result = context.Eval($"{nameof(instance)}.{nameof(instance.Method)}(1, 2)");

            Assert.AreEqual(2, result.Value);
        }
    }
}
