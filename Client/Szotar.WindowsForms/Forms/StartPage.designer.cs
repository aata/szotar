namespace Szotar.WindowsForms.Forms {
	partial class StartPage {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TableLayoutPanel mainTabTable;
			System.Windows.Forms.TableLayoutPanel recentDictionariesTable;
			System.Windows.Forms.Label label1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
			System.Windows.Forms.TableLayoutPanel recentListsTable;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label testingLabel;
			System.Windows.Forms.TableLayoutPanel listSearchTable;
			System.Windows.Forms.FlowLayoutPanel listSearchButtons;
			System.Windows.Forms.ColumnHeader recentDictionariesTitleColumn;
			System.Windows.Forms.ColumnHeader Name;
			System.Windows.Forms.ColumnHeader dictionariesNameColumn;
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
			this.practiceTable = new System.Windows.Forms.TableLayoutPanel();
			this.reportBug = new System.Windows.Forms.LinkLabel();
			this.dictionaryTab = new System.Windows.Forms.TabPage();
			this.practiceTab = new System.Windows.Forms.TabPage();
			this.tasks = new System.Windows.Forms.TabControl();
			this.mainTab = new System.Windows.Forms.TabPage();
			this.fileSystemWatcher = new System.IO.FileSystemWatcher();
			this.practiceList = new System.Windows.Forms.Button();
			this.openList = new System.Windows.Forms.Button();
			this.recentDictionaries = new Szotar.WindowsForms.Controls.ListViewNF();
			this.recentLists = new Szotar.WindowsForms.Controls.ListViewNF();
			this.dictionaries = new Szotar.WindowsForms.Controls.ListViewNF();
			this.dictionariesSizeColumn = new System.Windows.Forms.ColumnHeader();
			this.dictionariesAuthorColumn = new System.Windows.Forms.ColumnHeader();
			this.listSearch = new Szotar.WindowsForms.Controls.ListSearch();
			mainTabTable = new System.Windows.Forms.TableLayoutPanel();
			recentDictionariesTable = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			recentListsTable = new System.Windows.Forms.TableLayoutPanel();
			label3 = new System.Windows.Forms.Label();
			testingLabel = new System.Windows.Forms.Label();
			listSearchTable = new System.Windows.Forms.TableLayoutPanel();
			listSearchButtons = new System.Windows.Forms.FlowLayoutPanel();
			recentDictionariesTitleColumn = new System.Windows.Forms.ColumnHeader();
			Name = new System.Windows.Forms.ColumnHeader();
			dictionariesNameColumn = new System.Windows.Forms.ColumnHeader();
			mainTabTable.SuspendLayout();
			recentDictionariesTable.SuspendLayout();
			recentListsTable.SuspendLayout();
			this.practiceTable.SuspendLayout();
			this.dictionaryTab.SuspendLayout();
			this.practiceTab.SuspendLayout();
			this.tasks.SuspendLayout();
			this.mainTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
			listSearchTable.SuspendLayout();
			listSearchButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTabTable
			// 
			mainTabTable.ColumnCount = 2;
			mainTabTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			mainTabTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			mainTabTable.Controls.Add(recentDictionariesTable, 0, 0);
			mainTabTable.Controls.Add(recentListsTable, 1, 0);
			mainTabTable.Controls.Add(this.practiceTable, 0, 1);
			mainTabTable.Dock = System.Windows.Forms.DockStyle.Fill;
			mainTabTable.Location = new System.Drawing.Point(3, 3);
			mainTabTable.Name = "mainTabTable";
			mainTabTable.RowCount = 2;
			mainTabTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			mainTabTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			mainTabTable.Size = new System.Drawing.Size(547, 362);
			mainTabTable.TabIndex = 0;
			// 
			// recentDictionariesTable
			// 
			recentDictionariesTable.ColumnCount = 1;
			recentDictionariesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			recentDictionariesTable.Controls.Add(label1, 0, 0);
			recentDictionariesTable.Controls.Add(this.recentDictionaries, 0, 1);
			recentDictionariesTable.Dock = System.Windows.Forms.DockStyle.Fill;
			recentDictionariesTable.Location = new System.Drawing.Point(3, 3);
			recentDictionariesTable.Name = "recentDictionariesTable";
			recentDictionariesTable.RowCount = 2;
			recentDictionariesTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			recentDictionariesTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			recentDictionariesTable.Size = new System.Drawing.Size(267, 175);
			recentDictionariesTable.TabIndex = 0;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(109, 13);
			label1.TabIndex = 0;
			label1.Text = "Recent Dictionaries:";
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Dictionary");
			this.imageList.Images.SetKeyName(1, "List");
			// 
			// imageListSmall
			// 
			this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
			this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListSmall.Images.SetKeyName(0, "Dictionary");
			this.imageListSmall.Images.SetKeyName(1, "List");
			// 
			// recentListsTable
			// 
			recentListsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			recentListsTable.ColumnCount = 1;
			recentListsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			recentListsTable.Controls.Add(label3, 0, 0);
			recentListsTable.Controls.Add(this.recentLists, 0, 1);
			recentListsTable.Location = new System.Drawing.Point(276, 3);
			recentListsTable.Name = "recentListsTable";
			recentListsTable.RowCount = 2;
			recentListsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			recentListsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			recentListsTable.Size = new System.Drawing.Size(268, 175);
			recentListsTable.TabIndex = 2;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(3, 0);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(102, 13);
			label3.TabIndex = 0;
			label3.Text = "Recent Word Lists:";
			// 
			// practiceTable
			// 
			this.practiceTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.practiceTable.ColumnCount = 1;
			this.practiceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.practiceTable.Controls.Add(testingLabel, 0, 0);
			this.practiceTable.Controls.Add(this.reportBug, 0, 1);
			this.practiceTable.Location = new System.Drawing.Point(3, 184);
			this.practiceTable.Name = "practiceTable";
			this.practiceTable.RowCount = 2;
			this.practiceTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.practiceTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.practiceTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.practiceTable.Size = new System.Drawing.Size(267, 175);
			this.practiceTable.TabIndex = 3;
			// 
			// testingLabel
			// 
			testingLabel.AutoSize = true;
			testingLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			testingLabel.Location = new System.Drawing.Point(3, 0);
			testingLabel.Name = "testingLabel";
			testingLabel.Size = new System.Drawing.Size(252, 39);
			testingLabel.TabIndex = 3;
			testingLabel.Text = "This is an early testing version of Szótár. Not all of the features are there yet" +
				". Feedback is welcome:";
			// 
			// reportBug
			// 
			this.reportBug.AutoSize = true;
			this.reportBug.Location = new System.Drawing.Point(3, 39);
			this.reportBug.Name = "reportBug";
			this.reportBug.Size = new System.Drawing.Size(172, 13);
			this.reportBug.TabIndex = 4;
			this.reportBug.TabStop = true;
			this.reportBug.Text = "Report bugs or request features";
			this.reportBug.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportBug_LinkClicked);
			// 
			// dictionaryTab
			// 
			this.dictionaryTab.Controls.Add(this.dictionaries);
			this.dictionaryTab.Location = new System.Drawing.Point(4, 22);
			this.dictionaryTab.Name = "dictionaryTab";
			this.dictionaryTab.Padding = new System.Windows.Forms.Padding(3);
			this.dictionaryTab.Size = new System.Drawing.Size(553, 368);
			this.dictionaryTab.TabIndex = 0;
			this.dictionaryTab.Text = "Dictionaries";
			this.dictionaryTab.UseVisualStyleBackColor = true;
			// 
			// practiceTab
			// 
			this.practiceTab.Controls.Add(listSearchTable);
			this.practiceTab.Location = new System.Drawing.Point(4, 22);
			this.practiceTab.Name = "practiceTab";
			this.practiceTab.Padding = new System.Windows.Forms.Padding(3);
			this.practiceTab.Size = new System.Drawing.Size(553, 368);
			this.practiceTab.TabIndex = 1;
			this.practiceTab.Text = "Word Lists";
			this.practiceTab.UseVisualStyleBackColor = true;
			// 
			// tasks
			// 
			this.tasks.Controls.Add(this.mainTab);
			this.tasks.Controls.Add(this.dictionaryTab);
			this.tasks.Controls.Add(this.practiceTab);
			this.tasks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tasks.Location = new System.Drawing.Point(3, 3);
			this.tasks.Name = "tasks";
			this.tasks.SelectedIndex = 0;
			this.tasks.Size = new System.Drawing.Size(561, 394);
			this.tasks.TabIndex = 0;
			this.tasks.Selected += new System.Windows.Forms.TabControlEventHandler(this.tasks_Selected);
			// 
			// mainTab
			// 
			this.mainTab.Controls.Add(mainTabTable);
			this.mainTab.Location = new System.Drawing.Point(4, 22);
			this.mainTab.Name = "mainTab";
			this.mainTab.Padding = new System.Windows.Forms.Padding(3);
			this.mainTab.Size = new System.Drawing.Size(553, 368);
			this.mainTab.TabIndex = 2;
			this.mainTab.Text = "Szótár";
			this.mainTab.UseVisualStyleBackColor = true;
			// 
			// fileSystemWatcher
			// 
			this.fileSystemWatcher.EnableRaisingEvents = true;
			this.fileSystemWatcher.IncludeSubdirectories = true;
			this.fileSystemWatcher.SynchronizingObject = this;
			// 
			// listSearchTable
			// 
			listSearchTable.ColumnCount = 1;
			listSearchTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			listSearchTable.Controls.Add(this.listSearch, 0, 0);
			listSearchTable.Controls.Add(listSearchButtons, 0, 1);
			listSearchTable.Dock = System.Windows.Forms.DockStyle.Fill;
			listSearchTable.Location = new System.Drawing.Point(3, 3);
			listSearchTable.Name = "listSearchTable";
			listSearchTable.RowCount = 2;
			listSearchTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			listSearchTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			listSearchTable.Size = new System.Drawing.Size(547, 362);
			listSearchTable.TabIndex = 5;
			// 
			// listSearchButtons
			// 
			listSearchButtons.AutoSize = true;
			listSearchButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			listSearchButtons.Controls.Add(this.practiceList);
			listSearchButtons.Controls.Add(this.openList);
			listSearchButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			listSearchButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			listSearchButtons.Location = new System.Drawing.Point(0, 333);
			listSearchButtons.Margin = new System.Windows.Forms.Padding(0);
			listSearchButtons.Name = "listSearchButtons";
			listSearchButtons.Size = new System.Drawing.Size(547, 29);
			listSearchButtons.TabIndex = 11;
			// 
			// practiceList
			// 
			this.practiceList.Location = new System.Drawing.Point(469, 3);
			this.practiceList.Name = "practiceList";
			this.practiceList.Size = new System.Drawing.Size(75, 23);
			this.practiceList.TabIndex = 7;
			this.practiceList.Text = "&Practice";
			this.practiceList.UseVisualStyleBackColor = true;
			this.practiceList.Click += new System.EventHandler(this.practiceList_Click);
			// 
			// openList
			// 
			this.openList.Location = new System.Drawing.Point(388, 3);
			this.openList.Name = "openList";
			this.openList.Size = new System.Drawing.Size(75, 23);
			this.openList.TabIndex = 6;
			this.openList.Text = "&Open";
			this.openList.UseVisualStyleBackColor = true;
			this.openList.Click += new System.EventHandler(this.openList_Click);
			// 
			// recentDictionaries
			// 
			this.recentDictionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.recentDictionaries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            recentDictionariesTitleColumn});
			this.recentDictionaries.FullRowSelect = true;
			this.recentDictionaries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.recentDictionaries.LargeImageList = this.imageList;
			this.recentDictionaries.Location = new System.Drawing.Point(3, 16);
			this.recentDictionaries.Name = "recentDictionaries";
			this.recentDictionaries.Size = new System.Drawing.Size(261, 156);
			this.recentDictionaries.SmallImageList = this.imageListSmall;
			this.recentDictionaries.TabIndex = 1;
			this.recentDictionaries.UseCompatibleStateImageBehavior = false;
			this.recentDictionaries.View = System.Windows.Forms.View.Details;
			// 
			// recentLists
			// 
			this.recentLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.recentLists.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            Name});
			this.recentLists.FullRowSelect = true;
			this.recentLists.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.recentLists.LargeImageList = this.imageList;
			this.recentLists.Location = new System.Drawing.Point(3, 16);
			this.recentLists.Name = "recentLists";
			this.recentLists.Size = new System.Drawing.Size(262, 156);
			this.recentLists.SmallImageList = this.imageListSmall;
			this.recentLists.TabIndex = 1;
			this.recentLists.UseCompatibleStateImageBehavior = false;
			this.recentLists.View = System.Windows.Forms.View.Details;
			// 
			// dictionaries
			// 
			this.dictionaries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            dictionariesNameColumn,
            this.dictionariesSizeColumn,
            this.dictionariesAuthorColumn});
			this.dictionaries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dictionaries.FullRowSelect = true;
			this.dictionaries.LargeImageList = this.imageList;
			this.dictionaries.Location = new System.Drawing.Point(3, 3);
			this.dictionaries.MultiSelect = false;
			this.dictionaries.Name = "dictionaries";
			this.dictionaries.ShowItemToolTips = true;
			this.dictionaries.Size = new System.Drawing.Size(547, 362);
			this.dictionaries.SmallImageList = this.imageListSmall;
			this.dictionaries.TabIndex = 0;
			this.dictionaries.UseCompatibleStateImageBehavior = false;
			this.dictionaries.View = System.Windows.Forms.View.Details;
			this.dictionaries.ItemActivate += new System.EventHandler(this.OnDictionaryItemActivate);
			// 
			// dictionariesNameColumn
			// 
			dictionariesNameColumn.Text = "Name";
			dictionariesNameColumn.Width = 312;
			// 
			// dictionariesSizeColumn
			// 
			this.dictionariesSizeColumn.Text = "Entries";
			this.dictionariesSizeColumn.Width = 75;
			// 
			// dictionariesAuthorColumn
			// 
			this.dictionariesAuthorColumn.Text = "Author";
			this.dictionariesAuthorColumn.Width = 134;
			// 
			// listSearch
			// 
			this.listSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listSearch.Location = new System.Drawing.Point(0, 0);
			this.listSearch.Margin = new System.Windows.Forms.Padding(0);
			this.listSearch.MaxItems = 100;
			this.listSearch.Name = "listSearch";
			this.listSearch.Size = new System.Drawing.Size(547, 333);
			this.listSearch.TabIndex = 12;
			this.listSearch.ListsChosen += new System.EventHandler<Szotar.WindowsForms.Controls.ListsChosenEventArgs>(this.listSearch_ListsChosen);
			// 
			// StartPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(567, 400);
			this.Controls.Add(this.tasks);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(300, 200);
			this.Name = "StartPage";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.Text = "Szótár";
			mainTabTable.ResumeLayout(false);
			recentDictionariesTable.ResumeLayout(false);
			recentDictionariesTable.PerformLayout();
			recentListsTable.ResumeLayout(false);
			recentListsTable.PerformLayout();
			this.practiceTable.ResumeLayout(false);
			this.practiceTable.PerformLayout();
			this.dictionaryTab.ResumeLayout(false);
			this.practiceTab.ResumeLayout(false);
			this.tasks.ResumeLayout(false);
			this.mainTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
			listSearchTable.ResumeLayout(false);
			listSearchTable.PerformLayout();
			listSearchButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tasks;
		private Szotar.WindowsForms.Controls.ListViewNF dictionaries;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.TabPage mainTab;
		private Szotar.WindowsForms.Controls.ListViewNF recentDictionaries;
		private System.Windows.Forms.ImageList imageListSmall;
		private Szotar.WindowsForms.Controls.ListViewNF recentLists;
		private System.Windows.Forms.ColumnHeader dictionariesSizeColumn;
		private System.Windows.Forms.ColumnHeader dictionariesAuthorColumn;
		private System.Windows.Forms.TabPage dictionaryTab;
		private System.Windows.Forms.TabPage practiceTab;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.TableLayoutPanel practiceTable;
		private System.Windows.Forms.LinkLabel reportBug;
		private Szotar.WindowsForms.Controls.ListSearch listSearch;
		private System.Windows.Forms.Button practiceList;
		private System.Windows.Forms.Button openList;

	}
}