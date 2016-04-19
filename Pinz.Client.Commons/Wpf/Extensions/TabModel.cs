using System.Windows;

namespace Com.Pinz.Client.Commons.Wpf.Extensions
{
    public sealed class TabModel : DependencyObject
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TabModel));

        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanClose.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(TabModel));

        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsModified.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register("IsModified", typeof(bool), typeof(TabModel));

    }
}