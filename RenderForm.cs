using CoreVisualizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshVisualizer
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
        }

        private void OnAlignCamera(object sender, EventArgs e)
        {
            var control = sender as ToolStripMenuItem;
            var plane = (ViewPlane)Enum.Parse(typeof(ViewPlane), control.Text);
            renderControl.AlignCamera(plane);
        }
    }
}
