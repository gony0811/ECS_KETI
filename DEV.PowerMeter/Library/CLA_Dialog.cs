using MvvmFoundation.Wpf;
using SharedLibrary;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace DEV.PowerMeter.Library
{
    public class CLA_Dialog : Window, IComponentConnector
    {
        private int freeTrialRemainingDays;
        public const string MeterPartNumberKey = "MeterPartNumber";
        public const string MeterSerialNumberKey = "MeterSerialNumber";
        public const string FeaturePartNumberKey = "FeaturePartNumber";
        public const string FeatureNameKey = "FeatureName";
        public const string CMC_VersionKey = "Coherent Meter Connection Version";
        private bool _contentLoaded;

        public string Version { get; set; }

        public string MeterPartNumber { get; set; }

        public string MeterSerialNumber { get; set; }

        public string FeatureName { get; set; }

        public string FeaturePartNumber { get; set; }

        public string ExpirationText { get; set; }

        public int FreeTrialRemainingDays
        {
            get => this.FreeTrialRemainingDays;
            set
            {
                this.freeTrialRemainingDays = value;
                this.ExpirationText = value > 0 ? string.Format("{0} days remain in this free trial.", (object)value) : "Your initial Free Trial Period has expired.";
            }
        }

        public CLA_Dialog()
        {
            this.InitializeComponent();
            this.Version = AssemblyProperties.Current.AssemblyVersionMajorMinor;
            this.DataContext = (object)this;
        }

        public CLA_Dialog(
          string featurePartNumber,
          string meterPartNumber,
          string meterSerial,
          int freeTrialRemainingDays)
          : this()
        {
            this.MeterPartNumber = meterPartNumber;
            this.MeterSerialNumber = meterSerial;
            this.FeatureName = CMC_CLA.Current.FeatureName[featurePartNumber];
            this.FeaturePartNumber = featurePartNumber;
            this.FreeTrialRemainingDays = freeTrialRemainingDays;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CenterOnMain();
            this.AddHandler(Hyperlink.RequestNavigateEvent, (Delegate)new RoutedEventHandler(this.OnNavigationRequest));
        }

        public void OnNavigationRequest(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Hyperlink originalSource))
                return;
            Process.Start(originalSource.NavigateUri.ToString());
        }

        public ICommand CloseCommand => (ICommand)new RelayCommand(new Action(((Window)this).Close));

        private void CopyOrderingInfoToClipboard_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------- Begin Ordering Information ----------");
            stringBuilder.AppendLine("Coherent Meter Connection Version: " + this.Version);
            stringBuilder.AppendLine("MeterPartNumber: " + this.MeterPartNumber);
            stringBuilder.AppendLine("MeterSerialNumber: " + this.MeterSerialNumber);
            stringBuilder.AppendLine("FeatureName: " + this.FeatureName);
            stringBuilder.AppendLine("FeaturePartNumber: " + this.FeaturePartNumber);
            stringBuilder.AppendLine("------------- End Ordering Information ------------");
            Clipboard.SetText(stringBuilder.ToString());
            int num = (int)MessageBox.Show("You may paste this information into an email or any other document.", "Ordering Information Copied To Clipboard", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            Application.LoadComponent((object)this, new Uri("/CMC_Library;component/cmc_cla/cla_dialog.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((FrameworkElement)target).Loaded += new RoutedEventHandler(this.Window_Loaded);
                    break;
                case 2:
                    ((MenuItem)target).Click += new RoutedEventHandler(this.CopyOrderingInfoToClipboard_Click);
                    break;
                case 3:
                    ((ButtonBase)target).Click += new RoutedEventHandler(this.CopyOrderingInfoToClipboard_Click);
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
