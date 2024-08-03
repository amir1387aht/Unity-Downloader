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

        private UnityReleaseModule ParentModule;
        private UnityReleaseModule CurrentModule;

        private string CurrentFilePath = "";
        private string ProgressFilePath = "";
        private string LogsFilePath = "";

        private DownloadService downloader;

        private DateTime lastFileDataUpdate = DateTime.MinValue;
        private DateTime lastChunkDataUpdate = DateTime.MinValue;
        private DateTime lastSave = DateTime.MinValue;

        public enum ActionButtonEnum { Pause, Resume, Retry }
        public ActionButtonEnum currentAction;

        private FileStream downloadFileStream;

        private string tempDownloadPath = "";

        private const int timeToUpdateUI = 170, timeToSaveProgress = 5000; // MiliSeconds

        private CustomProgressBar[] progressBars = new CustomProgressBar[8];

        private DownloadPackage DownloadProgress;

        public AppDownloadForm(Form lastForm, UnityReleaseModule module)
        {
            LastForm = lastForm;
            CurrentModule = module;

            tempDownloadPath = Path.Combine(Path.GetTempPath(), "unity_downloader", MainForm.SelectedEditor);

            if (!Directory.Exists(tempDownloadPath)) Directory.CreateDirectory(tempDownloadPath);

            InitializeComponent();

            if(!SettingForm.IsSettingLoaded)
            {
                SettingForm.InitSettingPath();
                SettingForm.LoadSetting();
                SettingForm.ApplyDNS();
            }

            var downloadConfiguration = new DownloadConfiguration()
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

            SetActionButtonType(ActionButtonEnum.Pause);

            ActionButton.Enabled = false;
            DownloadProgressBar.Value = 0;
            FileNameLabel.Text = "Current File : " + CurrentModule.Name;
            StatusLabel.Text = "Status : Downloading...";
            DownloadedSpaceLabel.Text = "Downloaded : 0 MB (%0)";
            TransferRateLabel.Text = "Transfer Rate : 0 MB/Sec";
            TimeLeftLabel.Text = "Time Left : Calculating...";

            ShowPendingText(CurrentModule.SubModules);

            InitializeProgressBars();

            SetupForm();

            StartDownload();
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

        private async void StartDownload()
        {
            try
            {
                // Set file paths
                CurrentFilePath = Path.Combine(tempDownloadPath, $"{CurrentModule.Name}.{CurrentModule.Type.ToLower()}");
                ProgressFilePath = Path.Combine(tempDownloadPath, $"{CurrentModule.Name}.{CurrentModule.Type.ToLower()}.json.tmp");
                LogsFilePath = Path.Combine(tempDownloadPath, $"{CurrentModule.Name}.{CurrentModule.Type.ToLower()}.log");

                // Show Logs Path
                AddLog($"Logs file can be found in \"{LogsFilePath}\"");

                // Delete old logs
                File.WriteAllText(LogsFilePath, "");

                // Log the download start
                AddLog($"Downloading File: {CurrentModule.Url}");

                // Check if a progress file exists
                if (File.Exists(ProgressFilePath))
                {
                    // Prompt the user to continue from the saved progress
                    DialogResult result = MessageBox.Show(
                        $"A save file was found for {CurrentModule.Name}. Do you want to continue from the saved file?",
                        "System Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    // Handle the user's choice
                    if (result == DialogResult.Yes)
                    {
                        downloadFileStream = new FileStream(CurrentFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                        DownloadProgress = LoadProgress();

                        ResetChunkProgressBars();

                        await downloader.DownloadFileTaskAsync(DownloadProgress, CurrentModule.Url, downloadFileStream);
                    }
                    else
                    {
                        await downloader.DownloadFileTaskAsync(CurrentModule.Url, CurrentFilePath);
                    }
                }
                else
                {
                    await downloader.DownloadFileTaskAsync(CurrentModule.Url, CurrentFilePath);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the download process
                AddLog($"An error occurred while starting the download: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}, Close app and try again", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetChunkProgressBars()
        {
            if (DownloadProgress == null || DownloadProgress.Chunks == null || DownloadProgress.Chunks.Length == 0) return;

            for (int i = 0; i < progressBars.Length; i++)
            {
                UpdateProgressBar(i, DownloadProgress.Chunks[i].IsDownloadCompleted() ? 100 : 0);
            }
        }


        private void CheckForSaveProgress()
        {
            InvokeFunction(() =>
            {
                if ((DateTime.Now - lastSave).TotalMilliseconds < timeToSaveProgress) return;

                lastSave = DateTime.Now;

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
                downloadFileStream?.Close();
                return;
            }

            if (e.Error != null)
            {
                InvokeFunction(() =>
                {
                    StatusLabel.Text = "Status : An error was encountered during Downloading";
                    SetActionButtonType(ActionButtonEnum.Retry);
                    AddLog($"An error was encountered during Downloading, Error Message: {e.Error.Message}, Source: {e.Error.Source}, StackTrace:{e.Error.StackTrace}");
                });

                downloadFileStream?.Close();
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

            downloadFileStream?.Close();
        }

        private void OnInstallComplete(Exception e)
        {
            ActionButton.Enabled = true;
            CancelButton.Enabled = true;

            if (e != null)
            {
                StatusLabel.Text = "An error was encountered during Installing";
                AddLog($"An error was encountered during Installing, Error Message: {e.Message}, Source: {e.Source}, StackTrace:{e.StackTrace}");
                SetActionButtonType(ActionButtonEnum.Retry);
                return;
            }

            if (CurrentModule.SubModules.Count == 0 && ParentModule == null)
            {
                MessageBox.Show("Installed Module Successfully!", "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            // Prepare for next model
            ref UnityReleaseModule NextModel = ref (ParentModule == null ? ref CurrentModule : ref ParentModule);

            NextModel.SubModules.RemoveAt(0);

            ParentModule ??= CurrentModule;
            string nextModelName = NextModel.SubModules[0].Name;
            CurrentModule = MainForm.AllReleaseModules.Find(item => item.Name == nextModelName);

            ShowPendingText(ParentModule.SubModules);
            StartDownload();
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if ((DateTime.Now - lastFileDataUpdate).TotalMilliseconds < timeToUpdateUI) return;

            lastFileDataUpdate = DateTime.Now;

            InvokeFunction(() =>
            {
                DownloadProgressBar.Value = (int)e.ProgressPercentage;
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
                UpdateProgressBar(int.Parse(e.ProgressId), (int)e.ProgressPercentage);
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
                ShowPendingText(CurrentModule.SubModules);
            });
        }

        private void ShowPendingText(List<SubModule> subModules)
        {
            PendingLabel.Text = "Pending : -";

            if (subModules != null && subModules.Count != 0)
                PendingLabel.Text = "Pending : " + string.Join(", ", subModules.Select(x => x.Name));
        }

        private void SetupForm()
        {
            ChangeFormSizeToMaximum();
            SetFormLocationCenterOfParent();
        }

        private void ChangeFormSizeToMaximum() => Size = new Size(LastForm.Width, LastForm.Height - 1);
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

        private void AddLog(string text)
        {
            LogsList.Items.Add(text);
            File.AppendAllText(LogsFilePath, $"[{DateTime.Now}] {text}\n");
        }

        private void AppDownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            downloadFileStream?.Close();
        }
    }
}