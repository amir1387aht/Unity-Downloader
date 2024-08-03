namespace Unity_Downloader
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.HeaderLabel = new System.Windows.Forms.Label();
            this.EditorVersionLbl = new System.Windows.Forms.Label();
            this.EditorsListSelect = new System.Windows.Forms.DomainUpDown();
            this.AvailabelThingsToDownload = new System.Windows.Forms.Label();
            this.ItemsToDownload = new System.Windows.Forms.ListBox();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.LocateButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SettingButton = new System.Windows.Forms.Button();
            this.HeaderImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.HeaderImg)).BeginInit();
            this.SuspendLayout();
            // 
            // HeaderLabel
            // 
            this.HeaderLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HeaderLabel.Font = new System.Drawing.Font("Yu Gothic UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLabel.Location = new System.Drawing.Point(230, 21);
            this.HeaderLabel.Name = "HeaderLabel";
            this.HeaderLabel.Size = new System.Drawing.Size(220, 32);
            this.HeaderLabel.TabIndex = 0;
            this.HeaderLabel.Text = "Unity Downloader";
            // 
            // EditorVersionLbl
            // 
            this.EditorVersionLbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.EditorVersionLbl.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditorVersionLbl.Location = new System.Drawing.Point(172, 96);
            this.EditorVersionLbl.Name = "EditorVersionLbl";
            this.EditorVersionLbl.Size = new System.Drawing.Size(100, 19);
            this.EditorVersionLbl.TabIndex = 2;
            this.EditorVersionLbl.Text = "Editor Version : ";
            // 
            // EditorsListSelect
            // 
            this.EditorsListSelect.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.EditorsListSelect.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditorsListSelect.Location = new System.Drawing.Point(278, 94);
            this.EditorsListSelect.Name = "EditorsListSelect";
            this.EditorsListSelect.Size = new System.Drawing.Size(143, 25);
            this.EditorsListSelect.TabIndex = 4;
            this.EditorsListSelect.Text = "Editors";
            this.EditorsListSelect.SelectedItemChanged += new System.EventHandler(this.EditorsListSelect_SelectedItemChanged);
            // 
            // AvailabelThingsToDownload
            // 
            this.AvailabelThingsToDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AvailabelThingsToDownload.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AvailabelThingsToDownload.Location = new System.Drawing.Point(14, 140);
            this.AvailabelThingsToDownload.Name = "AvailabelThingsToDownload";
            this.AvailabelThingsToDownload.Size = new System.Drawing.Size(194, 19);
            this.AvailabelThingsToDownload.TabIndex = 5;
            this.AvailabelThingsToDownload.Text = "Available Items To Download : ";
            // 
            // ItemsToDownload
            // 
            this.ItemsToDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsToDownload.Font = new System.Drawing.Font("Yu Gothic UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemsToDownload.FormattingEnabled = true;
            this.ItemsToDownload.ItemHeight = 20;
            this.ItemsToDownload.Location = new System.Drawing.Point(35, 163);
            this.ItemsToDownload.Name = "ItemsToDownload";
            this.ItemsToDownload.Size = new System.Drawing.Size(515, 304);
            this.ItemsToDownload.TabIndex = 6;
            this.ItemsToDownload.SelectedIndexChanged += new System.EventHandler(this.ItemsToDownload_SelectedIndexChanged);
            // 
            // DownloadButton
            // 
            this.DownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadButton.Enabled = false;
            this.DownloadButton.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadButton.Location = new System.Drawing.Point(350, 491);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(200, 50);
            this.DownloadButton.TabIndex = 7;
            this.DownloadButton.Text = "Download";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // LocateButton
            // 
            this.LocateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocateButton.Enabled = false;
            this.LocateButton.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocateButton.Location = new System.Drawing.Point(35, 491);
            this.LocateButton.Name = "LocateButton";
            this.LocateButton.Size = new System.Drawing.Size(215, 50);
            this.LocateButton.TabIndex = 8;
            this.LocateButton.Text = "Locate And Install";
            this.LocateButton.UseVisualStyleBackColor = true;
            this.LocateButton.Click += new System.EventHandler(this.LocateButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // SettingButton
            // 
            this.SettingButton.BackgroundImage = global::Unity_Downloader.Properties.Resources.settings_490x512;
            this.SettingButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SettingButton.FlatAppearance.BorderSize = 0;
            this.SettingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingButton.Location = new System.Drawing.Point(12, 12);
            this.SettingButton.Name = "SettingButton";
            this.SettingButton.Size = new System.Drawing.Size(35, 36);
            this.SettingButton.TabIndex = 9;
            this.SettingButton.UseVisualStyleBackColor = true;
            this.SettingButton.Click += new System.EventHandler(this.SettingButton_Click);
            // 
            // HeaderImg
            // 
            this.HeaderImg.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HeaderImg.BackColor = System.Drawing.Color.Transparent;
            this.HeaderImg.Image = global::Unity_Downloader.Properties.Resources.unity;
            this.HeaderImg.Location = new System.Drawing.Point(176, 12);
            this.HeaderImg.Name = "HeaderImg";
            this.HeaderImg.Size = new System.Drawing.Size(45, 50);
            this.HeaderImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.HeaderImg.TabIndex = 1;
            this.HeaderImg.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.SettingButton);
            this.Controls.Add(this.LocateButton);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.ItemsToDownload);
            this.Controls.Add(this.AvailabelThingsToDownload);
            this.Controls.Add(this.EditorsListSelect);
            this.Controls.Add(this.EditorVersionLbl);
            this.Controls.Add(this.HeaderImg);
            this.Controls.Add(this.HeaderLabel);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unity Downloader";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.HeaderImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.PictureBox HeaderImg;
        private System.Windows.Forms.Label EditorVersionLbl;
        private System.Windows.Forms.DomainUpDown EditorsListSelect;
        private System.Windows.Forms.Label AvailabelThingsToDownload;
        private System.Windows.Forms.ListBox ItemsToDownload;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.Button LocateButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button SettingButton;
    }
}

