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
			System.Windows.Forms.Label authorLabel;
			System.Windows.Forms.Label urlLabel;
			System.Windows.Forms.Label firstLanguageLabel;
			System.Windows.Forms.Label secondLanguageLabel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryInfoEditor));
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
			nameLabel.AutoSize = true;
			nameLabel.Location = new System.Drawing.Point(7, 27);
			nameLabel.Name = "nameLabel";
			nameLabel.Size = new System.Drawing.Size(36, 13);
			nameLabel.TabIndex = 0;
			nameLabel.Text = "Name";
			// 
			// authorLabel
			// 
			authorLabel.AutoSize = true;
			authorLabel.Location = new System.Drawing.Point(7, 71);
			authorLabel.Name = "authorLabel";
			authorLabel.Size = new System.Drawing.Size(43, 13);
			authorLabel.TabIndex = 2;
			authorLabel.Text = "Author";
			// 
			// urlLabel
			// 
			urlLabel.AutoSize = true;
			urlLabel.Location = new System.Drawing.Point(7, 115);
			urlLabel.Name = "urlLabel";
			urlLabel.Size = new System.Drawing.Size(27, 13);
			urlLabel.TabIndex = 4;
			urlLabel.Text = "URL";
			// 
			// firstLanguageLabel
			// 
			firstLanguageLabel.AutoSize = true;
			firstLanguageLabel.Location = new System.Drawing.Point(7, 159);
			firstLanguageLabel.Name = "firstLanguageLabel";
			firstLanguageLabel.Size = new System.Drawing.Size(81, 13);
			firstLanguageLabel.TabIndex = 6;
			firstLanguageLabel.Text = "First language";
			// 
			// secondLanguageLabel
			// 
			secondLanguageLabel.AutoSize = true;
			secondLanguageLabel.Location = new System.Drawing.Point(7, 203);
			secondLanguageLabel.Name = "secondLanguageLabel";
			secondLanguageLabel.Size = new System.Drawing.Size(97, 13);
			secondLanguageLabel.TabIndex = 4;
			secondLanguageLabel.Text = "Second language";
			// 
			// save
			// 
			this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.save.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.save.Location = new System.Drawing.Point(243, 251);
			this.save.Name = "save";
			this.save.Size = new System.Drawing.Size(75, 23);
			this.save.TabIndex = 10;
			this.save.Text = "&Save";
			this.save.UseVisualStyleBackColor = true;
			// 
			// group
			// 
			this.group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
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
			this.group.Location = new System.Drawing.Point(13, 13);
			this.group.Name = "group";
			this.group.Size = new System.Drawing.Size(386, 232);
			this.group.TabIndex = 0;
			this.group.TabStop = false;
			// 
			// secondLanguage
			// 
			this.secondLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.secondLanguage.Location = new System.Drawing.Point(124, 198);
			this.secondLanguage.Name = "secondLanguage";
			this.secondLanguage.Size = new System.Drawing.Size(256, 22);
			this.secondLanguage.TabIndex = 9;
			// 
			// firstLanguage
			// 
			this.firstLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.firstLanguage.Location = new System.Drawing.Point(124, 154);
			this.firstLanguage.Name = "firstLanguage";
			this.firstLanguage.Size = new System.Drawing.Size(256, 22);
			this.firstLanguage.TabIndex = 7;
			// 
			// url
			// 
			this.url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.url.Location = new System.Drawing.Point(76, 110);
			this.url.Name = "url";
			this.url.Size = new System.Drawing.Size(304, 22);
			this.url.TabIndex = 5;
			// 
			// author
			// 
			this.author.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.author.Location = new System.Drawing.Point(76, 66);
			this.author.Name = "author";
			this.author.Size = new System.Drawing.Size(304, 22);
			this.author.TabIndex = 3;
			// 
			// name
			// 
			this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.name.Location = new System.Drawing.Point(76, 22);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(304, 22);
			this.name.TabIndex = 1;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(324, 251);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 11;
			this.cancel.Text = "&Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// DictionaryInfoEditor
			// 
			this.AcceptButton = this.save;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(411, 286);
			this.Controls.Add(this.group);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.save);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(100000, 324);
			this.MinimumSize = new System.Drawing.Size(427, 324);
			this.Name = "DictionaryInfoEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit {0}";
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