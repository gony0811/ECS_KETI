using Microsoft.Win32;
using SharedLibrary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEV.PowerMeter.Library.ImportExport
{
    public abstract class Dialog
    {
        public FileDialog FileDialog;

        public virtual bool? ShowDialog() => this.FileDialog.ShowDialog();

        public string Title
        {
            get => this.FileDialog.Title;
            set => this.FileDialog.Title = value;
        }

        public string FileName
        {
            get => this.FileDialog.FileName;
            set => this.FileDialog.FileName = value;
        }

        public string InitialDirectory
        {
            get => this.FileDialog.InitialDirectory;
            set => this.FileDialog.InitialDirectory = value;
        }

        public string Filter
        {
            get => this.FileDialog.Filter;
            set => this.FileDialog.Filter = value;
        }

        public string DefaultExt
        {
            get => this.FileDialog.DefaultExt;
            set => this.FileDialog.DefaultExt = value;
        }

        public int FilterIndex
        {
            get => this.FileDialog.FilterIndex;
            set => this.FileDialog.FilterIndex = value;
        }

        public bool AddExtension
        {
            get => this.FileDialog.AddExtension;
            set => this.FileDialog.AddExtension = value;
        }

        public bool CheckPathExists
        {
            get => this.FileDialog.CheckPathExists;
            set => this.FileDialog.CheckPathExists = value;
        }

        public string TrueFileNameWithCorrectExtension
        {
            get
            {
                string extension = Path.GetExtension(this.FileName);
                return ((IEnumerable<string>)ImporterExporter.ValidExtensions).Contains<string>(extension) ? Path.Combine(Path.GetDirectoryName(this.FileName), Path.GetFileNameWithoutExtension(this.FileName)) + this.setExtension(this.FilterIndex) : this.FileName;
            }
        }

        private string setExtension(int FilterIndex)
        {
            string str = "";
            switch (FilterIndex)
            {
                case 1:
                    str = ".csv";
                    break;
                case 2:
                    str = ".tsv";
                    break;
                case 3:
                    str = ".txt";
                    break;
                case 5:
                    if (StandardPathnames.GetExtension(this.FileName) == "")
                    {
                        str = ImporterExporter.DefaultExtension;
                        break;
                    }
                    break;
            }
            return str;
        }
    }
}
