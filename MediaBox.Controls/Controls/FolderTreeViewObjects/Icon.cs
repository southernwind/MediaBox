using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects {
	/// <summary>
	/// アイコン生成
	/// </summary>
	internal static class IconUtility {
		private const string Guid = "46EB5926-582E-4017-9FDF-E8998DAA0950";
		private static readonly IImageList _imageList;

		static IconUtility() {
			var guid = new Guid(Guid);
			SHGetImageList(SHIL_SMALL, ref guid, out _imageList);
		}

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttribs, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

		[DllImport("shell32.dll", EntryPoint = "#727")]
		private static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

		private enum SHGFI {
			SHGFI_ICON = 0x000000100,
			SHGFI_DISPLAYNAME = 0x000000200,
			SHGFI_TYPENAME = 0x000000400,
			SHGFI_ATTRIBUTES = 0x000000800,
			SHGFI_ICONLOCATION = 0x000001000,
			SHGFI_EXETYPE = 0x000002000,
			SHGFI_SYSICONINDEX = 0x000004000,
			SHGFI_LINKOVERLAY = 0x000008000,
			SHGFI_SELECTED = 0x000010000,
			SHGFI_ATTR_SPECIFIED = 0x000020000,
			SHGFI_LARGEICON = 0x000000000,
			SHGFI_SMALLICON = 0x000000001,
			SHGFI_OPENICON = 0x000000002,
			SHGFI_SHELLICONSIZE = 0x000000004,
			SHGFI_PIDL = 0x000000008,
			SHGFI_USEFILEATTRIBUTES = 0x000000010,
			SHGFI_ADDOVERLAYS = 0x000000020,
			SHGFI_OVERLAYINDEX = 0x000000040
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SHFILEINFO {
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		// アイコンサイズ
#pragma warning disable IDE0051 // Remove unused private members
		private const int SHIL_LARGE = 0;   // normally 32x32
		private const int SHIL_SMALL = 1;   // normally 16x16
		private const int SHIL_EXTRALARGE = 2;
		private const int SHIL_SYSSMALL = 3;   // like SHIL_SMALL, but tracks system small icon metric correctly
		private const int SHIL_LAST = SHIL_SYSSMALL;
#pragma warning restore IDE0051 // Remove unused private members

		[StructLayout(LayoutKind.Sequential)]
		private struct IMAGELISTDRAWPARAMS {
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

		[Flags]
		private enum ImageListDrawItemConstants {
			ILD_NORMAL = 0x0,
			ILD_TRANSPARENT = 0x1,
			ILD_BLEND25 = 0x2,
			ILD_SELECTED = 0x4,
			ILD_MASK = 0x10,
			ILD_IMAGE = 0x20,
			ILD_ROP = 0x40,
			ILD_OVERLAYMASK = 0xF00,
			ILD_PRESERVEALPHA = 0x1000,
			ILD_SCALE = 0x2000,
			ILD_DPISCALE = 0x4000
		}


		// interface COM IImageList
		[ComImport]
		[Guid(Guid)]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IImageList {
			[PreserveSig]
			int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);

			[PreserveSig]
			int ReplaceIcon(int i, IntPtr hicon, ref int pi);

			[PreserveSig]
			int SetOverlayImage(int iImage, int iOverlay);

			[PreserveSig]
			int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);

			[PreserveSig]
			int AddMasked(IntPtr hbmImage, int crMask, ref int pi);

			[PreserveSig]
			int Draw(ref IMAGELISTDRAWPARAMS pimldp);

			[PreserveSig]
			int Remove(int i);

			[PreserveSig]
			int GetIcon(int i, int flags, ref IntPtr picon);
		}

		/// <summary>
		/// アイコン取得
		/// </summary>
		/// <param name="path">対象のファイルもしくはフォルダ</param>
		/// <returns>生成された<see cref="BitmapSource"/></returns>
		public static BitmapSource GetIcon(string path) {
			SHGetFileInfo(path, 0, out var shinfo, (uint)Marshal.SizeOf(typeof(SHFILEINFO)), SHGFI.SHGFI_SYSICONINDEX);

			var hicon = IntPtr.Zero;
			_imageList.GetIcon(shinfo.iIcon, (int)ImageListDrawItemConstants.ILD_TRANSPARENT, ref hicon);

			return Imaging.CreateBitmapSourceFromHIcon(hicon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}

		/// <summary>
		/// <see cref="Icon"/>→<see cref="BitmapSource"/>変換
		/// </summary>
		/// <param name="icon">アイコン</param>
		/// <returns>生成された<see cref="BitmapSource"/></returns>
		public static BitmapSource ToImageSource(this Icon icon) {
			return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}
	}
}
