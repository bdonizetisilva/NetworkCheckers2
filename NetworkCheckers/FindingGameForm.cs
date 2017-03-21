using NetworkCheckers.Network.Services;
using System.Threading;
using System.Windows.Forms;

namespace NetworkCheckers
{
    using Utils;

    public partial class FindingGameForm : Form
    {
        private Thread _FindThread;

        public FindingGameForm()
        {
            InitializeComponent();

            this.findingBar.MarqueeAnimationSpeed = 100;

            this._FindThread = new Thread(this.FindThreadWork);
            this._FindThread.Start();
        }

        public void FindThreadWork()
        {
            ServerFinder finder = new ServerFinder(60);

            finder.Found += Finder_Found;

            Thread.Sleep(5000);

            ServiceInfo checker = new ServiceInfo("Checker", 87);

            ServerInfo server = new ServerInfo {checker};
            
            ServerPublisher publisher = new ServerPublisher(server, 60);

            
        }

        private void Finder_Found(object sender, ServerEventArgs e)
        {

            this._FindThread.SafeAbort();
        }
    }
}
