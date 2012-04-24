namespace Szotar.WindowsForms.Forms {
    partial class DictionaryInfoEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
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
        private void InitializeComponent () {
            System.Windows.Forms.Label nameLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryInfoEditor));
            System.Windows.Forms.Label authorLabel;
            System.Windows.Forms.Label urlLabel;
            System.Windows.Forms.Label firstLanguageLabel;
            System.Windows.Forms.Label secondLanguageLabel;
            this.save = new System.Windows.Forms.Button();
            this.group = new System.Windows.Forms.GroupBox();
            this.secondLanguage = new System.Windows.Forms.TextBox();
            this.firstLanguage = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.author = new System.Windows.Forms.TextBox();
            this.name = new System.Windows.Forms.TextBox();
            this.cancel = new System.Windows.Forms.Button();
            nameLabel = new System.Windows.Forms.Label();
            authorLabel = new System.Windows.Forms.Label();
            urlLabel = new System.Windows.Forms.Label();
            firstLanguageLabel = new System.Windows.Forms.Label();
            secondLanguageLabel = new System.Windows.Forms.Label();
            this.group.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            resources.ApplyResources(nameLabel, "nameLabel");
            nameLabel.Name = "nameLabel";
            // 
            // authorLabel
            // 
            resources.ApplyResources(authorLabel, "authorLabel");
            authorLabel.Name = "authorLabel";
            // 
            // urlLabel
            // 
            resources.ApplyResources(urlLabel, "urlLabel");
            urlLabel.Name = "urlLabel";
            // 
            // firstLanguageLabel
            // 
            resources.ApplyResources(firstLanguageLabel, "firstLanguageLabel");
            firstLanguageLabel.Name = "firstLanguageLabel";
            // 
            // secondLanguageLabel
            // 
            resources.ApplyResources(secondLanguageLabel, "secondLanguageLabel");
            secondLanguageLabel.Name = "secondLanguageLabel";
            // 
            // save
            // 
            resources.ApplyResources(this.save, "save");
            this.save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.save.Name = "save";
            this.save.UseVisualStyleBackColor = true;
            // 
            // group
            // 
            resources.ApplyResources(this.group, "group");
            this.group.Controls.Add(this.secondLanguage);
            this.group.Controls.Add(this.firstLanguage);
            this.group.Controls.Add(this.url);
            this.group.Controls.Add(this.author);
            this.group.Controls.Add(this.name);
            this.group.Controls.Add(secondLanguageLabel);
            this.group.Controls.Add(firstLanguageLabel);
            this.group.Controls.Add(urlLabel);
            this.group.Controls.Add(authorLabel);
            this.group.Controls.Add(nameLabel);
            this.group.Name = "group";
            this.group.TabStop = false;
            // 
            // secondLanguage
            // 
            resources.ApplyResources(this.secondLanguage, "secondLanguage");
            this.secondLanguage.Name = "secondLanguage";
            // 
            // firstLanguage
            // 
            resources.ApplyResources(this.firstLanguage, "firstLanguage");
            this.firstLanguage.Name = "firstLanguage";
            // 
            // url
            // 
            resources.ApplyResources(this.url, "url");
            this.url.Name = "url";
            // 
            // author
            // 
            resources.ApplyResources(this.author, "author");
            this.author.Name = "author";
            // 
            // name
            // 
            resources.ApplyResources(this.name, "name");
            this.name.Name = "name";
            // 
            // cancel
            // 
            resources.ApplyResources(this.cancel, "cancel");
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Name = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // DictionaryInfoEditor
            // 
            this.AcceptButton = this.save;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.Controls.Add(this.group);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.Name = "DictionaryInfoEditor";
            this.group.ResumeLayout(false);
            this.group.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button save;
		private System.Windows.Forms.GroupBox group;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.TextBox author;
		private System.Windows.Forms.TextBox url;
		private System.Windows.Forms.TextBox secondLanguage;
		private System.Windows.Forms.TextBox firstLanguage;


    }
}