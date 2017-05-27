using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NewMediaPlayer.Lyric
{
    class LyricDecompiler
    {
        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; }
        StreamReader fs;

        //Event
        public delegate void LyricNotFound();
        public static LyricNotFound LyricNotFoundTrigger;
        public LyricDecompiler(string LrcPath)
        {
            try
            {
                fs = new StreamReader(LrcPath, Encoding.UTF8);
            }
            catch(FileNotFoundException fne)
            {
                LogFile.WriteLog("WARNING", "Cannot load the lyric which suppose to be located in "+fne.FileName);
                LyricNotFoundTrigger();
            }
        }

        public Dictionary<double,string> DecompilerLyrics()
        {
            if (fs == null) return null;
            Dictionary<double, string> dir = new Dictionary<double, string>();
            using (fs)
            {
                String line;
                Regex rx = new Regex(@"\[.*?\]");
                while ((line = fs.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.StartsWith("[ti:"))
                        {
                            Title = SplitInfo(line);
                        }
                        else if (line.StartsWith("[ar:"))
                        {
                            Artist = SplitInfo(line);
                        }
                        else if (line.StartsWith("[al:"))
                        {
                            Album = SplitInfo(line);
                        }
                        else if (line.StartsWith("[by:"))
                        {
                            LrcBy = SplitInfo(line);
                        }
                        else if (line.StartsWith("[offset:"))
                        {
                            Offset = SplitInfo(line);
                        }
                        else
                        {
                            Regex regex = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);
                            MatchCollection mc = regex.Matches(line);
                            double time = TimeSpan.Parse("00:" + mc[0].Groups[1].Value).TotalSeconds;
                            string word = mc[0].Groups[2].Value;
                            dir.Add(time, word);
                        }
                    }
                }
            }
            LogFile.WriteLog("INFO", "Lyrics - Loaded " + dir.Count + " records of lyric");
            return dir;
        }

        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
    }
}
