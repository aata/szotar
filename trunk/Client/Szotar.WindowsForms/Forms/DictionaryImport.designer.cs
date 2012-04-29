namespace Szotar.WindowsForms.Forms {
	partial class DictionaryImport {
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
			System.Windows.Forms.Label prompt;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryImport));
			this.select = new System.Windows.Forms.ComboBox();
			this.content = new System.Windows.Forms.Panel();
			prompt = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// prompt
			// 
			resources.ApplyResources(prompt, "prompt");
			prompt.Name = "prompt";
			// 
			// select
			// 
			resources.ApplyResources(this.select, "select");
			this.select.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.select.FormattingEnabled = true;
			this.select.Name = "select";
			// 
			// content
			// 
			resources.ApplyResources(this.content, "content");
			this.content.Name = "content";
			// 
			// DictionaryImport
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.content);
			this.Controls.Add(this.select);
			this.Controls.Add(prompt);
			this.Name = "DictionaryImport";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox select;
		private System.Windows.Forms.Panel content;
	}
}