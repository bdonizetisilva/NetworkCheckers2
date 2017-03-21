namespace NetworkCheckers
{
    partial class InitialForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialForm));
            this.networkLabel = new System.Windows.Forms.Label();
            this.newGameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // networkLabel
            // 
            resources.ApplyResources(this.networkLabel, "networkLabel");
            this.networkLabel.BackColor = System.Drawing.Color.Transparent;
            this.networkLabel.ForeColor = System.Drawing.Color.Gray;
            this.networkLabel.Name = "networkLabel";
            // 
            // newGameButton
            // 
            this.newGameButton.BackColor = System.Drawing.Color.Gray;
            this.newGameButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.newGameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Green;
            this.newGameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.newGameButton, "newGameButton");
            this.newGameButton.ForeColor = System.Drawing.Color.White;
            this.newGameButton.Name = "newGameButton";
            this.newGameButton.UseVisualStyleBackColor = false;
            this.newGameButton.Click += new System.EventHandler(this.newGameButton_Click);
            // 
            // InitialForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.newGameButton);
            this.Controls.Add(this.networkLabel);
            this.Name = "InitialForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label networkLabel;
        private System.Windows.Forms.Button newGameButton;
    }
}