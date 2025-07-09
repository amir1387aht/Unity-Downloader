using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace Unity_Downloader
{
    internal class Utilities
    {
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string GetHumanReadableFileSize(long byteCount)
        {
            if (byteCount < 1024 * 1024)
                return (byteCount / 1024.0).ToString("F2") + " KB";
            else if (byteCount < 1024 * 1024 * 1024)
                return (byteCount / 1024.0 / 1024.0).ToString("F2") + " MB";
            else
                return (byteCount / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " GB";
        }

        public static void LocateItem(string sourceFilePath, UnityReleaseModule selectedModule, Action<Exception> OnInstallFinish)
        {
            try
            {
                if (selectedModule.ExtractedPathRename != null)
                {
                    selectedModule.ExtractedPathRename.From = MainForm.AppendUnityPath(selectedModule.ExtractedPathRename.From);
                    selectedModule.ExtractedPathRename.To = MainForm.AppendUnityPath(selectedModule.ExtractedPathRename.To);
                    HandleExtractedPathRename(sourceFilePath, selectedModule.ExtractedPathRename, OnInstallFinish);
                }
                else if (selectedModule.Destination != null)
                {
                    selectedModule.Destination = MainForm.AppendUnityPath(selectedModule.Destination);
                    MoveFileToDestination(sourceFilePath, selectedModule.Destination, OnInstallFinish);
                }
                else
                {
                    OnInstallFinish?.Invoke(new Exception($"No valid destination for {selectedModule.Name}"));
                }
            }
            catch (Exception ex)
            {
                OnInstallFinish?.Invoke(ex);
            }
        }

        private static void HandleExtractedPathRename(string sourceFilePath, ExtractedPathRename exRename, Action<Exception> OnInstallFinish)
        {
            try
            {
                Directory.CreateDirectory(exRename.To);

                string tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(sourceFilePath));
                if (Directory.Exists(tempExtractPath))
                    Directory.Delete(tempExtractPath, true);

                if (string.Equals(Path.GetExtension(sourceFilePath), ".zip", StringComparison.OrdinalIgnoreCase))
                {
                    ExtractZipFile(sourceFilePath, tempExtractPath);

                    string expectedRoot = Path.GetFileName(exRename.From.TrimEnd(Path.DirectorySeparatorChar));
                    string[] extractedDirs = Directory.GetDirectories(tempExtractPath);

                    string srcRoot = (extractedDirs.Length == 1 && Path.GetFileName(extractedDirs[0]) == expectedRoot)
                        ? extractedDirs[0]
                        : tempExtractPath;

                    CopyDirectoryRecursively(srcRoot, exRename.To);
                }
                else
                {
                    string dest = Path.Combine(exRename.To, Path.GetFileName(sourceFilePath));
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.Copy(sourceFilePath, dest, true);
                    File.Delete(sourceFilePath);

                    if (string.Equals(Path.GetExtension(dest), ".exe", StringComparison.OrdinalIgnoreCase))
                        if (!RunProccess(dest, out string output)) throw new Exception(output);

                    OnInstallFinish?.Invoke(null);
                    return;
                }

                Directory.Delete(tempExtractPath, true);
                OnInstallFinish?.Invoke(null);
            }
            catch (Exception ex)
            {
                OnInstallFinish?.Invoke(ex);
            }
        }

        private static void CopyDirectoryRecursively(string sourceDir, string destDir)
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
            }

            foreach (string filePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string targetPath = filePath.Replace(sourceDir, destDir);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                File.Copy(filePath, targetPath, true);
            }
        }

        private static void MoveFileToDestination(string sourceFilePath, string destinationPath, Action<Exception> OnInstallFinish)
        {
            try
            {
                if (string.Equals(Path.GetExtension(sourceFilePath), ".zip", StringComparison.OrdinalIgnoreCase))
                {
                    if (Directory.Exists(destinationPath))
                        Directory.Delete(destinationPath, true);

                    ExtractZipFile(sourceFilePath, destinationPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                    string dest = Path.Combine(destinationPath, Path.GetFileName(sourceFilePath));
                    File.Copy(sourceFilePath, dest, true);

                    if (string.Equals(Path.GetExtension(dest), ".exe", StringComparison.OrdinalIgnoreCase))
                        if (!RunProccess(dest, out string output)) throw new Exception(output);
                }

                OnInstallFinish?.Invoke(null);
            }
            catch (Exception ex)
            {
                OnInstallFinish?.Invoke(ex);
            }
        }

        public static bool RunProccess(string filePath, out string output)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { Verb = "runas" });
                output = "";
                return true;
            }
            catch (Exception ex)
            {
                output = $"Error running \"{filePath}\": {ex.Message}";
                return false;
            }
        }

        public static void SetDNS(string CombinedDNS)
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo["Description"].ToString() == GetConnectedNetwork())
                {
                    var newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = string.IsNullOrEmpty(CombinedDNS) ? null : CombinedDNS.Split(',');
                    mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                }
            }
        }

        public static string GetDNS()
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo["Description"].ToString() == GetConnectedNetwork())
                {
                    var dns = (string[])mo["DNSServerSearchOrder"];
                    return dns != null ? string.Join(",", dns) : "";
                }
            }
            return "";
        }

        public static string GetConnectedNetwork()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up && ni.GetIPProperties().GatewayAddresses.Count > 0)
                    return ni.Description;
            }
            return "";
        }

        public static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        }
    }
}
