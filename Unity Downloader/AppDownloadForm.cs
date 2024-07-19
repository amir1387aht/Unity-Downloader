using Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Unity_Downloader
{
    public partial class AppDownloadForm : Form
    {
        private Form LastForm = new Form();

        private UnityReleaseModule ParentModule;
        private UnityReleaseModule CurrentModule;

        private string CurrentFilePath = "";

        DownloadService downloader = new DownloadService(new DownloadConfiguration()
        {
            ChunkCount = 8,
            MaxTryAgainOnFailover = 5,
            MaximumMemoryBufferBytes = 1024 * 1024 * 50,
            ParallelDownload = true,
            ParallelCount = 4,
            Timeout = 5000,
        });

        public enum ActionButtonEnum
        { 
            Pause,
            Resume,
            Retry
        }

        public ActionButtonEnum currentAction;

        public AppDownloadForm(Form lastForm, UnityReleaseModule module)
        {
            LastForm = lastForm;
            CurrentModule = module;

            InitializeComponent();
            ChangeFormSizeToMinimum();

            downloader.DownloadStarted += OnDownloadStarted;

            downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;

            downloader.DownloadProgressChanged += OnDownloadProgressChanged;

            downloader.DownloadFileCompleted += OnDownloadFileCompleted;

            SetActionButtonType(ActionButtonEnum.Pause);

            SetChoosePanelVisibility(true);

            ActionButton.Enabled = false;

            UpdateUI(() =>
            {
                DownloadProgressBar.Value = 0;
                FileNameLabel.Text = "Current File : " + CurrentModule.Name;
                StatusLabel.Text = "Status : Downloading...";
                DownloadedSpaceLabel.Text = "Downloaded : 0 MB (%0)";
                TransferRateLabel.Text = "Transfer Rate : 0 MB/Sec";
                TimeLeftLabel.Text = "Time Left : Calculating...";

                ShowPendingText(CurrentModule.SubModules);
            });
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else action();
        }

        private void IDMButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InAppButton_Click(object sender, EventArgs e)
        {
            ClearChooseAction();

            StartDownload();
        }

        private async void StartDownload()
        {
            string path = "";

            if (CurrentModule.Destination != null)
                path = CurrentModule.Destination;
            else if (CurrentModule.ExtractedPathRename != null && CurrentModule.ExtractedPathRename.From != null)
                path = CurrentModule.ExtractedPathRename.From;

            CurrentFilePath = path;
            AddLog(CurrentModule.Url);
            await downloader.DownloadFileTaskAsync(CurrentModule.Url, MainForm.AppendUnityPath(path));
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateUI(() => {
                ActionButton.Enabled = true;
            });

            if (e.Cancelled)
            {
                UpdateUI(() => {
                    StatusLabel.Text = "Status : Cancelled";
                    SetActionButtonType(ActionButtonEnum.Retry);
                    AddLog("Download Cancelled.");
                });
                return;
            }

            if (e.Error != null)
            {
                UpdateUI(() => {
                    StatusLabel.Text = "Status : An error was encountered during Downloading";
                    SetActionButtonType(ActionButtonEnum.Retry);
                    AddLog($"An error was encountered during Downloading, Error Message: {e.Error.Message}, Source: {e.Error.Source}, StackTrace:{e.Error.StackTrace}");
                });
                return;
            }

            // TODO: handle wifi disconnect action

            UpdateUI(() => {
                StatusLabel.Text = "Status : Installing...";
                AddLog($"Installing {CurrentModule.Name}...");
                ActionButton.Enabled = false;
                CancelButton.Enabled = false;
            });

            // Install Item
            MainForm.LocateItem(CurrentFilePath, CurrentModule, OnInstallComplete);
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

            // TODO: prepare next item

            // FIXME: delete downloaded item from pending
            ShowPendingText(CurrentModule.SubModules);

            StartDownload();
        }

        private void OnDownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            UpdateUI(() =>
            {
                DownloadProgressBar.Value = (int)e.ProgressPercentage;
                ProgressLabel.Text = "%" + e.ProgressPercentage.ToString("F2");
                DownloadedSpaceLabel.Text = $"Downloaded : {GetHumanReadableSize(e.ReceivedBytesSize)} (total: {GetHumanReadableSize(e.TotalBytesToReceive)})";
                TransferRateLabel.Text = $"Transfer Rate : {GetHumanReadableSize((long)e.AverageBytesPerSecondSpeed)}/Sec";
            });
        }

        private void OnChunkDownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            
        }

        private void SetActionButtonType(ActionButtonEnum currentAction)
        {
            this.currentAction = currentAction;
            ActionButton.Text = currentAction.ToString();
        }

        private void OnDownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            UpdateUI(() =>
            {
                ActionButton.Enabled = true;

                AddLog($"Downloading File \"{Path.GetFileName(e.FileName)}\" Started.");

                DownloadProgressBar.Value = 0;
                FileNameLabel.Text = "Current File : " + Path.GetFileName(e.FileName);
                StatusLabel.Text = "Status : Downloading...";
                DownloadedSpaceLabel.Text = "Downloaded : 0 MB (%0)";
                TransferRateLabel.Text = "Transfer Rate : 0 MB/Sec";
                TimeLeftLabel.Text = "Time Left : Calculating...";

                ShowPendingText(CurrentModule.SubModules);
            });
        }

        private void ShowPendingText(List<SubModule> subModules)
        {
            PendingLabel.Text = "Pending : ";

            if(subModules == null)
            {
                PendingLabel.Text += "Nothing";
                return;
            }

            for (int i = 0; i < subModules.Count; i++)
            {
                if (i == subModules.Count - 1)
                    PendingLabel.Text += subModules[i];
                else PendingLabel.Text += subModules[i] + ", ";
            }
        }

        private void ClearChooseAction()
        {
            ChangeFormSizeToMaximum();
            SetFormLocationCenterOfParent();
            SetChoosePanelVisibility(false);
        }

        private void ChangeFormSizeToMinimum() => Size = new Size(LastForm.Width, 380);

        private void ChangeFormSizeToMaximum() => Size = new Size(LastForm.Width, LastForm.Height);

        private void SetFormLocationCenterOfParent() => Location = new Point(
            LastForm.Width / 2 - LastForm.Width / 2 + LastForm.Location.X,
            LastForm.Height / 2 - LastForm.Height / 2 + LastForm.Location.Y
        );

        private void SetChoosePanelVisibility(bool v) => ChoosePanel.Visible = v;

        private void ActionButton_Click(object sender, EventArgs e)
        {
            switch (currentAction)
            {
                case ActionButtonEnum.Pause:
                    downloader.Pause();
                    SetActionButtonType(ActionButtonEnum.Resume);
                    AddLog("Download Pauseded.");
                    break;
                case ActionButtonEnum.Resume:
                    downloader.Resume();
                    SetActionButtonType(ActionButtonEnum.Pause);
                    AddLog("Download Resumeed.");
                    break;
                case ActionButtonEnum.Retry:
                    SetActionButtonType(ActionButtonEnum.Pause);
                    AddLog("Retry To Download...");
                    StartDownload();
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            downloader.CancelAsync();

            Close();
        }

        private void AddLog(string text)
        {
            LogsList.Items.Add(text);
        }

        public static string GetHumanReadableSize(long byteCount)
        {
            if (byteCount < 1024 * 1024) // less than 1 MB
                return (byteCount / 1024.0).ToString("F2") + " KB";
            else if (byteCount < 1024 * 1024 * 1024) // less than 1 GB
                return (byteCount / 1024.0 / 1024.0).ToString("F2") + " MB";
            else // 1 GB or more
                return (byteCount / 1024.0 / 1024.0 / 1024.0).ToString("F2") + " GB";
        }
    }
}