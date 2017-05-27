using I18N;
using NewMediaPlayer.Sound;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NewMediaPlayer
{
    public class PropertyHelper
    {
        public static string propFile = AppDomain.CurrentDomain.BaseDirectory + @"\property.lups";
        public static bool SaveProperty()
        {
            try
            {
                using (FileStream fs = new FileStream(propFile, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, new SEBLE_Globle());
                }
                return true;
            }
            catch(Exception e)
            {
                LogFile.WriteLog("ERROR", e.Message);
                return false;
            }
        }

        public static bool ReadProperty()
        {
            try
            {
                if (!File.Exists(propFile)) return false;
                using (FileStream fs = new FileStream(propFile, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    SEBLE_Globle sebg = bf.Deserialize(fs) as SEBLE_Globle;

                    global.MUSIC_PATH = sebg.MSIC_P;
                    global.SELECTED_MUSIC = sebg.SEL_INX;
                    global.MUSIC_VOLUME = sebg.VOL;
                    global.EQUALIZER_SET = sebg.EQZ_SET;
                    global.EQUALIZER_SAVE = sebg.EQZ_SAVE;
                    global.PRELISTEN_VOLUME = sebg.P_L_VOL;
                    global.VER = sebg.version;
                    global.DISP_LYRIC = sebg.w;
                    global.SHOW_FFT = sebg.x;
                    global.SHOW_MUSIC_NAME = sebg.y;
                    global.SHOW_CUR_DURATION = sebg.z;
                    global.SUPPORT_VDESKTOP = sebg.vd;
                    global.SSTR = sebg.ss;
                    global.FFT_REF_FRQ = sebg.freq;
                    global.LANG = sebg.l;

                    if (sebg.DOWNL_S_P.Equals("NO_SELECT"))
                        global.DOWNLOAD_SAVE_PATH = global.MUSIC_PATH;
                    else
                        global.DOWNLOAD_SAVE_PATH = sebg.DOWNL_S_P;
                }
                return true;
            }
            catch(Exception e)
            {
                LogFile.WriteLog("ERROR", e.Message);
                return false;
            }
        }
    }
    [Serializable]
    class SEBLE_Globle
    {
        public string MSIC_P;
        public string version = "";
        public int SEL_INX;
        public float VOL;
        public int EQZ_SET = 0;
        public string DOWNL_S_P;
        public double P_L_VOL = 0d;
        public int[] EQZ_SAVE = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public bool w, x, y, z, vd;
        public ScalingStrategy ss;
        public Languages? l;
        public int freq = 45;
        public SEBLE_Globle()
        {
            version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MSIC_P = global.MUSIC_PATH;
            SEL_INX = global.SELECTED_MUSIC;
            VOL = global.MUSIC_VOLUME;
            EQZ_SET = global.EQUALIZER_SET;
            EQZ_SAVE = global.EQUALIZER_SAVE;
            DOWNL_S_P = global.DOWNLOAD_SAVE_PATH;
            P_L_VOL = global.PRELISTEN_VOLUME;
            w = global.DISP_LYRIC;
            x = global.SHOW_FFT;
            y = global.SHOW_MUSIC_NAME;
            z = global.SHOW_CUR_DURATION;
            vd = global.SUPPORT_VDESKTOP;
            ss = global.SSTR;
            freq = global.FFT_REF_FRQ;
            l = global.LANG;
        }
    }
}
