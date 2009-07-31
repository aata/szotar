namespace Szotar.WindowsForms.Controls {
    partial class LogViewer {
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
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.ColumnHeader columnHeader2;
            System.Windows.Forms.ColumnHeader columnHeader3;
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.metrics = new System.Windows.Forms.CheckBox();
            this.debug = new System.Windows.Forms.CheckBox();
            this.warning = new System.Windows.Forms.CheckBox();
            this.error = new System.Windows.Forms.CheckBox();
            this.list = new Szotar.WindowsForms.Controls.ListViewNF();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.list, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(515, 351);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.metrics);
            this.flowLayoutPanel1.Controls.Add(this.debug);
            this.flowLayoutPanel1.Controls.Add(this.warning);
            this.flowLayoutPanel1.Controls.Add(this.error);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(509, 23);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // metrics
            // 
            this.metrics.AutoSize = true;
            this.metrics.Location = new System.Drawing.Point(3, 3);
            this.metrics.Name = "metrics";
            this.metrics.Size = new System.Drawing.Size(60, 17);
            this.metrics.TabIndex = 2;
            this.metrics.Text = "Metrics";
            this.metrics.UseVisualStyleBackColor = true;
            // 
            // debug
            // 
            this.debug.AutoSize = true;
            this.debug.Location = new System.Drawing.Point(69, 3);
            this.debug.Name = "debug";
            this.debug.Size = new System.Drawing.Size(58, 17);
            this.debug.TabIndex = 3;
            this.debug.Text = "Debug";
            this.debug.UseVisualStyleBackColor = true;
            // 
            // warning
            // 
            this.warning.AutoSize = true;
            this.warning.Location = new System.Drawing.Point(133, 3);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(66, 17);
            this.warning.TabIndex = 4;
            this.warning.Text = "Warning";
            this.warning.UseVisualStyleBackColor = true;
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.Location = new System.Drawing.Point(205, 3);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(48, 17);
            this.error.TabIndex = 5;
            this.error.Text = "Error";
            this.error.UseVisualStyleBackColor = true;
            // 
            // list
            // 
            this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Location = new System.Drawing.Point(3, 32);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(509, 316);
            this.list.TabIndex = 1;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Type";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Time";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Message";
            columnHeader3.Width = 361;
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LogViewer";
            this.Size = new System.Drawing.Size(515, 351);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox metrics;
        private System.Windows.Forms.CheckBox debug;
        private System.Windows.Forms.CheckBox warning;
        private System.Windows.Forms.CheckBox error;
        private ListViewNF list;
    }
}
