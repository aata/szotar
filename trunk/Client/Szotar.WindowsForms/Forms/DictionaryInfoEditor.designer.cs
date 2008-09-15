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
			this.selectionPrompt = new System.Windows.Forms.Label();
			this.objects = new System.Windows.Forms.ComboBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.properties = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// selectionPrompt
			// 
			this.selectionPrompt.AutoSize = true;
			this.selectionPrompt.Location = new System.Drawing.Point(12, 15);
			this.selectionPrompt.Name = "selectionPrompt";
			this.selectionPrompt.Size = new System.Drawing.Size(28, 13);
			this.selectionPrompt.TabIndex = 1;
			this.selectionPrompt.Text = "Edit:";
			// 
			// objects
			// 
			this.objects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.objects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.objects.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.objects.FormattingEnabled = true;
			this.objects.Location = new System.Drawing.Point(76, 12);
			this.objects.Name = "objects";
			this.objects.Size = new System.Drawing.Size(424, 21);
			this.objects.TabIndex = 2;
			this.objects.SelectedIndexChanged += new System.EventHandler(this.dictionaries_SelectedIndexChanged);
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.Location = new System.Drawing.Point(425, 373);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 4;
			this.closeButton.Text = "&Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// properties
			// 
			this.properties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.properties.Location = new System.Drawing.Point(12, 39);
			this.properties.Name = "properties";
			this.properties.Size = new System.Drawing.Size(488, 328);
			this.properties.TabIndex = 5;
			this.properties.Visible = false;
			// 
			// DictionaryInfoEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(512, 408);
			this.Controls.Add(this.properties);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.objects);
			this.Controls.Add(this.selectionPrompt);
			this.MinimumSize = new System.Drawing.Size(264, 267);
			this.Name = "DictionaryInfoEditor";
			this.Text = "Edit Dictionary Info";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label selectionPrompt;
        private System.Windows.Forms.ComboBox objects;
        private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.PropertyGrid properties;


    }
}