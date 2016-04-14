
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Com.Pinz.Client.Commons.Wpf.Converter
{
    [TestClass]
    public class InverseBooleanConverterFixture
    {
        private InverseBooleanConverter converter;

        [TestInitialize]
        public void setUpFixture()
        {
            converter = new InverseBooleanConverter();
        }

        [TestMethod]
        public void ConvertTrue()
        {
            Assert.AreEqual(converter.Convert(true, typeof(bool), null, null), false);
        }

        [TestMethod]
        public void ConvertFalse()
        {
            Assert.AreEqual(converter.Convert(false, typeof(bool), null, null), true);
        }
    }
}
