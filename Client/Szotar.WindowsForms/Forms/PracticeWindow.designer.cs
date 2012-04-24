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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PracticeWindow));
            System.Windows.Forms.ToolStripMenuItem showStartPage;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            System.Windows.Forms.ToolStripMenuItem exit;
            this.practiceWeakPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.flashcardsMI = new System.Windows.Forms.ToolStripMenuItem();
            this.learnMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.panel = new System.Windows.Forms.Panel();
            file = new System.Windows.Forms.ToolStripMenuItem();
            showStartPage = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            exit = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // file
            // 
            resources.ApplyResources(file, "file");
            file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            showStartPage,
            this.practiceWeakPoints,
            toolStripSeparator2,
            this.flashcardsMI,
            this.learnMI,
            this.toolStripSeparator1,
            exit});
            file.Name = "file";
            // 
            // showStartPage
            // 
            resources.ApplyResources(showStartPage, "showStartPage");
            showStartPage.Name = "showStartPage";
            showStartPage.Click += new System.EventHandler(this.showStartPage_Click);
            // 
            // practiceWeakPoints
            // 
            resources.ApplyResources(this.practiceWeakPoints, "practiceWeakPoints");
            this.practiceWeakPoints.Name = "practiceWeakPoints";
            this.practiceWeakPoints.Click += new System.EventHandler(this.practiceWeakPoints_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // flashcardsMI
            // 
            resources.ApplyResources(this.flashcardsMI, "flashcardsMI");
            this.flashcardsMI.Name = "flashcardsMI";
            this.flashcardsMI.Click += new System.EventHandler(this.flashcardsMI_Click);
            // 
            // learnMI
            // 
            resources.ApplyResources(this.learnMI, "learnMI");
            this.learnMI.Name = "learnMI";
            this.learnMI.Click += new System.EventHandler(this.learnMI_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // exit
            // 
            resources.ApplyResources(exit, "exit");
            exit.Name = "exit";
            exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // mainMenu
            // 
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            file});
            this.mainMenu.Name = "mainMenu";
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // PracticeWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.mainMenu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "PracticeWindow";
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