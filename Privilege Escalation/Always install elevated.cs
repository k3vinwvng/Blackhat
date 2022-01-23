using System;
using Microsoft.Win32;

public static void AlwaysInstallElevated()
        {
            Console.WriteLine("AlwaysInstallElevated keys");

            string AlwaysInstallElevatedHKLM = GetRegValue("HKLM", "Software\\Policies\\Microsoft\\Windows\\Installer", "AlwaysInstallElevated");
            string AlwaysInstallElevatedHKCU = GetRegValue("HKCU", "Software\\Policies\\Microsoft\\Windows\\Installer", "AlwaysInstallElevated");

            if (!string.IsNullOrEmpty(AlwaysInstallElevatedHKLM))
            {
                Console.WriteLine("  HKLM:    {0}", AlwaysInstallElevatedHKLM);
            }

            if (!string.IsNullOrEmpty(AlwaysInstallElevatedHKCU))
            {
                Console.WriteLine("  HKCU:    {0}", AlwaysInstallElevatedHKCU);
            }
            else
            {
                Console.WriteLine("No registry keys found");
            }
        
