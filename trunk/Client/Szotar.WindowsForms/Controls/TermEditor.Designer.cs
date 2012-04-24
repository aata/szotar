namespace Szotar.WindowsForms.Controls {
    partial class TermEditor {
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

            Item = null;

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermEditor));
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.phraseLabel = new System.Windows.Forms.Label();
            this.phrase = new System.Windows.Forms.TextBox();
            this.translationLabel = new System.Windows.Forms.Label();
            this.translation = new System.Windows.Forms.TextBox();
            this.table.SuspendLayout();
            this.SuspendLayout();
            // 
            // table
            // 
            resources.ApplyResources(this.table, "table");
            this.table.Controls.Add(this.phraseLabel, 0, 0);
            this.table.Controls.Add(this.phrase, 1, 0);
            this.table.Controls.Add(this.translationLabel, 0, 1);
            this.table.Controls.Add(this.translation, 1, 1);
            this.table.Name = "table";
            // 
            // phraseLabel
            // 
            resources.ApplyResources(this.phraseLabel, "phraseLabel");
            this.phraseLabel.Name = "phraseLabel";
            // 
            // phrase
            // 
            resources.ApplyResources(this.phrase, "phrase");
            this.phrase.Name = "phrase";
            // 
            // translationLabel
            // 
            resources.ApplyResources(this.translationLabel, "translationLabel");
            this.translationLabel.Name = "translationLabel";
            // 
            // translation
            // 
            resources.ApplyResources(this.translation, "translation");
            this.translation.Name = "translation";
            // 
            // TermEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.table);
            this.Name = "TermEditor";
            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.Label phraseLabel;
        private System.Windows.Forms.TextBox phrase;
        private System.Windows.Forms.Label translationLabel;
        private System.Windows.Forms.TextBox translation;
    }
}
