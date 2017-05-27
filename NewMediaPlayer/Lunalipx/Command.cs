using System.Collections;

namespace NewMediaPlayer.Lunalipx
{
    public class Command
    {
        public static ArrayList cmd = new ArrayList();
        public static int MAX_MODE = 6;
        public static int SCRIPT_range = 6;
        public const int LUNA_PLAYS = 0x0111;
        public const int LUNA_SETVOL = 0x0222;
        public const int LUNA_SETEQU = 0x0333;
        public const int LUNA_PASS = 0x0444;
        public const int LUNA_PLAYN = 0x0555;
        public const int LUNA_LLOOP = 0x0FFF;
        public const int LUNA_SHUTDOWN_COM = 0x3FFF;
    }
}
