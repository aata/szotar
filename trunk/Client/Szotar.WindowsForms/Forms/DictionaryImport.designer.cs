﻿namespace Szotar.WindowsForms.Forms {
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
			this.select = new System.Windows.Forms.ComboBox();
			this.content = new System.Windows.Forms.Panel();
			prompt = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// prompt
			// 
			prompt.AutoSize = true;
			prompt.Location = new System.Drawing.Point(13, 13);
			prompt.Name = "prompt";
			prompt.Size = new System.Drawing.Size(106, 15);
			prompt.TabIndex = 0;
			prompt.Text = "Select an Importer:";
			// 
			// select
			// 
			this.select.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.select.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.select.FormattingEnabled = true;
			this.select.Location = new System.Drawing.Point(125, 10);
			this.select.Name = "select";
			this.select.Size = new System.Drawing.Size(415, 23);
			this.select.TabIndex = 0;
			// 
			// content
			// 
			this.content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.content.Location = new System.Drawing.Point(0, 40);
			this.content.Name = "content";
			this.content.Size = new System.Drawing.Size(552, 329);
			this.content.TabIndex = 2;
			// 
			// DictionaryImport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(552, 368);
			this.Controls.Add(this.content);
			this.Controls.Add(this.select);
			this.Controls.Add(prompt);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(400, 270);
			this.Name = "DictionaryImport";
			this.Text = "DictionaryImport";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox select;
		private System.Windows.Forms.Panel content;
	}
}