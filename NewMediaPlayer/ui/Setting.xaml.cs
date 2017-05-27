using I18N;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Sound;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NewMediaPlayer.ui
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {
        LDailog ld;
        PageLang PL;
        public Setting(LDailog _ld)
        {
            InitializeComponent();
            ld = _ld;
            fft.IsChecked = global.SHOW_FFT;
            name.IsChecked = global.SHOW_MUSIC_NAME;
            durtion.IsChecked = global.SHOW_CUR_DURATION;
            vdesk.IsEnabled = Environment.OSVersion.Version.Major == 10;
            vdesk.IsChecked = global.SUPPORT_VDESKTOP;
            diplrc.IsChecked = global.DISP_LYRIC;
            PL = I18NHelper.INSTANCE.GetReferrence("Setting");
            scaling.Items.Add(PL.GetContent("l1"));
            scaling.Items.Add(PL.GetContent("l2"));
            scaling.Items.Add(PL.GetContent("l3"));

            foreach(var i in I18NHelper.INSTANCE.GetReferrence("common").AllLang)
            {
                lang.Items.Add(i.Value);
            }

            if (global.LANG != null) lang.SelectedIndex = (int)global.LANG;
            else lang.SelectedIndex = 0;

            scaling.SelectedIndex = (int)global.SSTR;
            freq.Text = global.FFT_REF_FRQ.ToString();

            _ld.Title.Content = PL.GetContent("title");

            t1.Content = PL.GetContent("T1");
            t1s1.Content = PL.GetContent("T1_Sub1");
            t1s2.Content = PL.GetContent("T1_Sub2");
            t1s3.Content = PL.GetContent("T1_Sub3");
            t1s4.Content = PL.GetContent("T1_Sub4");
            t1s5.Content = PL.GetContent("T1_Sub5");

            t2.Content = PL.GetContent("T2");
            t2s1.Content = PL.GetContent("T2_Sub1");
            t2s2.Content = PL.GetContent("T2_Sub2");

            t3.Content = PL.GetContent("T3");
            t3s1.Content = PL.GetContent("T3_Sub1");

            success.Content = PL.GetContent("sucess");
            apply.Content = PL.GetContent("apy");
            plgmana.Content = PL.GetContent("plgm");
        }

        //Events
        public delegate void ScalingStrategyChange(ScalingStrategy ss);
        public static event ScalingStrategyChange OnScalingStrategyChange;

        private void StatusCheck(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch(cb.Name)
            {
                case "fft":
                    global.SHOW_FFT = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
                    break;
                case "name":
                    global.SHOW_MUSIC_NAME = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
                    break;
                case "durtion":
                    global.SHOW_CUR_DURATION = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
                    break;
                case "vdesk":
                    global.SUPPORT_VDESKTOP = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
                    break;
                case "diplrc":
                    global.DISP_LYRIC = cb.IsChecked.HasValue ? (bool)cb.IsChecked : IsInitialized;
                    break;
            }
        }

        private void scaling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScalingStrategy s;
            switch (scaling.SelectedIndex)
            {
                case 0:
                    s = ScalingStrategy.Decibel;
                    break;
                case 1:
                    s = ScalingStrategy.Linear;
                    break;
                case 2:
                    s = ScalingStrategy.Sqrt;
                    break;
                default:
                    s = ScalingStrategy.Linear;
                    break;
            }
            OnScalingStrategyChange?.Invoke(s);
        }

        //Apply
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            global.FFT_REF_FRQ = validate();
            bool b;
            if (b = PropertyHelper.SaveProperty())
            {
                success.Content = PL.GetContent("sucess");
            }
            else
            {
                success.Content = PL.GetContent("SaveFT");
            }
            try
            {
                new Thread(() =>
                {
                    Dispatcher.Invoke(() => notify.BeginAnimation(HeightProperty, new DoubleAnimation(0, 30, TimeSpan.FromSeconds(.5))));
                    Thread.Sleep(3500);
                    Dispatcher.Invoke(() => notify.BeginAnimation(HeightProperty, new DoubleAnimation(30, 0, TimeSpan.FromSeconds(.5))));
                }).Start();
            }
            catch { }
            new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent(b? "SaveST": "SaveFT"), PL.GetContent(b? "SaveSTc": "SaveFTc")).ShowDialog();
        }

        public int validate()
        {
            try
            {
                int a = int.Parse(freq.Text);
                if(a<=15&&a>=500)
                {
                    return 45;
                }
                return a;
            }
            catch { return 45; }
        }

        private void LangChange(object sender, SelectionChangedEventArgs e)
        {
            Languages? l = Languages.CHINESE;
            switch(lang.SelectedIndex)
            {
                case 0:
                    l = Languages.CHINESE;
                    break;
                case 1:
                    l = Languages.ENGLISH;
                    break;
                case 2:
                    l = Languages.RUSSIAN;
                    break;
            }
            global.LANG = l;
        }

        private void plgM(object sender, RoutedEventArgs e)
        {
            new LDailog(LunalipsContentUI.PLUGIN_MANAGER, "", true).ShowDialog();
        }
    }
}
