namespace Borderlands3ReadOnlyManager
{
    partial class SettingsForm
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
            this.SettingsFolderLabel = new System.Windows.Forms.Label();
            this.SettingsFolderTextBox = new System.Windows.Forms.TextBox();
            this.UserLabel = new System.Windows.Forms.Label();
            this.UserComboBox = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SelectFolderButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SettingsFolderLabel
            // 
            this.SettingsFolderLabel.AutoSize = true;
            this.SettingsFolderLabel.Location = new System.Drawing.Point(9, 14);
            this.SettingsFolderLabel.Name = "SettingsFolderLabel";
            this.SettingsFolderLabel.Size = new System.Drawing.Size(39, 13);
            this.SettingsFolderLabel.TabIndex = 0;
            this.SettingsFolderLabel.Text = "Folder:";
            // 
            // SettingsFolderTextBox
            // 
            this.SettingsFolderTextBox.Location = new System.Drawing.Point(47, 11);
            this.SettingsFolderTextBox.Name = "SettingsFolderTextBox";
            this.SettingsFolderTextBox.Size = new System.Drawing.Size(348, 20);
            this.SettingsFolderTextBox.TabIndex = 1;
            this.SettingsFolderTextBox.Leave += new System.EventHandler(this.SettingsFolderTextBox_Leave);
            this.SettingsFolderTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.SettingsFolderTextBox_PreviewKeyDown);
            // 
            // UserLabel
            // 
            this.UserLabel.AutoSize = true;
            this.UserLabel.Location = new System.Drawing.Point(9, 45);
            this.UserLabel.Name = "UserLabel";
            this.UserLabel.Size = new System.Drawing.Size(32, 13);
            this.UserLabel.TabIndex = 2;
            this.UserLabel.Text = "User:";
            // 
            // UserComboBox
            // 
            this.UserComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UserComboBox.FormattingEnabled = true;
            this.UserComboBox.Location = new System.Drawing.Point(47, 42);
            this.UserComboBox.Name = "UserComboBox";
            this.UserComboBox.Size = new System.Drawing.Size(388, 21);
            this.UserComboBox.TabIndex = 3;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 75);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(150, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Hide settings on main form";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // SelectFolderButton
            // 
            this.SelectFolderButton.Location = new System.Drawing.Point(401, 9);
            this.SelectFolderButton.Name = "SelectFolderButton";
            this.SelectFolderButton.Size = new System.Drawing.Size(34, 23);
            this.SelectFolderButton.TabIndex = 2;
            this.SelectFolderButton.Text = "...";
            this.SelectFolderButton.UseVisualStyleBackColor = true;
            this.SelectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Enabled = false;
            this.SaveButton.Location = new System.Drawing.Point(360, 71);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 102);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.SelectFolderButton);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.UserComboBox);
            this.Controls.Add(this.UserLabel);
            this.Controls.Add(this.SettingsFolderTextBox);
            this.Controls.Add(this.SettingsFolderLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SettingsFolderLabel;
        private System.Windows.Forms.TextBox SettingsFolderTextBox;
        private System.Windows.Forms.Label UserLabel;
        private System.Windows.Forms.ComboBox UserComboBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button SelectFolderButton;
        private System.Windows.Forms.Button SaveButton;
    }
}