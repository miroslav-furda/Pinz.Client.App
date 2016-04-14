using System.Windows;
using System.Windows.Input;

namespace Com.Pinz.Client.Commons.Wpf.Extensions
{
    public static class DoubleClickExtension
    {
        public static readonly DependencyProperty DoubleClickCommandProperty;
        public static readonly DependencyProperty DoubleClickCommandParameterProperty;

        static DoubleClickExtension()
        {
            DoubleClickCommandProperty = DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(DoubleClickExtension), new UIPropertyMetadata(null, OnDoubleClickCommandPropertyChanged));
            DoubleClickCommandParameterProperty = DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object), typeof(DoubleClickExtension), new UIPropertyMetadata(null));
        }

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickCommandProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        public static object GetDoubleClickCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(DoubleClickCommandParameterProperty);
        }

        public static void SetDoubleClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(DoubleClickCommandParameterProperty, value);
        }

        private static void OnDoubleClickCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            if (element != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    element.MouseDown += new MouseButtonEventHandler(Control_MouseDown);
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    element.MouseDown -= new MouseButtonEventHandler(Control_MouseDown);
                }
            }
        }

        private static void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var element = sender as UIElement;
                if (element != null)
                {
                    var command = GetDoubleClickCommand(element);
                    var parameter = GetDoubleClickCommandParameter(element);
                    if (command != null && command.CanExecute(parameter))
                    {
                        e.Handled = true;
                        command.Execute(parameter);
                    }
                }
            }
        }
    }
}
