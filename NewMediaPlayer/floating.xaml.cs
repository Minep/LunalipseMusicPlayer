using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using NewMediaPlayer.Sound;
using WindowsDesktop;

namespace NewMediaPlayer
{
    /// <summary>
    /// floating.xaml 的交互逻辑
    /// </summary>
    public partial class floating : Window
    {
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        public floating()
        {
            InitializeComponent();
            AllowsTransparency = true;
            this.Width = SystemParameters.WorkArea.Width;
            lrcdpL.Width = Width; ;
            this.Topmost = true;
            
            this.SourceInitialized += delegate
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            };
            InitializeFFT.OnSpectrumDrawnComplete += (isu) =>
            {
                fftContainer_ft.Source = isu;
            };
            Lyric.LyricDecompiler.LyricNotFoundTrigger += () =>
            {
                lrcdpL.Content = secLrc.Content = "";
            };
            Lyric.LyricsDisplay.OnLryicMatched += (lrc) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (lrc.Contains("|"))
                    {
                        string[] lrcs = lrc.Split('|');
                        lrcdpL.Content = global.DISP_LYRIC ? lrcs[0] : "";
                        secLrc.Content = global.DISP_LYRIC ? lrcs[1] : "";
                    }
                    else
                    {
                        lrcdpL.Content = global.DISP_LYRIC ? lrc : "";
                        secLrc.Content = "";
                    }

                }));
            };
            MainWindow.OnMusicChanged += (Mname) =>
            {
                name.Content = global.SHOW_MUSIC_NAME? Mname:"";
            };
            PlaySound.OnDurationChanged += (a, b) =>
            {
                duration.Dispatcher.Invoke(new Action(() =>
                {
                    duration.Content = global.SHOW_CUR_DURATION ? b : "";
                    if (!global.SHOW_MUSIC_NAME && name.Visibility == Visibility.Visible)
                    {
                        name.Visibility = Visibility.Hidden;
                    }
                    else if(global.SHOW_MUSIC_NAME && name.Visibility != Visibility.Visible)
                    {
                        name.Visibility = Visibility.Visible;
                    }
                }));
            };
            VirtualDesktop.CurrentChanged += (x, y) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if(global.SUPPORT_VDESKTOP)
                    {
                        var hld = new WindowInteropHelper(this).Handle;
                        if (!VirtualDesktopHelper.IsCurrentVirtualDesktop(hld))
                        {
                            this.MoveToDesktop(VirtualDesktop.Current);
                        }
                    }
                });
            };
        }

        private void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window w = (Window)sender;
            w.Topmost = true;
        }
    }
}
