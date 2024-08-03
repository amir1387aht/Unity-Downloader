using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Unity_Downloader
{
    public partial class SettingForm : Form
    {
        public static SettingData settingData;

        private static string settingSaveFile = "";
        private static string settingSaveDirectory = "unity_downloader";

        public static bool IsSettingLoaded = false;

        public SettingForm()
        {
            InitializeComponent();

            InitSettingPath();

            LoadSetting();

            LoadDNSSetting();
            LoadProxySetting();
        }

        public static void InitSettingPath()
        {
            settingSaveFile = Path.Combine(Path.GetTempPath(), settingSaveDirectory, "setting.json");
        }

        private void LoadDNSSetting()
        {
            ToggleDNSPanel(settingData.DNSEnabled, true);
            DNS_AlternateServer.Text = settingData.AlternateDNSServer;
            DNS_PrefrredServer.Text = settingData.PrefrredDNSServer;
        }

        private void LoadProxySetting()
        {
            ToggleProxyPanel(settingData.ProxyEnabled, true);
            Proxy_ServerPath.Text = settingData.ProxyPacFilePath;
            Proxy_BypassCheckBox.Checked = settingData.BypassProxyOnLocal;
        }

        private void DNSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleDNSPanel(DNSCheckBox.Checked);
        }

        private void ToggleDNSPanel(bool v, bool updateCheckBox = false)
        {
            DNSPanel.Enabled = v;

            if(updateCheckBox)
                DNSCheckBox.Checked = v;
        }

        private void ToggleProxyPanel(bool v, bool updateCheckBox = false)
        {
            Proxy_Panel.Enabled = v;

            if (updateCheckBox)
                ProxyCheckBox.Checked = v;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateDNS() || !ValidateProxy()) return;

            settingData = new SettingData(DNSCheckBox.Checked, DNS_PrefrredServer.Text, DNS_AlternateServer.Text, ProxyCheckBox.Checked, Proxy_BypassCheckBox.Checked, Proxy_ServerPath.Text);

            SaveSetting();

            ApplyDNS();

            Close();
        }

        public static void ApplyDNS()
        {
            Utilities.SetDNS(settingData.DNSEnabled ? string.Join(",", settingData.PrefrredDNSServer, settingData.AlternateDNSServer) : null);
        }

        private void ProxyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyPanel(ProxyCheckBox.Checked);
        }

        private bool ValidateProxy()
        {
            string pattern = @"^https?://(www\.)?.*\.pac$";

            if (!Regex.IsMatch(Proxy_ServerPath.Text, pattern))
            {
                MessageBox.Show($"{Proxy_ServerPath.Text} is not a valid .pac file path.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidateDNS()
        {
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

            if (!Regex.IsMatch(DNS_PrefrredServer.Text, pattern))
            {
                MessageBox.Show($"{DNS_PrefrredServer.Text} is not a valid IPv4 address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Regex.IsMatch(DNS_AlternateServer.Text, pattern))
            {
                MessageBox.Show($"{DNS_AlternateServer.Text} is not a valid IPv4 address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public static void LoadSetting()
        {
            try
            {
                if (!File.Exists(settingSaveFile))
                {
                    settingData = new SettingData();
                    return;
                }

                settingData = JsonConvert.DeserializeObject<SettingData>(File.ReadAllText(settingSaveFile));

                IsSettingLoaded = true;
            }
            catch(Exception e)
            {
                MessageBox.Show($"An error occurred during loading setting : {e.Message}, Close app and try again", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private static void SaveSetting()
        {
            try
            {
                File.WriteAllText(settingSaveFile, JsonConvert.SerializeObject(settingData));
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred during saving setting : {e.Message}, Close app and try again", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}

public class SettingData
{
    // DNS
    public bool DNSEnabled = false;
    public string PrefrredDNSServer = "178.22.122.100";
    public string AlternateDNSServer = "185.51.200.2";

    // Proxy
    public bool ProxyEnabled = false;
    public bool BypassProxyOnLocal = true;
    public string ProxyPacFilePath = "https://example.com/proxy/proxy.pac";

    public SettingData() { }

    public SettingData(bool DNSEnabled, string PrefrredDNSServer, string AlternateDNSServer, bool ProxyEnabled, bool BypassProxyOnLocal, string ProxyPacFilePath)
    {
        this.DNSEnabled = DNSEnabled;
        this.PrefrredDNSServer = PrefrredDNSServer;
        this.AlternateDNSServer = AlternateDNSServer;

        this.ProxyEnabled = ProxyEnabled;
        this.BypassProxyOnLocal = BypassProxyOnLocal;
        this.ProxyPacFilePath = ProxyPacFilePath;
    }
}