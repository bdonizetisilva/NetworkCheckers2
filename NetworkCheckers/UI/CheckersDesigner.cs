using System.Windows.Forms.Design;

namespace NetworkCheckers.UI
{
    class CheckersDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.Visible | SelectionRules.Moveable;
            }
        }
    }
}
