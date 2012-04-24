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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInfo));
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
            resources.ApplyResources(label1, "label1");
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Name = "label1";
            // 
            // nameInfo
            // 
            resources.ApplyResources(this.nameInfo, "nameInfo");
            this.nameInfo.Controls.Add(this.nameInfoTable);
            this.nameInfo.Name = "nameInfo";
            this.nameInfo.TabStop = false;
            // 
            // nameInfoTable
            // 
            resources.ApplyResources(this.nameInfoTable, "nameInfoTable");
            this.nameInfoTable.Controls.Add(this.nicknameLabel, 0, 1);
            this.nameInfoTable.Controls.Add(this.nameLabel, 0, 0);
            this.nameInfoTable.Controls.Add(this.name, 1, 0);
            this.nameInfoTable.Controls.Add(this.nickname, 1, 1);
            this.nameInfoTable.Name = "nameInfoTable";
            // 
            // nicknameLabel
            // 
            resources.ApplyResources(this.nicknameLabel, "nicknameLabel");
            this.nicknameLabel.Name = "nicknameLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // name
            // 
            resources.ApplyResources(this.name, "name");
            this.name.Name = "name";
            // 
            // nickname
            // 
            resources.ApplyResources(this.nickname, "nickname");
            this.nickname.Name = "nickname";
            // 
            // UserInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(label1);
            this.Controls.Add(this.nameInfo);
            this.Name = "UserInfo";
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
