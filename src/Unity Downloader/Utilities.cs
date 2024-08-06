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
            // Check If App Running On Administrator
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string GetHumanReadableFileSize(long byteCount)
        {
            // Get Human Readable file size
            if (byteCount < 1024 * 1024) // less than 1 MB
                return (byteCount / 1024.0).ToString("F2") + " KB";
            else if (byteCount < 1024 * 1024 * 1024) // less than 1 GB
                return (byteCount / 1024.0 / 1024.0).ToString("F2") + " MB";
            else // 1 GB or more
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

                    if (OnInstallFinish != null)
                        OnInstallFinish(null);
                }
                else if (selectedModule.Destination != null)
                {
                    selectedModule.Destination = MainForm.AppendUnityPath(selectedModule.Destination);

                    MoveFileToDestination(sourceFilePath, selectedModule.Destination, OnInstallFinish);

                    if (OnInstallFinish != null)
                        OnInstallFinish(null);
                }
                else
                {
                    if (OnInstallFinish != null)
                        OnInstallFinish(new Exception($"Not Found Valid Destination for {selectedModule.Name}.\n\nError : Destination and ExtractedPathRename Are Empty."));
                }
            }
            catch (Exception ex)
            {
                if (OnInstallFinish != null)
                    OnInstallFinish(ex);
            }
        }

        private static void HandleExtractedPathRename(string sourceFilePath, ExtractedPathRename extractedPathRename, Action<Exception> OnInstallFinish)
        {
            if (!Directory.Exists(extractedPathRename.To))
                Directory.CreateDirectory(extractedPathRename.To);

            string tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(sourceFilePath));

            if (Directory.Exists(tempExtractPath)) Directory.Delete(tempExtractPath, true);

            if (Path.GetExtension(sourceFilePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                ExtractZipFile(sourceFilePath, tempExtractPath);
            else
            {
                string destinationPath = Path.Combine(extractedPathRename.To, Path.GetFileName(sourceFilePath));

                File.Move(sourceFilePath, destinationPath);

                if (Path.GetExtension(destinationPath) == ".exe")
                {
                    if (!RunProccess(destinationPath, out string output))
                        OnInstallFinish?.Invoke(new Exception(output));
                }

                OnInstallFinish?.Invoke(null);

                return;
            }

            string fromLastDirectory = Path.GetFileName(extractedPathRename.From.TrimEnd(Path.DirectorySeparatorChar));
            string[] extractedDirectories = Directory.GetDirectories(tempExtractPath);

            if (extractedDirectories.Length == 1 && Path.GetFileName(extractedDirectories[0]) == fromLastDirectory)
            {
                // Ignore the first directory
                string[] files = Directory.GetFiles(extractedDirectories[0], "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string relativePath = file.Substring(extractedDirectories[0].Length + 1);
                    string destinationPath = Path.Combine(extractedPathRename.To, relativePath);

                    string dirName = Path.GetDirectoryName(destinationPath);

                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

                    if (!File.Exists(destinationPath))
                        File.Move(file, destinationPath);
                }
            }
            else
            {
                // Move the entire extracted content
                Directory.Move(tempExtractPath, extractedPathRename.To);
            }

            // Clean up temporary extraction directory
            Directory.Delete(tempExtractPath, true);

            OnInstallFinish?.Invoke(null);
        }

        public static bool RunProccess(string filePath, out string output)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true,
                Verb = "runas" // Run as administrator
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        output = $"Running \"{filePath}\" encountered an error. Please try Run It Manually";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                output = $"Running \"{filePath}\" encountered an error. Please try Run It Manually, \n\n Erorr : {ex.Message}";
                return false;
            }

            output = "";
            return true;
        }

        private static void MoveFileToDestination(string sourceFilePath, string destinationPath, Action<Exception> OnInstallFinish)
        {
            if (Path.GetExtension(sourceFilePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists(destinationPath))
                    Directory.Delete(destinationPath, true);

                ExtractZipFile(sourceFilePath, destinationPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourceFilePath));

                if (!File.Exists(destinationPath))
                    File.Copy(sourceFilePath, destinationPath);

                if (Path.GetExtension(destinationPath) == ".exe")
                    if (!RunProccess(destinationPath, out string output))
                        OnInstallFinish?.Invoke(new Exception(output));
            }

            OnInstallFinish?.Invoke(null);
        }

        public static void SetDNS(string CombinedDNS)
        {
            var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var networkConfigs = networkConfigMng.GetInstances();

            string connectedNetwork = GetConnectedNetwork();

            foreach (ManagementObject networkConfig in networkConfigs)
            {
                if (networkConfig["Description"].ToString().Equals(connectedNetwork))
                {
                    ManagementBaseObject newDNS = networkConfig.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = string.IsNullOrEmpty(CombinedDNS) ? null : CombinedDNS.Split(',');

                    networkConfig.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                }
            }
        }

        public static string GetDNS()
        {
            var networkConfigMng = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var networkConfigs = networkConfigMng.GetInstances();

            string connectedNetwork = GetConnectedNetwork();

            foreach (ManagementObject networkConfig in networkConfigs)
            {
                if (networkConfig["Description"].ToString().Equals(connectedNetwork))
                {
                    var dnsServers = (string[])networkConfig["DNSServerSearchOrder"];
                    return string.Join(",", dnsServers);
                }
            }

            return "No DNS found for this interface.";
        }

        public static string GetConnectedNetwork()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface ni in interfaces)
                if (ni.OperationalStatus == OperationalStatus.Up && ni.GetIPProperties().GatewayAddresses.Count > 0)
                    return ni.Description;

            return "";
        }

        public static void ExtractZipFile(string zipFilePath, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        }
    }
}
