using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft_Launcher_V2
{
    public partial class rundown : Form
    {
        public void ChangeProgress(int p)
        {
            try
            {
                guna2ProgressBar1.Value = p;
            }
            catch
            {

            }
        }

        public rundown()
        {
            InitializeComponent();
        }
    }
}
