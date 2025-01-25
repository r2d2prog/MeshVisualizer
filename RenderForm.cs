﻿using CoreVisualizer;
using System;
using System.Windows.Forms;
using GlmSharp;

namespace MeshVisualizer
{
    public partial class RenderForm : Form
    {
        private vec3 mousePoint;
        private LightingControl directionalLight;
        
        public RenderForm()
        {
            InitializeComponent();
            directionalLight = new LightingControl();
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

        private void OnRedrawLightDirection(object sender, PaintEventArgs e)
        {
            directionalLight.DrawLightDirection(e.Graphics);
        }

        private void SetLightDirection(vec3 direction, int index = 0)
        {
            renderControl.RenderHandler.DirectionalLights[index].Direction = direction;
        }

        private void OnChangeLightDirection(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePoint = new vec3(e.Location.X, e.Location.Y, 1);
                if (directionalLight.CalculateLightDirection(mousePoint))
                {
                    SetLightDirection(directionalLight.LightDirection3D);
                    toolStripLabel1.Invalidate();
                    renderControl.DoRender();
                }
            }
        }

        private void OnChangeLightHemisphere(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                directionalLight.ChangeLightHemisphere();
                SetLightDirection(directionalLight.LightDirection3D);
                toolStripLabel1.Invalidate();
                renderControl.DoRender();
            }
        }
    }
}
