using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NiL.JS;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestClass]
    public class MomentTests
    {
        private static readonly string MomentPath = "moment.js";

        private Module _module;
        private GlobalContext _context;

        [TestInitialize]
        public void Initialize()
        {
            using (var file = new FileStream(MomentPath, FileMode.Open))
            using (var fileReader = new StreamReader(file))
            {
                _context = new GlobalContext();
                _context.ActivateInCurrentThread();
                _module = new Module(fileReader.ReadToEnd());
            }

            _module.Run();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Deactivate();
        }

        [TestMethod]
        public void MomentToLocaleStringShouldWork()
        {
            var myString = @"moment(1000000000).toLocaleString()";
            var result = _module.Context.Eval(myString);
            Assert.AreEqual("Tue Jan 13 1970 00:46:40 GMT+1100", result.ToString());
        }
    }
}
