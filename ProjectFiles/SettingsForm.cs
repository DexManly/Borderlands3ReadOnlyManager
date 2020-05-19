using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Borderlands3ReadOnlyManager
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        #region Event Methods
        private void SelectFolderButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    SettingsFolderTextBox.Text = fbd.SelectedPath;
                    SetUserComboBoxOptions(SettingsFolderTextBox.Text);
                    SaveButton.Enabled = true;
                }
            }
        }

        private void SettingsFolderTextBox_Leave(object sender, EventArgs e)
        {
            ValidateFolderPathValue();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Remove("folderPath");
            config.AppSettings.Settings.Remove("userName");
            config.AppSettings.Settings.Remove("hideSettingInMainForm");
            config.AppSettings.Settings.Add("folderPath", SettingsFolderTextBox.Text);
            config.AppSettings.Settings.Add("userName", UserComboBox.Text);
            config.AppSettings.Settings.Add("hideSettingInMainForm", checkBox1.Checked.ToString());
            config.Save(ConfigurationSaveMode.Modified);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                string folderDir = config.AppSettings.Settings["folderPath"].Value;
                if (!string.IsNullOrWhiteSpace(folderDir) && Directory.Exists(folderDir))
                {
                    SettingsFolderTextBox.Text = folderDir;
                    SetUserComboBoxOptions(folderDir);
                    UserComboBox.SelectedIndex = UserComboBox.FindStringExact(config.AppSettings.Settings["userName"].Value);
                }

                if (bool.TryParse(config.AppSettings.Settings["hideSettingInMainForm"].Value, out bool hide))
                {
                    checkBox1.Checked = hide;
                }
            }
            catch
            {
                MessageBox.Show("Error reading configuration!", "Ooops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SettingsFolderTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidateFolderPathValue();
            }
        }
        #endregion

        #region Helper Methods
        private void ValidateFolderPathValue()
        {
            if (!string.IsNullOrWhiteSpace(SettingsFolderTextBox.Text) && Directory.Exists(SettingsFolderTextBox.Text))
            {
                SetUserComboBoxOptions(SettingsFolderTextBox.Text);
                SaveButton.Enabled = true;
            }
            else if (string.IsNullOrWhiteSpace(SettingsFolderTextBox.Text))
            {
                UserComboBox.Items.Clear();
                SaveButton.Enabled = false;
            }
            else
            {
                MessageBox.Show("Not a valid folder path!", "Invalid Setting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SettingsFolderTextBox.Text = "";
                SaveButton.Enabled = false;
            }
        }

        private void SetUserComboBoxOptions(string folderPath)
        {
            string currentlySelectedUser = UserComboBox.Text;

            string[] users = GetUsers(folderPath);
            UserComboBox.Items.Clear();
            UserComboBox.Items.AddRange(users);
            UserComboBox.SelectedIndex = UserComboBox.FindStringExact(currentlySelectedUser);
            if (UserComboBox.SelectedIndex == -1) 
                UserComboBox.SelectedIndex = 0;
        }

        private string[] GetUsers(string folderPath)
        {
            List<string> users = new List<string>();

            try
            {
                // Obtain our Access Control List (ACL)
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dirSec = dirInfo.GetAccessControl();

                foreach (FileSystemAccessRule rule in dirSec.GetAccessRules(true, true, typeof(NTAccount)))
                {
                    users.Add(rule.IdentityReference.ToString());
                }
            }
            catch
            {
            }

            return users.Distinct().ToArray();
        } 
        #endregion
    }
}
