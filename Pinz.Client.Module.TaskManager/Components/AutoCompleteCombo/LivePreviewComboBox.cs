using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Com.Pinz.Client.Module.TaskManager.Components.AutoCompleteCombo
{
    public class LivePreviewComboBox : ComboBox
    {
        public static readonly DependencyProperty LivePreviewItemProperty;

        static LivePreviewComboBox()
        {
            LivePreviewItemProperty = DependencyProperty.Register("LivePreviewItem", typeof(object), typeof(LivePreviewComboBox), new FrameworkPropertyMetadata(null));
        }

        public LivePreviewComboBox()
        {
            var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(IsDropDownOpenProperty, typeof(LivePreviewComboBox));
            var livePreviewComboBox = this;
            dependencyPropertyDescriptor.AddValueChanged(this, livePreviewComboBox.OnDropDownOpenChanged);
        }

        public object LivePreviewItem
        {
            get { return GetValue(LivePreviewItemProperty); }
            set { SetValue(LivePreviewItemProperty, RuntimeHelpers.GetObjectValue(value)); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var containerForItemOverride = base.GetContainerForItemOverride();
            var comboBoxItem = containerForItemOverride as ComboBoxItem;
            if (comboBoxItem != null)
            {
                var livePreviewComboBox = this;
                DependencyPropertyDescriptor.FromProperty(ComboBoxItem.IsHighlightedProperty, typeof(ComboBoxItem)).AddValueChanged(comboBoxItem, livePreviewComboBox.OnItemHighlighted);
            }
            return containerForItemOverride;
        }

        private void OnDropDownOpenChanged(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                LivePreviewItem = RuntimeHelpers.GetObjectValue(SelectedItem);
            }
            IsDropDownOpenChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnItemHighlighted(object sender, EventArgs e)
        {
            var comboBoxItem = sender as ComboBoxItem;
            if (comboBoxItem != null && comboBoxItem.IsHighlighted)
            {
                LivePreviewItem = RuntimeHelpers.GetObjectValue(comboBoxItem.DataContext);
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            LivePreviewItem = RuntimeHelpers.GetObjectValue(SelectedItem);
            base.OnSelectionChanged(e);
        }

        public event EventHandler IsDropDownOpenChanged;
    }
}