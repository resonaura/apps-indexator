using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AppsIndexator.API
{
    public class Win32
    {
        public const uint WM_COMMAND = 273U;
        public const uint WM_CLOSE = 16U;
        public const uint WM_SYSCOMMAND = 274U;
        public const int SHIL_LARGE = 0;
        public const int SHIL_SMALL = 1;
        public const int SHIL_EXTRALARGE = 2;
        public const int SHIL_SYSSMALL = 3;
        public const int SHIL_JUMBO = 4;
        public const uint ERROR_SUCCESS = 0U;
        public const uint ERROR_MORE_DATA = 234U;
        public const int CURSOR_HIDDEN = 0;
        public const int CURSOR_SHOWING = 1;
        public const int CURSOR_SUPPRESSED = 2;




        [DllImport("netapi32.dll")]
        public static extern int NetServerEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, out IntPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, Win32.SV_TYPE servertype, [MarshalAs(UnmanagedType.LPWStr)] string domain, IntPtr resume_handle);

        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr buffer);




        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo(out Win32.PERFORMANCE_INFORMATION pPerformanceInformation, [In] int cb);






        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public Win32.RECT rcImage;
        }

        public struct RECT
        {
        }

        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static implicit operator Point(Win32.POINT p)
            {
                return new Point(p.x, p.y);
            }

            public static implicit operator Win32.POINT(Point p)
            {
                return new Win32.POINT(p.X, p.Y);
            }
        }


        public enum NET_API_STATUS : uint
        {
            NERR_Success = 0U,
            ERROR_ACCESS_DENIED = 5U,
            ERROR_NOT_ENOUGH_MEMORY = 8U,
            ERROR_NOT_SUPPORTED = 50U,
            ERROR_INVALID_PARAMETER = 87U,
            ERROR_INVALID_NAME = 123U,
            ERROR_INVALID_LEVEL = 124U,
            ERROR_MORE_DATA = 234U,
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219U,
            NERR_ServerNotStarted = 2114U,
            NERR_RemoteErr = 2127U,
            NERR_WkstaNotStarted = 2138U,
            NERR_ServiceNotInstalled = 2184U,
            NERR_BadPassword = 2203U,
            NERR_UserNotFound = 2221U,
            NERR_NotPrimary = 2226U,
            NERR_SpeGroupOp = 2234U,
            NERR_PasswordTooShort = 2245U,
            NERR_InvalidComputer = 2351U,
            NERR_LastAdmin = 2452U,
            ERROR_NO_BROWSER_SERVERS_FOUND = 6118U,
            RPC_E_REMOTE_DISABLED = 2147549468U,
            RPC_S_SERVER_UNAVAILABLE = 2147944122U,
        }

        public enum SV_TYPE : uint
        {
            SV_TYPE_WORKSTATION = 1U,
            SV_TYPE_SERVER = 2U,
            SV_TYPE_SQLSERVER = 4U,
            SV_TYPE_DOMAIN_CTRL = 8U,
            SV_TYPE_DOMAIN_BAKCTRL = 16U,
            SV_TYPE_TIME_SOURCE = 32U,
            SV_TYPE_AFP = 64U,
            SV_TYPE_NOVELL = 128U,
            SV_TYPE_DOMAIN_MEMBER = 256U,
            SV_TYPE_PRINTQ_SERVER = 512U,
            SV_TYPE_DIALIN_SERVER = 1024U,
            SV_TYPE_SERVER_UNIX = 2048U,
            SV_TYPE_XENIX_SERVER = 2048U,
            SV_TYPE_NT = 4096U,
            SV_TYPE_WFW = 8192U,
            SV_TYPE_SERVER_MFPN = 16384U,
            SV_TYPE_SERVER_NT = 32768U,
            SV_TYPE_POTENTIAL_BROWSER = 65536U,
            SV_TYPE_BACKUP_BROWSER = 131072U,
            SV_TYPE_MASTER_BROWSER = 262144U,
            SV_TYPE_DOMAIN_MASTER = 524288U,
            SV_TYPE_SERVER_OSF = 1048576U,
            SV_TYPE_SERVER_VMS = 2097152U,
            SV_TYPE_WINDOWS = 4194304U,
            SV_TYPE_DFS = 8388608U,
            SV_TYPE_CLUSTER_NT = 16777216U,
            SV_TYPE_TERMINALSERVER = 33554432U,
            SV_TYPE_CLUSTER_VS_NT = 67108864U,
            SV_TYPE_DCE = 268435456U,
            SV_TYPE_ALTERNATE_XPORT = 536870912U,
            SV_TYPE_LOCAL_LIST_ONLY = 1073741824U,
            SV_TYPE_DOMAIN_ENUM = 2147483648U,
            SV_TYPE_ALL = 4294967295U,
        }

        public struct SERVER_INFO_101
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint sv101_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sv101_name;
            [MarshalAs(UnmanagedType.U4)]
            public uint sv101_version_major;
            [MarshalAs(UnmanagedType.U4)]
            public uint sv101_version_minor;
            [MarshalAs(UnmanagedType.U4)]
            public uint sv101_type;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sv101_comment;
        }

        public struct SERVER_INFO_100
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string sv100_name;
        }

        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        public struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public Win32.POINT ptScreenPos;
        }

        public enum TernaryRasterOperations : uint
        {
            BLACKNESS = 66U,
            NOTSRCERASE = 1114278U,
            NOTSRCCOPY = 3342344U,
            SRCERASE = 4457256U,
            DSTINVERT = 5570569U,
            PATINVERT = 5898313U,
            SRCINVERT = 6684742U,
            SRCAND = 8913094U,
            MERGEPAINT = 12255782U,
            MERGECOPY = 12583114U,
            SRCCOPY = 13369376U,
            SRCPAINT = 15597702U,
            PATCOPY = 15728673U,
            PATPAINT = 16452105U,
            WHITENESS = 16711778U,
            CAPTUREBLT = 1073741824U,
            NOMIRRORBITMAP = 2147483648U,
        }

        public struct PERFORMANCE_INFORMATION
        {
            public int cb;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }
        public static string GetSysTheme()
        {
            //Получаем из реестра тему
            var wpReg = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", false);
            string theme = wpReg.GetValue("SystemUsesLightTheme").ToString();

            //Закрываем работу с реестра
            wpReg.Close();

            return theme;
        }
        private static string lastTelegramNotify = "0";
        public static string GetTelegramNotifyCount(string fullwname)
        {
            try
            {
                if (fullwname.Split(' ').Length > 1)
                {
                    string rightPart = fullwname.Split(' ')[1];
                    if (rightPart != "" && rightPart != null)
                    {
                        string count = (rightPart.Split(')')[0]).Split('(')[1];
                        if (count != "" && count != null)
                        {
                            lastTelegramNotify = count;
                            return count;
                        }
                        else
                        {
                            return lastTelegramNotify;
                        }
                    }
                    else
                    {
                        return lastTelegramNotify;
                    }
                }
                return lastTelegramNotify;
            }
            catch (Exception)
            {
                Debug.WriteLine("Telegram get count error...");
                return lastTelegramNotify;
            }
        }


    }
}
