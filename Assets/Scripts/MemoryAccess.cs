using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class MemoryAccess
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    const int PROCESS_VM_READ = 0x0010;

    public static byte[] ReadMemory(int processId, IntPtr address, uint size)
    {
        IntPtr hProcess = OpenProcess(PROCESS_VM_READ, false, processId);

        if (hProcess == IntPtr.Zero)
        {
            throw new Exception("Failed to open process for reading.");
        }

        byte[] buffer = new byte[size];
        ReadProcessMemory(hProcess, address, buffer, size, out _);
        CloseHandle(hProcess);

        return buffer;
    }
}
