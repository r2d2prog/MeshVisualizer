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
            this.glControl = new SharpGL.OpenGLControl();
            ((System.ComponentModel.ISupportInitialize)(this.glControl)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.DrawFPS = false;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL3_3;
            this.glControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            this.glControl.RenderTrigger = SharpGL.RenderTrigger.Manual;
            this.glControl.Size = new System.Drawing.Size(150, 150);
            this.glControl.TabIndex = 0;
            this.glControl.OpenGLInitialized += new System.EventHandler(this.OnInit);
            // 
            // RenderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.glControl);
            this.Name = "RenderControl";
            ((System.ComponentModel.ISupportInitialize)(this.glControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl glControl;
    }
}
