namespace Szotar.WindowsForms.Preferences {
	partial class UserInfo {
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
			this.nameInfo = new System.Windows.Forms.GroupBox();
			this.nameInfoTable = new System.Windows.Forms.TableLayoutPanel();
			this.nicknameLabel = new System.Windows.Forms.Label();
			this.nameLabel = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.TextBox();
			this.nickname = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			this.nameInfo.SuspendLayout();
			this.nameInfoTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			label1.BackColor = System.Drawing.Color.Transparent;
			label1.Location = new System.Drawing.Point(5, 79);
			label1.Margin = new System.Windows.Forms.Padding(5);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(408, 185);
			label1.TabIndex = 1;
			label1.Text = "This information will be used in any lists you create or publish. It is not neces" +
				"sary to fill these in.";
			// 
			// nameInfo
			// 
			this.nameInfo.AutoSize = true;
			this.nameInfo.Controls.Add(this.nameInfoTable);
			this.nameInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.nameInfo.Location = new System.Drawing.Point(0, 0);
			this.nameInfo.Name = "nameInfo";
			this.nameInfo.Size = new System.Drawing.Size(419, 71);
			this.nameInfo.TabIndex = 0;
			this.nameInfo.TabStop = false;
			this.nameInfo.Text = "Name";
			// 
			// nameInfoTable
			// 
			this.nameInfoTable.AutoSize = true;
			this.nameInfoTable.ColumnCount = 2;
			this.nameInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.nameInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.nameInfoTable.Controls.Add(this.nicknameLabel, 0, 1);
			this.nameInfoTable.Controls.Add(this.nameLabel, 0, 0);
			this.nameInfoTable.Controls.Add(this.name, 1, 0);
			this.nameInfoTable.Controls.Add(this.nickname, 1, 1);
			this.nameInfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nameInfoTable.Location = new System.Drawing.Point(3, 16);
			this.nameInfoTable.Name = "nameInfoTable";
			this.nameInfoTable.RowCount = 2;
			this.nameInfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.nameInfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.nameInfoTable.Size = new System.Drawing.Size(413, 52);
			this.nameInfoTable.TabIndex = 0;
			// 
			// nicknameLabel
			// 
			this.nicknameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nicknameLabel.AutoSize = true;
			this.nicknameLabel.Location = new System.Drawing.Point(3, 26);
			this.nicknameLabel.Name = "nicknameLabel";
			this.nicknameLabel.Size = new System.Drawing.Size(55, 26);
			this.nicknameLabel.TabIndex = 4;
			this.nicknameLabel.Text = "Nickname";
			this.nicknameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nameLabel
			// 
			this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nameLabel.AutoSize = true;
			this.nameLabel.Location = new System.Drawing.Point(3, 0);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(55, 26);
			this.nameLabel.TabIndex = 0;
			this.nameLabel.Text = "Name";
			this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// name
			// 
			this.name.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.name.Location = new System.Drawing.Point(64, 3);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(346, 20);
			this.name.TabIndex = 2;
			// 
			// nickname
			// 
			this.nickname.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nickname.Location = new System.Drawing.Point(64, 29);
			this.nickname.Name = "nickname";
			this.nickname.Size = new System.Drawing.Size(346, 20);
			this.nickname.TabIndex = 3;
			// 
			// UserInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(label1);
			this.Controls.Add(this.nameInfo);
			this.Name = "UserInfo";
			this.Size = new System.Drawing.Size(419, 269);
			this.nameInfo.ResumeLayout(false);
			this.nameInfo.PerformLayout();
			this.nameInfoTable.ResumeLayout(false);
			this.nameInfoTable.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox nameInfo;
		private System.Windows.Forms.TableLayoutPanel nameInfoTable;
		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.TextBox nickname;
		private System.Windows.Forms.Label nicknameLabel;
	}
}
