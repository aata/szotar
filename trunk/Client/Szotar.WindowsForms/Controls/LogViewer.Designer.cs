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
			System.Windows.Forms.TableLayoutPanel table;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewer));
			System.Windows.Forms.FlowLayoutPanel checkboxes;
			System.Windows.Forms.ColumnHeader columnHeader1;
			System.Windows.Forms.ColumnHeader columnHeader2;
			System.Windows.Forms.ColumnHeader columnHeader3;
			this.metrics = new System.Windows.Forms.CheckBox();
			this.debug = new System.Windows.Forms.CheckBox();
			this.warning = new System.Windows.Forms.CheckBox();
			this.error = new System.Windows.Forms.CheckBox();
			this.list = new Szotar.WindowsForms.Controls.ListViewNF();
			table = new System.Windows.Forms.TableLayoutPanel();
			checkboxes = new System.Windows.Forms.FlowLayoutPanel();
			columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			table.SuspendLayout();
			checkboxes.SuspendLayout();
			this.SuspendLayout();
			// 
			// table
			// 
			resources.ApplyResources(table, "table");
			table.Controls.Add(checkboxes, 0, 0);
			table.Controls.Add(this.list, 0, 1);
			table.Name = "table";
			// 
			// checkboxes
			// 
			resources.ApplyResources(checkboxes, "checkboxes");
			checkboxes.Controls.Add(this.metrics);
			checkboxes.Controls.Add(this.debug);
			checkboxes.Controls.Add(this.warning);
			checkboxes.Controls.Add(this.error);
			checkboxes.Name = "checkboxes";
			// 
			// metrics
			// 
			resources.ApplyResources(this.metrics, "metrics");
			this.metrics.Name = "metrics";
			this.metrics.UseVisualStyleBackColor = true;
			// 
			// debug
			// 
			resources.ApplyResources(this.debug, "debug");
			this.debug.Name = "debug";
			this.debug.UseVisualStyleBackColor = true;
			// 
			// warning
			// 
			resources.ApplyResources(this.warning, "warning");
			this.warning.Name = "warning";
			this.warning.UseVisualStyleBackColor = true;
			// 
			// error
			// 
			resources.ApplyResources(this.error, "error");
			this.error.Name = "error";
			this.error.UseVisualStyleBackColor = true;
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			columnHeader1,
			columnHeader2,
			columnHeader3});
			this.list.Name = "list";
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			resources.ApplyResources(columnHeader1, "columnHeader1");
			// 
			// columnHeader2
			// 
			resources.ApplyResources(columnHeader2, "columnHeader2");
			// 
			// columnHeader3
			// 
			resources.ApplyResources(columnHeader3, "columnHeader3");
			// 
			// LogViewer
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(table);
			this.Name = "LogViewer";
			table.ResumeLayout(false);
			table.PerformLayout();
			checkboxes.ResumeLayout(false);
			checkboxes.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox metrics;
		private System.Windows.Forms.CheckBox debug;
		private System.Windows.Forms.CheckBox warning;
		private System.Windows.Forms.CheckBox error;
		private ListViewNF list;
	}
}
