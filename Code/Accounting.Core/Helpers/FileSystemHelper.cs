using System;
using System.Runtime.InteropServices;

namespace Accounting.Core.Helpers
{
	public static class FileSystemHelper
	{
		public static void ShellOpen(this string file)
		{
			ShellExecuteInfo lpExecInfo = new ShellExecuteInfo();
			lpExecInfo.Size = Marshal.SizeOf(lpExecInfo);
			lpExecInfo.File = file;
			lpExecInfo.Show = 1U;
			if (ShellExecuteEx(ref lpExecInfo))
				return;
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 1223)
				throw Marshal.GetExceptionForHR(lastWin32Error);
		}

		[DllImport("shell32.dll", SetLastError = true)]
		private static extern bool ShellExecuteEx(ref ShellExecuteInfo lpExecInfo);

		[Serializable]
		private struct ShellExecuteInfo
		{
			public int Size;
			public uint Mask;
			public IntPtr hwnd;
			public string Verb;
			public string File;
			public string Parameters;
			public string Directory;
			public uint Show;
			public IntPtr InstApp;
			public IntPtr IDList;
			public string Class;
			public IntPtr hkeyClass;
			public uint HotKey;
			public IntPtr Icon;
			public IntPtr Monitor;
		}
	}
}
