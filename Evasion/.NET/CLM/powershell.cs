using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Collections.ObjectModel;

public class Powershell
{
    public static void Main()
    {
        PowerShell ps1 = PowerShell.Create();
        ps1.AddCommand("Get-Process");
        Collection<PSObject> PSOutput = ps2.Invoke();
        foreach (PSObject outputItem in PSOutput)
        {
            if (outputItem != null)
            {
                Console.WriteLine(outputItem);
            }
        }


    }
}
