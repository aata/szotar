namespace Szotar.WindowsForms.Dialogs {
    partial class EditDictionaryItem {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem deleteMI;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditDictionaryItem));
            this.phraseLabel = new System.Windows.Forms.Label();
            this.phrase = new System.Windows.Forms.TextBox();
            this.translationsLabel = new System.Windows.Forms.Label();
            this.complete = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.translation = new System.Windows.Forms.TextBox();
            this.addTranslation = new System.Windows.Forms.Button();
            this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.translations = new Szotar.WindowsForms.Controls.ListViewNF();
            this.translationText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            deleteMI = new System.Windows.Forms.ToolStripMenuItem();
            this.listContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // deleteMI
            // 
            deleteMI.Name = "deleteMI";
            deleteMI.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            deleteMI.ShowShortcutKeys = false;
            deleteMI.Size = new System.Drawing.Size(100, 22);
            deleteMI.Text = "&Delete";
            deleteMI.Click += new System.EventHandler(this.deleteMI_Click);
            // 
            // phraseLabel
            // 
            this.phraseLabel.AutoSize = true;
            this.phraseLabel.Location = new System.Drawing.Point(13, 13);
            this.phraseLabel.Name = "phraseLabel";
            this.phraseLabel.Size = new System.Drawing.Size(44, 13);
            this.phraseLabel.TabIndex = 0;
            this.phraseLabel.Text = "Phrase:";
            // 
            // phrase
            // 
            this.phrase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.phrase.Location = new System.Drawing.Point(89, 10);
            this.phrase.Name = "phrase";
            this.phrase.Size = new System.Drawing.Size(379, 22);
            this.phrase.TabIndex = 1;
            // 
            // translationsLabel
            // 
            this.translationsLabel.AutoSize = true;
            this.translationsLabel.Location = new System.Drawing.Point(13, 37);
            this.translationsLabel.Name = "translationsLabel";
            this.translationsLabel.Size = new System.Drawing.Size(72, 13);
            this.translationsLabel.TabIndex = 2;
            this.translationsLabel.Text = "Translations:";
            // 
            // complete
            // 
            this.complete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.complete.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.complete.Location = new System.Drawing.Point(312, 208);
            this.complete.Name = "complete";
            this.complete.Size = new System.Drawing.Size(75, 23);
            this.complete.TabIndex = 6;
            this.complete.UseVisualStyleBackColor = true;
            this.complete.Click += new System.EventHandler(this.complete_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(393, 208);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // translation
            // 
            this.translation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.translation.Location = new System.Drawing.Point(89, 34);
            this.translation.Name = "translation";
            this.translation.Size = new System.Drawing.Size(298, 22);
            this.translation.TabIndex = 3;
            // 
            // addTranslation
            // 
            this.addTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addTranslation.Location = new System.Drawing.Point(393, 34);
            this.addTranslation.Name = "addTranslation";
            this.addTranslation.Size = new System.Drawing.Size(75, 23);
            this.addTranslation.TabIndex = 4;
            this.addTranslation.Text = "A&dd";
            this.addTranslation.UseVisualStyleBackColor = true;
            this.addTranslation.Click += new System.EventHandler(this.addTranslation_Click);
            // 
            // listContextMenu
            // 
            this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            deleteMI});
            this.listContextMenu.Name = "listContextMenu";
            this.listContextMenu.Size = new System.Drawing.Size(101, 26);
            this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenu_Opening);
            // 
            // translations
            // 
            this.translations.AllowDrop = true;
            this.translations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.translations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.translationText});
            this.translations.ContextMenuStrip = this.listContextMenu;
            this.translations.FullRowSelect = true;
            this.translations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.translations.Location = new System.Drawing.Point(89, 60);
            this.translations.MultiSelect = false;
            this.translations.Name = "translations";
            this.translations.Size = new System.Drawing.Size(379, 142);
            this.translations.TabIndex = 5;
            this.translations.UseCompatibleStateImageBehavior = false;
            this.translations.View = System.Windows.Forms.View.Details;
            this.translations.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.translations_ItemDrag);
            this.translations.DragDrop += new System.Windows.Forms.DragEventHandler(this.translations_DragDrop);
            this.translations.DragEnter += new System.Windows.Forms.DragEventHandler(this.translations_DragEnter);
            // 
            // translationText
            // 
            this.translationText.Text = "";
            // 
            // EditDictionaryItem
            // 
            this.AcceptButton = this.complete;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(480, 243);
            this.Controls.Add(this.translation);
            this.Controls.Add(this.addTranslation);
            this.Controls.Add(this.translations);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.complete);
            this.Controls.Add(this.translationsLabel);
            this.Controls.Add(this.phrase);
            this.Controls.Add(this.phraseLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(496, 281);
            this.Name = "EditDictionaryItem";
            this.listContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label phraseLabel;
        private System.Windows.Forms.TextBox phrase;
        private System.Windows.Forms.Label translationsLabel;
        private System.Windows.Forms.Button complete;
        private System.Windows.Forms.Button cancel;
        private Controls.ListViewNF translations;
        private System.Windows.Forms.TextBox translation;
        private System.Windows.Forms.Button addTranslation;
        private System.Windows.Forms.ColumnHeader translationText;
        private System.Windows.Forms.ContextMenuStrip listContextMenu;
    }
}