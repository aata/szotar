namespace Szotar.WindowsForms.Preferences {
	partial class ResetSettings {
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

		public override void Commit() {
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.warning = new System.Windows.Forms.Label();
			this.resetButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Controls.Add(this.warning);
			this.flowLayoutPanel.Controls.Add(this.resetButton);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(487, 353);
			this.flowLayoutPanel.TabIndex = 0;
			// 
			// warning
			// 
			this.warning.AutoSize = true;
			this.warning.Location = new System.Drawing.Point(3, 0);
			this.warning.Name = "warning";
			this.warning.Size = new System.Drawing.Size(481, 26);
			this.warning.TabIndex = 0;
			this.warning.Text = "This will reset all of the settings to their defaults. This will not affect store" +
				"d word lists or statistics. Only use this if necessary.";
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(3, 29);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(75, 23);
			this.resetButton.TabIndex = 1;
			this.resetButton.Text = "&Reset";
			this.resetButton.UseVisualStyleBackColor = true;
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// ResetSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel);
			this.Name = "ResetSettings";
			this.Size = new System.Drawing.Size(487, 353);
			this.flowLayoutPanel.ResumeLayout(false);
			this.flowLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private System.Windows.Forms.Label warning;
		private System.Windows.Forms.Button resetButton;
	}
}
