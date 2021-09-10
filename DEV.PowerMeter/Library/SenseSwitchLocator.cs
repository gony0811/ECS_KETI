using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace DEV.PowerMeter.Library
{
    public class SenseSwitchLocator : GenericLocator<SenseSwitch>
    {
        private static SenseSwitchLocator current;

        public static SenseSwitchLocator Current => SenseSwitchLocator.current ?? (SenseSwitchLocator.current = new SenseSwitchLocator());

        public void Load(Panel panel)
        {
            foreach (object child in panel.Children)
            {
                if (child is RibbonCheckBox ribbonCheckBox1)
                    this.Add(ribbonCheckBox1.Label, (CheckBox)ribbonCheckBox1);
            }
        }

        public void Add(string key, CheckBox item) => this.Add(key, new SenseSwitch(item));
    }
}
