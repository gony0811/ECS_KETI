using SharedLibrary;
using System.Runtime.Serialization;


namespace DEV.PowerMeter.Library
{
    [DataContract]
    public class MeterSettingsSerializer
    {
        public const string TopLevelName = "CMC_App";
        protected PropertySerializer Serializer;

        [DataMember]
        public string ScaleManagerHorizontal_Settings { get; set; }

        [DataMember]
        public string ScaleManagerVertical_Settings { get; set; }

        [DataMember]
        public string ScaleManagerTuning_Settings { get; set; }

        public MeterSettingsSerializer(
          Meter Meter,
          PreviewBufferController PreviewBufferController,
          IErrorReporter errorReporting)
        {
            this.Serializer = new PropertySerializer("CMC_App", new object[3]
            {
        (object) Meter,
        (object) PreviewBufferController,
        (object) this
            }, errorReporting);
            this.ScaleManagerHorizontal_Settings = this.ScaleManagerVertical_Settings = this.ScaleManagerTuning_Settings = "";
        }

        public void Save(string filename) => this.Serializer.SaveToFile(filename);

        public void Load(string filename) => this.Serializer.LoadFromFile(filename);

        public object[] Combine(object[] a, object[] b)
        {
            object[] objArray = new object[a.Length + b.Length];
            int num = 0;
            for (int index = 0; index < a.Length; ++index)
                objArray[num++] = a[index];
            for (int index = 0; index < b.Length; ++index)
                objArray[num++] = b[index];
            return objArray;
        }
    }
}
