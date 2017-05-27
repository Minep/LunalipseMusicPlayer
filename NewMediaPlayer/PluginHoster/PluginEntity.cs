using LunalipseAPI;
using LunalipseAPI.Graphics;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.PlayMode;

namespace NewMediaPlayer.PluginHoster
{
    public class PluginEntity
    {
        public MainUI ENTRY;
        public FFT LFFT;
        public ILunalipseDrawing LDrawing;
        public ILunalipx LPX;
        public IMode LMode;
        public bool modeI18NReq;
        
        public PluginEntity()
        {
            
        }
    }
}
