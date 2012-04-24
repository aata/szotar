namespace Szotar.WindowsForms.Forms {
	partial class ListBuilder {
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
            System.Windows.Forms.Label nameLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListBuilder));
            System.Windows.Forms.Label authorLabel;
            System.Windows.Forms.Label urlLabel;
            System.Windows.Forms.FlowLayoutPanel metaFlow;
            System.Windows.Forms.ToolStripMenuItem fileMenu;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            System.Windows.Forms.ToolStripMenuItem practiceMenu;
            System.Windows.Forms.ToolStripSeparator editSep2;
            System.Windows.Forms.ToolStripSeparator contextSep1;
            this.namePanel = new System.Windows.Forms.Panel();
            this.name = new System.Windows.Forms.TextBox();
            this.authorPanel = new System.Windows.Forms.Panel();
            this.author = new System.Windows.Forms.TextBox();
            this.urlPanel = new System.Windows.Forms.Panel();
            this.url = new System.Windows.Forms.TextBox();
            this.entriesPanel = new System.Windows.Forms.Panel();
            this.entriesLabel = new System.Windows.Forms.Label();
            this.showStartPage = new System.Windows.Forms.ToolStripMenuItem();
            this.close = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteList = new System.Windows.Forms.ToolStripMenuItem();
            this.flashcards = new System.Windows.Forms.ToolStripMenuItem();
            this.learn = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.undo = new System.Windows.Forms.ToolStripMenuItem();
            this.redo = new System.Windows.Forms.ToolStripMenuItem();
            this.editSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMI = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMI = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMI = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMI = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAsCsv = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.editSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.sort = new System.Windows.Forms.ToolStripMenuItem();
            this.editMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.swapAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutCM = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCM = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteCM = new System.Windows.Forms.ToolStripMenuItem();
            this.swap = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCM = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.meta = new System.Windows.Forms.Panel();
            this.shadow = new System.Windows.Forms.PictureBox();
            this.icon = new System.Windows.Forms.PictureBox();
            this.grid = new Szotar.WindowsForms.Controls.DictionaryGrid();
            nameLabel = new System.Windows.Forms.Label();
            authorLabel = new System.Windows.Forms.Label();
            urlLabel = new System.Windows.Forms.Label();
            metaFlow = new System.Windows.Forms.FlowLayoutPanel();
            fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            practiceMenu = new System.Windows.Forms.ToolStripMenuItem();
            editSep2 = new System.Windows.Forms.ToolStripSeparator();
            contextSep1 = new System.Windows.Forms.ToolStripSeparator();
            metaFlow.SuspendLayout();
            this.namePanel.SuspendLayout();
            this.authorPanel.SuspendLayout();
            this.urlPanel.SuspendLayout();
            this.entriesPanel.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.itemContextMenu.SuspendLayout();
            this.meta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            resources.ApplyResources(nameLabel, "nameLabel");
            nameLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            nameLabel.Name = "nameLabel";
            // 
            // authorLabel
            // 
            resources.ApplyResources(authorLabel, "authorLabel");
            authorLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            authorLabel.Name = "authorLabel";
            // 
            // urlLabel
            // 
            resources.ApplyResources(urlLabel, "urlLabel");
            urlLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            urlLabel.Name = "urlLabel";
            // 
            // metaFlow
            // 
            resources.ApplyResources(metaFlow, "metaFlow");
            metaFlow.Controls.Add(this.namePanel);
            metaFlow.Controls.Add(this.authorPanel);
            metaFlow.Controls.Add(this.urlPanel);
            metaFlow.Controls.Add(this.entriesPanel);
            metaFlow.Name = "metaFlow";
            // 
            // namePanel
            // 
            resources.ApplyResources(this.namePanel, "namePanel");
            this.namePanel.Controls.Add(this.name);
            this.namePanel.Controls.Add(nameLabel);
            this.namePanel.Name = "namePanel";
            // 
            // name
            // 
            resources.ApplyResources(this.name, "name");
            this.name.Name = "name";
            // 
            // authorPanel
            // 
            resources.ApplyResources(this.authorPanel, "authorPanel");
            this.authorPanel.Controls.Add(this.author);
            this.authorPanel.Controls.Add(authorLabel);
            this.authorPanel.Name = "authorPanel";
            // 
            // author
            // 
            resources.ApplyResources(this.author, "author");
            this.author.Name = "author";
            // 
            // urlPanel
            // 
            resources.ApplyResources(this.urlPanel, "urlPanel");
            this.urlPanel.Controls.Add(this.url);
            this.urlPanel.Controls.Add(urlLabel);
            this.urlPanel.Name = "urlPanel";
            // 
            // url
            // 
            resources.ApplyResources(this.url, "url");
            this.url.Name = "url";
            // 
            // entriesPanel
            // 
            resources.ApplyResources(this.entriesPanel, "entriesPanel");
            this.entriesPanel.Controls.Add(this.entriesLabel);
            this.entriesPanel.Name = "entriesPanel";
            // 
            // entriesLabel
            // 
            resources.ApplyResources(this.entriesLabel, "entriesLabel");
            this.entriesLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.entriesLabel.Name = "entriesLabel";
            // 
            // fileMenu
            // 
            resources.ApplyResources(fileMenu, "fileMenu");
            fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showStartPage,
            toolStripSeparator2,
            this.close,
            toolStripMenuItem1,
            this.deleteList});
            fileMenu.Name = "fileMenu";
            // 
            // showStartPage
            // 
            resources.ApplyResources(this.showStartPage, "showStartPage");
            this.showStartPage.Name = "showStartPage";
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // close
            // 
            resources.ApplyResources(this.close, "close");
            this.close.Name = "close";
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // deleteList
            // 
            resources.ApplyResources(this.deleteList, "deleteList");
            this.deleteList.Name = "deleteList";
            // 
            // practiceMenu
            // 
            resources.ApplyResources(practiceMenu, "practiceMenu");
            practiceMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flashcards,
            this.learn});
            practiceMenu.Name = "practiceMenu";
            // 
            // flashcards
            // 
            resources.ApplyResources(this.flashcards, "flashcards");
            this.flashcards.Name = "flashcards";
            this.flashcards.Click += new System.EventHandler(this.flashcards_Click);
            // 
            // learn
            // 
            resources.ApplyResources(this.learn, "learn");
            this.learn.Name = "learn";
            this.learn.Click += new System.EventHandler(this.learn_Click);
            // 
            // editSep2
            // 
            resources.ApplyResources(editSep2, "editSep2");
            editSep2.ForeColor = System.Drawing.SystemColors.ControlText;
            editSep2.Name = "editSep2";
            // 
            // contextSep1
            // 
            resources.ApplyResources(contextSep1, "contextSep1");
            contextSep1.Name = "contextSep1";
            // 
            // editMenu
            // 
            resources.ApplyResources(this.editMenu, "editMenu");
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undo,
            this.redo,
            this.editSep1,
            this.cutMI,
            this.copyMI,
            this.pasteMI,
            this.deleteMI,
            editSep2,
            this.copyAsCsv,
            this.pasteCSV,
            this.editSep3,
            this.sort,
            this.editMetadata,
            this.swapAll});
            this.editMenu.Name = "editMenu";
            // 
            // undo
            // 
            resources.ApplyResources(this.undo, "undo");
            this.undo.Name = "undo";
            // 
            // redo
            // 
            resources.ApplyResources(this.redo, "redo");
            this.redo.Name = "redo";
            // 
            // editSep1
            // 
            resources.ApplyResources(this.editSep1, "editSep1");
            this.editSep1.Name = "editSep1";
            // 
            // cutMI
            // 
            resources.ApplyResources(this.cutMI, "cutMI");
            this.cutMI.Name = "cutMI";
            // 
            // copyMI
            // 
            resources.ApplyResources(this.copyMI, "copyMI");
            this.copyMI.Name = "copyMI";
            // 
            // pasteMI
            // 
            resources.ApplyResources(this.pasteMI, "pasteMI");
            this.pasteMI.Name = "pasteMI";
            // 
            // deleteMI
            // 
            resources.ApplyResources(this.deleteMI, "deleteMI");
            this.deleteMI.Name = "deleteMI";
            // 
            // copyAsCsv
            // 
            resources.ApplyResources(this.copyAsCsv, "copyAsCsv");
            this.copyAsCsv.Name = "copyAsCsv";
            // 
            // pasteCSV
            // 
            resources.ApplyResources(this.pasteCSV, "pasteCSV");
            this.pasteCSV.Name = "pasteCSV";
            this.pasteCSV.Click += new System.EventHandler(this.pasteCSV_Click);
            // 
            // editSep3
            // 
            resources.ApplyResources(this.editSep3, "editSep3");
            this.editSep3.Name = "editSep3";
            // 
            // sort
            // 
            resources.ApplyResources(this.sort, "sort");
            this.sort.Name = "sort";
            // 
            // editMetadata
            // 
            resources.ApplyResources(this.editMetadata, "editMetadata");
            this.editMetadata.Checked = true;
            this.editMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.editMetadata.Name = "editMetadata";
            // 
            // swapAll
            // 
            resources.ApplyResources(this.swapAll, "swapAll");
            this.swapAll.Name = "swapAll";
            // 
            // mainMenu
            // 
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            this.editMenu,
            practiceMenu});
            this.mainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainMenu.Name = "mainMenu";
            // 
            // itemContextMenu
            // 
            resources.ApplyResources(this.itemContextMenu, "itemContextMenu");
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutCM,
            this.copyCM,
            this.pasteCM,
            contextSep1,
            this.swap,
            this.deleteCM});
            this.itemContextMenu.Name = "itemContextMenu";
            // 
            // cutCM
            // 
            resources.ApplyResources(this.cutCM, "cutCM");
            this.cutCM.Name = "cutCM";
            // 
            // copyCM
            // 
            resources.ApplyResources(this.copyCM, "copyCM");
            this.copyCM.Name = "copyCM";
            // 
            // pasteCM
            // 
            resources.ApplyResources(this.pasteCM, "pasteCM");
            this.pasteCM.Name = "pasteCM";
            // 
            // swap
            // 
            resources.ApplyResources(this.swap, "swap");
            this.swap.Name = "swap";
            // 
            // deleteCM
            // 
            resources.ApplyResources(this.deleteCM, "deleteCM");
            this.deleteCM.Name = "deleteCM";
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // meta
            // 
            resources.ApplyResources(this.meta, "meta");
            this.meta.Controls.Add(metaFlow);
            this.meta.Controls.Add(this.shadow);
            this.meta.Controls.Add(this.icon);
            this.meta.MaximumSize = new System.Drawing.Size(0, 75);
            this.meta.MinimumSize = new System.Drawing.Size(0, 45);
            this.meta.Name = "meta";
            // 
            // shadow
            // 
            resources.ApplyResources(this.shadow, "shadow");
            this.shadow.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.shadow.Image = global::Szotar.WindowsForms.Properties.Resources.TopInwardShadow;
            this.shadow.Name = "shadow";
            this.shadow.TabStop = false;
            // 
            // icon
            // 
            resources.ApplyResources(this.icon, "icon");
            this.icon.Image = global::Szotar.WindowsForms.Properties.Resources.GenericDocument96;
            this.icon.Name = "icon";
            this.icon.TabStop = false;
            // 
            // grid
            // 
            resources.ApplyResources(this.grid, "grid");
            this.grid.AllowNewItems = true;
            this.grid.ColumnRatio = 0.5F;
            this.grid.DataSource = null;
            this.grid.ItemContextMenu = this.itemContextMenu;
            this.grid.Name = "grid";
            this.grid.ShowMutableRows = false;
            // 
            // ListBuilder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid);
            this.Controls.Add(this.meta);
            this.Controls.Add(this.mainMenu);
            this.Name = "ListBuilder";
            metaFlow.ResumeLayout(false);
            metaFlow.PerformLayout();
            this.namePanel.ResumeLayout(false);
            this.namePanel.PerformLayout();
            this.authorPanel.ResumeLayout(false);
            this.authorPanel.PerformLayout();
            this.urlPanel.ResumeLayout(false);
            this.urlPanel.PerformLayout();
            this.entriesPanel.ResumeLayout(false);
            this.entriesPanel.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.itemContextMenu.ResumeLayout(false);
            this.meta.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.shadow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip itemContextMenu;
		private System.Windows.Forms.ToolStripMenuItem swap;
		private System.Windows.Forms.ToolStripMenuItem deleteCM;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Panel meta;
		private Szotar.WindowsForms.Controls.DictionaryGrid grid;
		private System.Windows.Forms.PictureBox icon;
		private System.Windows.Forms.PictureBox shadow;
		private System.Windows.Forms.Panel authorPanel;
		private System.Windows.Forms.TextBox author;
		private System.Windows.Forms.Panel namePanel;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.Panel urlPanel;
        private System.Windows.Forms.TextBox url;
		private System.Windows.Forms.ToolStripMenuItem close;
		private System.Windows.Forms.ToolStripMenuItem deleteList;
		private System.Windows.Forms.ToolStripMenuItem copyAsCsv;
		private System.Windows.Forms.ToolStripMenuItem pasteCSV;
		private System.Windows.Forms.ToolStripSeparator editSep3;
		private System.Windows.Forms.ToolStripMenuItem sort;
		private System.Windows.Forms.ToolStripMenuItem editMetadata;
		private System.Windows.Forms.ToolStripMenuItem swapAll;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem showStartPage;
		private System.Windows.Forms.ToolStripMenuItem flashcards;
		private System.Windows.Forms.ToolStripSeparator editSep1;
		private System.Windows.Forms.ToolStripMenuItem undo;
		private System.Windows.Forms.ToolStripMenuItem redo;
		private System.Windows.Forms.ToolStripMenuItem editMenu;
		private System.Windows.Forms.ToolStripMenuItem cutMI;
		private System.Windows.Forms.ToolStripMenuItem copyMI;
		private System.Windows.Forms.ToolStripMenuItem pasteMI;
		private System.Windows.Forms.ToolStripMenuItem cutCM;
		private System.Windows.Forms.ToolStripMenuItem copyCM;
		private System.Windows.Forms.ToolStripMenuItem pasteCM;
		private System.Windows.Forms.ToolStripMenuItem deleteMI;
		private System.Windows.Forms.ToolStripMenuItem learn;
        private System.Windows.Forms.Panel entriesPanel;
        private System.Windows.Forms.Label entriesLabel;
	}
}