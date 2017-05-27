using LunalipseAPI;
using LunalipseAPI.Generic;
using System.Collections;
using System.Windows.Forms;
using System;

namespace TestPlugin
{
    //Entry Point
    [LunalipsePluginInfo(Name ="TEST PLUGIN",Description ="Test plugin , Test UI function", Team ="Canterlot Computing Research Center",Version ="1.0.2.0",Author = "Lunaixsky")]
    public class LXPLMAIN : MainUI
    {
        public void Destroy()
        {
            MessageBox.Show("I am Unloaded");
        }

        public void GrabMusicList(ArrayList ml)
        {
            MessageBox.Show("I am a plugin2 and I am loaded.\nWe have " + ml.Count + " songs");
        }

        public void Initialize()
        {
            //MainUIEvent.Invoke(Setter.VOL, 0.75);
            //MainUIEvent.Invoke(Setter.MODE, 2);
            //MainUIEvent.Invoke(Setter.MUSIC, 3);
        }

        public void LunalipseExit()
        {
            
        }

        public void ModeChange(int mode)
        {
           // MessageBox.Show("Mode Changed : " + mode);
        }

        public void MusicChange(PlayInfo pi)
        {
            
        }

        public void VolumeChange(double vol)
        {
            
        }
    }
}
