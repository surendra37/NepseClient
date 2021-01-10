using System;

namespace NepseApp.Models
{

    public class TextBoxSettingsItem : SettingsItem
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Action<string> OnSave { get; set; }
        public Func<string> OnReset { get; set; }

        public override void Save()
        {
            OnSave?.Invoke(Value);
            base.Save();
        }

        public override void Reset()
        {
            Value = OnReset?.Invoke() ?? string.Empty;
            base.Reset();
        }
    }
}