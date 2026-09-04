using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace AppsIndexator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        const int CSIDL_COMMON_STARTMENU = 0x16;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (x, y) =>
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string tempPath = assemblyPath + "\\Temp\\"+unixTimestamp+"\\";
                if(!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                

                PrText.Text = "Получаем список приложений...";

                Task.Factory.StartNew(() =>
                {
                    string uninstall64Key = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                    
                    using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstall64Key))
                    {
                        string[] snames = rk.GetSubKeyNames();
                        int i = 0;
                        foreach (string skName in snames)
                        {
                            using (RegistryKey sk = rk.OpenSubKey(skName))
                            {
                                try
                                {

                                    var displayName = sk.GetValue("DisplayName");
                                    var location = (string)sk.GetValue("InstallLocation");

                                    if (displayName != null && location != null && location != "")
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            PrText.Text = "Процессим " + displayName.ToString();
                                            PrList.AppendText("Найдено приложение: " + displayName.ToString() + "\n");
                                            PrList.ScrollToEnd();
                                            PrBar.Value = (((double)i / (double)(snames.Length)) * 100 / 2);
                                            TTI.ProgressValue = PrBar.Value / 100;
                                        });
                                        if(Directory.Exists(location))
                                        {
                                            foreach(string file in Directory.GetFiles(location, "*.exe"))
                                            {
                                                Application.Current.Dispatcher.Invoke(() =>
                                                {
                                                    PrList.AppendText("Найден exe: " + file + ". \nПолучаю иконку..\n");
                                                    PrList.ScrollToEnd();
                                                });
                                                BitmapSource bitmapS = Icons.SourceFromPath(file);

                                                if(bitmapS != null)
                                                {
                                                    string xname = Path.GetFileNameWithoutExtension(file);
                                                    string wrpath = tempPath + "\\" + xname + ".png";
                                                    if(File.Exists(wrpath))
                                                    {
                                                        string uniquePath = tempPath + "\\" + displayName.ToString() + " unique";
                                                        if(!Directory.Exists(uniquePath))
                                                        {
                                                            Directory.CreateDirectory(uniquePath);
                                                        }
                                                        wrpath = uniquePath + "\\" + xname + ".png";
                                                    }
                                                    Icons.WritePng(wrpath, bitmapS);
                                                    Application.Current.Dispatcher.Invoke(() =>
                                                    {
                                                        PrList.AppendText("Иконка сохранена по адресу: "+ wrpath + "\n");
                                                        PrList.ScrollToEnd();
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                            i++;
                        }
                        
                        //label1.Text += " (" + lstDisplayHardware.Items.Count.ToString() + ")";
                    }
                    
                    string s = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs";
                    string s2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs";
                    string[] files1 = Directory.GetFiles(s, "*.lnk", SearchOption.AllDirectories);
                    string[] files2 = Directory.GetFiles(s2, "*.lnk", SearchOption.AllDirectories);
                    List<string> files = new List<string>();
                    files.AddRange(files1);
                    files.AddRange(files2);
                    int k = 0;
                    foreach (string f in files) {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PrText.Text = "Процессим " + Path.GetFileNameWithoutExtension(f);
                            PrList.AppendText("Найдено приложение (SM): " + Path.GetFileNameWithoutExtension(f) + "\n");
                            PrList.ScrollToEnd();
                            PrBar.Value = ((((double)k / (double)(files.Count)) * 100 / 2) + 50);
                            TTI.ProgressValue = PrBar.Value / 100;
                        });
                        string file = FileTools.GetRealAppPath(f);
                        if(file != null)
                        {
                            if(File.Exists(file))
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    PrList.AppendText("Найден exe: " + Path.GetFileNameWithoutExtension(file) + "\nПолучаю иконку...\n");
                                    PrList.ScrollToEnd();
                                });
                                BitmapSource bitmapS = Icons.SourceFromPath(file);

                                if (bitmapS != null)
                                {
                                    string xname = Path.GetFileNameWithoutExtension(file);
                                    string wrpath = tempPath + "\\" + xname + ".png";
                                    if (File.Exists(wrpath))
                                    {
                                        string uniquePath = tempPath + "\\" + Path.GetFileNameWithoutExtension(f) + " unique";
                                        if (!Directory.Exists(uniquePath))
                                        {
                                            Directory.CreateDirectory(uniquePath);
                                        }
                                        wrpath = uniquePath + "\\" + xname + ".png";
                                    }
                                    Icons.WritePng(wrpath, bitmapS);
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        PrList.AppendText("Иконка сохранена по адресу: " + wrpath + "\n");
                                        PrList.ScrollToEnd();
                                    });
                                }
                            }
                        }

                        k++;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PrText.Text = "Запрашиваем сохранение";
                        PrList.AppendText("Запрашиваем сохрание" + "\n");
                        PrList.ScrollToEnd();
                        PrBar.Value = 100;
                        TTI.ProgressValue = PrBar.Value / 100;
                    });

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();     
                    saveFileDialog1.Title = "Сохранить результат";
                    saveFileDialog1.CheckPathExists = true;
                    saveFileDialog1.DefaultExt = "zip";
                    saveFileDialog1.FileName = unixTimestamp + ".zip";
                    saveFileDialog1.Filter = "Zip-архив (*.zip)|*.zip";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;
                    if (saveFileDialog1.ShowDialog() == true)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PrText.Text = "Создаём архив...";
                            PrList.AppendText("Создаём архив...: " + "\n");
                            PrList.ScrollToEnd();
                        });
                        try
                        {
                            ZipFile.CreateFromDirectory(tempPath, saveFileDialog1.FileName, CompressionLevel.Optimal, false);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PrText.Text = "Работа приложения успешно завершена :)";
                                PrList.AppendText("Архив сохранён: " + saveFileDialog1.FileName + "\n");
                                PrList.ScrollToEnd();
                                TTI.ProgressValue = 0;
                            });
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                PrText.Text = "Ошибка создания архива...";
                                PrList.AppendText("Ошибка создания архива: \n" + ex.ToString());
                                PrList.ScrollToEnd();
                                TTI.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                                TTI.ProgressValue = 0;
                            });
                        }
                        
                    } else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PrText.Text = "Сохранение было отменено.. :(";
                            PrList.AppendText("Сохранение было отменено.. :(" + "\n");
                            PrList.ScrollToEnd();
                            PrBar.Value = 100;
                            TTI.ProgressValue = 0;
                        });
                    }
                    

                });
                
            };

        }
    }
}
