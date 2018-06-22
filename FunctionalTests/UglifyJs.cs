using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NiL.JS;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestFixture]
    public class UglifyJs
    {
        private static readonly string UglifyJsScriptPath = Environment.CurrentDirectory + "../../../../Tests/uglifyjs.js";

        private Module _module;
        private GlobalContext _context;

        [SetUp]
        public void Initialize()
        {
            using (var file = new FileStream(UglifyJsScriptPath, FileMode.Open))
            using (var fileReader = new StreamReader(file))
            {
                _context = new GlobalContext();
                _context.ActivateInCurrentThread();
                _module = new Module(fileReader.ReadToEnd());
            }

            _module.Run();
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Deactivate();
        }

        [Test]
        public void UglifyJsShouldWorkCorrectly()
        {
            var myString =
@"(function (fallback) {
    fallback = fallback || function () { };
})(null);";
            _module.Context.DefineVariable("code").Assign(myString);

            var result = _module.Context.Eval(
@"var ast = UglifyJS.parse(code);
ast.figure_out_scope();
compressor = UglifyJS.Compressor();
ast = ast.transform(compressor);
ast.print_to_string();");

            Assert.AreEqual("!function(fallback){fallback=fallback||function(){}}(null);", result.ToString());
        }
    }
}
