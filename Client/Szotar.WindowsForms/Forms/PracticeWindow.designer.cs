namespace Szotar.WindowsForms.Forms {
	partial class PracticeWindow {
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
            System.Windows.Forms.ToolStripMenuItem file;
            System.Windows.Forms.ToolStripMenuItem showStartPage;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripMenuItem exit;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PracticeWindow));
            this.flashcardsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.learnMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.panel = new System.Windows.Forms.Panel();
            this.practiceWeakPoints = new System.Windows.Forms.ToolStripMenuItem();
            file = new System.Windows.Forms.ToolStripMenuItem();
            showStartPage = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            exit = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // file
            // 
            file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            showStartPage,
            this.practiceWeakPoints,
            toolStripSeparator2,
            this.flashcardsMI,
            this.learnMI,
            this.toolStripSeparator1,
            exit});
            file.Name = "file";
            file.Size = new System.Drawing.Size(37, 20);
            file.Text = "&File";
            // 
            // showStartPage
            // 
            showStartPage.Name = "showStartPage";
            showStartPage.Size = new System.Drawing.Size(225, 22);
            showStartPage.Text = "&Show Start Page";
            showStartPage.Click += new System.EventHandler(this.showStartPage_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(222, 6);
            // 
            // flashcardsMI
            // 
            this.flashcardsMI.Name = "flashcardsMI";
            this.flashcardsMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.flashcardsMI.Size = new System.Drawing.Size(225, 22);
            this.flashcardsMI.Text = "Flashcards";
            this.flashcardsMI.Click += new System.EventHandler(this.flashcardsMI_Click);
            // 
            // learnMI
            // 
            this.learnMI.Name = "learnMI";
            this.learnMI.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.learnMI.Size = new System.Drawing.Size(225, 22);
            this.learnMI.Text = "Learn";
            this.learnMI.Click += new System.EventHandler(this.learnMI_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // exit
            // 
            exit.Name = "exit";
            exit.Size = new System.Drawing.Size(225, 22);
            exit.Text = "E&xit";
            exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            file});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(653, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "Main Menu";
            // 
            // panel
            // 
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel.Location = new System.Drawing.Point(0, 24);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(653, 424);
            this.panel.TabIndex = 1;
            // 
            // practiceWeakPoints
            // 
            this.practiceWeakPoints.Name = "practiceWeakPoints";
            this.practiceWeakPoints.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.practiceWeakPoints.Size = new System.Drawing.Size(225, 22);
            this.practiceWeakPoints.Text = "&Practice Weak Points";
            this.practiceWeakPoints.Click += new System.EventHandler(this.practiceWeakPoints_Click);
            // 
            // PracticeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 448);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(322, 212);
            this.Name = "PracticeWindow";
            this.Text = "Practice";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem flashcardsMI;
        private System.Windows.Forms.ToolStripMenuItem learnMI;
        private System.Windows.Forms.ToolStripMenuItem practiceWeakPoints;
	}
}