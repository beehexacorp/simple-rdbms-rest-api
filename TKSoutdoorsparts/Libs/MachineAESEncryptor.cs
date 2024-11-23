using System.Runtime.InteropServices;
using System;
using System.Diagnostics;


public static class MachineAESEncryptorExtensions
{
    public static string GetMachineGuid()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using var registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            return registryKey?.GetValue("MachineGuid")?.ToString() ?? throw new Exception("MachineGuid not found.");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return System.IO.File.ReadAllText("/etc/machine-id").Trim();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ioreg",
                Arguments = "-rd1 -c IOPlatformExpertDevice | grep IOPlatformUUID",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (string.IsNullOrWhiteSpace(output))
                {
                    throw new Exception("Failed to retrieve Hardware UUID.");
                }

                int start = output.IndexOf("\"") + 1;
                int end = output.LastIndexOf("\"");
                return output.Substring(start, end - start);
            }
        }
        throw new PlatformNotSupportedException($"{RuntimeInformation.OSDescription} is not supported.");
    }

    public static byte[] EncryptAES(this string plainText)
    {
        return plainText.EncryptAES(GetMachineGuid());
    }

    public static string DecryptAES(this byte[] cipher)
    {
        return cipher.DecryptAES(GetMachineGuid());
    }

}
