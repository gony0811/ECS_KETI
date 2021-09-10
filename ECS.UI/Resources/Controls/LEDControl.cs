using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ECS.UI.Resources.Controls
{
    public class LEDControl : CheckBox
    {
        public readonly static DependencyProperty OnColorProperty;
        public readonly static DependencyProperty OffColorProperty;

        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetCurrentValue(OnColorProperty, value); }
        }

        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetCurrentValue(OffColorProperty, value); }
        }

        static LEDControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LEDControl), new FrameworkPropertyMetadata(typeof(LEDControl)));

            OnColorProperty = DependencyProperty.Register("OnColor", typeof(Brush), typeof(LEDControl), new FrameworkPropertyMetadata(Brushes.LimeGreen));
            OnColorProperty = DependencyProperty.Register("OffColor", typeof(Brush), typeof(LEDControl), new FrameworkPropertyMetadata(Brushes.Maroon));
        }
    }
}
