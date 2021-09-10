
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    public class GenericLocator<T>
    {
        protected Dictionary<string, T> Locations;

        public T this[string key]
        {
            get => this.Locations[key];
            set => this.Locations[key] = value;
        }

        public GenericLocator() => this.Locations = new Dictionary<string, T>();

        public void Add(string key, T item) => this[key] = item;
    }
}
