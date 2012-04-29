namespace Szotar.WindowsForms.Preferences {
	partial class Practice {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Practice));
			this.defaultNumberOfItems = new System.Windows.Forms.Label();
			this.itemCount = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.itemCount)).BeginInit();
			this.SuspendLayout();
			// 
			// defaultNumberOfItems
			// 
			resources.ApplyResources(this.defaultNumberOfItems, "defaultNumberOfItems");
			this.defaultNumberOfItems.Name = "defaultNumberOfItems";
			// 
			// itemCount
			// 
			resources.ApplyResources(this.itemCount, "itemCount");
			this.itemCount.Maximum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.itemCount.Name = "itemCount";
			// 
			// Practice
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.itemCount);
			this.Controls.Add(this.defaultNumberOfItems);
			this.Name = "Practice";
			((System.ComponentModel.ISupportInitialize)(this.itemCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label defaultNumberOfItems;
		private System.Windows.Forms.NumericUpDown itemCount;
	}
}
