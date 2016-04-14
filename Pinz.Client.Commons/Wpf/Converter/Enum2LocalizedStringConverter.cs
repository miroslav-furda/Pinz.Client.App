using System;
using System.Resources;
using System.Windows;
using System.Windows.Data;

namespace Com.Pinz.Client.Commons.Wpf.Converter
{
    public class Enum2LocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && parameter != null && typeof(ResourceManager) == parameter.GetType() )
            {
                ResourceManager resManager = (ResourceManager)parameter;
                // Build a resource key based on the enum type and value
                string Key = value.GetType().Name + "_" + value.ToString();
                string Traduction = resManager.GetString(Key);
                if (Traduction == null)
                {
                    // Traduction could not be found in the current application resources
                    return "Enum2LocalizedStringConverter : " + Key + " could not be found !!";
                }
                return Traduction;
            }
            // return empty string if value is null
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}