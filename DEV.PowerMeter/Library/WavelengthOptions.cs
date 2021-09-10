using SharedLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DEV.PowerMeter.Library
{
    public class WavelengthOptions : IValueOptions, IEnumerable
    {
        public readonly WavelengthOption Disabled = new WavelengthOption(0U);
        public List<WavelengthOption> Options = new List<WavelengthOption>();
        public WavelengthOption SelectedWavelength;

        public IEnumerator<WavelengthOption> GetEnumerator() => (IEnumerator<WavelengthOption>)this.Options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        public object Selected => (object)this.SelectedWavelength;

        public WavelengthOptions(IEnumerable<uint> wavelengths, uint selected)
        {
            if (selected == 0U)
                this.Options.Add(this.Disabled);
            foreach (int wavelength in wavelengths)
            {
                WavelengthOption wavelengthOption = new WavelengthOption((uint)wavelength);
                if (wavelength == (int)selected)
                    this.SelectedWavelength = wavelengthOption;
                this.Options.Add(wavelengthOption);
            }
            this.SelectedWavelength = this.GetOption(selected);
            if (this.SelectedWavelength != null)
                return;
            this.SelectedWavelength = new WavelengthOption(selected);
            this.Options.Add(this.SelectedWavelength);
        }

        public bool ContainsValue(uint value) => this.Options.Any<WavelengthOption>((Func<WavelengthOption, bool>)(o => (int)o.Value == (int)value));

        public WavelengthOption GetOption(uint value) => this.Options.FirstOrDefault<WavelengthOption>((Func<WavelengthOption, bool>)(o => (int)o.Value == (int)value));

        public override string ToString() => this.ToString(new StringBuilderEx());

        protected string ToString(StringBuilderEx sb)
        {
            sb.Append("Sel: ");
            sb.Append((object)this.SelectedWavelength);
            return sb.ToString();
        }

        public string ToStringEx()
        {
            StringBuilderEx stringBuilderEx = new StringBuilderEx();
            stringBuilderEx.Append(this.ToString());
            stringBuilderEx.Append("\tOptions: ");
            stringBuilderEx.Append((IEnumerable)this.Options);
            return stringBuilderEx.ToString();
        }
    }
}
