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
    public partial class InitialForm : Form
    {
        public InitialForm()
        {
            InitializeComponent();
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            using (PlayerInfoForm pForm = new PlayerInfoForm())
            {
                pForm.ShowDialog(this);
            }

            using (FindingGameForm fForm = new FindingGameForm())
            {
                fForm.ShowDialog(this);
            }
        }
    }
}
