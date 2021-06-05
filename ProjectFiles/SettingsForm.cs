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
        #region Public Properties
        public string FolderPath { get; set; }
        public string UserName { get; set; }
        public bool HideSettingsInMainForm { get; set; }
        #endregion

        #region Constructors
        public SettingsForm()
        {
            InitializeComponent();
        }

        // Used when we already have settings but need to update them
        public SettingsForm(string folderPath, string userName, bool hideSettingsInMainForm)
        {
            InitializeComponent();

            // Try to use the folder path provided. If it no longer exists, leave default
            if (Directory.Exists(folderPath))
            {
                SettingsFolderTextBox.Text = folderPath;
                SetUserComboBoxOptions(folderPath);
                UserComboBox.SelectedIndex = UserComboBox.FindStringExact(userName);
                checkBox1.Checked = hideSettingsInMainForm;
            }
            else
            {
                MessageBox.Show("Error reading configuration! Please configure again.", "Ooops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

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
            FolderPath = SettingsFolderTextBox.Text;
            UserName = UserComboBox.Text;
            HideSettingsInMainForm = checkBox1.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
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
