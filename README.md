<img src="logo.png" width="80" alt="AppsIndexator Logo" />

# AppsIndexator

[![Language](https://img.shields.io/badge/Language-C%23-239120.svg?logo=csharp&logoColor=white)](#overview)
[![Framework](https://img.shields.io/badge/Framework-.NET%20Framework%204.7.2-512BD4.svg?logo=dotnet&logoColor=white)](#overview)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010-0078D6.svg?logo=windows&logoColor=white)](#disclaimer--legacy-notice)
[![APIs](https://img.shields.io/badge/APIs-Win32%20%7C%20Shell32%20%7C%20GDI32-lightgrey.svg)](#architecture)
[![Companion](https://img.shields.io/badge/Companion-FoxDock%20Ecosystem-ff69b4.svg)](https://github.com/resonaura/foxdock)
[![Status](https://img.shields.io/badge/Status-Historical%20Archive%20(2019)-yellow.svg)](#disclaimer--legacy-notice)

**AppsIndexator** is an automated Windows desktop utility built with C# and WPF that indexes all installed software on the system, traverses Start Menu shortcuts, and extracts high-resolution application icons directly from executables and DLLs using native Windows Shell APIs.

---

> [!WARNING]
> ### Disclaimer & Legacy Notice
> Developed in **September 2019** for Windows 10 and .NET Framework 4.7.2. Archived for historical and technical reference. It has not been tested or updated for Windows 11.

---

## 💡 Origin & Purpose

As part of developing [FoxDock](https://github.com/resonaura/foxdock) and the FoxDock IconPack Creator, there was a frequent need to gather large sets of application icons from installed programs to create rich dock theme packs. Doing this manually was tedious.

AppsIndexator was written to fully automate this workflow: scanning registry hives, resolving nested shortcut trees, extracting embedded PE resources, and converting native icon handles (`HICON`) into clean, transparent PNG image files organized in a temporary staging directory.

---

## 🛠️ Architecture

The utility combines registry traversal with low-level Win32 P/Invoke wrappers:

- **Registry Enumeration (`MainWindow.xaml.cs`)**:
  - Scans both 64-bit and 32-bit registry paths (`SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall` and `SOFTWARE\Wow6432Node\...`).
  - Reads `DisplayName`, `InstallLocation`, and associated primary `.exe` binaries.
- **Start Menu Shortcut Resolver (`FileTools.cs`)**:
  - Scans user (`%APPDATA%`) and system-wide (`%PROGRAMDATA%`) Start Menu shortcut directories (`*.lnk`).
  - Resolves target paths and arguments using the Windows Shell COM object (`WScript.Shell`).
- **Native Icon Extraction (`Icons.cs`, `API/Shell32.cs`, `API/GDI32.cs`)**:
  - Uses `SHGetFileInfo` and `ExtractIconEx` from `shell32.dll` to read high-resolution icon groups.
  - Converts native device-independent bitmaps (`HBITMAP`) to WPF `BitmapSource` via `Imaging.CreateBitmapSourceFromHBitmap`.
  - Serializes transparent icons into PNG files using `PngBitmapEncoder`.
- **WPF Taskbar Integration**:
  - Leverages `TaskbarItemInfo` to reflect live batch indexing progress and state directly in the Windows taskbar icon.

---

## 🔬 Historical Prototypes & Experiments

- **[`experiments/shicons/`](experiments/shicons/)**: The initial proof-of-concept created in August 2019 testing the extraction of raw icon indices from Windows system dynamic link libraries (`shell32.dll`).

---

## 📦 Project Structure

```
apps-indexator/
├── AppsIndexator.sln           # Visual Studio solution
├── AppsIndexator/
│   ├── MainWindow.xaml         # WPF progress and logging interface
│   ├── MainWindow.xaml.cs      # Core indexing pipeline & background tasks
│   ├── Icons.cs                # High-res icon extraction and PNG writer
│   ├── FileTools.cs            # Lnk shortcut resolution & path utilities
│   ├── API/
│   │   ├── Shell32.cs          # Shell32 P/Invoke definitions
│   │   ├── Win32.cs            # Win32 user/kernel P/Invoke signatures
│   │   └── GDI32.cs            # GDI32 graphic device context interop
│   ├── icon.ico                # Application icon
│   └── AppsIndexator.csproj    # Project file (.NET Framework 4.7.2)
├── experiments/
│   └── shicons/                # Shell32 icon extraction research prototype
└── logo.png                    # Project branding
```
