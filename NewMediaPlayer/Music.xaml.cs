using gMusic.API;
using System;
using System.Windows;
using LunaNetCore.Bodies;
using gMusic;
using System.Collections.ObjectModel;
using NewMediaPlayer.controler;
using gMusic.util;
using gMusic.MusicOL;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NewMediaPlayer.ui;
using NewMediaPlayer.Dialog;

namespace NewMediaPlayer
{
    /// <summary>
    /// Music.xaml 的交互逻辑
    /// </summary>
    public partial class Music : Window, IRequestEvent
    {
        LunaProxy lunapxy;
        MusicDtl md;
        int MODE = 0;
        string MusicN_For_LRC = "";

        string[] MODE_SET = new string[]
        {
            "单    曲",
            "歌    词",
        };
        
        ObservableCollection<MusicInfo> list = new ObservableCollection<MusicInfo>();
        public Music()
        {
            InitializeComponent();
            lunapxy = LunaProxy.INSTANCE(this);
            
        }

        

        public void AllReqCompletely()
        {
            
        }

        public void ReqTimeOut()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                loading.Visibility = Visibility.Hidden;
                new LDailog(LunalipsContentUI.TIPMESSAGE, "请求超时", "请求超时，请检查您的网络连接。").ShowDialog();
            }));
        }

        //Implements of interface
        public void ErrorOccurs(Exception e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LogFile.WriteLog("ERROR", e.ToString());
                new LDailog(LunalipsContentUI.TIPMESSAGE, "请求错误", "LunaNetCore抛出了一个异常：" + e.Message).ShowDialog();
            }));
        }

        public void HttpRequesting(string par1)
        {
            if (par1.Equals(EventType.E_SEARCH))
            {
                Dispatcher.Invoke(new Action(() => loading.Visibility = Visibility.Visible));
            }
        }

        public void HttpResponded(string par1, RResult par2)
        {
            Dispatcher.Invoke(new Action(() => loading.Visibility = Visibility.Hidden));
            switch (par1)
            {
                case EventType.E_SEARCH:
                    ProccessResultList(par2);
                    break;
                case EventType.E_LYRIC:
                    ShowLyric(par2);
                    break;
                case EventType.E_DETAIL:
                    md = ResultFormatter.getDetail(par2.ResultData);
                    InvokeChangeContent(LunalipsContentUI.MUSIC_DETAIL, md);
                    break;
            }
        }

        private void ShowLyric(RResult rr)
        {
            InvokeChangeContent(LunalipsContentUI.LYRIC_DISPLY, ResultFormatter.getLyrics(rr.ResultData), MusicN_For_LRC);
        }

        protected void InvokeChangeContent(LunalipsContentUI id, params object[] pArg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (CC.Content != null)
                {
                    CC.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1)));
                }
                switch (id)
                {
                    case LunalipsContentUI.MUSIC_DETAIL:
                        CC.Content = new musicDetail(pArg[0] as MusicDtl);
                        break;
                    case LunalipsContentUI.LYRIC_DISPLY:
                        CC.Content = new LyricPreview(pArg[0] as string, pArg[1] as string);
                        break;
                }
                CC.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1)));
            }));
        }

        private void ProccessResultList(RResult rr)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                list.Clear();
                IDictionary<int, MusicOvw> rs = ResultFormatter.MusicOverview(rr.ResultData);
                foreach (var b in rs)
                {
                    list.Add(new MusicInfo() { MusicN = b.Value.MusicName, artist = b.Value.Artist, ID = b.Value.ID });
                }
                music.ItemsSource = list;
            }));
        }

        

        private void EllipseMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string s = musicName.Text;
            int smode = SearchType.S_SONG;
            switch(MODE)
            {
                case 0:
                    smode = SearchType.S_SONG;
                    break;
            }
            {
                lunapxy.SearchGetSongs(s, smode, 0);
                lunapxy.ReqAsyn();
            }
        }

        private void music_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(music.SelectedIndex!=-1)
            {
                MusicInfo mi = music.SelectedItem as MusicInfo;
                if (MODE!=1)
                {
                    lunapxy.GetDetail(mi.ID);
                    music.SelectedIndex = -1;
                    
                }
                else
                {
                    lunapxy.GetLyric(mi.ID);
                    MusicN_For_LRC = mi.MusicN.Replace(":", " ");
                }
                lunapxy.ReqAsyn();
            }
        }

        private void ModeChange(object sender, RoutedEventArgs e)
        {
            MODE++;
            if(MODE>=MODE_SET.Length)
            {
                MODE = 0;
            }
            (sender as Button).Content = MODE_SET[MODE];
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            LunaProxy.DEINSTANCE();
        }

        private void setting_md(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new LDailog(LunalipsContentUI.OL_DL_SETTING, "配置联网乐库", true).ShowDialog();
        }
    }
}
