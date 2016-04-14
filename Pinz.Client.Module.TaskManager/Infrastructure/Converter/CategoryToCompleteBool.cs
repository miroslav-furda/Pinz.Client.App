using Com.Pinz.DomainModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Pinz.Client.Module.TaskManager.Infrastructure.Converter
{
    public class CategoryToCompleteBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TaskStatus.TaskComplete.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
