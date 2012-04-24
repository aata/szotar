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
            this.leftTermEditor = new Szotar.WindowsForms.Controls.TermEditor();
            this.rightTermEditor = new Szotar.WindowsForms.Controls.TermEditor();
            this.close = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.table.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftListName
            // 
            resources.ApplyResources(this.leftListName, "leftListName");
            this.leftListName.Name = "leftListName";
            // 
            // deleteRight
            // 
            resources.ApplyResources(this.deleteRight, "deleteRight");
            this.deleteRight.Name = "deleteRight";
            this.deleteRight.UseVisualStyleBackColor = true;
            this.deleteRight.Click += new System.EventHandler(this.deleteRight_Click);
            // 
            // deleteLeft
            // 
            resources.ApplyResources(this.deleteLeft, "deleteLeft");
            this.deleteLeft.Name = "deleteLeft";
            this.deleteLeft.UseVisualStyleBackColor = true;
            this.deleteLeft.Click += new System.EventHandler(this.deleteLeft_Click);
            // 
            // table
            // 
            resources.ApplyResources(this.table, "table");
            this.table.Controls.Add(this.rightListName, 0, 0);
            this.table.Controls.Add(this.leftTermEditor, 0, 1);
            this.table.Controls.Add(this.rightTermEditor, 1, 1);
            this.table.Controls.Add(this.deleteLeft, 0, 2);
            this.table.Controls.Add(this.deleteRight, 1, 2);
            this.table.Controls.Add(this.leftListName, 0, 0);
            this.table.Name = "table";
            // 
            // rightListName
            // 
            resources.ApplyResources(this.rightListName, "rightListName");
            this.rightListName.Name = "rightListName";
            // 
            // leftTermEditor
            // 
            resources.ApplyResources(this.leftTermEditor, "leftTermEditor");
            this.leftTermEditor.Item = null;
            this.leftTermEditor.Name = "leftTermEditor";
            // 
            // rightTermEditor
            // 
            resources.ApplyResources(this.rightTermEditor, "rightTermEditor");
            this.rightTermEditor.Item = null;
            this.rightTermEditor.Name = "rightTermEditor";
            // 
            // close
            // 
            resources.ApplyResources(this.close, "close");
            this.close.Name = "close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // next
            // 
            resources.ApplyResources(this.next, "next");
            this.next.Name = "next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // FindDuplicates
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.next);
            this.Controls.Add(this.close);
            this.Controls.Add(this.table);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FindDuplicates";
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