namespace Szotar.WindowsForms.Preferences {
	partial class Startup {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Startup));
            this.infoLabel = new System.Windows.Forms.Label();
            this.startPage = new System.Windows.Forms.RadioButton();
            this.dictionary = new System.Windows.Forms.RadioButton();
            this.practice = new System.Windows.Forms.RadioButton();
            this.list = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            resources.ApplyResources(this.infoLabel, "infoLabel");
            this.infoLabel.Name = "infoLabel";
            // 
            // startPage
            // 
            resources.ApplyResources(this.startPage, "startPage");
            this.startPage.Name = "startPage";
            this.startPage.TabStop = true;
            this.startPage.UseVisualStyleBackColor = true;
            // 
            // dictionary
            // 
            resources.ApplyResources(this.dictionary, "dictionary");
            this.dictionary.Name = "dictionary";
            this.dictionary.TabStop = true;
            this.dictionary.UseVisualStyleBackColor = true;
            // 
            // practice
            // 
            resources.ApplyResources(this.practice, "practice");
            this.practice.Name = "practice";
            this.practice.TabStop = true;
            this.practice.UseVisualStyleBackColor = true;
            // 
            // list
            // 
            resources.ApplyResources(this.list, "list");
            this.list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.list.FormattingEnabled = true;
            this.list.Name = "list";
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // Startup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.list);
            this.Controls.Add(this.practice);
            this.Controls.Add(this.dictionary);
            this.Controls.Add(this.startPage);
            this.Controls.Add(this.infoLabel);
            this.Name = "Startup";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label infoLabel;
		private System.Windows.Forms.RadioButton startPage;
		private System.Windows.Forms.RadioButton dictionary;
		private System.Windows.Forms.RadioButton practice;
		private System.Windows.Forms.ComboBox list;
	}
}
