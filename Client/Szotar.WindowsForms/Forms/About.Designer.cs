namespace Szotar.WindowsForms.Forms {
	partial class About {
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
            System.Windows.Forms.TableLayoutPanel table;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.webLink = new System.Windows.Forms.LinkLabel();
            this.version = new System.Windows.Forms.Label();
            this.productName = new System.Windows.Forms.Label();
            table = new System.Windows.Forms.TableLayoutPanel();
            table.SuspendLayout();
            this.SuspendLayout();
            // 
            // table
            // 
            table.ColumnCount = 1;
            table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            table.Controls.Add(this.webLink, 0, 2);
            table.Controls.Add(this.version, 0, 1);
            table.Controls.Add(this.productName, 0, 0);
            table.Dock = System.Windows.Forms.DockStyle.Fill;
            table.Location = new System.Drawing.Point(0, 0);
            table.Name = "table";
            table.Padding = new System.Windows.Forms.Padding(3);
            table.RowCount = 3;
            table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            table.Size = new System.Drawing.Size(321, 171);
            table.TabIndex = 3;
            // 
            // webLink
            // 
            this.webLink.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webLink.AutoSize = true;
            this.webLink.Location = new System.Drawing.Point(6, 142);
            this.webLink.Name = "webLink";
            this.webLink.Size = new System.Drawing.Size(309, 26);
            this.webLink.TabIndex = 5;
            this.webLink.TabStop = true;
            this.webLink.Text = "{0} on the web";
            this.webLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // version
            // 
            this.version.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.version.AutoSize = true;
            this.version.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.version.Location = new System.Drawing.Point(3, 93);
            this.version.Margin = new System.Windows.Forms.Padding(0);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(315, 49);
            this.version.TabIndex = 4;
            this.version.Text = "Version {0}";
            this.version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // productName
            // 
            this.productName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.productName.AutoSize = true;
            this.productName.Font = new System.Drawing.Font("Cambria", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productName.Location = new System.Drawing.Point(3, 3);
            this.productName.Margin = new System.Windows.Forms.Padding(0);
            this.productName.Name = "productName";
            this.productName.Size = new System.Drawing.Size(315, 90);
            this.productName.TabIndex = 1;
            this.productName.Text = "{0}";
            this.productName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 171);
            this.Controls.Add(table);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(243, 140);
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About {0}";
            table.ResumeLayout(false);
            table.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label version;
		private System.Windows.Forms.Label productName;
		private System.Windows.Forms.LinkLabel webLink;
	}
}