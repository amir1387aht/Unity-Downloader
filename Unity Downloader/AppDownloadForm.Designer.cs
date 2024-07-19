namespace Unity_Downloader
{
    partial class AppDownloadForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppDownloadForm));
            this.ExplainText = new System.Windows.Forms.Label();
            this.IDMButton = new System.Windows.Forms.Button();
            this.InAppButton = new System.Windows.Forms.Button();
            this.DownloadWithText = new System.Windows.Forms.Label();
            this.DownloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.DownloadedSpaceLabel = new System.Windows.Forms.Label();
            this.TransferRateLabel = new System.Windows.Forms.Label();
            this.TimeLeftLabel = new System.Windows.Forms.Label();
            this.ActionButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.PendingLabel = new System.Windows.Forms.Label();
            this.LogsList = new System.Windows.Forms.ListBox();
            this.ChoosePanel = new System.Windows.Forms.Panel();
            this.ChoosePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExplainText
            // 
            this.ExplainText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExplainText.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExplainText.Location = new System.Drawing.Point(11, 130);
            this.ExplainText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ExplainText.Name = "ExplainText";
            this.ExplainText.Size = new System.Drawing.Size(530, 290);
            this.ExplainText.TabIndex = 11;
            this.ExplainText.Text = resources.GetString("ExplainText.Text");
            // 
            // IDMButton
            // 
            this.IDMButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.IDMButton.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IDMButton.Location = new System.Drawing.Point(348, 66);
            this.IDMButton.Margin = new System.Windows.Forms.Padding(4);
            this.IDMButton.Name = "IDMButton";
            this.IDMButton.Size = new System.Drawing.Size(193, 47);
            this.IDMButton.TabIndex = 10;
            this.IDMButton.Text = "IDM";
            this.IDMButton.UseVisualStyleBackColor = true;
            this.IDMButton.Click += new System.EventHandler(this.IDMButton_Click);
            // 
            // InAppButton
            // 
            this.InAppButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.InAppButton.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InAppButton.Location = new System.Drawing.Point(15, 66);
            this.InAppButton.Margin = new System.Windows.Forms.Padding(4);
            this.InAppButton.Name = "InAppButton";
            this.InAppButton.Size = new System.Drawing.Size(193, 47);
            this.InAppButton.TabIndex = 9;
            this.InAppButton.Text = "In App";
            this.InAppButton.UseVisualStyleBackColor = true;
            this.InAppButton.Click += new System.EventHandler(this.InAppButton_Click);
            // 
            // DownloadWithText
            // 
            this.DownloadWithText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DownloadWithText.Font = new System.Drawing.Font("Yu Gothic UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadWithText.Location = new System.Drawing.Point(154, 3);
            this.DownloadWithText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DownloadWithText.Name = "DownloadWithText";
            this.DownloadWithText.Size = new System.Drawing.Size(255, 39);
            this.DownloadWithText.TabIndex = 1;
            this.DownloadWithText.Text = "Download With :";
            // 
            // DownloadProgressBar
            // 
            this.DownloadProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DownloadProgressBar.Location = new System.Drawing.Point(12, 12);
            this.DownloadProgressBar.Name = "DownloadProgressBar";
            this.DownloadProgressBar.Size = new System.Drawing.Size(558, 39);
            this.DownloadProgressBar.TabIndex = 11;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressLabel.Location = new System.Drawing.Point(260, 54);
            this.ProgressLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(33, 23);
            this.ProgressLabel.TabIndex = 12;
            this.ProgressLabel.Text = "0%";
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileNameLabel.Location = new System.Drawing.Point(23, 102);
            this.FileNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(112, 23);
            this.FileNameLabel.TabIndex = 13;
            this.FileNameLabel.Text = "Current File : ";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(23, 142);
            this.StatusLabel.Margin = new System.Windows.Forms.Padding(0);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(71, 23);
            this.StatusLabel.TabIndex = 14;
            this.StatusLabel.Text = "Status : ";
            // 
            // DownloadedSpaceLabel
            // 
            this.DownloadedSpaceLabel.AutoSize = true;
            this.DownloadedSpaceLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadedSpaceLabel.Location = new System.Drawing.Point(23, 182);
            this.DownloadedSpaceLabel.Margin = new System.Windows.Forms.Padding(0);
            this.DownloadedSpaceLabel.Name = "DownloadedSpaceLabel";
            this.DownloadedSpaceLabel.Size = new System.Drawing.Size(120, 23);
            this.DownloadedSpaceLabel.TabIndex = 15;
            this.DownloadedSpaceLabel.Text = "Downloaded : ";
            // 
            // TransferRateLabel
            // 
            this.TransferRateLabel.AutoSize = true;
            this.TransferRateLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TransferRateLabel.Location = new System.Drawing.Point(23, 222);
            this.TransferRateLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TransferRateLabel.Name = "TransferRateLabel";
            this.TransferRateLabel.Size = new System.Drawing.Size(122, 23);
            this.TransferRateLabel.TabIndex = 16;
            this.TransferRateLabel.Text = "Transfer Rate : ";
            // 
            // TimeLeftLabel
            // 
            this.TimeLeftLabel.AutoSize = true;
            this.TimeLeftLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLeftLabel.Location = new System.Drawing.Point(23, 262);
            this.TimeLeftLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TimeLeftLabel.Name = "TimeLeftLabel";
            this.TimeLeftLabel.Size = new System.Drawing.Size(94, 23);
            this.TimeLeftLabel.TabIndex = 17;
            this.TimeLeftLabel.Text = "Time Left : ";
            // 
            // ActionButton
            // 
            this.ActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionButton.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActionButton.Location = new System.Drawing.Point(408, 543);
            this.ActionButton.Name = "ActionButton";
            this.ActionButton.Size = new System.Drawing.Size(145, 45);
            this.ActionButton.TabIndex = 18;
            this.ActionButton.Text = "Pause";
            this.ActionButton.UseVisualStyleBackColor = true;
            this.ActionButton.Click += new System.EventHandler(this.ActionButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CancelButton.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(27, 543);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(145, 45);
            this.CancelButton.TabIndex = 19;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // PendingLabel
            // 
            this.PendingLabel.AutoSize = true;
            this.PendingLabel.Font = new System.Drawing.Font("Yu Gothic UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PendingLabel.Location = new System.Drawing.Point(23, 302);
            this.PendingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.PendingLabel.Name = "PendingLabel";
            this.PendingLabel.Size = new System.Drawing.Size(87, 23);
            this.PendingLabel.TabIndex = 20;
            this.PendingLabel.Text = "Pending : ";
            // 
            // LogsList
            // 
            this.LogsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogsList.FormattingEnabled = true;
            this.LogsList.ItemHeight = 16;
            this.LogsList.Location = new System.Drawing.Point(27, 341);
            this.LogsList.Name = "LogsList";
            this.LogsList.Size = new System.Drawing.Size(526, 148);
            this.LogsList.TabIndex = 21;
            // 
            // ChoosePanel
            // 
            this.ChoosePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChoosePanel.Controls.Add(this.ExplainText);
            this.ChoosePanel.Controls.Add(this.IDMButton);
            this.ChoosePanel.Controls.Add(this.DownloadWithText);
            this.ChoosePanel.Controls.Add(this.InAppButton);
            this.ChoosePanel.Location = new System.Drawing.Point(12, 12);
            this.ChoosePanel.Name = "ChoosePanel";
            this.ChoosePanel.Size = new System.Drawing.Size(558, 587);
            this.ChoosePanel.TabIndex = 101;
            // 
            // AppDownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 611);
            this.Controls.Add(this.ChoosePanel);
            this.Controls.Add(this.LogsList);
            this.Controls.Add(this.PendingLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ActionButton);
            this.Controls.Add(this.TimeLeftLabel);
            this.Controls.Add(this.TransferRateLabel);
            this.Controls.Add(this.DownloadedSpaceLabel);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.FileNameLabel);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.DownloadProgressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "AppDownloadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unity Downloader";
            this.ChoosePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button IDMButton;
        private System.Windows.Forms.Button InAppButton;
        private System.Windows.Forms.Label DownloadWithText;
        private System.Windows.Forms.Label ExplainText;
        private System.Windows.Forms.ProgressBar DownloadProgressBar;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label DownloadedSpaceLabel;
        private System.Windows.Forms.Label TransferRateLabel;
        private System.Windows.Forms.Label TimeLeftLabel;
        private System.Windows.Forms.Button ActionButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label PendingLabel;
        private System.Windows.Forms.ListBox LogsList;
        private System.Windows.Forms.Panel ChoosePanel;
    }
}