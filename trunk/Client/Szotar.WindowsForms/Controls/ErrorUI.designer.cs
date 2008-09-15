namespace Szotar.WindowsForms.Controls {
	partial class ErrorUI {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				errorBitmap.Dispose();

				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.icon = new System.Windows.Forms.PictureBox();
			this.stackTrace = new System.Windows.Forms.TextBox();
			this.text = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
			this.SuspendLayout();
			// 
			// icon
			// 
			this.icon.Location = new System.Drawing.Point(3, 3);
			this.icon.Name = "icon";
			this.icon.Size = new System.Drawing.Size(63, 68);
			this.icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.icon.TabIndex = 0;
			this.icon.TabStop = false;
			// 
			// stackTrace
			// 
			this.stackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.stackTrace.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.stackTrace.Location = new System.Drawing.Point(75, 74);
			this.stackTrace.Multiline = true;
			this.stackTrace.Name = "stackTrace";
			this.stackTrace.ReadOnly = true;
			this.stackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.stackTrace.Size = new System.Drawing.Size(320, 187);
			this.stackTrace.TabIndex = 1;
			// 
			// text
			// 
			this.text.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.text.Location = new System.Drawing.Point(72, 4);
			this.text.Name = "text";
			this.text.Size = new System.Drawing.Size(327, 67);
			this.text.TabIndex = 2;
			// 
			// ErrorUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.text);
			this.Controls.Add(this.stackTrace);
			this.Controls.Add(this.icon);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ErrorUI";
			this.Size = new System.Drawing.Size(398, 264);
			((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox icon;
		private System.Windows.Forms.TextBox stackTrace;
		private System.Windows.Forms.Label text;
	}
}
