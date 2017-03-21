using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkCheckers
{
    public partial class PlayerInfoForm : Form
    {
        public static string PlayerName;

        public PlayerInfoForm()
        {
            InitializeComponent();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            PlayerName = this.playerNameTextBox.Text;

            this.Close();
        }
    }
}
