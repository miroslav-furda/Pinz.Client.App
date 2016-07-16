using System.Collections.Generic;
using System.ComponentModel;

namespace Com.Pinz.Client.Module.TaskManager.Components.AutoCompleteCombo
{
    public class ViewModelll : INotifyPropertyChanged
    {
        private string selectedItem;

        public ViewModelll()
        {
            Items = new List<string>();
        }

        public List<string> Items { get; }

        public string SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                var propertyChangedEventHandler = PropertyChanged;
                propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}