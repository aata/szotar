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
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.listMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.save = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.copyAsCsv = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.sort = new System.Windows.Forms.ToolStripMenuItem();
			this.editMetadata = new System.Windows.Forms.ToolStripMenuItem();
			this.swapAll = new System.Windows.Forms.ToolStripMenuItem();
			this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.swap = new System.Windows.Forms.ToolStripMenuItem();
			this.remove = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.meta = new System.Windows.Forms.Panel();
			this.namePanel = new System.Windows.Forms.Panel();
			this.name = new System.Windows.Forms.TextBox();
			this.authorPanel = new System.Windows.Forms.Panel();
			this.author = new System.Windows.Forms.TextBox();
			this.urlPanel = new System.Windows.Forms.Panel();
			this.url = new System.Windows.Forms.TextBox();
			this.shadow = new System.Windows.Forms.PictureBox();
			this.icon = new System.Windows.Forms.PictureBox();
			this.grid = new Controls.DictionaryGrid();
			nameLabel = new System.Windows.Forms.Label();
			authorLabel = new System.Windows.Forms.Label();
			urlLabel = new System.Windows.Forms.Label();
			metaFlow = new System.Windows.Forms.FlowLayoutPanel();
			this.mainMenu.SuspendLayout();
			this.itemContextMenu.SuspendLayout();
			this.meta.SuspendLayout();
			metaFlow.SuspendLayout();
			this.namePanel.SuspendLayout();
			this.authorPanel.SuspendLayout();
			this.urlPanel.SuspendLayout();
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
			// mainMenu
			// 
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listMenu,
            this.copyAsMenu,
            this.toolsMenu});
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(361, 24);
			this.mainMenu.TabIndex = 5;
			this.mainMenu.Text = "Main Menu";
			// 
			// listMenu
			// 
			this.listMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.save,
            this.saveAs});
			this.listMenu.Name = "listMenu";
			this.listMenu.Size = new System.Drawing.Size(37, 20);
			this.listMenu.Text = "&List";
			// 
			// save
			// 
			this.save.Image = Properties.Resources.Floppy48;
			this.save.Name = "save";
			this.save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.save.Size = new System.Drawing.Size(195, 22);
			this.save.Text = "&Save";
			// 
			// saveAs
			// 
			this.saveAs.Name = "saveAs";
			this.saveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.S)));
			this.saveAs.Size = new System.Drawing.Size(195, 22);
			this.saveAs.Text = "Save &As...";
			// 
			// copyAsMenu
			// 
			this.copyAsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAsCsv});
			this.copyAsMenu.Name = "copyAsMenu";
			this.copyAsMenu.Size = new System.Drawing.Size(63, 20);
			this.copyAsMenu.Text = "&Copy As";
			// 
			// copyAsCsv
			// 
			this.copyAsCsv.Name = "copyAsCsv";
			this.copyAsCsv.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.C)));
			this.copyAsCsv.ShowShortcutKeys = false;
			this.copyAsCsv.Size = new System.Drawing.Size(215, 22);
			this.copyAsCsv.Text = "&Comma Separated Variables";
			// 
			// toolsMenu
			// 
			this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sort,
            this.editMetadata,
            this.swapAll});
			this.toolsMenu.Name = "toolsMenu";
			this.toolsMenu.Size = new System.Drawing.Size(48, 20);
			this.toolsMenu.Text = "&Tools";
			// 
			// sort
			// 
			this.sort.Name = "sort";
			this.sort.Size = new System.Drawing.Size(187, 22);
			this.sort.Text = "&Sort";
			// 
			// editMetadata
			// 
			this.editMetadata.Checked = true;
			this.editMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
			this.editMetadata.Name = "editMetadata";
			this.editMetadata.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.editMetadata.Size = new System.Drawing.Size(187, 22);
			this.editMetadata.Text = "&Edit metadata";
			// 
			// swapAll
			// 
			this.swapAll.Name = "swapAll";
			this.swapAll.Size = new System.Drawing.Size(187, 22);
			this.swapAll.Text = "S&wap all";
			// 
			// itemContextMenu
			// 
			this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.swap,
            this.remove});
			this.itemContextMenu.Name = "itemContextMenu";
			this.itemContextMenu.Size = new System.Drawing.Size(118, 48);
			// 
			// swap
			// 
			this.swap.Name = "swap";
			this.swap.Size = new System.Drawing.Size(117, 22);
			this.swap.Text = "&Swap";
			// 
			// remove
			// 
			this.remove.Name = "remove";
			this.remove.Size = new System.Drawing.Size(117, 22);
			this.remove.Text = "&Remove";
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
			// metaFlow
			// 
			metaFlow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			metaFlow.Controls.Add(this.namePanel);
			metaFlow.Controls.Add(this.authorPanel);
			metaFlow.Controls.Add(this.urlPanel);
			metaFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			metaFlow.Location = new System.Drawing.Point(73, 7);
			metaFlow.Name = "metaFlow";
			metaFlow.Size = new System.Drawing.Size(10754, 60);
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
			// shadow
			// 
			this.shadow.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.shadow.Dock = System.Windows.Forms.DockStyle.Top;
			this.shadow.Image = Properties.Resources.TopInwardShadow;
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
			this.icon.Image = Properties.Resources.GenericDocument96;
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
			this.Name = "ListBuilder";
			this.Text = "ListBuilder";
			this.mainMenu.ResumeLayout(false);
			this.mainMenu.PerformLayout();
			this.itemContextMenu.ResumeLayout(false);
			this.meta.ResumeLayout(false);
			metaFlow.ResumeLayout(false);
			metaFlow.PerformLayout();
			this.namePanel.ResumeLayout(false);
			this.namePanel.PerformLayout();
			this.authorPanel.ResumeLayout(false);
			this.authorPanel.PerformLayout();
			this.urlPanel.ResumeLayout(false);
			this.urlPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.shadow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem listMenu;
		private System.Windows.Forms.ToolStripMenuItem save;
		private System.Windows.Forms.ToolStripMenuItem saveAs;
		private System.Windows.Forms.ToolStripMenuItem copyAsMenu;
		private System.Windows.Forms.ToolStripMenuItem copyAsCsv;
		private System.Windows.Forms.ToolStripMenuItem toolsMenu;
		private System.Windows.Forms.ToolStripMenuItem sort;
		private System.Windows.Forms.ToolStripMenuItem editMetadata;
		private System.Windows.Forms.ContextMenuStrip itemContextMenu;
		private System.Windows.Forms.ToolStripMenuItem swap;
		private System.Windows.Forms.ToolStripMenuItem remove;
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
		private System.Windows.Forms.ToolStripMenuItem swapAll;
	}
}