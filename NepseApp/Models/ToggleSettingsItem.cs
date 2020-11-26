using System;

namespace NepseApp.Models
{
    public class ToggleSettingsItem : SettingsItem
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        public Action<bool> OnSave { get; set; }
        public Func<bool> OnReset { get; set; }

        public override void Save()
        {
            OnSave?.Invoke(IsChecked);
            base.Save();
        }

        public override void Reset()
        {
            IsChecked = OnReset?.Invoke() ?? false;
            base.Reset();
        }
    }
}