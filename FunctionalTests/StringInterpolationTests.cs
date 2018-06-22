using NUnit.Framework;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestFixture]
    public sealed class StringInterpolationTests
    {
        [Test]
        public void StringInterpolationAllowsStrings()
        {
            var context = new Context();
            var code = @"`This is a string`";
            var stringValue = context.Eval(code);

            Assert.AreEqual("This is a string", stringValue.Value);
        }

        [Test]
        public void StringInterpolationAllowsFunctions()
        {
            var context = new Context();
            var code = @"
var funcs = (x) => `This returns ${x}`;
funcs('Fred');
";
            var stringValue = context.Eval(code);
            Assert.AreEqual("This returns Fred", stringValue.Value);
        }

        [Test]
        public void StringInterpolationAllowsSlashes()
        {
            var context = new Context();
            var code = @"`This is a string such as http://www.google.com`";
            var stringValue = context.Eval(code);
            Assert.AreEqual("This is a string such as http://www.google.com", stringValue.Value);
        }

        [Test]
        public void StringAllowsSlashes()
        {
            var context = new Context();
            var code = @"'This is a string such as http://www.google.com'";
            var stringValue = context.Eval(code);
            Assert.AreEqual("This is a string such as http://www.google.com", stringValue.Value);
        }

        [Test]
        public void StringAllowsSlashesDoubleQuoted()
        {
            var context = new Context();
            var code = @"""This is a string such as http://www.google.com""";
            var stringValue = context.Eval(code);
            Assert.AreEqual("This is a string such as http://www.google.com", stringValue.Value);
        }

        [Test]
        public void StringInterpolationAllowsSubstititions()
        {
            var context = new Context();
            var code = @"var a=1234; `This is a string such as ${a}`";
            var stringValue = context.Eval(code);

            Assert.AreEqual("This is a string such as 1234", stringValue.Value);
        }
    }
}
