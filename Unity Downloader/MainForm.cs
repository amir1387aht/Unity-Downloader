using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Unity_Downloader
{
    public partial class MainForm : Form
    {
        private static string UnityHubDataPath = "C:\\Users\\{0}\\AppData\\Roaming\\UnityHub";
        private static string UnityHubEditorPath = "{0}\\secondaryInstallPath.json";
        private static string UnityEditorsPath = "";
        private static string EditorModulesPath = "{0}\\modules.json";

        public static string[] AllEditors;

        private static string SelectedEditor;
        private static string SelectedItemToDownload;

        public static List<UnityReleaseModule> AllReleaseModules;

        public static string AppendUnityPath(string path) => path.Replace("{UNITY_PATH}", $"{UnityEditorsPath.Replace("\\\\", "/")}/{SelectedEditor}");

        private static UnityReleaseModule FindReleaseModule(string name) => AllReleaseModules.Find(item => item.Name == name);
        private static UnityReleaseModule GetCurrentModule() => FindReleaseModule(SelectedItemToDownload.Replace("     ", ""));

        public MainForm()
        {
            InitializeComponent();
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            UnityHubDataPath = string.Format(UnityHubDataPath, Environment.UserName);

            if (!Directory.Exists(UnityHubDataPath))
            {
                MessageBox.Show($"It Seems Unity Hub Not Installed On Your Computer, Install It And Restart App.\n\nError : \"{UnityHubDataPath}\" Not Found.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            UnityHubEditorPath = string.Format(UnityHubEditorPath, UnityHubDataPath);

            if (!File.Exists(UnityHubEditorPath))
            {
                MessageBox.Show($"It Seems You Dont Set Editors Download Path In Unity Hub Corretly, Update Path On Unity Hub And Try Again\n\nError : \"{UnityHubEditorPath}\" Not Found.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            UnityEditorsPath = File.ReadAllText(UnityHubEditorPath).Replace("\"", "");

            AllEditors = Directory.GetDirectories(UnityEditorsPath);

            if (AllEditors.Length == 0)
            {
                MessageBox.Show($"It Seems You Dont Installed Any Unity Editor, Install One And try Again.\n\nError : No Editor Folder On \"{UnityEditorsPath}\" Found.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            for (int i = 0; i < AllEditors.Length; i++)
                EditorsListSelect.Items.Add(Path.GetFileName(AllEditors[i]));

            EditorsListSelect.SelectedIndex = 0;
        }

        private void EditorsListSelect_SelectedItemChanged(object sender, EventArgs e)
        {
            SelectedEditor = EditorsListSelect.SelectedItem as string;
            EditorModulesPath = string.Format(EditorModulesPath, AllEditors[EditorsListSelect.SelectedIndex]);
            LoadAvailableItems();
        }

        private void LoadAvailableItems()
        {
            string ModulesRawFile = File.ReadAllText(EditorModulesPath);

            AllReleaseModules = JsonConvert.DeserializeObject<List<UnityReleaseModule>>(ModulesRawFile);

            var SubModels = new List<SubModule>();

            foreach (var Module in AllReleaseModules)
            {
                if (SubModels.Find(item => item.Name == Module.Name) != null) continue;

                ItemsToDownload.Items.Add(Module.Name);

                if (Module.SubModules != null)
                {
                    foreach (var SubModule in Module.SubModules)
                    {
                        ItemsToDownload.Items.Add($"     {SubModule.Name}");

                        if(SubModule.SubModules == null)
                            SubModels.Add(SubModule);
                        else if(SubModule.SubModules.Count == 0)
                            SubModels.Add(SubModule);
                    }
                }
            }
        }

        private void ItemsToDownload_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedItemToDownload = ItemsToDownload.SelectedItem as string;

            DownloadButton.Enabled = true;
            LocateButton.Enabled = true;
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            AppDownloadForm df = new AppDownloadForm(this, GetCurrentModule());
            df.ShowDialog();
        }

        private void LocateButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            string ext = GetCurrentModule().Type.ToLower();

            fd.Filter = $"{char.ToUpper(ext[0]) + ext.Substring(1)} files (*.{ext})|*.{ext}";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                if(!File.Exists(fd.FileName))
                {
                    MessageBox.Show("Please Select a Valid File.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LocateItem(fd.FileName, GetCurrentModule(), null);
            }
        }

        public static void LocateItem(string sourceFilePath, UnityReleaseModule selectedModule, Action<Exception> OnFinish)
        {
            try
            {
                if (selectedModule.ExtractedPathRename != null)
                {
                    selectedModule.ExtractedPathRename.From = AppendUnityPath(selectedModule.ExtractedPathRename.From);
                    selectedModule.ExtractedPathRename.To = AppendUnityPath(selectedModule.ExtractedPathRename.To);

                    HandleExtractedPathRename(sourceFilePath, selectedModule.ExtractedPathRename);

                    if(OnFinish != null)
                        OnFinish(null);
                }
                else if (selectedModule.Destination != null)
                {
                    selectedModule.Destination = AppendUnityPath(selectedModule.Destination);

                    MoveFileToDestination(sourceFilePath, selectedModule.Destination);

                    if (OnFinish != null)
                        OnFinish(null);
                }
                else
                {
                    MessageBox.Show($"Not Found Valid Destination for {selectedModule.Name}.\n\nError : Destination and ExtractedPathRename Are Empty.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (OnFinish != null)
                        OnFinish(new Exception("Not Found Valid Destination"));
                }
            }
            catch(Exception ex)
            {
                if (OnFinish != null)
                    OnFinish(ex);
            }
        }

        private static void HandleExtractedPathRename(string sourceFilePath, ExtractedPathRename extractedPathRename)
        {
            if(!Directory.Exists(extractedPathRename.To))
                Directory.CreateDirectory(extractedPathRename.To);

            string tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(sourceFilePath));

            if (Directory.Exists(tempExtractPath)) Directory.Delete(tempExtractPath, true);

            if (Path.GetExtension(sourceFilePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                ZipFile.ExtractToDirectory(sourceFilePath, tempExtractPath);
            else
            {
                string destinationPath = Path.Combine(extractedPathRename.To, Path.GetFileName(sourceFilePath));

                File.Move(sourceFilePath, destinationPath);

                if(Path.GetExtension(destinationPath) == ".exe")
                    Process.Start(destinationPath);

                MessageBox.Show("Installed Module Successfully!", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

                    if(!Directory.Exists(dirName))
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

            MessageBox.Show("Installed Module Successfully!", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void MoveFileToDestination(string sourceFilePath, string destinationPath)
        {
            if (Path.GetExtension(sourceFilePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                string tempExtractPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(sourceFilePath));
                ZipFile.ExtractToDirectory(sourceFilePath, tempExtractPath);
                Directory.Move(tempExtractPath, destinationPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourceFilePath));

                if(!File.Exists(destinationPath))
                    File.Copy(sourceFilePath, destinationPath);

                if (Path.GetExtension(destinationPath) == ".exe")
                    Process.Start(destinationPath);
            }

            MessageBox.Show("Installed Module Successfully!", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class Eula
    {
        public string Url { get; set; }
        public string Integrity { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Message { get; set; }
        public string __typename { get; set; }
    }

    public class DigitalValue
    {
        public long Value { get; set; }
        public string Unit { get; set; }
        public string __typename { get; set; }
    }

    public class ExtractedPathRename
    {
        public string From { get; set; }
        public string To { get; set; }
        public string __typename { get; set; }
    }

    public class SubModule
    {
        public string Url { get; set; }
        public string Integrity { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DigitalValue DownloadSize { get; set; }
        public DigitalValue InstalledSize { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public ExtractedPathRename ExtractedPathRename { get; set; }
        public bool PreSelected { get; set; }
        public string Destination { get; set; }
        public List<Eula> Eula { get; set; }
        public List<SubModule> SubModules { get; set; }
        public string __typename { get; set; }
    }

    public class UnityReleaseModule
    {
        public string Url { get; set; }
        public string Integrity { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public long DownloadSize { get; set; }
        public long InstalledSize { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }
        public ExtractedPathRename ExtractedPathRename { get; set; }
        public bool PreSelected { get; set; }
        public string Destination { get; set; }
        public List<Eula> Eula { get; set; }
        public List<SubModule> SubModules { get; set; }
        public string __typename { get; set; }
        public string DownloadUrl { get; set; }
        public bool Visible { get; set; }
        public bool Selected { get; set; }
        public string Sync { get; set; }
        public string Parent { get; set; }
        public string EulaUrl1 { get; set; }
        public string EulaLabel1 { get; set; }
        public string EulaMessage { get; set; }
        public string RenameTo { get; set; }
        public string RenameFrom { get; set; }
        public bool Preselected { get; set; }
        public bool IsInstalled { get; set; }
        public bool NewerModuleInstalled { get; set; }
    }
}