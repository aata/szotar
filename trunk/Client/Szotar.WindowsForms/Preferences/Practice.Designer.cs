namespace Szotar.WindowsForms.Preferences {
    partial class Practice {
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
            this.defaultNumberOfItems = new System.Windows.Forms.Label();
            this.itemCount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.itemCount)).BeginInit();
            this.SuspendLayout();
            // 
            // defaultNumberOfItems
            // 
            this.defaultNumberOfItems.AutoSize = true;
            this.defaultNumberOfItems.Location = new System.Drawing.Point(0, 0);
            this.defaultNumberOfItems.Name = "defaultNumberOfItems";
            this.defaultNumberOfItems.Size = new System.Drawing.Size(144, 13);
            this.defaultNumberOfItems.TabIndex = 0;
            this.defaultNumberOfItems.Text = "Default number of items in list";
            // 
            // itemCount
            // 
            this.itemCount.Location = new System.Drawing.Point(3, 16);
            this.itemCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.itemCount.Name = "itemCount";
            this.itemCount.Size = new System.Drawing.Size(56, 20);
            this.itemCount.TabIndex = 1;
            // 
            // Practice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.itemCount);
            this.Controls.Add(this.defaultNumberOfItems);
            this.Name = "Practice";
            ((System.ComponentModel.ISupportInitialize)(this.itemCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label defaultNumberOfItems;
        private System.Windows.Forms.NumericUpDown itemCount;
    }
}
