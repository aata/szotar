namespace Szotar.WindowsForms.Controls {
    partial class QuizletSetSelector {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.searchTab = new System.Windows.Forms.TabPage();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.searchButton = new System.Windows.Forms.Button();
            this.searchBox = new Szotar.WindowsForms.Controls.SearchBox();
            this.searchResults = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.creatorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.termsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.idColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.manualTab = new System.Windows.Forms.TabPage();
            this.manualInputInstructions = new System.Windows.Forms.Label();
            this.manualInput = new System.Windows.Forms.TextBox();
            this.manualInputLabel = new System.Windows.Forms.Label();
            this.dialogButtonsPanel = new System.Windows.Forms.Panel();
            this.attribution = new System.Windows.Forms.LinkLabel();
            this.importButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabControl.SuspendLayout();
            this.searchTab.SuspendLayout();
            this.manualTab.SuspendLayout();
            this.dialogButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.searchTab);
            this.tabControl.Controls.Add(this.manualTab);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(591, 358);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            // 
            // searchTab
            // 
            this.searchTab.Controls.Add(this.progressBar);
            this.searchTab.Controls.Add(this.searchButton);
            this.searchTab.Controls.Add(this.searchBox);
            this.searchTab.Controls.Add(this.searchResults);
            this.searchTab.Location = new System.Drawing.Point(4, 22);
            this.searchTab.Name = "searchTab";
            this.searchTab.Padding = new System.Windows.Forms.Padding(3);
            this.searchTab.Size = new System.Drawing.Size(583, 332);
            this.searchTab.TabIndex = 0;
            this.searchTab.Text = "Search";
            this.searchTab.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(399, 7);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(71, 21);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(475, 7);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(101, 23);
            this.searchButton.TabIndex = 2;
            this.searchButton.Text = "&Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.Location = new System.Drawing.Point(6, 7);
            this.searchBox.Name = "searchBox";
            this.searchBox.PromptText = "Search";
            this.searchBox.Size = new System.Drawing.Size(387, 21);
            this.searchBox.TabIndex = 0;
            this.searchBox.Search += new System.EventHandler(this.searchBox_Search);
            // 
            // searchResults
            // 
            this.searchResults.AllowColumnReorder = true;
            this.searchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.creatorColumn,
            this.dateColumn,
            this.termsColumn,
            this.idColumn});
            this.searchResults.FullRowSelect = true;
            this.searchResults.Location = new System.Drawing.Point(6, 34);
            this.searchResults.MultiSelect = false;
            this.searchResults.Name = "searchResults";
            this.searchResults.Size = new System.Drawing.Size(571, 292);
            this.searchResults.TabIndex = 3;
            this.searchResults.UseCompatibleStateImageBehavior = false;
            this.searchResults.View = System.Windows.Forms.View.Details;
            this.searchResults.ItemActivate += new System.EventHandler(this.searchResults_ItemActivate);
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 250;
            // 
            // creatorColumn
            // 
            this.creatorColumn.Text = "Creator";
            this.creatorColumn.Width = 120;
            // 
            // dateColumn
            // 
            this.dateColumn.Text = "Date";
            this.dateColumn.Width = 100;
            // 
            // termsColumn
            // 
            this.termsColumn.Text = "Terms";
            // 
            // idColumn
            // 
            this.idColumn.Text = "ID";
            this.idColumn.Width = 50;
            // 
            // manualTab
            // 
            this.manualTab.Controls.Add(this.manualInputInstructions);
            this.manualTab.Controls.Add(this.manualInput);
            this.manualTab.Controls.Add(this.manualInputLabel);
            this.manualTab.Location = new System.Drawing.Point(4, 22);
            this.manualTab.Name = "manualTab";
            this.manualTab.Padding = new System.Windows.Forms.Padding(3);
            this.manualTab.Size = new System.Drawing.Size(583, 332);
            this.manualTab.TabIndex = 1;
            this.manualTab.Text = "Manual Entry";
            this.manualTab.UseVisualStyleBackColor = true;
            // 
            // manualInputInstructions
            // 
            this.manualInputInstructions.AutoSize = true;
            this.manualInputInstructions.Location = new System.Drawing.Point(9, 51);
            this.manualInputInstructions.Name = "manualInputInstructions";
            this.manualInputInstructions.Size = new System.Drawing.Size(374, 13);
            this.manualInputInstructions.TabIndex = 2;
            this.manualInputInstructions.Text = "E.g. http://quizlet.com/set/14753, 14753, or http://quizlet.com/export/14753";
            // 
            // manualInput
            // 
            this.manualInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.manualInput.Location = new System.Drawing.Point(9, 23);
            this.manualInput.Name = "manualInput";
            this.manualInput.Size = new System.Drawing.Size(540, 20);
            this.manualInput.TabIndex = 1;
            this.manualInput.TextChanged += new System.EventHandler(this.manualInput_TextChanged);
            this.manualInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.manualInput_KeyPress);
            // 
            // manualInputLabel
            // 
            this.manualInputLabel.AutoSize = true;
            this.manualInputLabel.Location = new System.Drawing.Point(6, 7);
            this.manualInputLabel.Name = "manualInputLabel";
            this.manualInputLabel.Size = new System.Drawing.Size(74, 13);
            this.manualInputLabel.TabIndex = 0;
            this.manualInputLabel.Text = "Set URL or ID";
            // 
            // dialogButtonsPanel
            // 
            this.dialogButtonsPanel.Controls.Add(this.attribution);
            this.dialogButtonsPanel.Controls.Add(this.importButton);
            this.dialogButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dialogButtonsPanel.Location = new System.Drawing.Point(0, 361);
            this.dialogButtonsPanel.Name = "dialogButtonsPanel";
            this.dialogButtonsPanel.Size = new System.Drawing.Size(591, 29);
            this.dialogButtonsPanel.TabIndex = 1;
            // 
            // attribution
            // 
            this.attribution.AutoSize = true;
            this.attribution.Location = new System.Drawing.Point(3, 4);
            this.attribution.Name = "attribution";
            this.attribution.Size = new System.Drawing.Size(121, 13);
            this.attribution.TabIndex = 1;
            this.attribution.TabStop = true;
            this.attribution.Text = "Powered by Quizlet.com";
            this.attribution.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.attribution_LinkClicked);
            // 
            // importButton
            // 
            this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.importButton.Location = new System.Drawing.Point(524, 3);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(64, 23);
            this.importButton.TabIndex = 0;
            this.importButton.Text = "&Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // QuizletSetSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dialogButtonsPanel);
            this.Controls.Add(this.tabControl);
            this.Name = "QuizletSetSelector";
            this.Size = new System.Drawing.Size(591, 390);
            this.tabControl.ResumeLayout(false);
            this.searchTab.ResumeLayout(false);
            this.manualTab.ResumeLayout(false);
            this.manualTab.PerformLayout();
            this.dialogButtonsPanel.ResumeLayout(false);
            this.dialogButtonsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage searchTab;
        private System.Windows.Forms.TabPage manualTab;
        private System.Windows.Forms.Button searchButton;
        private Szotar.WindowsForms.Controls.SearchBox searchBox;
        private System.Windows.Forms.ListView searchResults;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader creatorColumn;
        private System.Windows.Forms.ColumnHeader dateColumn;
        private System.Windows.Forms.ColumnHeader termsColumn;
        private System.Windows.Forms.ColumnHeader idColumn;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel dialogButtonsPanel;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Label manualInputInstructions;
        private System.Windows.Forms.TextBox manualInput;
        private System.Windows.Forms.Label manualInputLabel;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.LinkLabel attribution;
    }
}
