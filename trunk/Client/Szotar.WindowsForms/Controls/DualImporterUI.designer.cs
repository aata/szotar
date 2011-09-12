namespace Szotar.WindowsForms.Controls {
	partial class DualImporterUI {
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
            System.Windows.Forms.Label label1;
            this.methodSelect = new System.Windows.Forms.Panel();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.helpText3 = new System.Windows.Forms.Label();
            this.importTypeGenerate = new System.Windows.Forms.RadioButton();
            this.importTypeBlank = new System.Windows.Forms.RadioButton();
            this.helpText2 = new System.Windows.Forms.Label();
            this.helpText1 = new System.Windows.Forms.Label();
            this.importTypeDual = new System.Windows.Forms.RadioButton();
            this.methodSelectNext = new System.Windows.Forms.Button();
            this.fileSelect = new System.Windows.Forms.Panel();
            this.importerType = new System.Windows.Forms.ComboBox();
            this.browse = new System.Windows.Forms.Button();
            this.fileName = new System.Windows.Forms.TextBox();
            this.fileSelectNext = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pleaseSelect = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.methodSelect.SuspendLayout();
            this.table.SuspendLayout();
            this.fileSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(5, 5);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(276, 15);
            label1.TabIndex = 0;
            label1.Text = "Please select a method of importing the dictionary.";
            // 
            // methodSelect
            // 
            this.methodSelect.Controls.Add(this.table);
            this.methodSelect.Controls.Add(this.methodSelectNext);
            this.methodSelect.Controls.Add(label1);
            this.methodSelect.Location = new System.Drawing.Point(12, 14);
            this.methodSelect.MinimumSize = new System.Drawing.Size(333, 136);
            this.methodSelect.Name = "methodSelect";
            this.methodSelect.Size = new System.Drawing.Size(333, 257);
            this.methodSelect.TabIndex = 0;
            // 
            // table
            // 
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table.ColumnCount = 1;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.Controls.Add(this.helpText3, 0, 5);
            this.table.Controls.Add(this.importTypeGenerate, 0, 0);
            this.table.Controls.Add(this.importTypeBlank, 0, 4);
            this.table.Controls.Add(this.helpText2, 0, 1);
            this.table.Controls.Add(this.helpText1, 0, 3);
            this.table.Controls.Add(this.importTypeDual, 0, 2);
            this.table.Location = new System.Drawing.Point(8, 23);
            this.table.Name = "table";
            this.table.RowCount = 6;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.Size = new System.Drawing.Size(322, 202);
            this.table.TabIndex = 2;
            // 
            // helpText3
            // 
            this.helpText3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText3.AutoSize = true;
            this.helpText3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.helpText3.Location = new System.Drawing.Point(3, 150);
            this.helpText3.Name = "helpText3";
            this.helpText3.Size = new System.Drawing.Size(316, 30);
            this.helpText3.TabIndex = 16;
            this.helpText3.Text = "Use this method if either of the other two methods don\'t work or produce nonsensi" +
                "cal results.";
            // 
            // importTypeGenerate
            // 
            this.importTypeGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importTypeGenerate.AutoSize = true;
            this.importTypeGenerate.Location = new System.Drawing.Point(3, 3);
            this.importTypeGenerate.Name = "importTypeGenerate";
            this.importTypeGenerate.Size = new System.Drawing.Size(316, 19);
            this.importTypeGenerate.TabIndex = 8;
            this.importTypeGenerate.TabStop = true;
            this.importTypeGenerate.Text = "Generate the second half the dictionary by magic";
            this.importTypeGenerate.UseVisualStyleBackColor = true;
            // 
            // importTypeBlank
            // 
            this.importTypeBlank.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importTypeBlank.AutoSize = true;
            this.importTypeBlank.Location = new System.Drawing.Point(3, 128);
            this.importTypeBlank.Name = "importTypeBlank";
            this.importTypeBlank.Size = new System.Drawing.Size(316, 19);
            this.importTypeBlank.TabIndex = 15;
            this.importTypeBlank.TabStop = true;
            this.importTypeBlank.Text = "Don\'t put anything in the second half of the dictionary";
            this.importTypeBlank.UseVisualStyleBackColor = true;
            // 
            // helpText2
            // 
            this.helpText2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText2.AutoSize = true;
            this.helpText2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.helpText2.Location = new System.Drawing.Point(3, 25);
            this.helpText2.Name = "helpText2";
            this.helpText2.Size = new System.Drawing.Size(316, 30);
            this.helpText2.TabIndex = 9;
            this.helpText2.Text = "Use this method if you have one half of a dictionary, and want to generate a reve" +
                "rse half from the first.";
            // 
            // helpText1
            // 
            this.helpText1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText1.AutoSize = true;
            this.helpText1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.helpText1.Location = new System.Drawing.Point(3, 80);
            this.helpText1.Name = "helpText1";
            this.helpText1.Size = new System.Drawing.Size(316, 45);
            this.helpText1.TabIndex = 13;
            this.helpText1.Text = "Use this method if you have two dictionaries where one translates in the opposite" +
                " direction to the other, and you want to combine them.";
            // 
            // importTypeDual
            // 
            this.importTypeDual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importTypeDual.AutoSize = true;
            this.importTypeDual.Location = new System.Drawing.Point(3, 58);
            this.importTypeDual.Name = "importTypeDual";
            this.importTypeDual.Size = new System.Drawing.Size(316, 19);
            this.importTypeDual.TabIndex = 12;
            this.importTypeDual.TabStop = true;
            this.importTypeDual.Text = "Import both halves of the dictionary separately";
            this.importTypeDual.UseVisualStyleBackColor = true;
            // 
            // methodSelectNext
            // 
            this.methodSelectNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.methodSelectNext.Enabled = false;
            this.methodSelectNext.Location = new System.Drawing.Point(255, 231);
            this.methodSelectNext.Name = "methodSelectNext";
            this.methodSelectNext.Size = new System.Drawing.Size(75, 23);
            this.methodSelectNext.TabIndex = 4;
            this.methodSelectNext.Text = "&Next";
            this.methodSelectNext.UseVisualStyleBackColor = true;
            // 
            // fileSelect
            // 
            this.fileSelect.Controls.Add(this.importerType);
            this.fileSelect.Controls.Add(this.browse);
            this.fileSelect.Controls.Add(this.fileName);
            this.fileSelect.Controls.Add(this.fileSelectNext);
            this.fileSelect.Controls.Add(this.label2);
            this.fileSelect.Controls.Add(this.pleaseSelect);
            this.fileSelect.Location = new System.Drawing.Point(12, 277);
            this.fileSelect.MinimumSize = new System.Drawing.Size(333, 136);
            this.fileSelect.Name = "fileSelect";
            this.fileSelect.Size = new System.Drawing.Size(333, 136);
            this.fileSelect.TabIndex = 0;
            // 
            // importerType
            // 
            this.importerType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.importerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.importerType.FormattingEnabled = true;
            this.importerType.Location = new System.Drawing.Point(8, 68);
            this.importerType.Name = "importerType";
            this.importerType.Size = new System.Drawing.Size(284, 23);
            this.importerType.TabIndex = 3;
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(298, 24);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(32, 23);
            this.browse.TabIndex = 2;
            this.browse.Text = "...";
            this.browse.UseVisualStyleBackColor = true;
            // 
            // fileName
            // 
            this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileName.Location = new System.Drawing.Point(8, 24);
            this.fileName.Name = "fileName";
            this.fileName.Size = new System.Drawing.Size(284, 23);
            this.fileName.TabIndex = 1;
            // 
            // fileSelectNext
            // 
            this.fileSelectNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelectNext.Enabled = false;
            this.fileSelectNext.Location = new System.Drawing.Point(255, 110);
            this.fileSelectNext.Name = "fileSelectNext";
            this.fileSelectNext.Size = new System.Drawing.Size(75, 23);
            this.fileSelectNext.TabIndex = 4;
            this.fileSelectNext.Text = "&Next";
            this.fileSelectNext.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Choose an importer to use:";
            // 
            // pleaseSelect
            // 
            this.pleaseSelect.AutoSize = true;
            this.pleaseSelect.Location = new System.Drawing.Point(5, 5);
            this.pleaseSelect.Name = "pleaseSelect";
            this.pleaseSelect.Size = new System.Drawing.Size(289, 15);
            this.pleaseSelect.TabIndex = 0;
            this.pleaseSelect.Text = "Please select the file for the first half of the dictionary.";
            // 
            // DualImporterUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fileSelect);
            this.Controls.Add(this.methodSelect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DualImporterUI";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(685, 522);
            this.methodSelect.ResumeLayout(false);
            this.methodSelect.PerformLayout();
            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            this.fileSelect.ResumeLayout(false);
            this.fileSelect.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel methodSelect;
		private System.Windows.Forms.Button methodSelectNext;
		private System.Windows.Forms.Panel fileSelect;
		private System.Windows.Forms.Button fileSelectNext;
		private System.Windows.Forms.Button browse;
		private System.Windows.Forms.TextBox fileName;
		private System.Windows.Forms.ComboBox importerType;
		private System.Windows.Forms.Label pleaseSelect;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label helpText3;
		private System.Windows.Forms.RadioButton importTypeBlank;
		private System.Windows.Forms.Label helpText1;
		private System.Windows.Forms.RadioButton importTypeDual;
		private System.Windows.Forms.Label helpText2;
		private System.Windows.Forms.RadioButton importTypeGenerate;
		private System.Windows.Forms.TableLayoutPanel table;
	}
}
