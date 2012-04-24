namespace Szotar.WindowsForms.Preferences {
	partial class Display {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display));
            this.listFontLabel = new System.Windows.Forms.Label();
            this.listFontButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listFontLabel
            // 
            resources.ApplyResources(this.listFontLabel, "listFontLabel");
            this.listFontLabel.Name = "listFontLabel";
            // 
            // listFontButton
            // 
            resources.ApplyResources(this.listFontButton, "listFontButton");
            this.listFontButton.Name = "listFontButton";
            this.listFontButton.UseVisualStyleBackColor = true;
            // 
            // Display
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listFontButton);
            this.Controls.Add(this.listFontLabel);
            this.Name = "Display";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label listFontLabel;
		private System.Windows.Forms.Button listFontButton;
	}
}
