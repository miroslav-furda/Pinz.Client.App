using System.Collections.Generic;
using System.ComponentModel;

namespace Com.Pinz.Client.Module.TaskManager.Components.AutoCompleteCombo
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string selectedItem;

        public ViewModel()
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