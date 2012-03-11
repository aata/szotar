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
            System.Windows.Forms.ToolStripMenuItem file;
            System.Windows.Forms.ToolStripMenuItem close;
            System.Windows.Forms.ToolStripSeparator fileSep;
            System.Windows.Forms.ToolStripMenuItem tools;
            System.Windows.Forms.ToolStripMenuItem debugLog;
            System.Windows.Forms.ToolStripSeparator toolsSep;
            System.Windows.Forms.ToolStripMenuItem options;
            System.Windows.Forms.TableLayoutPanel mainTabTable;
            System.Windows.Forms.TableLayoutPanel recentDictionariesTable;
            System.Windows.Forms.ColumnHeader recentDictionariesTitleColumn;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
            System.Windows.Forms.Button practiceList;
            System.Windows.Forms.Button openList;
            System.Windows.Forms.Button newList;
            System.Windows.Forms.ColumnHeader dictionariesNameColumn;
            System.Windows.Forms.ToolStripMenuItem help;
            System.Windows.Forms.ToolStripMenuItem about;
            this.openListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.exitProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.recentItems = new Szotar.WindowsForms.Controls.ListViewNF();
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.listContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.flashcardsListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.learnListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.tasks = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.dictionaryTab = new System.Windows.Forms.TabPage();
            this.importDictionary = new System.Windows.Forms.Button();
            this.openDictionary = new System.Windows.Forms.Button();
            this.dictionaries = new Szotar.WindowsForms.Controls.ListViewNF();
            this.dictionariesSizeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dictionariesAuthorColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.practiceTab = new System.Windows.Forms.TabPage();
            this.listSearch = new Szotar.WindowsForms.Controls.ListSearch();
            file = new System.Windows.Forms.ToolStripMenuItem();
            close = new System.Windows.Forms.ToolStripMenuItem();
            fileSep = new System.Windows.Forms.ToolStripSeparator();
            tools = new System.Windows.Forms.ToolStripMenuItem();
            debugLog = new System.Windows.Forms.ToolStripMenuItem();
            toolsSep = new System.Windows.Forms.ToolStripSeparator();
            options = new System.Windows.Forms.ToolStripMenuItem();
            mainTabTable = new System.Windows.Forms.TableLayoutPanel();
            recentDictionariesTable = new System.Windows.Forms.TableLayoutPanel();
            recentDictionariesTitleColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            practiceList = new System.Windows.Forms.Button();
            openList = new System.Windows.Forms.Button();
            newList = new System.Windows.Forms.Button();
            dictionariesNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            help = new System.Windows.Forms.ToolStripMenuItem();
            about = new System.Windows.Forms.ToolStripMenuItem();
            mainTabTable.SuspendLayout();
            recentDictionariesTable.SuspendLayout();
            this.listContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.menu.SuspendLayout();
            this.tasks.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.dictionaryTab.SuspendLayout();
            this.practiceTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // openListMI
            // 
            this.openListMI.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.openListMI.Name = "openListMI";
            this.openListMI.Size = new System.Drawing.Size(152, 22);
            this.openListMI.Text = "&Open";
            this.openListMI.Click += new System.EventHandler(this.openListMI_Click);
            // 
            // file
            // 
            file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            close,
            fileSep,
            this.exitProgram});
            file.Name = "file";
            file.Size = new System.Drawing.Size(37, 20);
            file.Text = "&File";
            // 
            // close
            // 
            close.Name = "close";
            close.ShortcutKeyDisplayString = "Alt+F4";
            close.Size = new System.Drawing.Size(145, 22);
            close.Text = "&Close";
            close.Click += new System.EventHandler(this.close_Click);
            // 
            // fileSep
            // 
            fileSep.Name = "fileSep";
            fileSep.Size = new System.Drawing.Size(142, 6);
            // 
            // exitProgram
            // 
            this.exitProgram.Name = "exitProgram";
            this.exitProgram.ShortcutKeyDisplayString = "";
            this.exitProgram.Size = new System.Drawing.Size(145, 22);
            this.exitProgram.Text = "E&xit {0}";
            this.exitProgram.Click += new System.EventHandler(this.exitProgram_Click);
            // 
            // tools
            // 
            tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            debugLog,
            toolsSep,
            options});
            tools.Name = "tools";
            tools.Size = new System.Drawing.Size(48, 20);
            tools.Text = "&Tools";
            // 
            // debugLog
            // 
            debugLog.Name = "debugLog";
            debugLog.Size = new System.Drawing.Size(132, 22);
            debugLog.Text = "&Debug Log";
            debugLog.Click += new System.EventHandler(this.debugLog_Click);
            // 
            // toolsSep
            // 
            toolsSep.Name = "toolsSep";
            toolsSep.Size = new System.Drawing.Size(129, 6);
            // 
            // options
            // 
            options.Name = "options";
            options.Size = new System.Drawing.Size(132, 22);
            options.Text = "&Options";
            options.Click += new System.EventHandler(this.options_Click);
            // 
            // mainTabTable
            // 
            mainTabTable.ColumnCount = 2;
            mainTabTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            mainTabTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            mainTabTable.Controls.Add(recentDictionariesTable, 0, 0);
            mainTabTable.Dock = System.Windows.Forms.DockStyle.Fill;
            mainTabTable.Location = new System.Drawing.Point(0, 0);
            mainTabTable.Margin = new System.Windows.Forms.Padding(0);
            mainTabTable.Name = "mainTabTable";
            mainTabTable.RowCount = 2;
            mainTabTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            mainTabTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            mainTabTable.Size = new System.Drawing.Size(555, 363);
            mainTabTable.TabIndex = 0;
            // 
            // recentDictionariesTable
            // 
            recentDictionariesTable.ColumnCount = 1;
            mainTabTable.SetColumnSpan(recentDictionariesTable, 2);
            recentDictionariesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            recentDictionariesTable.Controls.Add(this.recentItems, 0, 1);
            recentDictionariesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            recentDictionariesTable.Location = new System.Drawing.Point(3, 3);
            recentDictionariesTable.Name = "recentDictionariesTable";
            recentDictionariesTable.RowCount = 2;
            mainTabTable.SetRowSpan(recentDictionariesTable, 2);
            recentDictionariesTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            recentDictionariesTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            recentDictionariesTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            recentDictionariesTable.Size = new System.Drawing.Size(549, 357);
            recentDictionariesTable.TabIndex = 0;
            // 
            // recentItems
            // 
            this.recentItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.recentItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            recentDictionariesTitleColumn});
            this.recentItems.ContextMenuStrip = this.listContextMenu;
            this.recentItems.FullRowSelect = true;
            this.recentItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.recentItems.LargeImageList = this.imageList;
            this.recentItems.Location = new System.Drawing.Point(0, 0);
            this.recentItems.Margin = new System.Windows.Forms.Padding(0);
            this.recentItems.Name = "recentItems";
            this.recentItems.ShowItemToolTips = true;
            this.recentItems.Size = new System.Drawing.Size(549, 357);
            this.recentItems.SmallImageList = this.imageListSmall;
            this.recentItems.TabIndex = 2;
            this.recentItems.UseCompatibleStateImageBehavior = false;
            this.recentItems.View = System.Windows.Forms.View.Details;
            // 
            // listContextMenu
            // 
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openListMI,
            this.newListMI,
            this.listContextMenuSeparator,
            this.flashcardsListMI,
            this.learnListMI});
            this.listContextMenu.Name = "listContextMenu";
            this.listContextMenu.Size = new System.Drawing.Size(153, 120);
            this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenu_Opening);
            // 
            // newListMI
            // 
            this.newListMI.Name = "newListMI";
            this.newListMI.Size = new System.Drawing.Size(152, 22);
            this.newListMI.Text = "&New Word List";
            this.newListMI.Click += new System.EventHandler(this.newListMI_Click);
            // 
            // listContextMenuSeparator
            // 
            this.listContextMenuSeparator.Name = "listContextMenuSeparator";
            this.listContextMenuSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // flashcardsListMI
            // 
            this.flashcardsListMI.Name = "flashcardsListMI";
            this.flashcardsListMI.Size = new System.Drawing.Size(152, 22);
            this.flashcardsListMI.Text = "&Familiarize";
            this.flashcardsListMI.Click += new System.EventHandler(this.flashcardsListMI_Click);
            // 
            // learnListMI
            // 
            this.learnListMI.Name = "learnListMI";
            this.learnListMI.Size = new System.Drawing.Size(152, 22);
            this.learnListMI.Text = "&Learn";
            this.learnListMI.Click += new System.EventHandler(this.learnListMI_Click);
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
            // practiceList
            // 
            practiceList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            practiceList.Location = new System.Drawing.Point(474, 335);
            practiceList.Name = "practiceList";
            practiceList.Size = new System.Drawing.Size(75, 23);
            practiceList.TabIndex = 9;
            practiceList.Text = "&Practice";
            practiceList.UseVisualStyleBackColor = true;
            practiceList.Click += new System.EventHandler(this.practiceList_Click);
            // 
            // openList
            // 
            openList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            openList.Location = new System.Drawing.Point(393, 335);
            openList.Name = "openList";
            openList.Size = new System.Drawing.Size(75, 23);
            openList.TabIndex = 8;
            openList.Text = "&Open";
            openList.UseVisualStyleBackColor = true;
            openList.Click += new System.EventHandler(this.openList_Click);
            // 
            // newList
            // 
            newList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            newList.Location = new System.Drawing.Point(312, 335);
            newList.Name = "newList";
            newList.Size = new System.Drawing.Size(75, 23);
            newList.TabIndex = 7;
            newList.Text = "&New";
            newList.UseVisualStyleBackColor = true;
            newList.Click += new System.EventHandler(this.newList_Click);
            // 
            // dictionariesNameColumn
            // 
            dictionariesNameColumn.Text = "Name";
            dictionariesNameColumn.Width = 312;
            // 
            // help
            // 
            help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            about});
            help.Name = "help";
            help.Size = new System.Drawing.Size(44, 20);
            help.Text = "&Help";
            // 
            // about
            // 
            about.Name = "about";
            about.Size = new System.Drawing.Size(107, 22);
            about.Text = "&About";
            about.Click += new System.EventHandler(this.about_Click);
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            file,
            tools,
            help});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(569, 24);
            this.menu.TabIndex = 1;
            this.menu.Text = "menu";
            // 
            // tasks
            // 
            this.tasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tasks.Controls.Add(this.mainTab);
            this.tasks.Controls.Add(this.dictionaryTab);
            this.tasks.Controls.Add(this.practiceTab);
            this.tasks.Location = new System.Drawing.Point(3, 27);
            this.tasks.Margin = new System.Windows.Forms.Padding(0);
            this.tasks.Name = "tasks";
            this.tasks.SelectedIndex = 0;
            this.tasks.Size = new System.Drawing.Size(563, 389);
            this.tasks.TabIndex = 2;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(mainTabTable);
            this.mainTab.Location = new System.Drawing.Point(4, 22);
            this.mainTab.Name = "mainTab";
            this.mainTab.Size = new System.Drawing.Size(555, 363);
            this.mainTab.TabIndex = 2;
            this.mainTab.Text = "Szótár";
            this.mainTab.UseVisualStyleBackColor = true;
            // 
            // dictionaryTab
            // 
            this.dictionaryTab.Controls.Add(this.importDictionary);
            this.dictionaryTab.Controls.Add(this.openDictionary);
            this.dictionaryTab.Controls.Add(this.dictionaries);
            this.dictionaryTab.ImageKey = "(none)";
            this.dictionaryTab.Location = new System.Drawing.Point(4, 22);
            this.dictionaryTab.Name = "dictionaryTab";
            this.dictionaryTab.Size = new System.Drawing.Size(555, 363);
            this.dictionaryTab.TabIndex = 0;
            this.dictionaryTab.Text = "Dictionaries";
            this.dictionaryTab.UseVisualStyleBackColor = true;
            // 
            // importDictionary
            // 
            this.importDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.importDictionary.Location = new System.Drawing.Point(3, 337);
            this.importDictionary.Name = "importDictionary";
            this.importDictionary.Size = new System.Drawing.Size(75, 23);
            this.importDictionary.TabIndex = 7;
            this.importDictionary.Text = "&Import...";
            this.importDictionary.UseVisualStyleBackColor = true;
            this.importDictionary.Click += new System.EventHandler(this.importDictionary_Click);
            // 
            // openDictionary
            // 
            this.openDictionary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openDictionary.Location = new System.Drawing.Point(477, 337);
            this.openDictionary.Name = "openDictionary";
            this.openDictionary.Size = new System.Drawing.Size(75, 23);
            this.openDictionary.TabIndex = 6;
            this.openDictionary.Text = "&Open";
            this.openDictionary.UseVisualStyleBackColor = true;
            this.openDictionary.Click += new System.EventHandler(this.openDictionary_Click);
            // 
            // dictionaries
            // 
            this.dictionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dictionaries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            dictionariesNameColumn,
            this.dictionariesSizeColumn,
            this.dictionariesAuthorColumn});
            this.dictionaries.FullRowSelect = true;
            this.dictionaries.LargeImageList = this.imageList;
            this.dictionaries.Location = new System.Drawing.Point(3, 3);
            this.dictionaries.Margin = new System.Windows.Forms.Padding(0);
            this.dictionaries.MultiSelect = false;
            this.dictionaries.Name = "dictionaries";
            this.dictionaries.ShowItemToolTips = true;
            this.dictionaries.Size = new System.Drawing.Size(549, 331);
            this.dictionaries.SmallImageList = this.imageListSmall;
            this.dictionaries.TabIndex = 5;
            this.dictionaries.UseCompatibleStateImageBehavior = false;
            this.dictionaries.View = System.Windows.Forms.View.Details;
            this.dictionaries.ItemActivate += new System.EventHandler(this.OnDictionaryItemActivate);
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
            // practiceTab
            // 
            this.practiceTab.Controls.Add(this.listSearch);
            this.practiceTab.Controls.Add(newList);
            this.practiceTab.Controls.Add(openList);
            this.practiceTab.Controls.Add(practiceList);
            this.practiceTab.ImageKey = "(none)";
            this.practiceTab.Location = new System.Drawing.Point(4, 22);
            this.practiceTab.Name = "practiceTab";
            this.practiceTab.Size = new System.Drawing.Size(555, 363);
            this.practiceTab.TabIndex = 1;
            this.practiceTab.Text = "Word Lists";
            this.practiceTab.UseVisualStyleBackColor = true;
            // 
            // listSearch
            // 
            this.listSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listSearch.ListContextMenuStrip = this.listContextMenu;
            this.listSearch.Location = new System.Drawing.Point(3, 3);
            this.listSearch.MaxItems = 100;
            this.listSearch.Name = "listSearch";
            this.listSearch.Size = new System.Drawing.Size(549, 326);
            this.listSearch.TabIndex = 6;
            this.listSearch.ListsChosen += new System.EventHandler<Szotar.WindowsForms.Controls.ListsChosenEventArgs>(this.listSearch_ListsChosen);
            // 
            // StartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 419);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.tasks);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "StartPage";
            this.Text = "Szótár";
            mainTabTable.ResumeLayout(false);
            recentDictionariesTable.ResumeLayout(false);
            this.listContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.tasks.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.dictionaryTab.ResumeLayout(false);
            this.practiceTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ImageList imageListSmall;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.ContextMenuStrip listContextMenu;
		private System.Windows.Forms.MenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem exitProgram;
		private System.Windows.Forms.TabControl tasks;
		private System.Windows.Forms.TabPage mainTab;
        private Szotar.WindowsForms.Controls.ListViewNF recentItems;
		private System.Windows.Forms.TabPage dictionaryTab;
		private System.Windows.Forms.Button importDictionary;
		private System.Windows.Forms.Button openDictionary;
		private Szotar.WindowsForms.Controls.ListViewNF dictionaries;
		private System.Windows.Forms.ColumnHeader dictionariesSizeColumn;
		private System.Windows.Forms.ColumnHeader dictionariesAuthorColumn;
		private System.Windows.Forms.TabPage practiceTab;
		private Szotar.WindowsForms.Controls.ListSearch listSearch;
		private System.Windows.Forms.ToolStripSeparator listContextMenuSeparator;
		private System.Windows.Forms.ToolStripMenuItem learnListMI;
        private System.Windows.Forms.ToolStripMenuItem newListMI;
        private System.Windows.Forms.ToolStripMenuItem flashcardsListMI;
        private System.Windows.Forms.ToolStripMenuItem openListMI;

	}
}