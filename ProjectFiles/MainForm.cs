using Borderlands3ReadOnlyManager.HelperClasses;
using Gibbed.Borderlands3.SaveFormats;
using OakSave;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Borderlands3ReadOnlyManager
{
    public partial class MainForm : Form
    {
        private readonly string HIDDEN = "[hidden]";
        private readonly int DIR_TRUNCATE_LEN = 85; 

        private string _directory = "";
        private string _ntAccountName = "";
        private bool _hideInfo = false;

        public MainForm()
        {
            InitializeComponent();
            ReadSettingsAndSetLabels();
            ReadGridSortOrderAndApply();
            MaybePromptForInitialSettings();
        }

        private void ReadSettingsAndSetLabels()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            _directory = config.AppSettings.Settings["folderPath"].Value;
            _ntAccountName = config.AppSettings.Settings["userName"].Value;

            if (bool.TryParse(config.AppSettings.Settings["hideSettingInMainForm"].Value, out bool hide))
            {
                _hideInfo = hide;
                
                labelFolderContent.Text = _hideInfo ? HIDDEN : GetDirectoryTextFormatted();
                toolTip1.SetToolTip(labelFolderContent, _hideInfo ? "" : GetDirectoryToolTipText());
                
                labelUserContent.Text = _hideInfo ? HIDDEN : _ntAccountName;
            }
        }

        private void ReadGridSortOrderAndApply()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            string sortedColumnIndex = config.AppSettings.Settings["sortedColumnIndex"].Value;
            if (int.TryParse(sortedColumnIndex, out int intSortedColumnIndex))
            {
                string sortedColumnOrder = config.AppSettings.Settings["sortedColumnOrder"].Value;
                if (Enum.TryParse(sortedColumnOrder, out SortOrder sortOrder))
                {
                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            dataGridView1.Sort(dataGridView1.Columns[intSortedColumnIndex], ListSortDirection.Ascending);
                            break;
                        case SortOrder.Descending:
                            dataGridView1.Sort(dataGridView1.Columns[intSortedColumnIndex], ListSortDirection.Descending);
                            break;
                        case SortOrder.None:
                        default:
                            break;
                    }
                }
            }
        }

        private void MaybePromptForInitialSettings()
        {
            if (string.IsNullOrWhiteSpace(_directory) || string.IsNullOrWhiteSpace(_ntAccountName))
            {
                MessageBox.Show($"Please update settings on the next screen.{Environment.NewLine}{Environment.NewLine}NOTE: This is a new tool, you should regularly back up your data and make sure cloud saves are turned off before proceeding! The author is not responsible for any loss of data.", "New User / Settings Missing", MessageBoxButtons.OK, MessageBoxIcon.None);
                ChangeSettings();
            }
        }

        private void ChangeSettings()
        {
            SettingsForm settings = new SettingsForm();
            settings.StartPosition = FormStartPosition.CenterParent;
            var result = settings.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ReadSettingsAndSetLabels();
                SetDataGridViewData();
            }
        }

        private string GetDirectoryTextFormatted()
        {
            if (_directory.Length >= DIR_TRUNCATE_LEN)
                return _directory.Substring(0, DIR_TRUNCATE_LEN) + "...";
            else
                return _directory;
        }

        private string GetDirectoryToolTipText()
        {
            if (_directory.Length >= 50)
                return _directory;
            else
                return "";
        }

        private void SetDataGridViewData()
        {
            // Keep track of selected cell and file name before clearing and refreshing datagridview
            int selectedColIndex = 0;
            string selectedFileName = "";
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowIndex = dataGridView1.SelectedCells[0].RowIndex;
                selectedColIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                selectedFileName = dataGridView1.Rows[selectedrowIndex].Cells[0].Value.ToString();
                Console.WriteLine("Selected File Name: " + selectedFileName);
            }

            // keep track of any sorting that was applied
            int? sortedColumnIndex = null;
            SortOrder sortOrder = SortOrder.None;
            if (dataGridView1.SortedColumn != null)
            {
                sortedColumnIndex = dataGridView1.SortedColumn.Index;
                sortOrder = dataGridView1.SortOrder;
            }

            ResetDataGridViewWithFiles();
            ResetDataGridViewSortIfAny(sortedColumnIndex, sortOrder);
            ResetCurrentlySelectedCell(selectedFileName, selectedColIndex);
        }

        private void ResetDataGridViewWithFiles()
        {
            dataGridView1.Rows.Clear();

            IEnumerable<FileInfo> fileInfoList = new List<FileInfo>();
            if (Directory.Exists(_directory))
                fileInfoList = new DirectoryInfo(_directory).EnumerateFiles("*.sav");

            // Get meta data from files.
            foreach (FileInfo fileInfo in fileInfoList)
            {
                try
                {
                    Borderlands3SaveFile saveFile = new Borderlands3SaveFile(fileInfo);
                    if (saveFile.IsGSAVFile)
                    {
                        dataGridView1.Rows.Add(saveFile.FileName, 
                                               saveFile.MetaData.NickName, 
                                               saveFile.MetaData.ClassName, 
                                               saveFile.MetaData.PlayerLevel, 
                                               fileInfo.LastWriteTime, saveFile.
                                               IsFileReadOnly(_ntAccountName));
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void ResetDataGridViewSortIfAny(int? sortedColumnIndex, SortOrder sortOrder)
        {
            if (sortedColumnIndex.HasValue)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        dataGridView1.Sort(dataGridView1.Columns[sortedColumnIndex.Value], ListSortDirection.Ascending);
                        break;
                    case SortOrder.Descending:
                        dataGridView1.Sort(dataGridView1.Columns[sortedColumnIndex.Value], ListSortDirection.Descending);
                        break;
                    case SortOrder.None:
                    default:
                        break;
                }
            }
        }

        private void ResetCurrentlySelectedCell(string selectedFileName, int selectedColIndex)
        {
            if (!string.IsNullOrWhiteSpace(selectedFileName))
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value.Equals(selectedFileName))
                    {
                        row.Cells[selectedColIndex].Selected = true;
                        break;
                    }
                }
            }
        }

        private void SetReadOnlyForFile(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                fi.IsReadOnly = true;
                FileSecurity fs = new FileSecurity();
                fs.AddAccessRule(new FileSystemAccessRule(_ntAccountName, FileSystemRights.Write, AccessControlType.Deny));
                fi.SetAccessControl(fs);
            }
            catch
            {
            }
        }

        private void RemoveReadOnlyOnFile(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                FileSecurity fs = new FileSecurity();
                fs.RemoveAccessRule(new FileSystemAccessRule(_ntAccountName, FileSystemRights.Write, AccessControlType.Deny));
                fi.SetAccessControl(fs);
                fi.IsReadOnly = false;
            }
            catch
            {
            }
        }

        private void SaveSortOrderInfoToConfig()
        {
            string sortedIndex = "";
            string sortedOrder = "";

            if (dataGridView1.SortedColumn != null)
            {
                sortedIndex = dataGridView1.SortedColumn.Index.ToString();
                sortedOrder = dataGridView1.SortOrder.ToString();
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Remove("sortedColumnIndex");
            config.AppSettings.Settings.Remove("sortedColumnOrder");
            config.AppSettings.Settings.Add("sortedColumnIndex", sortedIndex);
            config.AppSettings.Settings.Add("sortedColumnOrder", sortedOrder);
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns.IndexOf(ReadOnly) && e.RowIndex != -1)
            {
                if ((bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                {
                    SetReadOnlyForFile(Path.Combine(_directory, dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                }
                else
                {
                    RemoveReadOnlyOnFile(Path.Combine(_directory, dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                }
            }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edit on each click on column of checkbox
            if (e.ColumnIndex == dataGridView1.Columns.IndexOf(ReadOnly) && e.RowIndex != -1)
            {
                dataGridView1.EndEdit();
            }
        }

        private void ChangeSettingsButton_Click(object sender, EventArgs e)
        {
            if (_hideInfo)
            {
                var result = MessageBox.Show($"Settings will not be hidden in settings form. {Environment.NewLine}Do you want to continue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    ChangeSettings();
            }
            else
            {
                ChangeSettings();
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            SetDataGridViewData();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSortOrderInfoToConfig();
        }
    }
}
