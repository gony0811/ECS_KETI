
using System.IO;
using System.Text;

namespace DEV.PowerMeter.Library
{
    public class FileX
    {
        public static StreamWriter CreateTextUTF8(string filename) => new StreamWriter(filename, false, Encoding.UTF8);

        public static StreamWriter AppendTextUTF8(string filename) => new StreamWriter(filename, true, Encoding.UTF8);
    }
}
