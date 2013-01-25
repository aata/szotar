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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
            System.Windows.Forms.ToolStripMenuItem close;
            System.Windows.Forms.ToolStripSeparator fileSep;
            System.Windows.Forms.ToolStripMenuItem tools;
            System.Windows.Forms.ToolStripMenuItem debugLog;
            System.Windows.Forms.ToolStripSeparator toolsSep;
            System.Windows.Forms.ToolStripMenuItem options;
            System.Windows.Forms.ToolStripMenuItem help;
            System.Windows.Forms.ToolStripMenuItem about;
            this.newListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.practiceRandom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importDictionaryMI = new System.Windows.Forms.ToolStripMenuItem();
            this.importListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.exitProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.findDuplicates = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.newListCM = new System.Windows.Forms.ToolStripMenuItem();
            this.listContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.flashcardsListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.learnListMI = new System.Windows.Forms.ToolStripMenuItem();
            this.recentItems = new Szotar.WindowsForms.Controls.ListSearch();
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
            resources.ApplyResources(file, "file");
            file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newListMI,
            this.practiceRandom,
            close,
            this.toolStripSeparator1,
            this.importDictionaryMI,
            this.importListMI,
            fileSep,
            this.exitProgram});
            file.Name = "file";
            // 
            // newListMI
            // 
            resources.ApplyResources(this.newListMI, "newListMI");
            this.newListMI.Name = "newListMI";
            this.newListMI.Click += new System.EventHandler(this.newListMI_Click);
            // 
            // practiceRandom
            // 
            resources.ApplyResources(this.practiceRandom, "practiceRandom");
            this.practiceRandom.Name = "practiceRandom";
            this.practiceRandom.Click += new System.EventHandler(this.practiceRandom_Click);
            // 
            // close
            // 
            resources.ApplyResources(close, "close");
            close.Name = "close";
            close.Click += new System.EventHandler(this.close_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // importDictionaryMI
            // 
            resources.ApplyResources(this.importDictionaryMI, "importDictionaryMI");
            this.importDictionaryMI.Name = "importDictionaryMI";
            this.importDictionaryMI.Click += new System.EventHandler(this.importDictionaryMI_Click);
            // 
            // importListMI
            // 
            resources.ApplyResources(this.importListMI, "importListMI");
            this.importListMI.Name = "importListMI";
            this.importListMI.Click += new System.EventHandler(this.importListMI_Click);
            // 
            // fileSep
            // 
            resources.ApplyResources(fileSep, "fileSep");
            fileSep.Name = "fileSep";
            // 
            // exitProgram
            // 
            resources.ApplyResources(this.exitProgram, "exitProgram");
            this.exitProgram.Name = "exitProgram";
            this.exitProgram.Click += new System.EventHandler(this.exitProgram_Click);
            // 
            // tools
            // 
            resources.ApplyResources(tools, "tools");
            tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            debugLog,
            this.findDuplicates,
            toolsSep,
            options});
            tools.Name = "tools";
            // 
            // debugLog
            // 
            resources.ApplyResources(debugLog, "debugLog");
            debugLog.Name = "debugLog";
            debugLog.Click += new System.EventHandler(this.debugLog_Click);
            // 
            // findDuplicates
            // 
            resources.ApplyResources(this.findDuplicates, "findDuplicates");
            this.findDuplicates.Name = "findDuplicates";
            this.findDuplicates.Click += new System.EventHandler(this.findDuplicates_Click);
            // 
            // toolsSep
            // 
            resources.ApplyResources(toolsSep, "toolsSep");
            toolsSep.Name = "toolsSep";
            // 
            // options
            // 
            resources.ApplyResources(options, "options");
            options.Name = "options";
            options.Click += new System.EventHandler(this.options_Click);
            // 
            // help
            // 
            resources.ApplyResources(help, "help");
            help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            about});
            help.Name = "help";
            // 
            // about
            // 
            resources.ApplyResources(about, "about");
            about.Name = "about";
            about.Click += new System.EventHandler(this.about_Click);
            // 
            // menu
            // 
            resources.ApplyResources(this.menu, "menu");
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            file,
            tools,
            help});
            this.menu.Name = "menu";
            // 
            // listContextMenu
            // 
            resources.ApplyResources(this.listContextMenu, "listContextMenu");
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openListMI,
            this.newListCM,
            this.listContextMenuSeparator,
            this.flashcardsListMI,
            this.learnListMI});
            this.listContextMenu.Name = "listContextMenu";
            this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenu_Opening);
            // 
            // openListMI
            // 
            resources.ApplyResources(this.openListMI, "openListMI");
            this.openListMI.Name = "openListMI";
            this.openListMI.Click += new System.EventHandler(this.openListMI_Click);
            // 
            // newListCM
            // 
            resources.ApplyResources(this.newListCM, "newListCM");
            this.newListCM.Name = "newListCM";
            this.newListCM.Click += new System.EventHandler(this.newListMI_Click);
            // 
            // listContextMenuSeparator
            // 
            resources.ApplyResources(this.listContextMenuSeparator, "listContextMenuSeparator");
            this.listContextMenuSeparator.Name = "listContextMenuSeparator";
            // 
            // flashcardsListMI
            // 
            resources.ApplyResources(this.flashcardsListMI, "flashcardsListMI");
            this.flashcardsListMI.Name = "flashcardsListMI";
            this.flashcardsListMI.Click += new System.EventHandler(this.flashcardsListMI_Click);
            // 
            // learnListMI
            // 
            resources.ApplyResources(this.learnListMI, "learnListMI");
            this.learnListMI.Name = "learnListMI";
            this.learnListMI.Click += new System.EventHandler(this.learnListMI_Click);
            // 
            // recentItems
            // 
            resources.ApplyResources(this.recentItems, "recentItems");
            this.recentItems.ListContextMenuStrip = this.listContextMenu;
            this.recentItems.MaxItems = 100;
            this.recentItems.Name = "recentItems";
            this.recentItems.SearchTerm = "";
            this.recentItems.ShowDictionaries = true;
            this.recentItems.ShowListItems = true;
            this.recentItems.ShowTags = true;
            // 
            // StartPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.recentItems);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "StartPage";
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
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem importDictionaryMI;
		private System.Windows.Forms.ToolStripMenuItem importListMI;

	}
}