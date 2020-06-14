using System.ServiceProcess;

namespace SystemTimeSync
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new ServiceCore()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
