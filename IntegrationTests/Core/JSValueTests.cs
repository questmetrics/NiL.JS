using System;
using NUnit.Framework;
using NiL.JS.Core;

namespace IntegrationTests.Core
{
    [TestFixture]
    public class JSValueTests
    {
        [SetUp]
        public void TestInitialize()
        {
            new GlobalContext().ActivateInCurrentThread();
        }

        [TearDown]
        public void MyTestMethod()
        {
            Context.CurrentContext.GlobalContext.Deactivate();
        }

        [Test]
        public void PrimitiveTypeShouldBeWrappedAsClass()
        {
            var wrappedObject = JSValue.Wrap(1);

            Assert.AreEqual(JSValueType.Object, wrappedObject.ValueType);
        }

        [Test]
        public void PrimitiveTypeShouldBeMarshaledAsPrimitive()
        {
            var wrappedObject = JSValue.Marshal(1);

            Assert.AreEqual(JSValueType.Integer, wrappedObject.ValueType);
        }

        private enum TestEnum : byte
        {
            First = 0,
            // Missing = 1,
            Second = 2,
            Another = 3,
            YetAnother = 4
        }

        [Test]
        public void ShouldConvertStringToEnumValue()
        {
            var convertableJsString = JSValue.Marshal(nameof(TestEnum.Second)) as IConvertible;

            var enumValue = convertableJsString.ToType(typeof(TestEnum), null);

            Assert.AreEqual(TestEnum.Second, enumValue);
        }

        [Test]
        public void ShouldConvertNumberInStringToEnumValue()
        {
            var convertableJsString = JSValue.Marshal(((int)(TestEnum.Second)).ToString()) as IConvertible;

            var enumValue = convertableJsString.ToType(typeof(TestEnum), null);

            Assert.AreEqual(TestEnum.Second, enumValue);
        }

        [Test]
        public void ShouldConvertIntToEnumValue()
        {
            var convertableJsString = JSValue.Marshal((int)(TestEnum.Second)) as IConvertible;

            var enumValue = convertableJsString.ToType(typeof(TestEnum), null);

            Assert.AreEqual(TestEnum.Second, enumValue);
        }

        [Test]
        public void ShouldReturnNullIfCannotConvert()
        {
            var convertableJsString = JSValue.Marshal("bazinga!") as IConvertible;

            var enumValue = convertableJsString.ToType(typeof(TestEnum), null);

            Assert.AreEqual(null, enumValue);
        }
    }
}
