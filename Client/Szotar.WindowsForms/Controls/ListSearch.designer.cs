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
			this.search = new System.Windows.Forms.Button();
			this.searchBox = new Szotar.WindowsForms.Controls.SearchBox();
			this.results = new Szotar.WindowsForms.Controls.ListViewNF();
			this.firstColumn = new System.Windows.Forms.ColumnHeader();
			this.secondColumn = new System.Windows.Forms.ColumnHeader();
			this.thirdColumn = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// search
			// 
			this.search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.search.Location = new System.Drawing.Point(393, 0);
			this.search.Name = "search";
			this.search.Size = new System.Drawing.Size(100, 23);
			this.search.TabIndex = 1;
			this.search.Text = "&Search";
			this.search.UseVisualStyleBackColor = true;
			// 
			// searchBox
			// 
			this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.searchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.searchBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this.searchBox.Location = new System.Drawing.Point(0, 0);
			this.searchBox.Name = "searchBox";
			this.searchBox.PromptText = "Search";
			this.searchBox.Size = new System.Drawing.Size(387, 20);
			this.searchBox.TabIndex = 0;
			// 
			// results
			// 
			this.results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.firstColumn,
            this.secondColumn,
            this.thirdColumn});
			this.results.FullRowSelect = true;
			this.results.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.results.Location = new System.Drawing.Point(0, 26);
			this.results.Name = "results";
			this.results.ShowItemToolTips = true;
			this.results.Size = new System.Drawing.Size(493, 314);
			this.results.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.results.TabIndex = 2;
			this.results.UseCompatibleStateImageBehavior = false;
			this.results.View = System.Windows.Forms.View.Details;
			this.results.ItemActivate += new System.EventHandler(this.results_ItemActivate);
			// 
			// firstColumn
			// 
			this.firstColumn.Text = "";
			this.firstColumn.Width = 177;
			// 
			// secondColumn
			// 
			this.secondColumn.Text = "";
			this.secondColumn.Width = 183;
			// 
			// thirdColumn
			// 
			this.thirdColumn.Text = "";
			this.thirdColumn.Width = 121;
			// 
			// ListSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.searchBox);
			this.Controls.Add(this.results);
			this.Controls.Add(this.search);
			this.DoubleBuffered = true;
			this.Name = "ListSearch";
			this.Size = new System.Drawing.Size(493, 340);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button search;
		private Szotar.WindowsForms.Controls.ListViewNF results;
		private System.Windows.Forms.ColumnHeader firstColumn;
		private System.Windows.Forms.ColumnHeader secondColumn;
		private System.Windows.Forms.ColumnHeader thirdColumn;
		private Szotar.WindowsForms.Controls.SearchBox searchBox;
	}
}
