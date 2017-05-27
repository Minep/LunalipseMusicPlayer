using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NewMediaPlayer.PluginHoster
{
    class PluginInfo : INotifyPropertyChanged
    {
        private string pluginName;
        private Visibility Activated;
        public string PLGName
        {
            get
            {
                return pluginName;
            }
            set
            {
                pluginName = value;
            }
        }

        public Visibility PLGActivated
        {
            get
            {
                return Activated;
            }
            set
            {
                Activated = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PLGActivated"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
