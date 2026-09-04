using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppsIndexator
{
    public static class FileTools
    {
        /// <summary>
        /// Функция для получения пути к проводнику
        /// </summary>
        /// <returns>Путь</returns>
        public static string GetExplorerPath()
        {
            return Environment.GetEnvironmentVariable("windir") + "\\explorer.exe";
        }
        /// <summary>
        /// Функция для получения пути к последним файлам
        /// </summary>
        /// <returns>Путь</returns>
        public static string GetRecentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        }
        /// <summary>
        /// Проверка файла на то, является ли он ярлыком
        /// </summary>
        /// <param name="path">Путь к ярлыку</param>
        /// <returns></returns>
        public static bool IsLink(string path)
        {

            string pathOnly = System.IO.Path.GetDirectoryName(path);
            string filenameOnly = System.IO.Path.GetFileName(path);


            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");

            Object shell = Activator.CreateInstance(shellAppType);
            Folder folder = (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { pathOnly });

            FolderItem folderItem = folder.ParseName(filenameOnly);

            if (folderItem != null)
            {
                return folderItem.IsLink;
            }
            return false; // not found
        }
        /// <summary>
        /// Получение исходного пути ярлыка
        /// </summary>
        /// <param name="shortcutFilename">Путь к ярлыку</param>
        /// <returns></returns>
        public static string GetShortcutTarget(string shortcutFilename)
        {

            string pathOnly;
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            if (File.Exists(Path.GetTempPath() + "\\" + filenameOnly))
            {
                pathOnly = Path.GetTempPath();
            }
            else
            {
                File.Copy(shortcutFilename, Path.GetTempPath() + "\\" + filenameOnly);
                pathOnly = Path.GetTempPath();
            }

            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");

            Object shell = Activator.CreateInstance(shellAppType);
            Folder folder = (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { pathOnly });

            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                if (folderItem.IsLink)
                {
                    try
                    {
                        Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                        return link.Path;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message + " beda #7");
                    }

                }
                return shortcutFilename;
            }
            return string.Empty;  // not found


        }
        /// <summary>
        /// Получение пути к приложению
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Путь</returns>
        public static string GetRealAppPath(string path)
        {
            if (IsLink(path)) //Если путь - ярлык
            {
                return GetShortcutTarget(path); //Получаем путь из ярлыка
            }
            else
            {
                return path; //В противном случае - возвращаем - путь который был
            }

        }
        /// <summary>
        /// Функция получения названия исполняемого файла из пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Название исполняемого файла</returns>
        public static string AppFromPath(string path)
        {
            string app_name = System.IO.Path.GetFileNameWithoutExtension(path); //Получаем файлнейм
            return app_name; //Возвращаем правильный файлнейм
        }
    }
}
