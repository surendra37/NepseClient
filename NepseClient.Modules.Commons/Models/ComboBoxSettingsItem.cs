using System;
using System.Collections.Generic;

namespace NepseClient.Modules.Commons.Models
{
    public class ComboBoxSettingsItem : SettingsItem
    {
        private IEnumerable<object> _items;
        public IEnumerable<object> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public string DisplayMemberPath { get; set; } = "DisplayName";

        public Action<object> OnSave { get; set; }
        public Func<object> OnReset { get; set; }

        public override void Save()
        {
            OnSave(SelectedItem);
        }

        public override void Reset()
        {
            SelectedItem = OnReset();
        }
    }
}