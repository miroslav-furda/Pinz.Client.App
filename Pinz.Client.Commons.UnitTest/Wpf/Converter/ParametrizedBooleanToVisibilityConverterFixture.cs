using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace Com.Pinz.Client.Commons.Wpf.Converter
{
    [TestClass]
    public class ParametrizedBooleanToVisibilityConverterFixture
    {
        private ParametrizedBooleanToVisibilityConverter converter;

        [TestInitialize]
        public void setUpFixture()
        {
            converter = new ParametrizedBooleanToVisibilityConverter();
        }

        [TestMethod]
        public void ConvertTrue_To_Visible()
        {
            Assert.AreEqual(converter.Convert(true, typeof(bool), null, null), Visibility.Visible);
        }

        [TestMethod]
        public void ConvertFalse_To_Collapsed()
        {
            Assert.AreEqual(converter.Convert(false, typeof(bool), null, null), Visibility.Collapsed);
        }
    }
}
