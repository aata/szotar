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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditPracticeItem));
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
            resources.ApplyResources(this.ok, "ok");
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Name = "ok";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // phrase
            // 
            resources.ApplyResources(this.phrase, "phrase");
            this.phrase.Name = "phrase";
            // 
            // translation
            // 
            resources.ApplyResources(this.translation, "translation");
            this.translation.Name = "translation";
            // 
            // phraseLabel
            // 
            resources.ApplyResources(this.phraseLabel, "phraseLabel");
            this.phraseLabel.Name = "phraseLabel";
            // 
            // translationLabel
            // 
            resources.ApplyResources(this.translationLabel, "translationLabel");
            this.translationLabel.Name = "translationLabel";
            // 
            // EditPracticeItem
            // 
            this.AcceptButton = this.ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.Controls.Add(this.translationLabel);
            this.Controls.Add(this.phraseLabel);
            this.Controls.Add(this.translation);
            this.Controls.Add(this.phrase);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditPracticeItem";
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