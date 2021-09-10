
using System.Windows;
using System.Windows.Controls;

namespace DEV.PowerMeter.Library
{   public class SenseSwitch
    {
        public CheckBox Checkbox { get; protected set; }

        public bool? IsChecked => this.Checkbox.IsChecked;

        public bool IsThreeState => this.Checkbox.IsThreeState;

        public event RoutedEventHandler Checked
        {
            add => this.Checkbox.Checked += value;
            remove => this.Checkbox.Checked -= value;
        }

        public event RoutedEventHandler Unchecked
        {
            add => this.Checkbox.Unchecked += value;
            remove => this.Checkbox.Unchecked -= value;
        }

        public event RoutedEventHandler Click
        {
            add => this.Checkbox.Click += value;
            remove => this.Checkbox.Click -= value;
        }

        public SenseSwitch(CheckBox checkbox) => this.Checkbox = checkbox;
    }
}
