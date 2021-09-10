
using Microsoft.Win32;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class ImportDialog : Dialog
    {
        public ImportDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckPathExists = true;
            openFileDialog.AddExtension = true;
            openFileDialog.Filter = "Comma-Separated Files (*.csv)|*.csv|Tab-Separated Files (*.tsv)|*.tsv|Tab-Separated Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.DefaultExt = ImporterExporter.DefaultExtension;
            this.FileDialog = (FileDialog)openFileDialog;
        }
    }
}
