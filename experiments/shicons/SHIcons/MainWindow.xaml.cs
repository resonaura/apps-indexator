using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;

namespace SHIcons
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private static BitmapSource GetSourceFromIcon(Icon icon)
        {
            BitmapSource result = null;
            if (icon != null)
            {
                IntPtr hbmp = icon.ToBitmap().GetHbitmap();
                result = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(hbmp);
            }
            return result;
        }
        public MainWindow()
        {
            InitializeComponent();

            for(int i = 0; i<200; i++)
            {
                StackPanel sc = new StackPanel();
                Label label = new Label();
                Image image = new Image();
                image.Source = GetSourceFromIcon(Win32E.GetShellIcon(i));
                label.Content = i;
                sc.Orientation = Orientation.Horizontal;
                image.Width = 48;
                image.Height = 48;
                sc.Children.Add(image);
                sc.Children.Add(label);
                IconWrapper.Children.Add(sc);
            }

        }
    }
}
