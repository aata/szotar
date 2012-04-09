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
            this.table.ColumnCount = 2;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.Controls.Add(this.phraseLabel, 0, 0);
            this.table.Controls.Add(this.phrase, 1, 0);
            this.table.Controls.Add(this.translationLabel, 0, 1);
            this.table.Controls.Add(this.translation, 1, 1);
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.Name = "table";
            this.table.RowCount = 3;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.Size = new System.Drawing.Size(226, 169);
            this.table.TabIndex = 0;
            // 
            // phraseLabel
            // 
            this.phraseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.phraseLabel.AutoSize = true;
            this.phraseLabel.Location = new System.Drawing.Point(3, 0);
            this.phraseLabel.Name = "phraseLabel";
            this.phraseLabel.Size = new System.Drawing.Size(59, 26);
            this.phraseLabel.TabIndex = 0;
            this.phraseLabel.Text = "Phrase";
            this.phraseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // phrase
            // 
            this.phrase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.phrase.Location = new System.Drawing.Point(68, 3);
            this.phrase.Name = "phrase";
            this.phrase.Size = new System.Drawing.Size(155, 20);
            this.phrase.TabIndex = 1;
            // 
            // translationLabel
            // 
            this.translationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.translationLabel.AutoSize = true;
            this.translationLabel.Location = new System.Drawing.Point(3, 26);
            this.translationLabel.Name = "translationLabel";
            this.translationLabel.Size = new System.Drawing.Size(59, 26);
            this.translationLabel.TabIndex = 2;
            this.translationLabel.Text = "Translation";
            this.translationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // translation
            // 
            this.translation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.translation.Location = new System.Drawing.Point(68, 29);
            this.translation.Name = "translation";
            this.translation.Size = new System.Drawing.Size(155, 20);
            this.translation.TabIndex = 3;
            // 
            // TermEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.table);
            this.Name = "TermEditor";
            this.Size = new System.Drawing.Size(226, 169);
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
