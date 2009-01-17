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
			System.Windows.Forms.TabPage dictionaryTab;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
			System.Windows.Forms.TabPage practiceTab;
			this.dictionaries = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.listSearch = new Szotar.WindowsForms.Controls.ListSearch();
			this.tasks = new System.Windows.Forms.TabControl();
			this.panel = new System.Windows.Forms.Panel();
			this.logo = new System.Windows.Forms.PictureBox();
			dictionaryTab = new System.Windows.Forms.TabPage();
			practiceTab = new System.Windows.Forms.TabPage();
			dictionaryTab.SuspendLayout();
			practiceTab.SuspendLayout();
			this.tasks.SuspendLayout();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
			this.SuspendLayout();
			// 
			// dictionaryTab
			// 
			dictionaryTab.Controls.Add(this.dictionaries);
			dictionaryTab.Location = new System.Drawing.Point(4, 22);
			dictionaryTab.Name = "dictionaryTab";
			dictionaryTab.Padding = new System.Windows.Forms.Padding(3);
			dictionaryTab.Size = new System.Drawing.Size(509, 312);
			dictionaryTab.TabIndex = 0;
			dictionaryTab.Text = "Dictionary";
			dictionaryTab.UseVisualStyleBackColor = true;
			// 
			// dictionaries
			// 
			this.dictionaries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dictionaries.LargeImageList = this.imageList;
			this.dictionaries.Location = new System.Drawing.Point(3, 3);
			this.dictionaries.MultiSelect = false;
			this.dictionaries.Name = "dictionaries";
			this.dictionaries.Size = new System.Drawing.Size(503, 306);
			this.dictionaries.SmallImageList = this.imageList;
			this.dictionaries.TabIndex = 0;
			this.dictionaries.UseCompatibleStateImageBehavior = false;
			this.dictionaries.View = System.Windows.Forms.View.Tile;
			this.dictionaries.ItemActivate += new System.EventHandler(this.OnItemActivate);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Dictionary");
			// 
			// practiceTab
			// 
			practiceTab.Controls.Add(this.listSearch);
			practiceTab.Location = new System.Drawing.Point(4, 22);
			practiceTab.Name = "practiceTab";
			practiceTab.Padding = new System.Windows.Forms.Padding(3);
			practiceTab.Size = new System.Drawing.Size(509, 312);
			practiceTab.TabIndex = 1;
			practiceTab.Text = "Practice";
			practiceTab.UseVisualStyleBackColor = true;
			// 
			// listSearch
			// 
			this.listSearch.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listSearch.Location = new System.Drawing.Point(3, 3);
			this.listSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.listSearch.Name = "listSearch";
			this.listSearch.Size = new System.Drawing.Size(503, 306);
			this.listSearch.TabIndex = 0;
			// 
			// tasks
			// 
			this.tasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tasks.Controls.Add(dictionaryTab);
			this.tasks.Controls.Add(practiceTab);
			this.tasks.Location = new System.Drawing.Point(6, 68);
			this.tasks.Name = "tasks";
			this.tasks.SelectedIndex = 0;
			this.tasks.Size = new System.Drawing.Size(517, 338);
			this.tasks.TabIndex = 0;
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(153)))), ((int)(((byte)(102)))));
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.logo);
			this.panel.Location = new System.Drawing.Point(-1, -2);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(531, 64);
			this.panel.TabIndex = 1;
			// 
			// logo
			// 
			this.logo.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.logo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("logo.BackgroundImage")));
			this.logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.logo.Location = new System.Drawing.Point(7, 8);
			this.logo.Name = "logo";
			this.logo.Size = new System.Drawing.Size(515, 53);
			this.logo.TabIndex = 0;
			this.logo.TabStop = false;
			// 
			// StartPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(529, 412);
			this.Controls.Add(this.panel);
			this.Controls.Add(this.tasks);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "StartPage";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.Text = "Szótár";
			dictionaryTab.ResumeLayout(false);
			practiceTab.ResumeLayout(false);
			this.tasks.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tasks;
		private Szotar.WindowsForms.Controls.ListSearch listSearch;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.PictureBox logo;
		private System.Windows.Forms.ListView dictionaries;
		private System.Windows.Forms.ImageList imageList;

	}
}