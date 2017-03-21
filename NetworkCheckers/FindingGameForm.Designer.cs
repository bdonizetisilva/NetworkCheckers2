namespace NetworkCheckers
{
    partial class FindingGameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindingGameForm));
            this.findingLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.findingBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // findingLabel
            // 
            this.findingLabel.AutoSize = true;
            this.findingLabel.Location = new System.Drawing.Point(38, 85);
            this.findingLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.findingLabel.Name = "findingLabel";
            this.findingLabel.Size = new System.Drawing.Size(299, 26);
            this.findingLabel.TabIndex = 0;
            this.findingLabel.Text = "Procurando um oponente digno...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(135, 111);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Aguarde...";
            // 
            // findingBar
            // 
            this.findingBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.findingBar.Location = new System.Drawing.Point(37, 249);
            this.findingBar.Name = "findingBar";
            this.findingBar.Size = new System.Drawing.Size(294, 23);
            this.findingBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.findingBar.TabIndex = 2;
            // 
            // FindingGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(369, 372);
            this.Controls.Add(this.findingBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.findingLabel);
            this.Font = new System.Drawing.Font("Calibri Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FindingGameForm";
            this.Text = "FindingGameForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label findingLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar findingBar;
    }
}