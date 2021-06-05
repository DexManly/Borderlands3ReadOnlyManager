namespace Borderlands3ReadOnlyManager
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NickName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Class = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HotKey = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ReadOnly = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FolderLabel = new System.Windows.Forms.Label();
            this.UserLabel = new System.Windows.Forms.Label();
            this.ChangeSettingsButton = new System.Windows.Forms.Button();
            this.labelUserContent = new System.Windows.Forms.Label();
            this.labelFolderContent = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.AuthorLabel = new System.Windows.Forms.Label();
            this.GitRepoLabel = new System.Windows.Forms.Label();
            this.toggleLbl = new System.Windows.Forms.Label();
            this.borderlands3SaveFileBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderlands3SaveFileBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.NickName,
            this.Class,
            this.Level,
            this.DateTime,
            this.HotKey,
            this.ReadOnly});
            this.dataGridView1.Location = new System.Drawing.Point(12, 72);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.Size = new System.Drawing.Size(551, 321);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FileName.DataPropertyName = "FileName";
            this.FileName.FillWeight = 111.9289F;
            this.FileName.HeaderText = "File Name";
            this.FileName.MinimumWidth = 80;
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Width = 80;
            // 
            // NickName
            // 
            this.NickName.DataPropertyName = "NickName";
            this.NickName.HeaderText = "Nick Name";
            this.NickName.Name = "NickName";
            this.NickName.ReadOnly = true;
            // 
            // Class
            // 
            this.Class.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Class.DataPropertyName = "ClassName";
            this.Class.HeaderText = "Class";
            this.Class.MinimumWidth = 70;
            this.Class.Name = "Class";
            this.Class.ReadOnly = true;
            this.Class.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Class.Width = 70;
            // 
            // Level
            // 
            this.Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Level.DataPropertyName = "PlayerLevel";
            this.Level.HeaderText = "Lvl";
            this.Level.MinimumWidth = 50;
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Level.Width = 50;
            // 
            // DateTime
            // 
            this.DateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DateTime.DataPropertyName = "LastWriteTime";
            this.DateTime.HeaderText = "Last Modified";
            this.DateTime.MinimumWidth = 140;
            this.DateTime.Name = "DateTime";
            this.DateTime.ReadOnly = true;
            this.DateTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DateTime.Width = 140;
            // 
            // HotKey
            // 
            this.HotKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.HotKey.DataPropertyName = "HotKeyEnabled";
            this.HotKey.HeaderText = "HotKey Enable";
            this.HotKey.MinimumWidth = 50;
            this.HotKey.Name = "HotKey";
            this.HotKey.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.HotKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.HotKey.Width = 50;
            // 
            // ReadOnly
            // 
            this.ReadOnly.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ReadOnly.DataPropertyName = "IsReadOnly";
            this.ReadOnly.FillWeight = 76.14214F;
            this.ReadOnly.HeaderText = "Read Only";
            this.ReadOnly.MinimumWidth = 50;
            this.ReadOnly.Name = "ReadOnly";
            this.ReadOnly.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ReadOnly.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ReadOnly.Width = 50;
            // 
            // FolderLabel
            // 
            this.FolderLabel.AutoSize = true;
            this.FolderLabel.Location = new System.Drawing.Point(12, 18);
            this.FolderLabel.Name = "FolderLabel";
            this.FolderLabel.Size = new System.Drawing.Size(39, 13);
            this.FolderLabel.TabIndex = 3;
            this.FolderLabel.Text = "Folder:";
            // 
            // UserLabel
            // 
            this.UserLabel.AutoSize = true;
            this.UserLabel.Location = new System.Drawing.Point(12, 44);
            this.UserLabel.Name = "UserLabel";
            this.UserLabel.Size = new System.Drawing.Size(32, 13);
            this.UserLabel.TabIndex = 4;
            this.UserLabel.Text = "User:";
            // 
            // ChangeSettingsButton
            // 
            this.ChangeSettingsButton.Location = new System.Drawing.Point(503, 39);
            this.ChangeSettingsButton.Name = "ChangeSettingsButton";
            this.ChangeSettingsButton.Size = new System.Drawing.Size(60, 23);
            this.ChangeSettingsButton.TabIndex = 5;
            this.ChangeSettingsButton.Text = "Settings";
            this.ChangeSettingsButton.UseVisualStyleBackColor = true;
            this.ChangeSettingsButton.Click += new System.EventHandler(this.ChangeSettingsButton_Click);
            // 
            // labelUserContent
            // 
            this.labelUserContent.AutoSize = true;
            this.labelUserContent.Location = new System.Drawing.Point(57, 44);
            this.labelUserContent.Name = "labelUserContent";
            this.labelUserContent.Size = new System.Drawing.Size(45, 13);
            this.labelUserContent.TabIndex = 7;
            this.labelUserContent.Text = "[hidden]";
            // 
            // labelFolderContent
            // 
            this.labelFolderContent.AutoSize = true;
            this.labelFolderContent.Location = new System.Drawing.Point(57, 18);
            this.labelFolderContent.Name = "labelFolderContent";
            this.labelFolderContent.Size = new System.Drawing.Size(45, 13);
            this.labelFolderContent.TabIndex = 6;
            this.labelFolderContent.Text = "[hidden]";
            // 
            // AuthorLabel
            // 
            this.AuthorLabel.AutoSize = true;
            this.AuthorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AuthorLabel.ForeColor = System.Drawing.Color.Silver;
            this.AuthorLabel.Location = new System.Drawing.Point(497, 392);
            this.AuthorLabel.Name = "AuthorLabel";
            this.AuthorLabel.Size = new System.Drawing.Size(66, 12);
            this.AuthorLabel.TabIndex = 8;
            this.AuthorLabel.Text = "By: dex_manly";
            // 
            // GitRepoLabel
            // 
            this.GitRepoLabel.AutoSize = true;
            this.GitRepoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitRepoLabel.ForeColor = System.Drawing.Color.Silver;
            this.GitRepoLabel.Location = new System.Drawing.Point(12, 392);
            this.GitRepoLabel.Name = "GitRepoLabel";
            this.GitRepoLabel.Size = new System.Drawing.Size(217, 12);
            this.GitRepoLabel.TabIndex = 9;
            this.GitRepoLabel.Text = "GitHub: DexManly/Borderlands3ReadOnlyManager";
            // 
            // toggleLbl
            // 
            this.toggleLbl.AutoSize = true;
            this.toggleLbl.Location = new System.Drawing.Point(454, 18);
            this.toggleLbl.Name = "toggleLbl";
            this.toggleLbl.Size = new System.Drawing.Size(109, 13);
            this.toggleLbl.TabIndex = 10;
            this.toggleLbl.Text = "ReadOnly Hotkey: F8";
            // 
            // borderlands3SaveFileBindingSource
            // 
            this.borderlands3SaveFileBindingSource.DataSource = typeof(Borderlands3ReadOnlyManager.HelperClasses.Borderlands3SaveFile);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 405);
            this.Controls.Add(this.toggleLbl);
            this.Controls.Add(this.labelUserContent);
            this.Controls.Add(this.labelFolderContent);
            this.Controls.Add(this.ChangeSettingsButton);
            this.Controls.Add(this.UserLabel);
            this.Controls.Add(this.FolderLabel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.AuthorLabel);
            this.Controls.Add(this.GitRepoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Borderlands 3 Read Only Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.borderlands3SaveFileBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label FolderLabel;
        private System.Windows.Forms.Label UserLabel;
        private System.Windows.Forms.Button ChangeSettingsButton;
        private System.Windows.Forms.Label labelUserContent;
        private System.Windows.Forms.Label labelFolderContent;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label AuthorLabel;
        private System.Windows.Forms.Label GitRepoLabel;
        private System.Windows.Forms.Label toggleLbl;
        private System.Windows.Forms.BindingSource borderlands3SaveFileBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn NickName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Class;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HotKey;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ReadOnly;
    }
}

