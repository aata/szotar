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
			this.results = new System.Windows.Forms.ListView();
			this.nameColumn = new System.Windows.Forms.ColumnHeader();
			this.dateColumn = new System.Windows.Forms.ColumnHeader();
			this.termsColumn = new System.Windows.Forms.ColumnHeader();
			this.acceptButton = new System.Windows.Forms.Button();
			this.searchBox = new Controls.SearchBox();
			this.SuspendLayout();
			// 
			// search
			// 
			this.search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.search.Location = new System.Drawing.Point(390, 3);
			this.search.Name = "search";
			this.search.Size = new System.Drawing.Size(100, 23);
			this.search.TabIndex = 1;
			this.search.Text = "&Search";
			this.search.UseVisualStyleBackColor = true;
			// 
			// results
			// 
			this.results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.dateColumn,
            this.termsColumn});
			this.results.FullRowSelect = true;
			this.results.Location = new System.Drawing.Point(4, 30);
			this.results.Name = "results";
			this.results.Size = new System.Drawing.Size(486, 311);
			this.results.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.results.TabIndex = 2;
			this.results.UseCompatibleStateImageBehavior = false;
			this.results.View = System.Windows.Forms.View.Details;
			this.results.ItemActivate += new System.EventHandler(this.results_ItemActivate);
			// 
			// nameColumn
			// 
			this.nameColumn.Text = "Name";
			this.nameColumn.Width = 236;
			// 
			// dateColumn
			// 
			this.dateColumn.Text = "Date Modified";
			this.dateColumn.Width = 101;
			// 
			// termsColumn
			// 
			this.termsColumn.Text = "Terms";
			this.termsColumn.Width = 61;
			// 
			// acceptButton
			// 
			this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.acceptButton.Location = new System.Drawing.Point(373, 347);
			this.acceptButton.Name = "acceptButton";
			this.acceptButton.Size = new System.Drawing.Size(117, 23);
			this.acceptButton.TabIndex = 4;
			this.acceptButton.Text = "&Practice";
			this.acceptButton.UseVisualStyleBackColor = true;
			this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
			// 
			// searchBox
			// 
			this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.searchBox.ForeColor = System.Drawing.SystemColors.GrayText;
			this.searchBox.Location = new System.Drawing.Point(4, 3);
			this.searchBox.Name = "searchBox";
			this.searchBox.PromptColor = System.Drawing.SystemColors.GrayText;
			this.searchBox.PromptFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
			this.searchBox.PromptText = "Search";
			this.searchBox.Size = new System.Drawing.Size(380, 20);
			this.searchBox.TabIndex = 3;
			// 
			// ListSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.acceptButton);
			this.Controls.Add(this.searchBox);
			this.Controls.Add(this.results);
			this.Controls.Add(this.search);
			this.DoubleBuffered = true;
			this.Name = "ListSearch";
			this.Size = new System.Drawing.Size(493, 373);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button search;
		private System.Windows.Forms.ListView results;
		private System.Windows.Forms.ColumnHeader nameColumn;
		private System.Windows.Forms.ColumnHeader dateColumn;
		private System.Windows.Forms.ColumnHeader termsColumn;
		private Szotar.WindowsForms.Controls.SearchBox searchBox;
		private System.Windows.Forms.Button acceptButton;
	}
}
