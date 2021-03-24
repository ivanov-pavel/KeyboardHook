using System;
using System.Threading;

namespace KeyboardHook
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			foreach (var filter in USBPcapLib.USBPcapClient.find_usbpcap_filters())
			{
				Console.WriteLine(filter);
				Console.WriteLine(USBPcapLib.USBPcapClient.enumerate_print_usbpcap_interactive(filter));
			}
			var usbClient = new USBPcapLib.USBPcapClient(@"\\.\USBPcap2", 3);
			usbClient.DataRead += OnDataRead;
			usbClient.start_capture();
			while (true)
			{
				Thread.Sleep(1);
			}
		}

		private static void OnDataRead(object sender, USBPcapLib.DataEventArgs e)
		{
			Console.WriteLine(BitConverter.ToString(e.Data));
			if ((e.Data.Length >= 3) && (e.Data[2] != 0))
			{
				Console.WriteLine(e.Data[2]);
			}
		}
	}
}
