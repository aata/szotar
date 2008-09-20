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
			this.listFontLabel = new System.Windows.Forms.Label();
			this.listFontButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listFontLabel
			// 
			this.listFontLabel.AutoSize = true;
			this.listFontLabel.Location = new System.Drawing.Point(4, 4);
			this.listFontLabel.Name = "listFontLabel";
			this.listFontLabel.Size = new System.Drawing.Size(44, 13);
			this.listFontLabel.TabIndex = 0;
			this.listFontLabel.Text = "List font";
			// 
			// listFontButton
			// 
			this.listFontButton.AutoSize = true;
			this.listFontButton.Location = new System.Drawing.Point(7, 21);
			this.listFontButton.Name = "listFontButton";
			this.listFontButton.Size = new System.Drawing.Size(111, 23);
			this.listFontButton.TabIndex = 1;
			this.listFontButton.Text = "Font";
			this.listFontButton.UseVisualStyleBackColor = true;
			// 
			// Display
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listFontButton);
			this.Controls.Add(this.listFontLabel);
			this.Name = "Display";
			this.Size = new System.Drawing.Size(499, 309);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label listFontLabel;
		private System.Windows.Forms.Button listFontButton;
	}
}
