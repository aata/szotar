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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionarySectionUI));
            this.browse = new System.Windows.Forms.Button();
            this.fileName = new System.Windows.Forms.TextBox();
            this.pleaseSelect = new System.Windows.Forms.Label();
            this.fileSelectNext = new System.Windows.Forms.Button();
            this.generateSecondHalf = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // browse
            // 
            resources.ApplyResources(this.browse, "browse");
            this.browse.Name = "browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // fileName
            // 
            resources.ApplyResources(this.fileName, "fileName");
            this.fileName.Name = "fileName";
            this.fileName.TextChanged += new System.EventHandler(this.fileName_TextChanged);
            // 
            // pleaseSelect
            // 
            resources.ApplyResources(this.pleaseSelect, "pleaseSelect");
            this.pleaseSelect.Name = "pleaseSelect";
            // 
            // fileSelectNext
            // 
            resources.ApplyResources(this.fileSelectNext, "fileSelectNext");
            this.fileSelectNext.Name = "fileSelectNext";
            this.fileSelectNext.UseVisualStyleBackColor = true;
            this.fileSelectNext.Click += new System.EventHandler(this.fileSelectNext_Click);
            // 
            // generateSecondHalf
            // 
            resources.ApplyResources(this.generateSecondHalf, "generateSecondHalf");
            this.generateSecondHalf.Checked = true;
            this.generateSecondHalf.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generateSecondHalf.Name = "generateSecondHalf";
            this.generateSecondHalf.UseVisualStyleBackColor = true;
            // 
            // DictionarySectionUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generateSecondHalf);
            this.Controls.Add(this.fileSelectNext);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.fileName);
            this.Controls.Add(this.pleaseSelect);
            this.Name = "DictionarySectionUI";
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
