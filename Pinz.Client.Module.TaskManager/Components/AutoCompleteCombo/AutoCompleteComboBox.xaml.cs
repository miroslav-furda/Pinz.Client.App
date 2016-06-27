using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Com.Pinz.Client.Module.TaskManager.Components.AutoCompleteCombo
{
    public partial class AutoCompleteComboBox
    {
        public delegate void CbSelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

        public static readonly DependencyProperty ItemsSourceProperty;

        public static readonly DependencyProperty SelectedItemProperty;

        public static readonly DependencyProperty SelectedIndexProperty;

        public static readonly DependencyProperty SelectedValueProperty;

        public static readonly DependencyProperty SelectedTextProperty;

        public static readonly DependencyProperty SelectionStartProperty;

        public static readonly DependencyProperty CaseSensitiveProperty;

        public static readonly DependencyProperty MaxDropDownHeightProperty;

        private bool updating;

        static AutoCompleteComboBox()
        {
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AutoCompleteComboBox));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoCompleteComboBox));
            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AutoCompleteComboBox));
            SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(AutoCompleteComboBox));
            SelectedTextProperty = DependencyProperty.Register("SelectedText", typeof(string), typeof(AutoCompleteComboBox));
            SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(AutoCompleteComboBox));
            CaseSensitiveProperty = DependencyProperty.Register("CaseSensitive", typeof(bool), typeof(AutoCompleteComboBox), new PropertyMetadata(false));
            MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(int), typeof(AutoCompleteComboBox), new PropertyMetadata(150));
        }

        public AutoCompleteComboBox()
        {
            updating = false;
            InitializeComponent();
            var autoCompleteComboBox = this;
            Cbx.IsDropDownOpenChanged += autoCompleteComboBox.cbx_IsDropDownOpenChanged;
        }

        public bool CaseSensitive
        {
            get { return (bool) GetValue(CaseSensitiveProperty); }
            set { SetValue(CaseSensitiveProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public int MaxDropDownHeight
        {
            get { return (int) GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int) GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public string SelectedText
        {
            get { return (string) GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        public int SelectionStart
        {
            get { return (int) GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        private void cbx_IsDropDownOpenChanged(object sender, EventArgs e)
        {
            if (Cbx.IsDropDownOpen)
            {
                return;
            }
            updating = true;
            try
            {
                Cbx.SelectedItem = null;
                Tbx.SelectionStart = Tbx.Text.Length;
                Tbx.Focus();
            }
            finally
            {
                updating = false;
            }
        }

        private void cbx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && (e.Key == Key.Return || e.Key == Key.Return) && Cbx.LivePreviewItem != null)
            {
                Cbx.SelectedItem = RuntimeHelpers.GetObjectValue(Cbx.LivePreviewItem);
                Tbx.SelectionStart = Tbx.Text.Length;
                Cbx.IsDropDownOpen = false;
                Tbx.Focus();
                e.Handled = true;
            }
        }

        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            try
            {
                SelectedText = (Cbx.SelectedItem as IComboBoxProperties)?.Name;
                SelectedItem = Cbx.SelectedItem;
                SelectedValue = Cbx.SelectedValue;
                var cbSelectionChangedEventHandler = CbSelectionChanged;
                cbSelectionChangedEventHandler?.Invoke(RuntimeHelpers.GetObjectValue(sender), e);
            }
            finally
            {
                updating = false;
            }
        }

        private void tbx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                updating = true;
                try
                {
                    if (Cbx.Items.Count > 0)
                    {
                        Cbx.IsDropDownOpen = true;
                        var objectValue = RuntimeHelpers.GetObjectValue(Cbx.Items[0]);
                        Cbx.LivePreviewItem = RuntimeHelpers.GetObjectValue(objectValue);
                        Cbx.Focus();
                    }
                }
                finally
                {
                    updating = false;
                }
            }
        }

        private void tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (updating)
            {
                return;
            }
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            if (Tbx.Text == string.Empty)
            {
                Cbx.IsDropDownOpen = false;
                Cbx.Items.Filter = null;
            }
            else
            {
                updating = true;
                try
                {
                    Cbx.IsDropDownOpen = true;
                    if (Cbx.SelectedIndex != 0)
                    {
                        Cbx.Items.Filter = item =>
                        {
                            if (Tbx.Text.Length <= 0) return true;
                            var val = item as IComboBoxProperties;

                            if (val != null)
                            {
                                return val.Name != null && val.Name.ToLower().Contains(Tbx.Text.ToLower());
                            }
                            return true;
                        };
                    }
                    Tbx.Focus();
                }
                finally
                {
                    updating = false;
                }
            }
        }

        public event CbSelectionChangedEventHandler CbSelectionChanged;
    }
}