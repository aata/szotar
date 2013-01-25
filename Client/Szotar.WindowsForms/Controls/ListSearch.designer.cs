namespace Szotar.WindowsForms.Controls {
	partial class ListSearch {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListSearch));
            this.search = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.searchBox = new Szotar.WindowsForms.Controls.SearchBox();
            this.results = new Szotar.WindowsForms.Controls.ListViewNF();
            this.firstColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.secondColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.thirdColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // search
            // 
            resources.ApplyResources(this.search, "search");
            this.search.Name = "search";
            this.search.UseVisualStyleBackColor = true;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Dictionary");
            this.imageList.Images.SetKeyName(1, "List");
            this.imageList.Images.SetKeyName(2, "Tag");
            // 
            // imageListSmall
            // 
            this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
            this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSmall.Images.SetKeyName(0, "Dictionary");
            this.imageListSmall.Images.SetKeyName(1, "List");
            this.imageListSmall.Images.SetKeyName(2, "Tag");
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            // 
            // searchBox
            // 
            resources.ApplyResources(this.searchBox, "searchBox");
            this.searchBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.searchBox.Name = "searchBox";
            // 
            // results
            // 
            resources.ApplyResources(this.results, "results");
            this.results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.firstColumn,
            this.secondColumn,
            this.thirdColumn});
            this.results.FullRowSelect = true;
            this.results.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.results.LargeImageList = this.imageList;
            this.results.Name = "results";
            this.results.ShowItemToolTips = true;
            this.results.SmallImageList = this.imageListSmall;
            this.results.UseCompatibleStateImageBehavior = false;
            this.results.View = System.Windows.Forms.View.Details;
            this.results.ItemActivate += new System.EventHandler(this.ResultsItemActivate);
            // 
            // firstColumn
            // 
            resources.ApplyResources(this.firstColumn, "firstColumn");
            // 
            // secondColumn
            // 
            resources.ApplyResources(this.secondColumn, "secondColumn");
            // 
            // thirdColumn
            // 
            resources.ApplyResources(this.thirdColumn, "thirdColumn");
            // 
            // ListSearch
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.results);
            this.Controls.Add(this.search);
            this.DoubleBuffered = true;
            this.Name = "ListSearch";
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button search;
		private Szotar.WindowsForms.Controls.ListViewNF results;
		private System.Windows.Forms.ColumnHeader firstColumn;
		private System.Windows.Forms.ColumnHeader secondColumn;
		private System.Windows.Forms.ColumnHeader thirdColumn;
		private Szotar.WindowsForms.Controls.SearchBox searchBox;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ImageList imageListSmall;
	}
}
