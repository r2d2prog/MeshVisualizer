using CoreVisualizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

        private void OnLoadModel(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Kaydara (*.fbx)|*.fbx|Wavefront (*.obj)|*.obj|Glb (*.glb)|*.glb" +
                                     "|Stl (*.stl)|*.stl|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 5;
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                renderControl.LoadModel(openFileDialog1.FileName);
                renderControl.DoRender();
            }
        }

        private void OnRasterModeChange(object sender, EventArgs e)
        {
            var control = sender as ToolStripMenuItem;
            var checkState = control.Checked;
            control.Checked = !checkState;
            if(renderControl.ActiveModel != null)
            {
                var id = int.Parse(control.Tag.ToString());
                var mode = (RasterizationMode)id;
                renderControl.ActiveModel.SetRasterizationMode(mode, control.Checked);
                renderControl.DoRender();
            }
        }
    }
}
