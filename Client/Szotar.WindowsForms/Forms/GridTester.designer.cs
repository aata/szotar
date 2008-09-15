namespace Szotar.WindowsForms.Forms {
	partial class GridTester {
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
			this.dgrid = new Szotar.WindowsForms.Controls.DictionaryGrid();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// dgrid
			// 
			this.dgrid.AllowNewItems = false;
			this.dgrid.DataSource = null;
			this.dgrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dgrid.Location = new System.Drawing.Point(12, 12);
			this.dgrid.Name = "dgrid";
			this.dgrid.ShowMutableRows = false;
			this.dgrid.Size = new System.Drawing.Size(291, 282);
			this.dgrid.TabIndex = 0;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.Location = new System.Drawing.Point(501, 12);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(262, 534);
			this.propertyGrid.TabIndex = 1;
			// 
			// GridTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(775, 558);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.dgrid);
			this.Name = "GridTester";
			this.Text = "GridTester";
			this.ResumeLayout(false);

		}

		#endregion

		private Szotar.WindowsForms.Controls.DictionaryGrid dgrid;
		private System.Windows.Forms.PropertyGrid propertyGrid;
	}
}