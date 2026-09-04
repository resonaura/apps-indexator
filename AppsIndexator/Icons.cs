using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace AppsIndexator
{
    class Icons
    {
        /// <summary>
        /// Функция обрезки пустого пространства из битмапа
        /// </summary>
        /// <param name="bmp">Изначальный рисунок</param>
        /// <returns>Конечный рисунок</returns>
        private static Bitmap CropWhiteSpace(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            int white = 0xffffff;

            bool allWhiteRow(int r)
            {
                for (int i = 0; i < w; ++i)
                    if ((bmp.GetPixel(i, r).ToArgb() & white) != white)
                        return false;
                return true;
            }

            bool allWhiteColumn(int c)
            {
                for (int i = 0; i < h; ++i)
                    if ((bmp.GetPixel(c, i).ToArgb() & white) != white)
                        return false;
                return true;
            }

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (!allWhiteRow(row))
                    break;
                topmost = row;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (!allWhiteRow(row))
                    break;
                bottommost = row;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (!allWhiteColumn(col))
                    break;
                leftmost = col;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (!allWhiteColumn(col))
                    break;
                rightmost = col;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }

        /// <summary>
        /// Функция для заменты прозрачности на определённый цвет
        /// </summary>
        /// <param name="bitmap">Изначальный рисунок</param>
        /// <param name="color">Конечный рисунок</param>
        /// <returns></returns>
        static Bitmap Process(Bitmap bitmap, Color color)
        {
            Bitmap temp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(temp);
            g.Clear(color);
            g.DrawImage(bitmap, Point.Empty);
            return temp;
        }
        private static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
        /// <summary>
        /// Функция для оптимизации рисунка
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Optimize(Bitmap bmp)
        {
            return ResizeBitmap(bmp, 128, 128);
        }
        public static BitmapSource GetSourceFromBitmap(Bitmap bitmap)
        {
            BitmapSource result = null;
            if (bitmap != null)
            {
                IntPtr hbmp = bitmap.GetHbitmap();
                result = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                API.GDI32.DeleteObject(hbmp);
            }
            return result;
        }
        public static BitmapSource GetSourceFromIcon(Icon icon)
        {
            BitmapSource result = null;
            if (icon != null)
            {
                result = GetSourceFromBitmap(Optimize(icon.ToBitmap()));
            }
            return result;
        }
        public static void WritePng(string fileName, BitmapSource bmp)
        {

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            BitmapFrame outputFrame = BitmapFrame.Create(bmp);
            encoder.Frames.Add(outputFrame);

            using (FileStream file = File.OpenWrite(fileName))
            {
                encoder.Save(file);
            }
        }
        public static BitmapSource SourceFromPath(string path)
        {
            API.Shell32.SHFILEINFO psfi = new API.Shell32.SHFILEINFO();
            int dwFileAttributes = 2048;
            API.Shell32.SHGFI uFlags = API.Shell32.SHGFI.SHGFI_SYSICONINDEX;
            if (API.Shell32.SHGetFileInfo(path, dwFileAttributes, out psfi, (uint)Marshal.SizeOf((object)psfi), uFlags) == 0)
            {
                return (BitmapSource)null;
            }

            int i = psfi.iIcon;

            return GetSourceFromIcon(GetShellIcon(i));
        }
        private static API.Shell32.IImageList GetShellList(int id)
        {
            Guid riid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            API.Shell32.SHGetImageList(id, ref riid, out API.Shell32.IImageList ppv);

            return ppv;
        }
        public static Icon GetShellIcon(int i)
        {
            try
            {
                API.Shell32.IImageList ppv = GetShellList(4);
                IntPtr picon = IntPtr.Zero;
                int flags = 0;

                ppv.GetIcon(i, flags, ref picon);
                if (picon != null && System.Drawing.Icon.FromHandle(picon) != null)
                {
                    Icon icon = (Icon)System.Drawing.Icon.FromHandle(picon).Clone();
                    double realwidth = CropWhiteSpace(Process(icon.ToBitmap(), Color.FromArgb(255, 255, 255, 255))).Width;
                    if (realwidth <= 100)
                    {
                        API.Shell32.IImageList ppv_low = GetShellList(2);
                        IntPtr picon2 = IntPtr.Zero;

                        ppv_low.GetIcon(i, flags, ref picon2);
                        if (picon2 != null && System.Drawing.Icon.FromHandle(picon2) != null)
                        {
                            icon = (Icon)System.Drawing.Icon.FromHandle(picon2).Clone();
                        }
                    }

                    return icon;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                //Если таки ошибка - выводим её в консоль
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
