using Borderlands3ReadOnlyManager.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Borderlands3ReadOnlyManager
{
    public partial class MainForm : Form
    {
        #region Constants
        private readonly string HIDDEN = "[hidden]";
        private readonly int DIR_TRUNCATE_LEN = 65;
        private const int HOTKEY_MSG = 0x0312;
        private readonly DataGridViewCellStyle READONLY_RED_STYLE = new DataGridViewCellStyle() { BackColor = Color.LightCoral };
        private readonly DataGridViewCellStyle DEFAULT_STYLE = new DataGridViewCellStyle() { BackColor = Color.Empty };
        #endregion

        #region Member Variables
        private FileSystemWatcher _fileSystemWatcher = null;
        private string _directory = "";
        private string _ntAccountName = "";
        private bool _hideInfo = false;
        private List<string> _isHotKeyEnabledList = new List<string>();
        private SortableBindingList<Borderlands3SaveFile> _borderlands3FilesList = new SortableBindingList<Borderlands3SaveFile>(); 
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false; // Not a setting you can set in designer so I have to set it here :(
            ReadSettings();
            SetLabels();
            SetDataGridViewData();
            StartFileWatcher();
            RegisterHotKeys();
        } 
        #endregion

        #region File System Watcher
        private void StartFileWatcher()
        {
            if (string.IsNullOrWhiteSpace(_directory)) return;

            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = _directory;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess
                                            | NotifyFilters.LastWrite
                                            | NotifyFilters.FileName;
            _fileSystemWatcher.Filter = "*.sav";
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Created += OnCreate;
            _fileSystemWatcher.Deleted += OnDeleted;
            _fileSystemWatcher.Renamed += OnRenamed;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // TODO
            var file = _borderlands3FilesList.FirstOrDefault(a => a.FileName == e.Name);
            if (file != null)
            {
                Console.WriteLine("OnChange " + e.FullPath);
                file.ReReadFile(new FileInfo(e.FullPath));
                // Try to update row now. No lag please
                var row = dataGridView1.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[0].Value.ToString().Equals(e.Name)).FirstOrDefault();
                if (row != null)
                    dataGridView1.InvalidateRow(row.Index);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("OnRenamed " + e.OldFullPath + " to " + e.FullPath);
            // Handle only the Name changes... if it was an extension change (ex 5.sav~tmp123) do nothing...
            if (Path.GetExtension(e.FullPath).Equals(".sav", StringComparison.OrdinalIgnoreCase))
            {
                var file = _borderlands3FilesList.FirstOrDefault(a => a.FileName == e.OldName || a.FileName == e.Name);
                if (file != null)
                {
                    file.ReReadFile(new FileInfo(e.FullPath));
                    // Try to update row now. No lag please
                    var row = dataGridView1.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[0].Value.ToString().Equals(e.OldName)).FirstOrDefault();
                    if (row != null)
                        dataGridView1.InvalidateRow(row.Index);
                }
            }
        }

        private void OnCreate(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"OnCreate: {e.FullPath}");
            try
            {
                FileInfo fileInfo = new FileInfo(e.FullPath);
                Borderlands3SaveFile saveFile = new Borderlands3SaveFile(fileInfo, _ntAccountName, _isHotKeyEnabledList);
                if (saveFile.IsGSAVFile)
                {
                    Invoke(new Action(delegate () { _borderlands3FilesList.Add(saveFile); }));
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"OnDeleted: {e.FullPath}");
            var file = _borderlands3FilesList.FirstOrDefault(a => a.FileName == e.Name);
            if (file != null)
                Invoke(new Action(delegate () { _borderlands3FilesList.Remove(file); }));
        }
        #endregion

        #region Read/Save Config Settings
        /// <summary>
        /// Read Settings from config. If no settings are set yet, propmpt user...if no settings are returned, we close the application.
        /// </summary>
        private void ReadSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            _directory = config.AppSettings.Settings["folderPath"].Value;
            
            _ntAccountName = config.AppSettings.Settings["userName"].Value;
            
            // New feature. If we have it in config, use it. We will fall back to default empty list if not. (format: "1.sav|2.sav|9.sav")
            if (!string.IsNullOrWhiteSpace(config.AppSettings.Settings["HotkeyList"]?.Value))
                _isHotKeyEnabledList = config.AppSettings.Settings["HotkeyList"].Value.Split('|').ToList();

            if (bool.TryParse(config.AppSettings.Settings["hideSettingInMainForm"].Value, out bool hide))
            {
                _hideInfo = hide;
            }

            MaybePromptForInitialSettings();
            
            TryReadingGridSortOrderAndApply();
        }

        private void SaveSettingsToConfig()
        {
            // Find all files set as hotfix now. This keeps our file list clean if we get deletes we don't keep track of
            var hotlistRightNow = _borderlands3FilesList.Where(a => a.HotKeyEnabled).Select(a => a.FileName);
            
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
            config.AppSettings.Settings.Remove("folderPath");
            config.AppSettings.Settings.Remove("userName");
            config.AppSettings.Settings.Remove("hideSettingInMainForm");
            config.AppSettings.Settings.Remove("HotkeyList");

            config.AppSettings.Settings.Add("sortedColumnIndex", sortedIndex);
            config.AppSettings.Settings.Add("sortedColumnOrder", sortedOrder);
            config.AppSettings.Settings.Add("folderPath", _directory);
            config.AppSettings.Settings.Add("userName", _ntAccountName);
            config.AppSettings.Settings.Add("hideSettingInMainForm", _hideInfo.ToString());
            config.AppSettings.Settings.Add("HotkeyList", string.Join("|", hotlistRightNow)); // Save list of files currently macro. 1.sav|2.sav|3.sav
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void TryReadingGridSortOrderAndApply()
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
                if (!ChangedSettings(init: true))
                {
                    Load += (s, e) => Close(); // Can't exit during constructor but we can tell it to close after;
                    return;
                }
            }
        }

        private bool ChangedSettings(bool init = false)
        {
            SettingsForm settings = init ? new SettingsForm() : new SettingsForm(_directory, _ntAccountName, _hideInfo);
            settings.StartPosition = FormStartPosition.CenterParent;

            var result = settings.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                // Update settings if user said Ok (passes field validation needed items)
                _directory = settings.FolderPath;
                _ntAccountName = settings.UserName;
                _hideInfo = settings.HideSettingsInMainForm;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Label Setting Methods
        private void SetLabels()
        {
            labelFolderContent.Text = _hideInfo ? HIDDEN : GetDirectoryTextFormatted();
            toolTip1.SetToolTip(labelFolderContent, _hideInfo ? "" : GetDirectoryToolTipText());
            labelUserContent.Text = _hideInfo ? HIDDEN : _ntAccountName;
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
        #endregion

        #region DataGridView Helper Methods
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
                    Borderlands3SaveFile saveFile = new Borderlands3SaveFile(fileInfo, _ntAccountName, _isHotKeyEnabledList);
                    if (saveFile.IsGSAVFile)
                    {
                        _borderlands3FilesList.Add(saveFile);
                    }
                }
                catch (Exception)
                {
                }
            }

            dataGridView1.DataSource = _borderlands3FilesList;
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

        private void MaybeHandleHotKeyToggle(DataGridViewCellMouseEventArgs e)
        {
            int hotkeyColIndx = dataGridView1.Columns.IndexOf(HotKey);
            int fileNameColIndex = dataGridView1.Columns.IndexOf(FileName);

            if (e.ColumnIndex == hotkeyColIndx)
            {
                string hotkeyChangeFileName = dataGridView1.Rows[e.RowIndex].Cells[fileNameColIndex].Value.ToString();
                if ((bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                {
                    _isHotKeyEnabledList.Add(hotkeyChangeFileName);
                }
                else
                {
                    _isHotKeyEnabledList.Remove(hotkeyChangeFileName);
                }
            }
        }
        #endregion

        #region Form Events
        /// <summary>
        /// This is some trickery to make sure the check boxes actually toggle when you click on them. I would assume this would EndEdit after the click but nope.
        /// </summary>
        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;
            int readOnlyColIndx = dataGridView1.Columns.IndexOf(ReadOnly);
            int hotkeyColIndx = dataGridView1.Columns.IndexOf(HotKey);

            // End of edit on each click on column of checkbox
            if (e.ColumnIndex == hotkeyColIndx || e.ColumnIndex == readOnlyColIndx)
            {
                dataGridView1.EndEdit();
            }

            MaybeHandleHotKeyToggle(e);
        }

        private void ChangeSettingsButton_Click(object sender, EventArgs e)
        {
            if (_hideInfo)
            {
                var result = MessageBox.Show($"Settings will not be hidden in settings form. {Environment.NewLine}Do you want to continue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes && ChangedSettings())
                {
                    SetLabels();
                    SetDataGridViewData();
                    // StartFileWatcher(); ???
                }
            }
            else
            {
                if (ChangedSettings())
                {
                    SetLabels();
                    SetDataGridViewData();
                    // StartFileWatcher(); ???
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int readOnlyColIndx = dataGridView1.Columns.IndexOf(ReadOnly);

            if ((bool)dataGridView1.Rows[e.RowIndex].Cells[readOnlyColIndx].Value)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle = READONLY_RED_STYLE;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle = DEFAULT_STYLE;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettingsToConfig();
            UnregisterHotKey(this.Handle, 0);
            if (_fileSystemWatcher != null)
                _fileSystemWatcher.Dispose();
        } 
        #endregion

        #region Macro Setting
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void RegisterHotKeys()
        {
            RegisterHotKey(this.Handle, 0, (int)KeyModifier.None, Keys.F8.GetHashCode());
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == HOTKEY_MSG)
            {
                int hotKeyCol = dataGridView1.Columns.IndexOf(HotKey);
                int readOnlyColIndx = dataGridView1.Columns.IndexOf(ReadOnly);

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if ((bool)dataGridView1.Rows[i].Cells[hotKeyCol].Value)
                    {
                        string fileName = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        var saveFile = _borderlands3FilesList.First(a => a.FileName == fileName);
                        if (saveFile != null)
                        {
                            saveFile.IsReadOnly = !saveFile.IsReadOnly;
                        }
                        dataGridView1.InvalidateRow(i);
                    }
                }
            }
        }

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
        #endregion
    }
}
