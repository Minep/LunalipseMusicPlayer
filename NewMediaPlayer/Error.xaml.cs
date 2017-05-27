using System.Windows;

namespace NewMediaPlayer
{
    /// <summary>
    /// Error.xaml 的交互逻辑
    /// </summary>
    public partial class Error : Window
    {
        public Error(string[] msg)
        {
            InitializeComponent();
            ermsg.Content = msg[0];
            emsg.Content = msg[2];
            line.Content = msg[3];
            code.Content = msg[1];
        }
    }
}
