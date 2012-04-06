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
            System.Windows.Forms.Label authorLabel;
            System.Windows.Forms.Label urlLabel;
            System.Windows.Forms.FlowLayoutPanel metaFlow;
            System.Windows.Forms.ToolStripMenuItem fileMenu;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            System.Windows.Forms.ToolStripMenuItem practiceMenu;
            System.Windows.Forms.ToolStripSeparator editSep2;
            System.Windows.Forms.ToolStripSeparator contextSep1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListBuilder));
            this.namePanel = new System.Windows.Forms.Panel();
            this.name = new System.Windows.Forms.TextBox();
            this.authorPanel = new System.Windows.Forms.Panel();
            this.author = new System.Windows.Forms.TextBox();
            this.urlPanel = new System.Windows.Forms.Panel();
            this.url = new System.Windows.Forms.TextBox();
            this.entriesPanel = new System.Windows.Forms.Panel();
            this.entriesLabel = new System.Windows.Forms.Label();
            this.showStartPage = new System.Windows.Forms.ToolStripMenuItem();
            this.save = new System.Windows.Forms.ToolStripMenuItem();
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
            nameLabel.AutoSize = true;
            nameLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            nameLabel.Location = new System.Drawing.Point(3, 4);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(39, 13);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Name:";
            // 
            // authorLabel
            // 
            authorLabel.AutoSize = true;
            authorLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            authorLabel.Location = new System.Drawing.Point(3, 4);
            authorLabel.Name = "authorLabel";
            authorLabel.Size = new System.Drawing.Size(46, 13);
            authorLabel.TabIndex = 0;
            authorLabel.Text = "Author:";
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            urlLabel.Location = new System.Drawing.Point(3, 4);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new System.Drawing.Size(30, 13);
            urlLabel.TabIndex = 0;
            urlLabel.Text = "URL:";
            // 
            // metaFlow
            // 
            metaFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            metaFlow.Controls.Add(this.namePanel);
            metaFlow.Controls.Add(this.authorPanel);
            metaFlow.Controls.Add(this.urlPanel);
            metaFlow.Controls.Add(this.entriesPanel);
            metaFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            metaFlow.Location = new System.Drawing.Point(73, 7);
            metaFlow.Name = "metaFlow";
            metaFlow.Size = new System.Drawing.Size(20501, 60);
            metaFlow.TabIndex = 2;
            // 
            // namePanel
            // 
            this.namePanel.AutoSize = true;
            this.namePanel.Controls.Add(this.name);
            this.namePanel.Controls.Add(nameLabel);
            this.namePanel.Location = new System.Drawing.Point(3, 3);
            this.namePanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.namePanel.Name = "namePanel";
            this.namePanel.Size = new System.Drawing.Size(209, 26);
            this.namePanel.TabIndex = 0;
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Location = new System.Drawing.Point(48, 1);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(161, 22);
            this.name.TabIndex = 1;
            // 
            // authorPanel
            // 
            this.authorPanel.AutoSize = true;
            this.authorPanel.Controls.Add(this.author);
            this.authorPanel.Controls.Add(authorLabel);
            this.authorPanel.Location = new System.Drawing.Point(3, 32);
            this.authorPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.authorPanel.Name = "authorPanel";
            this.authorPanel.Size = new System.Drawing.Size(209, 26);
            this.authorPanel.TabIndex = 0;
            // 
            // author
            // 
            this.author.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.author.Location = new System.Drawing.Point(48, 1);
            this.author.Name = "author";
            this.author.Size = new System.Drawing.Size(161, 22);
            this.author.TabIndex = 1;
            // 
            // urlPanel
            // 
            this.urlPanel.AutoSize = true;
            this.urlPanel.Controls.Add(this.url);
            this.urlPanel.Controls.Add(urlLabel);
            this.urlPanel.Location = new System.Drawing.Point(218, 3);
            this.urlPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.urlPanel.Name = "urlPanel";
            this.urlPanel.Size = new System.Drawing.Size(209, 26);
            this.urlPanel.TabIndex = 0;
            // 
            // url
            // 
            this.url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.url.Location = new System.Drawing.Point(48, 1);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(161, 22);
            this.url.TabIndex = 1;
            // 
            // entriesPanel
            // 
            this.entriesPanel.AutoSize = true;
            this.entriesPanel.Controls.Add(this.entriesLabel);
            this.entriesPanel.Location = new System.Drawing.Point(218, 32);
            this.entriesPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.entriesPanel.Name = "entriesPanel";
            this.entriesPanel.Size = new System.Drawing.Size(6, 17);
            this.entriesPanel.TabIndex = 1;
            // 
            // entriesLabel
            // 
            this.entriesLabel.AutoSize = true;
            this.entriesLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.entriesLabel.Location = new System.Drawing.Point(3, 4);
            this.entriesLabel.Name = "entriesLabel";
            this.entriesLabel.Size = new System.Drawing.Size(0, 13);
            this.entriesLabel.TabIndex = 0;
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showStartPage,
            toolStripSeparator2,
            this.save,
            this.close,
            toolStripMenuItem1,
            this.deleteList});
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new System.Drawing.Size(37, 20);
            fileMenu.Text = "&File";
            // 
            // showStartPage
            // 
            this.showStartPage.Name = "showStartPage";
            this.showStartPage.Size = new System.Drawing.Size(159, 22);
            this.showStartPage.Text = "S&how Start Page";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // save
            // 
            this.save.Image = global::Szotar.WindowsForms.Properties.Resources.Floppy48;
            this.save.Name = "save";
            this.save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.save.Size = new System.Drawing.Size(159, 22);
            this.save.Text = "&Save";
            // 
            // close
            // 
            this.close.Name = "close";
            this.close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.close.Size = new System.Drawing.Size(159, 22);
            this.close.Text = "&Close";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(156, 6);
            // 
            // deleteList
            // 
            this.deleteList.Name = "deleteList";
            this.deleteList.Size = new System.Drawing.Size(159, 22);
            this.deleteList.Text = "&Delete this List";
            // 
            // practiceMenu
            // 
            practiceMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flashcards,
            this.learn});
            practiceMenu.Name = "practiceMenu";
            practiceMenu.Size = new System.Drawing.Size(61, 20);
            practiceMenu.Text = "&Practice";
            // 
            // flashcards
            // 
            this.flashcards.Name = "flashcards";
            this.flashcards.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.flashcards.Size = new System.Drawing.Size(171, 22);
            this.flashcards.Text = "&Familiarize";
            this.flashcards.Click += new System.EventHandler(this.flashcards_Click);
            // 
            // learn
            // 
            this.learn.Name = "learn";
            this.learn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.learn.Size = new System.Drawing.Size(171, 22);
            this.learn.Text = "&Learn";
            this.learn.Click += new System.EventHandler(this.learn_Click);
            // 
            // editSep2
            // 
            editSep2.ForeColor = System.Drawing.SystemColors.ControlText;
            editSep2.Name = "editSep2";
            editSep2.Size = new System.Drawing.Size(211, 6);
            // 
            // contextSep1
            // 
            contextSep1.Name = "contextSep1";
            contextSep1.Size = new System.Drawing.Size(141, 6);
            // 
            // editMenu
            // 
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
            this.editMenu.Size = new System.Drawing.Size(39, 20);
            this.editMenu.Text = "&Edit";
            // 
            // undo
            // 
            this.undo.Name = "undo";
            this.undo.ShortcutKeyDisplayString = "Ctrl+Z";
            this.undo.Size = new System.Drawing.Size(214, 22);
            this.undo.Text = "&Undo";
            // 
            // redo
            // 
            this.redo.Name = "redo";
            this.redo.ShortcutKeyDisplayString = "Ctrl+Y";
            this.redo.Size = new System.Drawing.Size(214, 22);
            this.redo.Text = "&Redo";
            // 
            // editSep1
            // 
            this.editSep1.Name = "editSep1";
            this.editSep1.Size = new System.Drawing.Size(211, 6);
            // 
            // cutMI
            // 
            this.cutMI.Name = "cutMI";
            this.cutMI.ShortcutKeyDisplayString = "Ctrl+X";
            this.cutMI.Size = new System.Drawing.Size(214, 22);
            this.cutMI.Text = "Cu&t";
            // 
            // copyMI
            // 
            this.copyMI.Name = "copyMI";
            this.copyMI.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyMI.Size = new System.Drawing.Size(214, 22);
            this.copyMI.Text = "&Copy";
            // 
            // pasteMI
            // 
            this.pasteMI.Name = "pasteMI";
            this.pasteMI.ShortcutKeyDisplayString = "Ctrl+V";
            this.pasteMI.Size = new System.Drawing.Size(214, 22);
            this.pasteMI.Text = "&Paste";
            // 
            // deleteMI
            // 
            this.deleteMI.Name = "deleteMI";
            this.deleteMI.ShortcutKeyDisplayString = "Del";
            this.deleteMI.Size = new System.Drawing.Size(214, 22);
            this.deleteMI.Text = "&Delete";
            // 
            // copyAsCsv
            // 
            this.copyAsCsv.Name = "copyAsCsv";
            this.copyAsCsv.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.copyAsCsv.Size = new System.Drawing.Size(214, 22);
            this.copyAsCsv.Text = "Cop&y as CSV";
            // 
            // pasteCSV
            // 
            this.pasteCSV.Name = "pasteCSV";
            this.pasteCSV.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.V)));
            this.pasteCSV.Size = new System.Drawing.Size(214, 22);
            this.pasteCSV.Text = "P&aste CSV";
            this.pasteCSV.Click += new System.EventHandler(this.pasteCSV_Click);
            // 
            // editSep3
            // 
            this.editSep3.Name = "editSep3";
            this.editSep3.Size = new System.Drawing.Size(211, 6);
            // 
            // sort
            // 
            this.sort.Name = "sort";
            this.sort.Size = new System.Drawing.Size(214, 22);
            this.sort.Text = "&Sort";
            // 
            // editMetadata
            // 
            this.editMetadata.Checked = true;
            this.editMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.editMetadata.Name = "editMetadata";
            this.editMetadata.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editMetadata.Size = new System.Drawing.Size(214, 22);
            this.editMetadata.Text = "&Edit metadata";
            // 
            // swapAll
            // 
            this.swapAll.Name = "swapAll";
            this.swapAll.Size = new System.Drawing.Size(214, 22);
            this.swapAll.Text = "S&wap all";
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileMenu,
            this.editMenu,
            practiceMenu});
            this.mainMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(361, 24);
            this.mainMenu.TabIndex = 5;
            this.mainMenu.Text = "Main Menu";
            // 
            // itemContextMenu
            // 
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutCM,
            this.copyCM,
            this.pasteCM,
            contextSep1,
            this.swap,
            this.deleteCM});
            this.itemContextMenu.Name = "itemContextMenu";
            this.itemContextMenu.Size = new System.Drawing.Size(145, 120);
            // 
            // cutCM
            // 
            this.cutCM.Name = "cutCM";
            this.cutCM.ShortcutKeyDisplayString = "Ctrl+X";
            this.cutCM.Size = new System.Drawing.Size(144, 22);
            this.cutCM.Text = "C&ut";
            // 
            // copyCM
            // 
            this.copyCM.Name = "copyCM";
            this.copyCM.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyCM.Size = new System.Drawing.Size(144, 22);
            this.copyCM.Text = "&Copy";
            // 
            // pasteCM
            // 
            this.pasteCM.Name = "pasteCM";
            this.pasteCM.ShortcutKeyDisplayString = "Ctrl+V";
            this.pasteCM.Size = new System.Drawing.Size(144, 22);
            this.pasteCM.Text = "&Paste";
            // 
            // swap
            // 
            this.swap.Name = "swap";
            this.swap.Size = new System.Drawing.Size(144, 22);
            this.swap.Text = "&Swap";
            // 
            // deleteCM
            // 
            this.deleteCM.Name = "deleteCM";
            this.deleteCM.ShortcutKeyDisplayString = "Del";
            this.deleteCM.Size = new System.Drawing.Size(144, 22);
            this.deleteCM.Text = "&Delete";
            // 
            // meta
            // 
            this.meta.Controls.Add(metaFlow);
            this.meta.Controls.Add(this.shadow);
            this.meta.Controls.Add(this.icon);
            this.meta.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.meta.Location = new System.Drawing.Point(0, 420);
            this.meta.MaximumSize = new System.Drawing.Size(0, 75);
            this.meta.MinimumSize = new System.Drawing.Size(0, 45);
            this.meta.Name = "meta";
            this.meta.Size = new System.Drawing.Size(361, 70);
            this.meta.TabIndex = 7;
            // 
            // shadow
            // 
            this.shadow.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.shadow.Dock = System.Windows.Forms.DockStyle.Top;
            this.shadow.Image = global::Szotar.WindowsForms.Properties.Resources.TopInwardShadow;
            this.shadow.Location = new System.Drawing.Point(0, 0);
            this.shadow.Name = "shadow";
            this.shadow.Size = new System.Drawing.Size(361, 8);
            this.shadow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.shadow.TabIndex = 1;
            this.shadow.TabStop = false;
            // 
            // icon
            // 
            this.icon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.icon.Image = global::Szotar.WindowsForms.Properties.Resources.GenericDocument96;
            this.icon.Location = new System.Drawing.Point(0, 7);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(67, 60);
            this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.icon.TabIndex = 0;
            this.icon.TabStop = false;
            // 
            // grid
            // 
            this.grid.AllowNewItems = true;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.ColumnRatio = 0.5F;
            this.grid.DataSource = null;
            this.grid.ItemContextMenu = this.itemContextMenu;
            this.grid.Location = new System.Drawing.Point(0, 24);
            this.grid.Name = "grid";
            this.grid.ShowMutableRows = false;
            this.grid.Size = new System.Drawing.Size(361, 379);
            this.grid.TabIndex = 8;
            // 
            // ListBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 490);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.meta);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ListBuilder";
            this.Text = "ListBuilder";
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
		private System.Windows.Forms.ToolStripMenuItem save;
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