namespace Szotar.WindowsForms.Controls {
    partial class DictionarySectionUI {
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
            this.browse = new System.Windows.Forms.Button();
            this.fileName = new System.Windows.Forms.TextBox();
            this.pleaseSelect = new System.Windows.Forms.Label();
            this.fileSelectNext = new System.Windows.Forms.Button();
            this.generateSecondHalf = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(288, 29);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(37, 27);
            this.browse.TabIndex = 5;
            this.browse.Text = "...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // fileName
            // 
            this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileName.Location = new System.Drawing.Point(7, 33);
            this.fileName.Name = "fileName";
            this.fileName.Size = new System.Drawing.Size(275, 23);
            this.fileName.TabIndex = 4;
            this.fileName.TextChanged += new System.EventHandler(this.fileName_TextChanged);
            // 
            // pleaseSelect
            // 
            this.pleaseSelect.AutoSize = true;
            this.pleaseSelect.Location = new System.Drawing.Point(3, 12);
            this.pleaseSelect.Name = "pleaseSelect";
            this.pleaseSelect.Size = new System.Drawing.Size(168, 15);
            this.pleaseSelect.TabIndex = 3;
            this.pleaseSelect.Text = "Please select the file to import.";
            // 
            // fileSelectNext
            // 
            this.fileSelectNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectNext.Enabled = false;
            this.fileSelectNext.Location = new System.Drawing.Point(250, 87);
            this.fileSelectNext.Name = "fileSelectNext";
            this.fileSelectNext.Size = new System.Drawing.Size(75, 23);
            this.fileSelectNext.TabIndex = 6;
            this.fileSelectNext.Text = "&Next";
            this.fileSelectNext.UseVisualStyleBackColor = true;
            this.fileSelectNext.Click += new System.EventHandler(this.fileSelectNext_Click);
            // 
            // generateSecondHalf
            // 
            this.generateSecondHalf.AutoSize = true;
            this.generateSecondHalf.Checked = true;
            this.generateSecondHalf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateSecondHalf.Location = new System.Drawing.Point(6, 63);
            this.generateSecondHalf.Name = "generateSecondHalf";
            this.generateSecondHalf.Size = new System.Drawing.Size(302, 19);
            this.generateSecondHalf.TabIndex = 7;
            this.generateSecondHalf.Text = "Generate second half of dictionary from the first half";
            this.generateSecondHalf.UseVisualStyleBackColor = true;
            // 
            // DictionarySectionUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generateSecondHalf);
            this.Controls.Add(this.fileSelectNext);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.fileName);
            this.Controls.Add(this.pleaseSelect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DictionarySectionUI";
            this.Size = new System.Drawing.Size(328, 113);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.Label pleaseSelect;
        private System.Windows.Forms.Button fileSelectNext;
        private System.Windows.Forms.CheckBox generateSecondHalf;
    }
}
