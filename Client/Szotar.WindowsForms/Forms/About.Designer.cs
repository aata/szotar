namespace Szotar.WindowsForms.Forms {
	partial class About {
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
			System.Windows.Forms.TableLayoutPanel table;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
			this.webLink = new System.Windows.Forms.LinkLabel();
			this.version = new System.Windows.Forms.Label();
			this.productName = new System.Windows.Forms.Label();
			table = new System.Windows.Forms.TableLayoutPanel();
			table.SuspendLayout();
			this.SuspendLayout();
			// 
			// table
			// 
			resources.ApplyResources(table, "table");
			table.Controls.Add(this.webLink, 0, 2);
			table.Controls.Add(this.version, 0, 1);
			table.Controls.Add(this.productName, 0, 0);
			table.Name = "table";
			// 
			// webLink
			// 
			resources.ApplyResources(this.webLink, "webLink");
			this.webLink.Name = "webLink";
			this.webLink.TabStop = true;
			// 
			// version
			// 
			resources.ApplyResources(this.version, "version");
			this.version.Name = "version";
			// 
			// productName
			// 
			resources.ApplyResources(this.productName, "productName");
			this.productName.Name = "productName";
			// 
			// About
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(table);
			this.Name = "About";
			table.ResumeLayout(false);
			table.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label version;
		private System.Windows.Forms.Label productName;
		private System.Windows.Forms.LinkLabel webLink;
	}
}