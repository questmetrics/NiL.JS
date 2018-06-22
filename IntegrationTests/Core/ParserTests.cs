using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS.Core;

namespace IntegrationTests.Core
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ValidateShouldNotUpdateIndexIfReturnedFalse()
        {
            var index = 0;
            var text = "1234";

            var result = Parser.Validate(text, "12", ref index);

            Assert.IsFalse(result);
            Assert.AreEqual(0, index);
        }
    }
}
