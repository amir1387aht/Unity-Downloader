using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static Unity_Downloader.AppDownloadForm;

namespace Unity_Downloader
{
    public partial class MainForm : Form
    {
        private static string UnityHubDataPath = "C:\\Users\\{0}\\AppData\\Roaming\\UnityHub";
        private static string UnityHubEditorPath = "{0}\\secondaryInstallPath.json";
        private static string UnityEditorsPath = "";
        private static string EditorModulesDefaultePath = "{0}\\modules.json";
        private static string EditorModulesPath = EditorModulesDefaultePath;

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Utilities.IsAdministrator())
            {
                MessageBox.Show($"Please Run App as Administrator", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

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

            EditorModulesPath = EditorModulesDefaultePath;

            EditorModulesPath = string.Format(EditorModulesPath, AllEditors[EditorsListSelect.SelectedIndex]);

            LoadAvailableItems();
        }

        private void LoadAvailableItems()
        {
            ItemsToDownload.Items.Clear();

            if(!File.Exists(EditorModulesPath))
            {
                AllEditors = AllEditors.Where(element => element != SelectedEditor).ToArray();
                EditorsListSelect.Items.Remove(SelectedEditor);

                if (EditorsListSelect.Items.Count > 0)
                    EditorsListSelect.SelectedIndex = 0;
                else
                {
                    MessageBox.Show($"It Seems You Dont Installed Any Unity Editor, Install One And try Again.\n\nError : No Valid Editor Folder With modules.json On \"{UnityEditorsPath}\" Found.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                return;
            }

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

                Utilities.LocateItem(fd.FileName, GetCurrentModule(), OnInstallFinish);
            }
        }

        private void OnInstallFinish(Exception e)
        {
            if (e != null)
            {
                MessageBox.Show($"An error was encountered during Installing, Error Message: {e.Message}, Source: {e.Source}, StackTrace:{e.StackTrace}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Installed Module Successfully!", "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            var SettingForm = new SettingForm();
            SettingForm.ShowDialog();
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