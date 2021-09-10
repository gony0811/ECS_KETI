using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;


namespace DEV.PowerMeter.Library.ViewModels
{
    [DataContract]
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void _OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(name));
        }

        public void OnPropertyChanged(string name) => this._OnPropertyChanged(name);

        protected void OnPropertyChanged(string name, string value) => this._OnPropertyChanged(name);

        public void OnPropertyChanged(string name, object value) => this.OnPropertyChanged(name, this.ValueToString(value));

        protected bool OnPropertyChanged<T>(ref T value, T newValue, [CallerMemberName] string name = null)
        {
            if (object.Equals((object)value, (object)newValue))
                return false;
            value = newValue;
            this._OnPropertyChanged(name);
            return true;
        }

        public void AddHandler(string name, Action handler) => PropertyChangedEventManager.AddHandler((INotifyPropertyChanged)this, (EventHandler<PropertyChangedEventArgs>)((s, e) => handler()), name);

        public void AddHandler(string name, EventHandler<PropertyChangedEventArgs> handler) => PropertyChangedEventManager.AddHandler((INotifyPropertyChanged)this, handler, name);

        public void RemoveHandler(string name, EventHandler<PropertyChangedEventArgs> handler) => PropertyChangedEventManager.RemoveHandler((INotifyPropertyChanged)this, handler, name);

        public string ValueToString(object value) => value != null ? value.ToString() : "[null]";

        [Conditional("TRACE")]
        public static void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PROPERTY_CHANGE")]
        public virtual void TracePropertyChange(string format, params object[] args)
        {
        }

        [Conditional("TRACE_PROPERTY_CHANGING")]
        public virtual void TracePropertyChanging(string name, object value, object newValue) => ViewModelBase.Trace("PropertyChanging {0}: {1} -> {2}", (object)name, (object)this.ValueToString(value), (object)this.ValueToString(newValue));
    }
}
