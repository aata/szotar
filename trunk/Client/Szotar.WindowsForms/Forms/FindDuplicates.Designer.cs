namespace Szotar.WindowsForms.Forms {
    partial class FindDuplicates {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindDuplicates));
            this.leftListName = new System.Windows.Forms.Label();
            this.deleteRight = new System.Windows.Forms.Button();
            this.deleteLeft = new System.Windows.Forms.Button();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.rightListName = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.leftTermEditor = new Szotar.WindowsForms.Controls.TermEditor();
            this.rightTermEditor = new Szotar.WindowsForms.Controls.TermEditor();
            this.table.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftListName
            // 
            this.leftListName.AutoSize = true;
            this.leftListName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftListName.Location = new System.Drawing.Point(3, 0);
            this.leftListName.Name = "leftListName";
            this.leftListName.Size = new System.Drawing.Size(261, 13);
            this.leftListName.TabIndex = 3;
            this.leftListName.Text = "label1";
            this.leftListName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // deleteRight
            // 
            this.deleteRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteRight.Location = new System.Drawing.Point(456, 71);
            this.deleteRight.Name = "deleteRight";
            this.deleteRight.Size = new System.Drawing.Size(75, 23);
            this.deleteRight.TabIndex = 3;
            this.deleteRight.Text = "D&elete";
            this.deleteRight.UseVisualStyleBackColor = true;
            this.deleteRight.Click += new System.EventHandler(this.deleteRight_Click);
            // 
            // deleteLeft
            // 
            this.deleteLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteLeft.Location = new System.Drawing.Point(189, 71);
            this.deleteLeft.Name = "deleteLeft";
            this.deleteLeft.Size = new System.Drawing.Size(75, 23);
            this.deleteLeft.TabIndex = 1;
            this.deleteLeft.Text = "&Delete";
            this.deleteLeft.UseVisualStyleBackColor = true;
            this.deleteLeft.Click += new System.EventHandler(this.deleteLeft_Click);
            // 
            // table
            // 
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table.AutoSize = true;
            this.table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.table.ColumnCount = 2;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.Controls.Add(this.rightListName, 0, 0);
            this.table.Controls.Add(this.leftTermEditor, 0, 1);
            this.table.Controls.Add(this.rightTermEditor, 1, 1);
            this.table.Controls.Add(this.deleteLeft, 0, 2);
            this.table.Controls.Add(this.deleteRight, 1, 2);
            this.table.Controls.Add(this.leftListName, 0, 0);
            this.table.Location = new System.Drawing.Point(13, 13);
            this.table.Name = "table";
            this.table.RowCount = 3;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.table.Size = new System.Drawing.Size(534, 97);
            this.table.TabIndex = 0;
            // 
            // rightListName
            // 
            this.rightListName.AutoSize = true;
            this.rightListName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightListName.Location = new System.Drawing.Point(270, 0);
            this.rightListName.Name = "rightListName";
            this.rightListName.Size = new System.Drawing.Size(261, 13);
            this.rightListName.TabIndex = 4;
            this.rightListName.Text = "label1";
            this.rightListName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.Location = new System.Drawing.Point(475, 117);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 5;
            this.close.Text = "&Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // next
            // 
            this.next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.next.Location = new System.Drawing.Point(394, 117);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 23);
            this.next.TabIndex = 4;
            this.next.Text = "&Next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // leftTermEditor
            // 
            this.leftTermEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.leftTermEditor.Item = null;
            this.leftTermEditor.Location = new System.Drawing.Point(3, 16);
            this.leftTermEditor.Name = "leftTermEditor";
            this.leftTermEditor.Size = new System.Drawing.Size(261, 49);
            this.leftTermEditor.TabIndex = 0;
            // 
            // rightTermEditor
            // 
            this.rightTermEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rightTermEditor.Item = null;
            this.rightTermEditor.Location = new System.Drawing.Point(270, 16);
            this.rightTermEditor.Name = "rightTermEditor";
            this.rightTermEditor.Size = new System.Drawing.Size(261, 49);
            this.rightTermEditor.TabIndex = 2;
            // 
            // FindDuplicates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(562, 152);
            this.Controls.Add(this.next);
            this.Controls.Add(this.close);
            this.Controls.Add(this.table);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FindDuplicates";
            this.Text = "Find Duplicate Terms";
            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label leftListName;
        private System.Windows.Forms.Button deleteRight;
        private System.Windows.Forms.Button deleteLeft;
        private Controls.TermEditor rightTermEditor;
        private Controls.TermEditor leftTermEditor;
        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.Label rightListName;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button next;
    }
}