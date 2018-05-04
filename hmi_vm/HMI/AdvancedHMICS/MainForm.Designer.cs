namespace AdvancedHMICS
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.QuickStartLabel = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.LicenseNote = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBox1
            // 
            this.PictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PictureBox1.BackgroundImage")));
            this.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PictureBox1.Location = new System.Drawing.Point(450, 11);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(322, 47);
            this.PictureBox1.TabIndex = 45;
            this.PictureBox1.TabStop = false;
            // 
            // QuickStartLabel
            // 
            this.QuickStartLabel.AutoSize = true;
            this.QuickStartLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.QuickStartLabel.ForeColor = System.Drawing.Color.White;
            this.QuickStartLabel.Location = new System.Drawing.Point(12, 8);
            this.QuickStartLabel.Name = "QuickStartLabel";
            this.QuickStartLabel.Size = new System.Drawing.Size(273, 104);
            this.QuickStartLabel.TabIndex = 44;
            this.QuickStartLabel.Text = resources.GetString("QuickStartLabel.Text");
            this.QuickStartLabel.Visible = false;
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Label1.Location = new System.Drawing.Point(12, 520);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(194, 32);
            this.Label1.TabIndex = 43;
            this.Label1.Text = "For Development Source Code Visit\r\nhttp://www.advancedhmi.com";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LicenseNote
            // 
            this.LicenseNote.AutoSize = true;
            this.LicenseNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LicenseNote.ForeColor = System.Drawing.Color.White;
            this.LicenseNote.Location = new System.Drawing.Point(19, 264);
            this.LicenseNote.Name = "LicenseNote";
            this.LicenseNote.Size = new System.Drawing.Size(746, 32);
            this.LicenseNote.TabIndex = 54;
            this.LicenseNote.Text = "By Using This Software You Agree to the UsageAndLicense.txt\r\nAdvancedHMI is licen" +
    "sed under a GPL model which means you must pass on the full source to the end us" +
    "er.";
            this.LicenseNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LicenseNote.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.LicenseNote);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.QuickStartLabel);
            this.Controls.Add(this.Label1);
            this.Name = "MainForm";
            this.Text = "AdvancedHMI V3.99x";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Label QuickStartLabel;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label LicenseNote;
    }
}

