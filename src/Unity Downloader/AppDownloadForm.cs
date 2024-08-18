using Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unity_Downloader
{
    public partial class AppDownloadForm : Form
    {
        private Form LastForm = new Form();

        private UnityReleaseModule CurrentModule;
        private List<UnityReleaseModule> PendingSubModules = new List<UnityReleaseModule>();

        private string CurrentFilePath = "";
        private string ProgressFilePath = "";
        private string LogsFilePath = "";

        private DownloadService downloader;

        private DateTime lastFileDataUpdate = DateTime.MinValue;
        private DateTime lastChunkDataUpdate = DateTime.MinValue;
        private DateTime lastSaveUpdate = DateTime.MinValue;

        public enum ActionButtonEnum { Pause, Resume, Retry }
        public ActionButtonEnum currentAction;

        private string tempDownloadPath = "";

        private const int timeToUpdateUI = 170, timeToSaveProgress = 5000; // MiliSeconds

        private CustomProgressBar[] progressBars = new CustomProgressBar[8];

        private DownloadPackage DownloadProgress;

        public AppDownloadForm(Form lastForm, UnityReleaseModule module)
        {
            LastForm = lastForm;
            CurrentModule = module;

            if (CurrentModule.SubModules != null && CurrentModule.SubModules.Count != 0)
                PendingSubModules = FindAllSubModules();

            tempDownloadPath = Path.Combine(Path.GetTempPath(), "unity_downloader", MainForm.SelectedEditor);

            if (!Directory.Exists(tempDownloadPath)) Directory.CreateDirectory(tempDownloadPath);

            InitializeComponent();

            if (!SettingForm.IsSettingLoaded)
            {
                SettingForm.InitSettingPath();
                SettingForm.LoadSetting();
                SettingForm.ApplyDNS();
            }

            InitDownloadProvider();

            SetActionButtonType(ActionButtonEnum.Pause);

            ActionButton.Enabled = false;
            DownloadProgressBar.Value = 0;
            FileNameLabel.Text = "Current File : " + CurrentModule.Name;
            StatusLabel.Text = "Status : Downloading...";
            DownloadedSpaceLabel.Text = "Downloaded : 0 MB (%0)";
            TransferRateLabel.Text = "Transfer Rate : 0 MB/Sec";
            TimeLeftLabel.Text = "Time Left : Calculating...";

            ShowPendingText();

            InitializeProgressBars();

            SetupForm();

            StartDownload();
        }

        private void InitDownloadProvider(DownloadConfiguration dc = null)
        {
            var downloadConfiguration = dc ?? new DownloadConfiguration()
            {
                ChunkCount = progressBars.Length,
                MaxTryAgainOnFailover = 5,
                MaximumMemoryBufferBytes = 1024 * 1024 * 50,
                ParallelDownload = true,
                ParallelCount = 4,
                Timeout = 5000,
            };

            if (SettingForm.settingData.ProxyEnabled)
            {
                downloadConfiguration.RequestConfiguration = new RequestConfiguration();
                downloadConfiguration.RequestConfiguration.Proxy = new System.Net.WebProxy()
                {
                    Address = new Uri(SettingForm.settingData.ProxyPacFilePath),
                    BypassProxyOnLocal = SettingForm.settingData.BypassProxyOnLocal
                };
            }

            downloader = new DownloadService(downloadConfiguration);

            downloader.DownloadStarted += OnDownloadStarted;
            downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;
            downloader.DownloadProgressChanged += OnDownloadProgressChanged;
            downloader.DownloadFileCompleted += OnDownloadFileCompleted;
        }

        public List<UnityReleaseModule> FindAllSubModules()
        {
            var matchedSubModules = new List<UnityReleaseModule>();

            var allModules = MainForm.AllReleaseModules;
            var subModels = CurrentModule.SubModules;

            foreach (var module in subModels)
            {
                AddSubModulesRecursively(allModules, matchedSubModules, module);
            }

            return matchedSubModules;
        }

        private void AddSubModulesRecursively(List<UnityReleaseModule> allModules, List<UnityReleaseModule> matchedSubModules, SubModule subModule)
        {
            var matchedModule = allModules.Find(m => m.Name == subModule.Name);
            if (matchedModule != null)
            {
                matchedSubModules.Add(matchedModule);
            }

            if (subModule.SubModules != null && subModule.SubModules.Count > 0)
            {
                foreach (var nestedSubModule in subModule.SubModules)
                {
                    AddSubModulesRecursively(allModules, matchedSubModules, nestedSubModule);
                }
            }
        }

        private void InitializeProgressBars()
        {
            for (int i = 0; i < progressBars.Length; i++)
            {
                progressBars[i] = new CustomProgressBar
                {
                    Location = new Point((i * 52) + 7, 15),
                    Size = new Size(55, 15),
                    ForeColor = Color.Blue,
                    Anchor = AnchorStyles.Top
                };

                Controls.Add(progressBars[i]); // Add progress bar to the form
            }
        }

        public void UpdateProgressBar(int chunkIndex, int value)
        {
            if (chunkIndex >= 0 && chunkIndex < progressBars.Length)
            {
                progressBars[chunkIndex].Value = value;
            }
        }

        private bool isInitialDownload = true;

        private async void StartDownload()
        {
            try
            {
                // Reset Action Button
                SetActionButtonType(ActionButtonEnum.Pause);

                // Reset Progresses
                ResetChunkProgressBars();

                // Set file paths
                CurrentFilePath = Path.Combine(tempDownloadPath, $"{CurrentModule.Name}.{CurrentModule.Type.ToLower()}");
                ProgressFilePath = Path.Combine(tempDownloadPath, $"{CurrentModule.Name}.{CurrentModule.Type.ToLower()}.json.tmp");
                LogsFilePath = Path.Combine(tempDownloadPath, $"Log.log");

                // Show Download State
                AddLog($"-------------------- [{DateTime.Now}] New Download Starting. Name: {CurrentModule.Name}, Url: {CurrentModule.Url}, SubModels: {GetModelSubModels()} --------------------", false);

                // Show Logs Path
                AddLog($"Logs file can be found in \"{LogsFilePath}\"");

                // Log the download start
                AddLog($"Downloading File: {CurrentModule.Url}");

                // Check if a progress file exists
                if (File.Exists(ProgressFilePath))
                {
                    // Prompt the user to continue from the saved progress
                    DialogResult result = isInitialDownload ? MessageBox.Show(
                        $"A save file was found for {CurrentModule.Name}. Do you want to continue from the saved file?",
                        "System Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    ) : DialogResult.Yes;

                    // Handle the user's choice
                    if (result == DialogResult.Yes)
                    {
                        DownloadProgress = LoadProgress();

                        if (DownloadProgress.IsSaveComplete)
                        {
                            AddLog("Found an Exiting Completed Save. Skipping This Item.");
                            OnInstallComplete(null);
                        }
                        else
                            await downloader.DownloadFileTaskAsync(DownloadProgress, CurrentModule.Url);
                    }
                    else
                        await downloader.DownloadFileTaskAsync(CurrentModule.Url, CurrentFilePath);
                }
                else
                    await downloader.DownloadFileTaskAsync(CurrentModule.Url, CurrentFilePath);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the download process and close form
                AddLog($"An error occurred while starting the download: {ex.Message}");
                MessageBox.Show($"An error occurred during starting: {ex.Message}, Close app and try again", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void ResetChunkProgressBars()
        {
            if (DownloadProgress == null || DownloadProgress.Chunks == null || DownloadProgress.Chunks.Length == 0)
            {
                for (int i = 0; i < progressBars.Length; i++)
                    UpdateProgressBar(i, 0);

                return;
            }

            for (int i = 0; i < progressBars.Length; i++)
                UpdateProgressBar(i, DownloadProgress.Chunks[i].IsDownloadCompleted() ? 100 : 0);
        }


        private void CheckForSaveProgress()
        {
            InvokeFunction(() =>
            {
                if ((DateTime.Now - lastSaveUpdate).TotalMilliseconds < timeToSaveProgress) return;

                lastSaveUpdate = DateTime.Now;

                SaveProgress();
            });
        }

        private void SaveProgress()
        {
            var progress = downloader.Package;
            var progressJson = Newtonsoft.Json.JsonConvert.SerializeObject(progress);

            File.WriteAllText(ProgressFilePath, progressJson);
        }

        private DownloadPackage LoadProgress() =>
          Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadPackage>(File.ReadAllText(ProgressFilePath));

        private void InvokeFunction(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SaveProgress();

            InvokeFunction(() => { ActionButton.Enabled = true; });

            if (e.Cancelled)
            {
                InvokeFunction(() =>
                {
                    StatusLabel.Text = "Status : Cancelled";
                    SetActionButtonType(ActionButtonEnum.Retry);
                    AddLog("Download Cancelled.");
                });
                return;
            }

            if (e.Error != null)
            {
                InvokeFunction(() =>
                {
                    StatusLabel.Text = "Status : An error was encountered during Downloading";
                    SetActionButtonType(ActionButtonEnum.Retry);
                    AddLog($"An error was encountered during Downloading, Error Message: {e.Error.Message}, Source: {e.Error.Source}, StackTrace:{e.Error.StackTrace}");

                    if(e.Error.Message.Contains("The remote server returned an error: (416)"))
                    {
                        AddLog("Found an Knowen Issue, try downloaing again with diffrent setting.");

                        var downloadConfiguration = new DownloadConfiguration()
                        {
                            ChunkCount = 0,
                            MaxTryAgainOnFailover = 5,
                            MaximumMemoryBufferBytes = 1024 * 1024 * 50,
                            ParallelDownload = false,
                            Timeout = 5000,
                        };

                        InitDownloadProvider(downloadConfiguration);

                        StartDownload();
                    }
                });

                return;
            }

            InvokeFunction(async () =>
            {
                StatusLabel.Text = "Status : Installing...";

                AddLog($"Installing {CurrentModule.Name}...");

                ActionButton.Enabled = false;
                CancelButton.Enabled = false;

                DownloadProgressBar.Value = 100;
                ProgressLabel.Text = "%100";

                await Task.Delay(300);

                Utilities.LocateItem(CurrentFilePath, CurrentModule, OnInstallComplete);
            });

        }

        private async void OnInstallComplete(Exception e)
        {
            await Task.Delay(300); // Avoid Instant Installing

            InvokeFunction(() =>
            {
                ActionButton.Enabled = true;
                CancelButton.Enabled = true;

                if (e != null)
                {
                    StatusLabel.Text = "An error was encountered during Installing";
                    AddLog($"An error was encountered during Installing, Error Message: {e.Message}, Source: {e.Source}, StackTrace:{e.StackTrace}");
                    MessageBox.Show($"An error was encountered during Installing, Error Message: {e.Message}, Source: {e.Source}, StackTrace:{e.StackTrace}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetActionButtonType(ActionButtonEnum.Retry);
                    return;
                }

                if (PendingSubModules.Count == 0)
                {
                    MessageBox.Show("Installed Module Successfully!", "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                    return;
                }

                var ModuleToDownload = PendingSubModules.First();
                PendingSubModules.Remove(ModuleToDownload);
                CurrentModule = ModuleToDownload;

                ShowPendingText();

                isInitialDownload = false;

                InitDownloadProvider();

                StartDownload();
            });
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if ((DateTime.Now - lastFileDataUpdate).TotalMilliseconds < timeToUpdateUI) return;

            lastFileDataUpdate = DateTime.Now;

            InvokeFunction(() =>
            {
                DownloadProgressBar.Value = Math.Min((int)e.ProgressPercentage, 100);
                ProgressLabel.Text = "%" + e.ProgressPercentage.ToString("F2");
                DownloadedSpaceLabel.Text = $"Downloaded : {Utilities.GetHumanReadableFileSize(e.ReceivedBytesSize)} (total: {Utilities.GetHumanReadableFileSize(e.TotalBytesToReceive)})";
                TransferRateLabel.Text = $"Transfer Rate : {Utilities.GetHumanReadableFileSize((long)e.AverageBytesPerSecondSpeed)}/Sec";
                TimeLeftLabel.Text = $"Time Left : {GetDownloadTimeLeft(e)}";
            });

            CheckForSaveProgress();
        }

        public static string GetDownloadTimeLeft(DownloadProgressChangedEventArgs e)
        {
            if (e.AverageBytesPerSecondSpeed > 0)
            {
                long bytesRemaining = e.TotalBytesToReceive - e.ReceivedBytesSize;
                TimeSpan timeLeft = TimeSpan.FromSeconds(bytesRemaining / e.AverageBytesPerSecondSpeed);
                return $"{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
            }
            else return "Calculating...";
        }

        private void OnChunkDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if ((DateTime.Now - lastChunkDataUpdate).TotalMilliseconds < timeToUpdateUI) return;

            lastChunkDataUpdate = DateTime.Now;

            InvokeFunction(() =>
            {
                UpdateProgressBar(int.Parse(e.ProgressId), Math.Min(Math.Max(0, (int)e.ProgressPercentage), 100));
            });
        }

        private void SetActionButtonType(ActionButtonEnum currentAction)
        {
            this.currentAction = currentAction;
            ActionButton.Text = currentAction.ToString();
        }

        private void OnDownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            InvokeFunction(() =>
            {
                ActionButton.Enabled = true;
                AddLog($"Downloading File \"{Path.GetFileName(e.FileName)}\" Started.");
                DownloadProgressBar.Value = 0;
                FileNameLabel.Text = "Current File : " + Path.GetFileName(e.FileName);
                StatusLabel.Text = "Status : Downloading...";
                DownloadedSpaceLabel.Text = "Downloaded : 0 MB (%0)";
                TransferRateLabel.Text = "Transfer Rate : 0 MB/Sec";
                TimeLeftLabel.Text = "Time Left : Calculating...";
                ResetChunkProgressBars();
                ShowPendingText();
            });
        }

        private void ShowPendingText() =>
            PendingLabel.Text = GetModelSubModels();

        private string GetModelSubModels()
        {
            string result = "Pending : -";

            if (PendingSubModules.Count != 0)
                result = "Pending : " + string.Join(", ", PendingSubModules.Select(x => x.Name));

            return result;
        }

        private void SetupForm()
        {
            ChangeFormSizeToMaximum();
            SetFormLocationCenterOfParent();
        }

        private void ChangeFormSizeToMaximum() => Size = new Size(LastForm.Width, LastForm.Height);
        private void SetFormLocationCenterOfParent() => Location = new Point(LastForm.Width / 2 - LastForm.Width / 2 + LastForm.Location.X, LastForm.Height / 2 - LastForm.Height / 2 + LastForm.Location.Y);

        private void ActionButton_Click(object sender, EventArgs e)
        {
            switch (currentAction)
            {
                case ActionButtonEnum.Pause:
                    downloader.Pause();
                    SetActionButtonType(ActionButtonEnum.Resume);
                    AddLog("Download Paused.");
                    StatusLabel.Text = "Status : Paused";
                    break;
                case ActionButtonEnum.Resume:
                    downloader.Resume();
                    SetActionButtonType(ActionButtonEnum.Pause);
                    AddLog("Download Resumed.");
                    StatusLabel.Text = "Status : Downloading...";
                    break;
                case ActionButtonEnum.Retry:
                    SetActionButtonType(ActionButtonEnum.Pause);
                    AddLog("Retrying To Download...");
                    StartDownload();
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            SaveProgress();

            downloader.CancelAsync();

            Close();
        }

        private void AddLog(string text, bool showInUI = true)
        {
            if(showInUI)
                LogsList.Items.Add(text);

            File.AppendAllText(LogsFilePath, $"[{DateTime.Now}] {text}\n");
        }
    }
}