namespace CoreVisualizer
{
    partial class RenderControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl = new OpenGL.GlControl();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.glControl.ColorBits = ((uint)(24u));
            this.glControl.DepthBits = ((uint)(16u));
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.MultisampleBits = ((uint)(4u));
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(600, 400);
            this.glControl.StencilBits = ((uint)(8u));
            this.glControl.TabIndex = 0;
            this.glControl.Load += new System.EventHandler(this.OnInit);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // RenderControl
            // 
            this.Controls.Add(this.glControl);
            this.Name = "RenderControl";
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGL.GlControl glControl;
    }
}
