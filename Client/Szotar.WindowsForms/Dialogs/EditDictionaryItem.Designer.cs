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
			resources.ApplyResources(deleteMI, "deleteMI");
			deleteMI.Name = "deleteMI";
			deleteMI.Click += new System.EventHandler(this.deleteMI_Click);
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
			// translationsLabel
			// 
			resources.ApplyResources(this.translationsLabel, "translationsLabel");
			this.translationsLabel.Name = "translationsLabel";
			// 
			// complete
			// 
			resources.ApplyResources(this.complete, "complete");
			this.complete.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.complete.Name = "complete";
			this.complete.UseVisualStyleBackColor = true;
			this.complete.Click += new System.EventHandler(this.complete_Click);
			// 
			// cancel
			// 
			resources.ApplyResources(this.cancel, "cancel");
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Name = "cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// translation
			// 
			resources.ApplyResources(this.translation, "translation");
			this.translation.Name = "translation";
			this.translation.KeyUp += new System.Windows.Forms.KeyEventHandler(this.translation_KeyUp);
			// 
			// addTranslation
			// 
			resources.ApplyResources(this.addTranslation, "addTranslation");
			this.addTranslation.Name = "addTranslation";
			this.addTranslation.UseVisualStyleBackColor = true;
			this.addTranslation.Click += new System.EventHandler(this.addTranslation_Click);
			// 
			// listContextMenu
			// 
			resources.ApplyResources(this.listContextMenu, "listContextMenu");
			this.listContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			deleteMI});
			this.listContextMenu.Name = "listContextMenu";
			this.listContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listContextMenu_Opening);
			// 
			// translations
			// 
			resources.ApplyResources(this.translations, "translations");
			this.translations.AllowDrop = true;
			this.translations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.translationText});
			this.translations.ContextMenuStrip = this.listContextMenu;
			this.translations.FullRowSelect = true;
			this.translations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.translations.MultiSelect = false;
			this.translations.Name = "translations";
			this.translations.UseCompatibleStateImageBehavior = false;
			this.translations.View = System.Windows.Forms.View.Details;
			this.translations.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.translations_ItemDrag);
			this.translations.DragDrop += new System.Windows.Forms.DragEventHandler(this.translations_DragDrop);
			this.translations.DragEnter += new System.Windows.Forms.DragEventHandler(this.translations_DragEnter);
			// 
			// translationText
			// 
			resources.ApplyResources(this.translationText, "translationText");
			// 
			// EditDictionaryItem
			// 
			this.AcceptButton = this.complete;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.Controls.Add(this.translation);
			this.Controls.Add(this.addTranslation);
			this.Controls.Add(this.translations);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.complete);
			this.Controls.Add(this.translationsLabel);
			this.Controls.Add(this.phrase);
			this.Controls.Add(this.phraseLabel);
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