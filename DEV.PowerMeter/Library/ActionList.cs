using System;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    public class ActionList
    {
        public List<Action> Actions = new List<Action>();

        public void Clear() => this.Actions.Clear();

        public void Add(Action action) => this.Actions.Add(action);

        public void InvokeAll()
        {
            foreach (Action action in this.Actions)
                action();
        }
    }
}
