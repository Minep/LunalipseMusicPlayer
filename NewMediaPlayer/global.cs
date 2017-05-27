using I18N;
using NewMediaPlayer.Sound;

namespace NewMediaPlayer
{

    public class global
    {
        public static string MUSIC_PATH = "";
        public static int SELECTED_MUSIC = 0;
        public static float MUSIC_VOLUME = 0.7f;
        public static int EQUALIZER_SET = 0;
        public static int[] EQUALIZER_SAVE = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int PLAY_MODE = 0;    //Max: 5
        public static string DOWNLOAD_SAVE_PATH = "NO_SELECT";
        public static double PRELISTEN_VOLUME = 0.7;    //试听歌曲音量
        public static string VER = "";

        public static bool DISP_LYRIC = true;
        public static bool SHOW_FFT = true;
        public static bool SHOW_MUSIC_NAME = true;
        public static bool SHOW_CUR_DURATION = true;
        public static bool SUPPORT_VDESKTOP = true;

        public static ScalingStrategy SSTR = ScalingStrategy.Linear;
        public static int FFT_REF_FRQ = 45;

        public static Languages? LANG = Languages.CHINESE;
    }
}
