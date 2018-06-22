﻿using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NiL.JS;
using NiL.JS.Core;

namespace FunctionalTests
{
    [TestFixture]
    public class JsFunfuzz
    {
        private static readonly string JsFunfuzzScriptPath = Environment.CurrentDirectory + "../../../../Tests/jsfunfuzz.js";

        private Module _module;
        private StringBuilder _output;
        private TextWriter _oldOutput;
        private TextWriter _oldErrOutput;
        private GlobalContext _context;

        [SetUp]
        public void Initialize()
        {
            _context = new GlobalContext();
            _context.ActivateInCurrentThread();

            try
            {
                using (var file = new FileStream(JsFunfuzzScriptPath, FileMode.Open))
                using (var fileReader = new StreamReader(file))
                    _module = new Module(fileReader.ReadToEnd());
            }
            catch
            {
                _context.Deactivate();
                throw;
            }

            _output = new StringBuilder();
            _oldErrOutput = Console.Error;
            _oldOutput = Console.Out;
            Console.SetOut(new StringWriter(_output));
            Console.SetError(new StringWriter(_output));
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Deactivate();

            Console.SetOut(_oldOutput);
            Console.SetError(_oldErrOutput);
        }

        [Test]
        public void AsRhino()
        {
            _module.Context.DefineVariable("stderr").Assign(JSValue.Null);
            _module.Context.DefineVariable("print").Assign(new Action<object>(x => _output.AppendLine(x.ToString())));

            _module.Run();

            Assert.AreEqual("", _output.ToString());
        }

        [Test]
        public void AsJavaScriptCore()
        {
            _module.Context.DefineVariable("print").Assign(new Action<object>(x => _output.AppendLine(x.ToString())));
            _module.Context.DefineVariable("debug").Assign(new Action(() => { }));

            _module.Run();

            var expected = @"***
Targeting JavaScriptCore / WebKit.

";
            Assert.AreEqual(expected, _output.ToString());
        }

        [Test]
        public void AsSpiderMonkey()
        {
            _module.Context.DefineVariable("print").Assign(new Action<object>(x => _output.AppendLine(x.ToString())));
            _module.Context.DefineVariable("line2pc").Assign(new Action(() => { }));
            _module.Context.DefineVariable("version").Assign(new Action(() => { }));
            _module.Context.DefineVariable("options").Assign(new Action(() => { }));

            _module.Run();

            var expected = @"***
Targeting SpiderMonkey / Gecko.

";
            Assert.AreEqual(expected, _output.ToString());
        }
    }
}
