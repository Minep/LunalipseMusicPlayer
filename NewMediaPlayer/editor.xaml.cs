using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace NewMediaPlayer
{
    /// <summary>
    /// editor.xaml 的交互逻辑
    /// </summary>
    public partial class editor : Window
    {
        public editor()
        {
            InitializeComponent();
        }

        string currentPath = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            openLunapx(Convert.ToInt32(((string)s.Content).Split(' ')[1]));
        }

        public void openLunapx(int num)
        {
            currentPath = "Scripts/prg" + num + ".lunapx";
            if (!File.Exists(currentPath))
            {
                File.Create(currentPath);
            }
            using (StreamReader sr = new StreamReader(currentPath, Encoding.UTF8))
            {
                String s = sr.ReadToEnd();
                edt.Document = new FlowDocument(new Paragraph(new Run(s)));
                FlowDocument doc = new FlowDocument();
                Paragraph p = new Paragraph();
                Run r = new Run(s);
                p.Inlines.Add(r);
                doc.Blocks.Add(p);
                edt.Document = doc;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string wx = System.Windows.Markup.XamlWriter.Save(edt.Document);
            using (StreamWriter sw = new StreamWriter(new FileStream(currentPath,FileMode.Open),Encoding.UTF8))
            {
                sw.Write(wx);
                sw.Flush();
            }
        }
    }
}
