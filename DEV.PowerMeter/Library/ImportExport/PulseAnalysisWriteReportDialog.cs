using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class PulseAnalysisWriteReportDialog : ExportDialog
    {
        public const string AnalysisReportFilenameDefault = "PulseAnalysis.csv";

        public PulseAnalyzer PulseAnalyzer { get; protected set; }

        public bool RequestCanceled { get; set; }

        public PulseAnalysisWriteReportDialog(PhoenixMeter phoenixMeter)
        {
            this.PulseAnalyzer = phoenixMeter.Computations.PulseAnalyzer;
            if (this.PulseAnalyzer.Results.Count <= 0 && MessageBox.Show("Zero pulses were detected, so the Pulse Analysis report would be empty.\n\nPress OK to write an empty report.\n\nElse press Cancel.", "No Pulse data to export", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
            {
                this.RequestCanceled = true;
            }
            else
            {
                string path = string.IsNullOrEmpty(phoenixMeter.CaptureBuffer.FileBasename) ? "PulseAnalysis.csv" : Path.GetFileNameWithoutExtension(phoenixMeter.CaptureBuffer.FileBasename) + " PulseAnalysis.csv";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export Pulse Analysis Report";
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
                saveFileDialog.FileName = path;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = ".csv";
                this.FileDialog = (FileDialog)saveFileDialog;
            }
        }

        public override bool? ShowDialog()
        {
            if (this.RequestCanceled)
                return new bool?();
            bool? nullable1;
            bool? nullable2 = nullable1 = this.FileDialog.ShowDialog();
            bool flag = true;
            if (!(nullable2.GetValueOrDefault() == flag & nullable2.HasValue))
                return nullable1;
            this.PulseAnalyzer.WriteReport(this.FileName);
            return nullable1;
        }
    }
}
