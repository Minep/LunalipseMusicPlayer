using NewMediaPlayer.controler;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.ui;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NewMediaPlayer.Dialog
{
    /// <summary>
    /// LDailog.xaml 的交互逻辑
    /// </summary>
    public partial class LDailog : Window
    {
        public LDailog(LunalipsContentUI lcui,string header,params object[] args)
        {
            InitializeComponent();
            Title.Content = header;
            InvokeChangeContent(lcui, null, args);
        }

        public LDailog(LunalipsContentUI lcui, string header,bool needResize, params object[] args)
        {
            InitializeComponent();
            Title.Content = header;
            ResizeWindow(lcui, args);
        }

        public LDailog(object CONTENT_INSTANCE,double width,double height)
        {
            Height = height + 55;
            Width = width + 30;
            inner.Content = CONTENT_INSTANCE;
        }

        public LDailog(string i_,params object[] paras)
        {
            LunalipsContentUI lcui = (LunalipsContentUI)Enum.Parse(typeof(LunalipsContentUI), i_);
            InvokeChangeContent(lcui, null, paras);
        }

        private void ResizeWindow(LunalipsContentUI lcui_, params object[] args_)
        {
            switch (lcui_)
            {
                case LunalipsContentUI.OL_DL_SETTING:
                    OLmusicLib oll = new OLmusicLib(this);
                    this.Height = oll.Height + 55;
                    this.Width = oll.Width + 30;
                    InvokeChangeContent(lcui_, oll, args_);
                    break;
                case LunalipsContentUI.MAINSETTING:
                    Setting ST = new Setting(this);
                    this.Height = ST.Height + 55;
                    this.Width = ST.Width + 30;
                    InvokeChangeContent(lcui_, ST, args_);
                    break;
                case LunalipsContentUI.PLUGIN_MANAGER:
                    PluginManager pm = new PluginManager(this);
                    this.Height = pm.Height + 55;
                    this.Width = pm.Width + 30;
                    InvokeChangeContent(lcui_, pm, args_);
                    break;
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected void InvokeChangeContent(LunalipsContentUI id,object a, params object[] pArg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                switch (id)
                {
                    case LunalipsContentUI.TIPMESSAGE:
                        LDlg_Tip ldtp = new LDlg_Tip(pArg[0] as string, this);
                        inner.Content = ldtp;
                        break;
                    case LunalipsContentUI.CTRL_JUMP_TO_LINE:
                        PrgCounter pc = new PrgCounter(pArg[0] as SyntaxParser, pArg[1] as MainWindow, this);
                        inner.Content = pc;
                        break;
                    case LunalipsContentUI.OL_DL_SETTING:
                        inner.Content = (a as OLmusicLib);
                        break;
                    case LunalipsContentUI.MAINSETTING:
                        inner.Content = a as Setting;
                        break;
                    case LunalipsContentUI.PLUGIN_MANAGER:
                        inner.Content = a as PluginManager;
                        break;
                }
            }));
        }
    }
}
