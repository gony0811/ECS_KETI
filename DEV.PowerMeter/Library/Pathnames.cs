using SharedLibrary;
using System;
using System.IO;

namespace DEV.PowerMeter.Library
{
    public static class Pathnames
    {
        private static string _CMC_APP_DATA_ROOT;
        private static string _CMC_APP_DIAGNOSTICS;
        private static string _SerializationFolder;
        public static string TRANSCRIPT_SUFFIX = "Transcript.lst";
        public static string DUMP_SUFFIX = "Dump.lst";
        public static string DETAILS_SUFFIX = "Details.lst";
        public const string SettingsExtension = ".phocon";
        public const string SettingsFieldSeparator = "--";
        public const string MatchAny = ".*";
        public const string SettingsFilter = "PHOCON Files (*.phocon)|*.phocon";
        public const string ExportSettingsDefaultFilename = "CMC Settings";
        public const string ImportExportBufferFilename = "Buffer1.csv";

        private static AssemblyProperties AssemblyProperties => AssemblyProperties.Current;

        private static string CMC_APP_PRODUCT_NAME => Pathnames.AssemblyProperties.AssemblyProduct;

        private static string CMC_APP_BASENAME => Pathnames.CMC_APP_PRODUCT_NAME + " " + Pathnames.AssemblyProperties.AssemblyVersionMajorMinor;

        public static string CMC_APP_DATA_ROOT
        {
            get
            {
                if (Pathnames._CMC_APP_DATA_ROOT == null)
                {
                    Pathnames._CMC_APP_DATA_ROOT = Path.Combine(StandardPathnames.LocalApplicationData, Pathnames.AssemblyProperties.AssemblyCompany, Pathnames.CMC_APP_BASENAME);
                    Pathnames.CreateFolderIfMissing(Pathnames._CMC_APP_DATA_ROOT);
                }
                return Pathnames._CMC_APP_DATA_ROOT;
            }
            set => Pathnames._CMC_APP_DATA_ROOT = value;
        }

        private static string CreateRootSubfolder(string subfolder)
        {
            string foldername = Path.Combine(Pathnames.CMC_APP_DATA_ROOT, subfolder);
            Pathnames.CreateFolderIfMissing(foldername);
            return foldername;
        }

        public static string CMC_APP_DIAGNOSTICS => Pathnames._CMC_APP_DIAGNOSTICS ?? (Pathnames._CMC_APP_DIAGNOSTICS = Pathnames.CreateRootSubfolder("Diagnostics"));

        public static string SerializationFolder
        {
            get => Pathnames._SerializationFolder ?? (Pathnames._SerializationFolder = Pathnames.CreateRootSubfolder("Settings"));
            set => Pathnames._SerializationFolder = value;
        }

        public static string TRANSCRIPT_PATHNAME => Path.Combine(Pathnames.CMC_APP_DIAGNOSTICS, StandardPathnames.TimestampString + " " + Pathnames.TRANSCRIPT_SUFFIX);

        public static string DUMP_PATHNAME => Path.Combine(Pathnames.CMC_APP_DIAGNOSTICS, StandardPathnames.TimestampString + " " + Pathnames.DUMP_SUFFIX);

        public static string DETAILS_PATHNAME => Path.Combine(Pathnames.CMC_APP_DIAGNOSTICS, StandardPathnames.TimestampString + " " + Pathnames.DETAILS_SUFFIX);

        public static string SavePathname(Sensor Sensor) => Path.Combine(Pathnames.SerializationFolder, Pathnames.SaveFilename(Sensor));

        public static string SaveFilename(Sensor Sensor)
        {
            if (!Sensor.IsInitialized)
                throw new NotSupportedException("Pathnames: UnInitialized sensor ");
            return Pathnames.SaveFilename(Sensor.SensorType.ToString(), Sensor.ModelName, Sensor.SerialNumber);
        }

        public static string SaveFilename(string type, string model, string serial) => Pathnames.RemoveInvalidFileNameCharacters(type + "--" + model + "--" + serial + ".phocon");

        public static string FindLoadFilename(Sensor Sensor)
        {
            string path = Pathnames.SavePathname(Sensor);
            return File.Exists(path) ? path : (string)null;
        }

        public static string RemoveInvalidFileNameCharacters(string s) => s.Replace("\"", "").Replace("|", "").Replace("<", "").Replace(">", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\\", "").Replace("/", "");

        public static void CreateFolderIfMissing(string foldername)
        {
            if (Directory.Exists(foldername))
                return;
            Directory.CreateDirectory(foldername);
        }

        public static string ImportExportBufferDefault => Path.Combine(StandardPathnames.DesktopFolder, "Buffer1.csv");

        public static string AppDesktopFolderName => Pathnames.CMC_APP_PRODUCT_NAME;
    }
}
