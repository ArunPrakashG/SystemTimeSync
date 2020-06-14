using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace SystemTimeSync
{
	public struct SystemTime
	{
		public ushort Year;
		public ushort Month;
		public ushort DayOfWeek;
		public ushort Day;
		public ushort Hour;
		public ushort Minute;
		public ushort Second;
		public ushort Millisecond;
	};

	public partial class ServiceCore : ServiceBase
	{
		public ServiceCore()
		{
			InitializeComponent();
		}

		[DllImport("kernel32.dll", EntryPoint = "GetSystemTime", SetLastError = true)]
		public static extern void Win32GetSystemTime(ref SystemTime sysTime);

		[DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
		public static extern bool Win32SetSystemTime(ref SystemTime sysTime);

		public static DateTime GetNetworkTime()
		{
			const byte serverReplyTime = 40;
			const string ntpServer = "time.windows.com";
			byte[] ntpData = new byte[48];
			ntpData[0] = 0x1B;
			IPAddress[] addresses = Dns.GetHostEntry(ntpServer).AddressList;
			IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);

			using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
			{
				socket.Connect(ipEndPoint);
				socket.ReceiveTimeout = 3000;
				socket.Send(ntpData);
				socket.Receive(ntpData);
				socket.Close();
			}

			ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
			ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
			intPart = SwapEndianness(intPart);
			fractPart = SwapEndianness(fractPart);
			ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
			DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
			return networkDateTime.ToLocalTime();
		}

		private static uint SwapEndianness(ulong x)
		{
			return (uint)(((x & 0x000000ff) << 24) +
						   ((x & 0x0000ff00) << 8) +
						   ((x & 0x00ff0000) >> 8) +
						   ((x & 0xff000000) >> 24));
		}

		private bool IsNetworkConnected()
		{
			using (Ping ping = new Ping())
			{
				PingReply reply = ping.Send("8.8.8.8", 3000);
				return reply.Status == IPStatus.Success;
			}
		}

		protected override void OnStart(string[] args)
		{
			if (!IsNetworkConnected())
			{
				EventLog.WriteEntry("Network connection is not available to sync.", EventLogEntryType.Error);
				return;
			}

			DateTime receivedTime = GetNetworkTime();
			SystemTime updatedTime = new SystemTime
			{
				Year = (ushort)receivedTime.Year,
				Month = (ushort)receivedTime.Month,
				Day = (ushort)receivedTime.Day,
				Hour = (ushort)receivedTime.Hour,
				Minute = (ushort)receivedTime.Minute,
				Second = (ushort)receivedTime.Second,
				DayOfWeek = (ushort)receivedTime.DayOfWeek,
				Millisecond = (ushort)receivedTime.Millisecond
			};
			
			if(Win32SetSystemTime(ref updatedTime))
			{
				EventLog.WriteEntry("Sync completed.", EventLogEntryType.Information);
				return;
			}

			EventLog.WriteEntry("Sync failed.", EventLogEntryType.Warning);
		}

		protected override void OnStop()
		{
		}
	}
}
