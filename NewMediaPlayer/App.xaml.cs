using I18N;
using NewMediaPlayer.controler;
using NewMediaPlayer.Dialog;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace NewMediaPlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        I18NHelper I18NH;
        public App()
        {
            
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogFile.WriteLog("UNEXPECTED ERROR", e.Exception.Message);
            e.Handled = true;
        }

        private void CheckAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                // It is not possible to launch a ClickOnce app as administrator directly,  
                // so instead we launch the app as administrator in a new process.  
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator  
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process  
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    
                }

                // Shut down the current process  
                Environment.Exit(0);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LogFile.InitializeLogFile();
            LogFile.WriteLog("INFO", "Reading conifg");
            bool tmp_ = PropertyHelper.ReadProperty();
            I18NH = I18NHelper.INSTANCE;
            I18NH.LANGUAGES = global.LANG ?? Languages.CHINESE;
            I18NH.parseLangFile();
            PageLang PL = I18NH.GetReferrence("Cfg");
            StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
            if (global.VER == null && tmp_)
            {
                new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("CfgIncmp"), PL.GetContent("CfgIncmpC")).Show();
                PropertyHelper.SaveProperty();
            }
            else if (tmp_)
            {
                string s = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (!global.VER.Equals(s))
                {
                    new LDailog(LunalipsContentUI.TIPMESSAGE, PL.GetContent("CfgOld"), String.Format(PL.GetContent("CfgOldC"), global.VER, s)).Show();
                }
            }
            //如果不是管理员，程序会直接退出，并使用管理员身份重新运行。  
        }
    }
}
