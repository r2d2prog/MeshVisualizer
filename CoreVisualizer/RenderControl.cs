using SharpGL;
using SharpGL.SceneGraph.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreVisualizer
{
    public partial class RenderControl : UserControl
    {
        private ICollection<Shader> Shaders { get; set; }
        private ICollection<ShaderProgram> Programs { get; set; }
        public RenderControl()
        {
            InitializeComponent();
        }

        public void DrawGrid(object sender, RenderEventArgs args)
        {

        }

        private void OnInit(object sender, EventArgs e)
        {

        }
    }
}
