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
            this.languageLabel = new System.Windows.Forms.Label();
            this.englishUS = new System.Windows.Forms.RadioButton();
            this.hungarian = new System.Windows.Forms.RadioButton();
            this.information = new System.Windows.Forms.Label();
            this.englishGB = new System.Windows.Forms.RadioButton();
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
            // languageLabel
            // 
            resources.ApplyResources(this.languageLabel, "languageLabel");
            this.languageLabel.Name = "languageLabel";
            // 
            // englishUS
            // 
            resources.ApplyResources(this.englishUS, "englishUS");
            this.englishUS.Name = "englishUS";
            this.englishUS.UseVisualStyleBackColor = true;
            // 
            // hungarian
            // 
            resources.ApplyResources(this.hungarian, "hungarian");
            this.hungarian.Name = "hungarian";
            this.hungarian.UseVisualStyleBackColor = true;
            // 
            // information
            // 
            resources.ApplyResources(this.information, "information");
            this.information.ForeColor = System.Drawing.SystemColors.GrayText;
            this.information.Name = "information";
            // 
            // englishGB
            // 
            resources.ApplyResources(this.englishGB, "englishGB");
            this.englishGB.Name = "englishGB";
            this.englishGB.UseVisualStyleBackColor = true;
            // 
            // Display
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.englishGB);
            this.Controls.Add(this.information);
            this.Controls.Add(this.hungarian);
            this.Controls.Add(this.englishUS);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.listFontButton);
            this.Controls.Add(this.listFontLabel);
            this.Name = "Display";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label listFontLabel;
		private System.Windows.Forms.Button listFontButton;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.RadioButton englishUS;
        private System.Windows.Forms.RadioButton hungarian;
        private System.Windows.Forms.Label information;
        private System.Windows.Forms.RadioButton englishGB;
	}
}
