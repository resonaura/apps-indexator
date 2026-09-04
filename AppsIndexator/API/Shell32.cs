using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using Shell32;
using System.IO;
using System.Drawing.Imaging;
using static System.Environment;
using System.Text;

namespace AppsIndexator.API
{
    class Shell32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;

            public SHFILEINFO(bool b)
            {
                this.hIcon = IntPtr.Zero;
                this.iIcon = 0;
                this.dwAttributes = 0U;
                this.szDisplayName = "";
                this.szTypeName = "";
            }
        }

        [Flags]
        public enum SHGFI
        {
            SHGFI_ICON = 256,
            SHGFI_DISPLAYNAME = 512,
            SHGFI_TYPENAME = 1024,
            SHGFI_ATTRIBUTES = 2048,
            SHGFI_ICONLOCATION = 4096,
            SHGFI_EXETYPE = 8192,
            SHGFI_SYSICONINDEX = 16384,
            SHGFI_LINKOVERLAY = 32768,
            SHGFI_SELECTED = 65536,
            SHGFI_ATTR_SPECIFIED = 131072,
            SHGFI_LARGEICON = 0,
            SHGFI_SMALLICON = 1,
            SHGFI_OPENICON = 2,
            SHGFI_SHELLICONSIZE = 4,
            SHGFI_PIDL = 8,
            SHGFI_USEFILEATTRIBUTES = 16,
            SHGFI_ADDOVERLAYS = 32,
            SHGFI_OVERLAYINDEX = 64,
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);

        [DllImport("shell32.dll", EntryPoint = "#727")]
        public static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

        /// <summary> 
        /// Get an environment folder path for Windows environment folders 
        /// </summary> 
        /// <returns>A string pointing to the special path</returns> 
        /// <remarks></remarks> 
        public static string GetSFPath(SpecialFolder folder)
        {
            StringBuilder lpszPath = new StringBuilder(260);
            API.Shell32.SHGetFolderPath(IntPtr.Zero, (int)folder, IntPtr.Zero, 0, lpszPath);
            return lpszPath.ToString();
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [ComImport]
        public interface IImageList
        {
            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int ReplaceIcon(int i, IntPtr hicon, ref int pi);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int SetOverlayImage(int iImage, int iOverlay);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int AddMasked(IntPtr hbmImage, int crMask, ref int pi);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Draw(ref Win32.IMAGELISTDRAWPARAMS pimldp);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Remove(int i);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetIcon(int i, int flags, ref IntPtr picon);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetImageInfo(int i, ref Win32.IMAGEINFO pImageInfo);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int Clone(ref Guid riid, ref IntPtr ppv);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetImageRect(int i, ref Win32.RECT prc);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetIconSize(ref int cx, ref int cy);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int SetIconSize(int cx, int cy);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetImageCount(ref int pi);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int SetImageCount(int uNewCount);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int SetBkColor(int clrBk, ref int pclr);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetBkColor(ref int pclr);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int EndDrag();

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int DragEnter(IntPtr hwndLock, int x, int y);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int DragLeave(IntPtr hwndLock);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int DragMove(int x, int y);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int DragShowNolock(int fShow);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetDragImage(ref Win32.POINT ppt, ref Win32.POINT pptHotspot, ref Guid riid, ref IntPtr ppv);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetItemFlags(int i, ref int dwFlags);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetOverlayImage(int iOverlay, ref int piIndex);
        }
        /// <summary>
        /// Функция для получения кол-ва элементов Корзины
        /// </summary>
        /// <returns></returns>
        public static int TrashCount()
        {
            try
            {
                Shell shell = new Shell();
                Folder recycleBin = shell.NameSpace(10);
                return recycleBin.Items().Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
