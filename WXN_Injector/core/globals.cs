using System;

namespace WXN_Injector.core
{
    public static class globals
    {
        public static UpdateVar<string> SelectedDllPath = new UpdateVar<string>();
        public static ProcessInfoTemplate selectedItem = null;
    }
    public class UpdateVar<T>
    {
        private T _value;

        public Action ValueChanged;

        public T Value
        {
            get => _value;

            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        protected virtual void OnValueChanged() => ValueChanged?.Invoke();
    }
}