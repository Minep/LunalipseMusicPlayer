using System;
using System.IO;
using System.Text;

namespace NewMediaPlayer
{
    public class LogFile
    {
        static FileStream fs;
        static StreamWriter sw;
        public static void InitializeLogFile()
        {
            DateTime dt = DateTime.Now;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\logs\" + dt.ToString("yyyy-MM-dd HH-mm-ss") + ".log";
            if(!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            if (!File.Exists(path)) File.Create(path).Close();
            fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            sw = new StreamWriter(fs, Encoding.UTF8);
        }

        public static void WriteLog(string type,string info)
        {
            if (fs != null && sw != null)
            {
                DateTime dt = DateTime.Now;
                string e_info = "[" + dt.ToString("HH:mm:ss.ms") + "][" + type + "]";
                sw.WriteLine(e_info + info);
            }

        }

        public static void Release()
        {
            sw.Close();
            fs.Close();
        }
    }
}
