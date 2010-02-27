namespace Szotar.WindowsForms.Dialogs {
	partial class EditPracticeItem {
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
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.phrase = new System.Windows.Forms.TextBox();
			this.translation = new System.Windows.Forms.TextBox();
			this.phraseLabel = new System.Windows.Forms.Label();
			this.translationLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ok
			// 
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.Location = new System.Drawing.Point(189, 61);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 23);
			this.ok.TabIndex = 4;
			this.ok.Text = "&OK";
			this.ok.UseVisualStyleBackColor = true;
			// 
			// cancel
			// 
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(270, 61);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 5;
			this.cancel.Text = "&Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// phrase
			// 
			this.phrase.Location = new System.Drawing.Point(79, 9);
			this.phrase.Name = "phrase";
			this.phrase.Size = new System.Drawing.Size(266, 20);
			this.phrase.TabIndex = 1;
			// 
			// translation
			// 
			this.translation.Location = new System.Drawing.Point(79, 35);
			this.translation.Name = "translation";
			this.translation.Size = new System.Drawing.Size(266, 20);
			this.translation.TabIndex = 3;
			// 
			// phraseLabel
			// 
			this.phraseLabel.AutoSize = true;
			this.phraseLabel.Location = new System.Drawing.Point(12, 9);
			this.phraseLabel.Name = "phraseLabel";
			this.phraseLabel.Size = new System.Drawing.Size(40, 13);
			this.phraseLabel.TabIndex = 0;
			this.phraseLabel.Text = "Phrase";
			// 
			// translationLabel
			// 
			this.translationLabel.AutoSize = true;
			this.translationLabel.Location = new System.Drawing.Point(12, 38);
			this.translationLabel.Name = "translationLabel";
			this.translationLabel.Size = new System.Drawing.Size(59, 13);
			this.translationLabel.TabIndex = 2;
			this.translationLabel.Text = "Translation";
			// 
			// EditPracticeItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(357, 91);
			this.Controls.Add(this.translationLabel);
			this.Controls.Add(this.phraseLabel);
			this.Controls.Add(this.translation);
			this.Controls.Add(this.phrase);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "EditPracticeItem";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit entry";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ok;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.TextBox phrase;
		private System.Windows.Forms.TextBox translation;
		private System.Windows.Forms.Label phraseLabel;
		private System.Windows.Forms.Label translationLabel;
	}
}