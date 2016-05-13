using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    class MyToolStrip : ToolStrip
    {
        //By Doc Brown @ https://stackoverflow.com/questions/472301/toolstrip-sometimes-not-responding-to-a-mouse-click
        //Originally from https://blogs.msdn.microsoft.com/rickbrew/2006/01/09/how-to-enable-click-through-for-net-2-0-toolstrip-and-menustrip/
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0021 &&
                m.Result == (IntPtr)2)
            {
                m.Result = (IntPtr)1;
            }
        }
    }
}
