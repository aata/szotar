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
            System.Windows.Forms.ToolStripMenuItem help;
            System.Windows.Forms.ToolStripMenuItem about;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
            this.newListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.practiceRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.exitProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.newListCM = new System.Windows.Forms.ToolStripMenuItem();
            this.listContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.flashcardsListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.learnListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.recentItems = new Szotar.WindowsForms.Controls.ListSearch();
            this.findDuplicates = new System.Windows.Forms.ToolStripMenuItem();
            file = new System.Windows.Forms.ToolStripMenuItem();
            close = new System.Windows.Forms.ToolStripMenuItem();
            fileSep = new System.Windows.Forms.ToolStripSeparator();
            tools = new System.Windows.Forms.ToolStripMenuItem();
            debugLog = new System.Windows.Forms.ToolStripMenuItem();
            toolsSep = new System.Windows.Forms.ToolStripSeparator();
            options = new System.Windows.Forms.ToolStripMenuItem();
            help = new System.Windows.Forms.ToolStripMenuItem();
            about = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.listContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // file
            // 
            file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newListMI,
            this.practiceRandom,
            close,
            fileSep,
            this.exitProgram});
            file.Name = "file";
            file.Size = new System.Drawing.Size(37, 20);
            file.Text = "&File";
            // 
            // newListMI
            // 
            this.newListMI.Name = "newListMI";
            this.newListMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newListMI.Size = new System.Drawing.Size(225, 22);
            this.newListMI.Text = "&New Word List";
            this.newListMI.Click += new System.EventHandler(this.newListMI_Click);
            // 
            // practiceRandom
            // 
            this.practiceRandom.Name = "practiceRandom";
            this.practiceRandom.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.practiceRandom.Size = new System.Drawing.Size(225, 22);
            this.practiceRandom.Text = "&Practice Weak Points";
            this.practiceRandom.Click += new System.EventHandler(this.practiceRandom_Click);
            // 
            // close
            // 
            close.Name = "close";
            close.ShortcutKeyDisplayString = "Alt+F4";
            close.Size = new System.Drawing.Size(225, 22);
            close.Text = "&Close";
            close.Click += new System.EventHandler(this.close_Click);
            // 
            // fileSep
            // 
            fileSep.Name = "fileSep";
            fileSep.Size = new System.Drawing.Size(222, 6);
            // 
            // exitProgram
            // 
            this.exitProgram.Name = "exitProgram";
            this.exitProgram.ShortcutKeyDisplayString = "";
            this.exitProgram.Size = new System.Drawing.Size(225, 22);
            this.exitProgram.Text = "E&xit {0}";
            this.exitProgram.Click += new System.EventHandler(this.exitProgram_Click);
            // 
            // tools
            // 
            tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            debugLog,
            this.findDuplicates,
            toolsSep,
            options});
            tools.Name = "tools";
            tools.Size = new System.Drawing.Size(48, 20);
            tools.Text = "&Tools";
            // 
            // debugLog
            // 
            debugLog.Name = "debugLog";
            debugLog.Size = new System.Drawing.Size(186, 22);
            debugLog.Text = "&Debug Log";
            debugLog.Click += new System.EventHandler(this.debugLog_Click);
            // 
            // toolsSep
            // 
            toolsSep.Name = "toolsSep";
            toolsSep.Size = new System.Drawing.Size(183, 6);
            // 
            // options
            // 
            options.Name = "options";
            options.Size = new System.Drawing.Size(186, 22);
            options.Text = "&Options";
            options.Click += new System.EventHandler(this.options_Click);
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
            // listContextMenu
            // 
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openListMI,
            this.newListCM,
            this.listContextMenuSeparator,
            this.flashcardsListMI,
            this.learnListMI});
            this.listContextMenu.Name = "listContextMenu";
            this.listContextMenu.Size = new System.Drawing.Size(152, 98);
            this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenu_Opening);
            // 
            // openListMI
            // 
            this.openListMI.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.openListMI.Name = "openListMI";
            this.openListMI.Size = new System.Drawing.Size(151, 22);
            this.openListMI.Text = "&Open";
            this.openListMI.Click += new System.EventHandler(this.openListMI_Click);
            // 
            // newListCM
            // 
            this.newListCM.Name = "newListCM";
            this.newListCM.Size = new System.Drawing.Size(151, 22);
            this.newListCM.Text = "&New Word List";
            this.newListCM.Click += new System.EventHandler(this.newListMI_Click);
            // 
            // listContextMenuSeparator
            // 
            this.listContextMenuSeparator.Name = "listContextMenuSeparator";
            this.listContextMenuSeparator.Size = new System.Drawing.Size(148, 6);
            // 
            // flashcardsListMI
            // 
            this.flashcardsListMI.Name = "flashcardsListMI";
            this.flashcardsListMI.Size = new System.Drawing.Size(151, 22);
            this.flashcardsListMI.Text = "&Familiarize";
            this.flashcardsListMI.Click += new System.EventHandler(this.flashcardsListMI_Click);
            // 
            // learnListMI
            // 
            this.learnListMI.Name = "learnListMI";
            this.learnListMI.Size = new System.Drawing.Size(151, 22);
            this.learnListMI.Text = "&Learn";
            this.learnListMI.Click += new System.EventHandler(this.learnListMI_Click);
            // 
            // recentItems
            // 
            this.recentItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentItems.ListContextMenuStrip = this.listContextMenu;
            this.recentItems.Location = new System.Drawing.Point(0, 24);
            this.recentItems.MaxItems = 100;
            this.recentItems.Name = "recentItems";
            this.recentItems.Padding = new System.Windows.Forms.Padding(3);
            this.recentItems.SearchTerm = "";
            this.recentItems.ShowDictionaries = true;
            this.recentItems.ShowListItems = true;
            this.recentItems.ShowTags = true;
            this.recentItems.Size = new System.Drawing.Size(569, 395);
            this.recentItems.TabIndex = 2;
            // 
            // findDuplicates
            // 
            this.findDuplicates.Name = "findDuplicates";
            this.findDuplicates.Size = new System.Drawing.Size(186, 22);
            this.findDuplicates.Text = "Find D&uplicate Terms";
            this.findDuplicates.Click += new System.EventHandler(this.findDuplicates_Click);
            // 
            // StartPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 419);
            this.Controls.Add(this.recentItems);
            this.Controls.Add(this.menu);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "StartPage";
            this.Text = "Szótár";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.listContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem exitProgram;
        private Controls.ListSearch recentItems;
        private System.Windows.Forms.ContextMenuStrip listContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openListMI;
        private System.Windows.Forms.ToolStripMenuItem newListCM;
        private System.Windows.Forms.ToolStripSeparator listContextMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem flashcardsListMI;
        private System.Windows.Forms.ToolStripMenuItem learnListMI;
        private System.Windows.Forms.ToolStripMenuItem newListMI;
        private System.Windows.Forms.ToolStripMenuItem practiceRandom;
        private System.Windows.Forms.ToolStripMenuItem findDuplicates;

	}
}