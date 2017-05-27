using I18N;
using LunalipseAPI.I18N;
using LunalipseAPI.PlayMode;
using System.Windows.Forms;

namespace TestPlugin
{
    [LunalipseCustomMode(I18Npresent = true)]
    public class MyMode : IMode
    {
        public void BeingInitialize()
        {
            RegLang();
            ModeManager.RegistMode(100, "MyMode1");
        }

        void RegLang()
        {
            switch (I18NProxy.CURRENT)
            {
                case Languages.CHINESE:
                    I18NProxy.AddLang(Referrence.MAINWINDOW, "MyMode1", "你好");
                    break;
                case Languages.ENGLISH:
                    I18NProxy.AddLang(Referrence.MAINWINDOW, "MyMode1", "Hello");
                    break;
                case Languages.RUSSIAN:
                    I18NProxy.AddLang(Referrence.MAINWINDOW, "MyMode1", "Привет");
                    break;
            }
        }

        public void ModeBehavior(ref int MUSIC_SELECTED, int modeID)
        {
            switch(modeID)
            {
                case 100:
                    MessageBox.Show("Hi! This is custom mode!");
                    MUSIC_SELECTED++;
                    break;
            }
        }
    }
}
