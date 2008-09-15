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
			this.infoLabel = new System.Windows.Forms.Label();
			this.startPage = new System.Windows.Forms.RadioButton();
			this.dictionary = new System.Windows.Forms.RadioButton();
			this.practice = new System.Windows.Forms.RadioButton();
			this.list = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// infoLabel
			// 
			this.infoLabel.AutoSize = true;
			this.infoLabel.Location = new System.Drawing.Point(4, 4);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new System.Drawing.Size(84, 13);
			this.infoLabel.TabIndex = 0;
			this.infoLabel.Text = "When {0} starts:";
			// 
			// startPage
			// 
			this.startPage.AutoSize = true;
			this.startPage.Location = new System.Drawing.Point(7, 21);
			this.startPage.Name = "startPage";
			this.startPage.Size = new System.Drawing.Size(123, 17);
			this.startPage.TabIndex = 1;
			this.startPage.TabStop = true;
			this.startPage.Text = "Show the Start Page";
			this.startPage.UseVisualStyleBackColor = true;
			// 
			// dictionary
			// 
			this.dictionary.AutoSize = true;
			this.dictionary.Location = new System.Drawing.Point(7, 44);
			this.dictionary.Name = "dictionary";
			this.dictionary.Size = new System.Drawing.Size(108, 17);
			this.dictionary.TabIndex = 1;
			this.dictionary.TabStop = true;
			this.dictionary.Text = "Open a dictionary";
			this.dictionary.UseVisualStyleBackColor = true;
			// 
			// practice
			// 
			this.practice.AutoSize = true;
			this.practice.Location = new System.Drawing.Point(7, 94);
			this.practice.Name = "practice";
			this.practice.Size = new System.Drawing.Size(131, 17);
			this.practice.TabIndex = 1;
			this.practice.TabStop = true;
			this.practice.Text = "Open practice window";
			this.practice.UseVisualStyleBackColor = true;
			// 
			// list
			// 
			this.list.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point(25, 67);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(264, 21);
			this.list.TabIndex = 2;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			// 
			// Startup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.list);
			this.Controls.Add(this.practice);
			this.Controls.Add(this.dictionary);
			this.Controls.Add(this.startPage);
			this.Controls.Add(this.infoLabel);
			this.Name = "Startup";
			this.Size = new System.Drawing.Size(292, 201);
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
