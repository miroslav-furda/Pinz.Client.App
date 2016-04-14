using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Com.Pinz.Client.Commons.Wpf.Converter
{
    [TestClass]
    public class Enum2LocalizedStringConverterFixture
    {
        private Enum2LocalizedStringConverter converter;

        [TestInitialize]
        public void setUpFixture()
        {
            converter = new Enum2LocalizedStringConverter();
        }

        [TestMethod]
        public void ConvertCompleted()
        {
            string convertedValue = converter.Convert(TestEnumeration.Inverted, typeof(TestEnumeration), Properties.Resources.ResourceManager, null).ToString();
            Assert.AreSame(Properties.Resources.TestEnumeration_Inverted, convertedValue);
        }

        [TestMethod]
        public void ConvertFailed()
        {
            Assert.AreEqual(converter.Convert(TestEnumeration.Normal, typeof(TestEnumeration), Properties.Resources.ResourceManager, null),
                "Enum2LocalizedStringConverter : TestEnumeration_Normal could not be found !!");
        }

        enum TestEnumeration
        {
            Normal, Inverted
        }
    }

}
