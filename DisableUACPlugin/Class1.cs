using Microsoft.Win32;
using System.Diagnostics;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace DisableUACPlugin
{
	public class DisableUAC : IGameLaunchingPlugin
	{
		public void OnAfterGameLaunched(IGame game, IAdditionalApplication app, IEmulator emulator)
		{

		}

		public void OnBeforeGameLaunching(IGame game, IAdditionalApplication app, IEmulator emulator)
		{
			if (emulator.Title.Contains("UAC") || game.ApplicationPath.ToLower().EndsWith(".exe") || game.ApplicationPath.ToLower().EndsWith(".lnk"))
			{
				string new_cmd = $@" /I /run /tn ""TempDisableUAC""";
				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.FileName = "schtasks";
				startInfo.Arguments = new_cmd;
				startInfo.Verb = "runas";
				startInfo.UseShellExecute = false;
				startInfo.CreateNoWindow = true;
				var TaskProcess = System.Threading.Tasks.Task.Run(() => Process.Start(startInfo));
				TaskProcess.Wait();

				RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
				int valueUAC = (int)key.GetValue("ConsentPromptBehaviorAdmin", 5);
				int i = 0;
				while(valueUAC != 0 && i<30)
				{
					i++;
					Thread.Sleep(100);
					valueUAC = (int)key.GetValue("ConsentPromptBehaviorAdmin", 5);
				}
			}
		}

		public void OnGameExited()
		{

		}
	}
}