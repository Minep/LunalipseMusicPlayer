using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using I18N;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.Lyric;
using NewMediaPlayer.PluginHoster;
using NewMediaPlayer.Sound;
using NullStudio.Utils.Keyboardhook;
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NewMediaPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlaySound ps;
        bool isCallByUser = false, pressed = false;
        LyricsDisplay ldip;
        FileStream fs;
        SyntaxParser sp = null;
        key_hook kh;
        I18NHelper I18NH;
        PageLang PL;
        PluginHelper PH;

        public delegate void MusicChanged(string name);
        public delegate void ExecuteCompletely();
        public static event MusicChanged OnMusicChanged;
        public event ExecuteCompletely OnExecuteCompletely;

        ArrayList al;
        public MainWindow()
        {
            InitializeComponent();
            PH = PluginHelper.INSTANCE;
            PH.GetPluginList();
            LogFile.WriteLog("INFO", "Initialize Lunalipse sound component completely.");
            ps = new Sound.PlaySound(Dur, curr, fftContainer);
            ldip = new LyricsDisplay();
            //Events
            LogFile.WriteLog("INFO", "Registering Events.");
            ps.OnPlaySoundComplete += Ps_opsc;
            ps.OnProgressUpdated += Ps_OnProgressUpdated;
            ps.OnSoundLoadedComplete += Ps_OnSoundLoadedComplete;
            PlaySound.OnDurationChanged += (dur, formated) =>
            {
                if(ldip!=null)
                {
                    ldip.IsMeetLrcBlock(dur);
                }
            };
            LogFile.WriteLog("INFO", "Initialize Lunalipse FFT spectrum component completely.");
            InitializeFFT.OnSpectrumDrawnComplete += (isu) =>
            {
                fftContainer.Source = isu;
            };
            if (!Directory.Exists("i18n")) Directory.CreateDirectory("i18n");

            I18NH = I18NHelper.INSTANCE;
            PL = I18NH.GetReferrence("MainWindow");
            /*----Check the Property-----*/
            vol_adj.Value = global.MUSIC_VOLUME;
            
            /*----Check the Folder availability ----*/
            if(!String.IsNullOrEmpty(global.MUSIC_PATH))
            {
                if(!Directory.Exists(global.MUSIC_PATH+"/Lyrics"))
                {
                    Directory.CreateDirectory(global.MUSIC_PATH + "/Lyrics");
                    LogFile.WriteLog("INFO", "No Lyrics folder present. Create automatically");
                }
            }
            else LogFile.WriteLog("WARNING", "Music path not set yet.");
            InitialPrivateMode();
            RegistKeyPress();
            GetList();
            ApplyingUIText();
            RegEvent();
        }

        

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { };
        }


        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var f_music = new System.Windows.Forms.FolderBrowserDialog();
            if(f_music.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                global.MUSIC_PATH = f_music.SelectedPath;
                if(global.MUSIC_PATH!="")
                {
                    GetList();
                }
            }
        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicList.SelectedIndex == -1)
            {
                return;
            }
            global.SELECTED_MUSIC = MusicList.SelectedIndex;
            try
            {
               
                if (fs != null) fs.Close();
                string noExtension = System.IO.Path.GetFileNameWithoutExtension(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString());
                LogFile.WriteLog("INFO", "Now playing : " + noExtension);
                fs = new FileStream(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
                Playing.Content = noExtension;
                OnMusicChanged(noExtension);
                LyricDecompiler ld = new LyricDecompiler(global.MUSIC_PATH + @"\Lyrics\" + noExtension + ".lrc");
                ldip.LrcParse(ld.DecompilerLyrics());
                ps.PlayIt(fs, System.IO.Path.GetExtension(global.MUSIC_PATH + @"\" + MusicList.SelectedItem.ToString()));
                MusicList.SelectedIndex = -1;
            }
            catch(Exception ex)
            {
                LogFile.WriteLog("ERROR", ex.Message);
            }
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (fs != null) fs.Close();
            ps.ISO_DISPOSE();
            LogFile.Release();
        }

        private void savePpt_Click(object sender, RoutedEventArgs e)
        {
            PropertyHelper.SaveProperty();
        }

        private void EllipseMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            
        }

        private void vol_adj_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ps!=null)ps.setVolume((float)e.NewValue);
            vol.Content = Math.Round(e.NewValue * 100d) + "%";
            global.MUSIC_VOLUME = (float)e.NewValue;
            PH?.FireSequence(APIBridge.VOL_C, global.MUSIC_VOLUME);
        }

        private void duration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isCallByUser)
            {
                ps.setNewPosition((e.NewValue - e.OldValue)>0?1:-1);    
            }
        }

        private void Ellipse_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Mainbtn.Fill = ps.ChangePlayStatus()?new SolidColorBrush(Color.FromArgb(193,92,184,92)):new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
        }

        private void duration_MouseEnter(object sender, MouseEventArgs e)
        {
            //isCallByUser = true;
        }

        private void duration_MouseLeave(object sender, MouseEventArgs e)
        {
            //isCallByUser = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Equalizer eqz = new Equalizer();
            eqz.ShowInTaskbar = false;
            eqz.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            kh.Stop();
            Application.Current.Shutdown();
        }

        private void _Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ModeChang(true);
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            pressed = true;
            Ps_opsc();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            editor ed = new editor();
            ed.Show();
        }

        private void netMusic_Click(object sender, RoutedEventArgs e)
        {
            Music m = new Music();
            //m.ShowInTaskbar = false;
            m.Show();
        }

        private void programe_Click(object sender, RoutedEventArgs e)
        {
            if (sp == null)
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("GErr"), PL.GetContent("GE_C1")).ShowDialog();
                return;
            }
            LDailog ld = new LDailog(LunalipsContentUI.CTRL_JUMP_TO_LINE, "", sp, this);
            ld.Show();
        }

        
        private void Lpsetting(object sender, MouseButtonEventArgs e)
        {
            new LDailog(LunalipsContentUI.MAINSETTING, "Lunalipse全局设置",true).ShowDialog();
        }

        
    }
}
