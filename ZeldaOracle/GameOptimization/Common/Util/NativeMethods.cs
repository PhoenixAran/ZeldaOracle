﻿using System;
using System.Runtime.InteropServices;

namespace ZeldaOracle.Common.Util {
	/// <summary>A class for storing native P/Invoke methods.</summary>
	public static class NativeMethods {

		/// <summary>The standard output device. Initially, this is the active
		/// console screen buffer, CONOUT$.</summary>
		public const uint StdOutputHandle = 0xFFFFFFF5;

		public static readonly IntPtr DefaultStdOut = new IntPtr(7);

		/// <summary>Allocates a new console for the calling process.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		/// <summary>Detaches the calling process from its console.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		/// <summary>Retrieves a handle to the specified standard device
		/// (standard input, standard output, or standard error).</summary>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(uint nStdHandle);

		/// <summary>Sets the handle for the specified standard device
		/// (standard input, standard output, or standard error).</summary>
		[DllImport("kernel32.dll")]
		public static extern void SetStdHandle(uint nStdHandle, IntPtr hHandle);

		/// <summary>Sets the title for the current console window.</summary>
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleTitle(string lpConsoleTitle);
	}
}
