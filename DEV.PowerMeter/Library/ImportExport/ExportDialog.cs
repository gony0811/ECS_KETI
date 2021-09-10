
using Microsoft.Win32;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class ExportDialog : Dialog
    {
        public ExportDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Comma-Separated Files (*.csv)|*.csv|Tab-Separated Files (*.tsv)|*.tsv|Tab-Separated Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = ImporterExporter.DefaultExtension;
            this.FileDialog = (FileDialog)saveFileDialog;
        }
    }
}
