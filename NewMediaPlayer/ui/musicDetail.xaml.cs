using gMusic.MusicOL;
using gMusic.util;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// musicDetail.xaml 的交互逻辑
    /// </summary>
    public partial class musicDetail : Page
    {
        Downloader der;
        MusicDtl md;
        string ext;
        public musicDetail(MusicDtl _md)
        {
            InitializeComponent();
            prev.LoadedBehavior = MediaState.Play;
            prev.UnloadedBehavior = MediaState.Stop;
            prev.Volume = global.PRELISTEN_VOLUME;
            md = _md;
            RegistEvent();
            ProccessDetail();
            prev.Source = new Uri(md.perlisten);
        }

        public void RegistEvent()
        {
            der = new Downloader();
            double total = 0;
            string t = "";
            double prec = 0;
            der.OnDataSetup += (x) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    t = sizeCalc.Calc(x);
                    status.Content = "正在下载：0MB / " + t;
                    total = x;
                    prgs.Maximum = x;
                }));
            };
            der.OnDownloadFinish += (x, y) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!x)
                    {
                        status.Content = "";
                        prgV.Content = "当前没有任务";
                        prgs.Value = 0;
                    }
                    else
                    {
                        LogFile.WriteLog("ERROR", y.Message);
                        Console.WriteLine(y.ToString());
                    }
                }));
            };
            der.OnTaskUpdate += (x) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    prec = x / total * 100d;
                    status.Content = "正在下载：" + sizeCalc.Calc(x) + " / " + t;
                    prgV.Content = decimal.Round(new decimal(prec), 1).ToString() + "%";
                    prgs.Value = x;
                }));
            };
        }

        public void ProccessDetail()
        {
            if (md == null) return;
            Dispatcher.Invoke(new Action(() =>
            {
                name.Content = md.musicN;
                album.Source = new BitmapImage(new Uri(md.picURL));
                bitrate.Content = md.brate;
                singer.Content = md.Artist;
                bandName.Content = md.AblumName;
                string[] s = md.L_URL.Split('.');
                fomate.Content = ext = s[s.Length - 1].ToUpperInvariant();
                hsize.Content = sizeCalc.Calc(long.Parse(md.SIZE[0].ToString()));
                msize.Content = sizeCalc.Calc(long.Parse(md.SIZE[1].ToString()));
                lsize.Content = sizeCalc.Calc(long.Parse(md.SIZE[2].ToString()));
            }));
        }

        private void DownloadSongs(object sender, RoutedEventArgs e)
        {
            if (md == null) return;
            Button b = sender as Button;
            string u = "";
            long bv = 0;
            switch (b.Name)
            {
                case "l":
                    u = md.L_URL;
                    bv = md.SIZE[2];
                    break;
                case "m":
                    u = md.M_URL;
                    bv = md.SIZE[1];
                    break;
                case "h":
                    u = md.H_URL;
                    bv = md.SIZE[0];
                    break;
            }
            RunDownload(u, bv);
        }

        public void RunDownload(string _u, long a)
        {
            Console.WriteLine(_u);
            Thread t = new Thread(new ThreadStart(() =>
            {
                der.DownloadFile(_u, String.Format(global.DOWNLOAD_SAVE_PATH + "/{0}.{1}", md.musicN.Replace(':', ' '), ext.ToLowerInvariant()), a);
            }));
            t.Start();
        }
    }
}
