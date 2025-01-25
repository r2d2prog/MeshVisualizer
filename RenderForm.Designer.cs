namespace MeshVisualizer
{
    partial class RenderForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.renderControl = new CoreVisualizer.RenderControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.перспективнаяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ортографическаяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.xYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.затененнаяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сеткаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.точкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.xYToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.yXToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.zXToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.xZToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.yZToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.zYToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStripDropDownButton4 = new System.Windows.Forms.ToolStripDropDownButton();
            this.сглаживаниеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьToolStripMenuItem1,
            this.toolStripSeparator1,
            this.выходToolStripMenuItem});
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.открытьToolStripMenuItem.Text = "Файл";
            // 
            // открытьToolStripMenuItem1
            // 
            this.открытьToolStripMenuItem1.Name = "открытьToolStripMenuItem1";
            this.открытьToolStripMenuItem1.Size = new System.Drawing.Size(121, 22);
            this.открытьToolStripMenuItem1.Text = "Открыть";
            this.открытьToolStripMenuItem1.Click += new System.EventHandler(this.OnLoadModel);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(118, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem1});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.настройкиToolStripMenuItem.Text = "Инструменты";
            // 
            // настройкиToolStripMenuItem1
            // 
            this.настройкиToolStripMenuItem1.Name = "настройкиToolStripMenuItem1";
            this.настройкиToolStripMenuItem1.Size = new System.Drawing.Size(134, 22);
            this.настройкиToolStripMenuItem1.Text = "Настройки";
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.renderControl);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(800, 375);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(800, 426);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // renderControl
            // 
            this.renderControl.ActiveModel = null;
            this.renderControl.BackColor = System.Drawing.SystemColors.ControlText;
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Location = new System.Drawing.Point(0, 0);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(800, 375);
            this.renderControl.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripSeparator2,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton3,
            this.toolStripSeparator3,
            this.toolStripLabel2,
            this.toolStripLabel1,
            this.toolStripDropDownButton4});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(532, 51);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.перспективнаяToolStripMenuItem,
            this.ортографическаяToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(75, 48);
            this.toolStripDropDownButton1.Text = "Проекция";
            // 
            // перспективнаяToolStripMenuItem
            // 
            this.перспективнаяToolStripMenuItem.Checked = true;
            this.перспективнаяToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.перспективнаяToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.перспективнаяToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.перспективнаяToolStripMenuItem.Name = "перспективнаяToolStripMenuItem";
            this.перспективнаяToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.перспективнаяToolStripMenuItem.Text = "Перспективная";
            // 
            // ортографическаяToolStripMenuItem
            // 
            this.ортографическаяToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ортографическаяToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ортографическаяToolStripMenuItem.Name = "ортографическаяToolStripMenuItem";
            this.ортографическаяToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ортографическаяToolStripMenuItem.Text = "Ортографическая";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 51);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xYToolStripMenuItem,
            this.yXToolStripMenuItem,
            this.zXToolStripMenuItem,
            this.xZToolStripMenuItem,
            this.yZToolStripMenuItem,
            this.zYToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(101, 48);
            this.toolStripDropDownButton2.Text = "Выравнивание";
            // 
            // xYToolStripMenuItem
            // 
            this.xYToolStripMenuItem.Name = "xYToolStripMenuItem";
            this.xYToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.xYToolStripMenuItem.Text = "XY";
            this.xYToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // yXToolStripMenuItem
            // 
            this.yXToolStripMenuItem.Name = "yXToolStripMenuItem";
            this.yXToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.yXToolStripMenuItem.Text = "YX";
            this.yXToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // zXToolStripMenuItem
            // 
            this.zXToolStripMenuItem.Name = "zXToolStripMenuItem";
            this.zXToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.zXToolStripMenuItem.Text = "ZX";
            this.zXToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // xZToolStripMenuItem
            // 
            this.xZToolStripMenuItem.Name = "xZToolStripMenuItem";
            this.xZToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.xZToolStripMenuItem.Text = "XZ";
            this.xZToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // yZToolStripMenuItem
            // 
            this.yZToolStripMenuItem.Name = "yZToolStripMenuItem";
            this.yZToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.yZToolStripMenuItem.Text = "YZ";
            this.yZToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // zYToolStripMenuItem
            // 
            this.zYToolStripMenuItem.Name = "zYToolStripMenuItem";
            this.zYToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
            this.zYToolStripMenuItem.Text = "ZY";
            this.zYToolStripMenuItem.Click += new System.EventHandler(this.OnAlignCamera);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.затененнаяToolStripMenuItem,
            this.сеткаToolStripMenuItem,
            this.точкиToolStripMenuItem});
            this.toolStripDropDownButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton3.Image")));
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(95, 48);
            this.toolStripDropDownButton3.Text = "Растеризация";
            // 
            // затененнаяToolStripMenuItem
            // 
            this.затененнаяToolStripMenuItem.Checked = true;
            this.затененнаяToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.затененнаяToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.затененнаяToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.затененнаяToolStripMenuItem.Name = "затененнаяToolStripMenuItem";
            this.затененнаяToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.затененнаяToolStripMenuItem.Tag = "1";
            this.затененнаяToolStripMenuItem.Text = "Затененная";
            this.затененнаяToolStripMenuItem.Click += new System.EventHandler(this.OnRasterModeChange);
            // 
            // сеткаToolStripMenuItem
            // 
            this.сеткаToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.сеткаToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.сеткаToolStripMenuItem.Name = "сеткаToolStripMenuItem";
            this.сеткаToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.сеткаToolStripMenuItem.Tag = "2";
            this.сеткаToolStripMenuItem.Text = "Сетка";
            this.сеткаToolStripMenuItem.Click += new System.EventHandler(this.OnRasterModeChange);
            // 
            // точкиToolStripMenuItem
            // 
            this.точкиToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.точкиToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.точкиToolStripMenuItem.Name = "точкиToolStripMenuItem";
            this.точкиToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.точкиToolStripMenuItem.Tag = "4";
            this.точкиToolStripMenuItem.Text = "Точки";
            this.точкиToolStripMenuItem.Click += new System.EventHandler(this.OnRasterModeChange);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 51);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xYToolStripMenuItem1,
            this.yXToolStripMenuItem1,
            this.zXToolStripMenuItem1,
            this.xZToolStripMenuItem1,
            this.yZToolStripMenuItem1,
            this.zYToolStripMenuItem1});
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(96, 48);
            this.toolStripLabel2.Text = "Освещение 1:";
            // 
            // xYToolStripMenuItem1
            // 
            this.xYToolStripMenuItem1.Name = "xYToolStripMenuItem1";
            this.xYToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.xYToolStripMenuItem1.Text = "XY";
            this.xYToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // yXToolStripMenuItem1
            // 
            this.yXToolStripMenuItem1.Name = "yXToolStripMenuItem1";
            this.yXToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.yXToolStripMenuItem1.Text = "YX";
            this.yXToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // zXToolStripMenuItem1
            // 
            this.zXToolStripMenuItem1.Name = "zXToolStripMenuItem1";
            this.zXToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.zXToolStripMenuItem1.Text = "ZX";
            this.zXToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // xZToolStripMenuItem1
            // 
            this.xZToolStripMenuItem1.Name = "xZToolStripMenuItem1";
            this.xZToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.xZToolStripMenuItem1.Text = "XZ";
            this.xZToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // yZToolStripMenuItem1
            // 
            this.yZToolStripMenuItem1.Name = "yZToolStripMenuItem1";
            this.yZToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.yZToolStripMenuItem1.Text = "YZ";
            this.yZToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // zYToolStripMenuItem1
            // 
            this.zYToolStripMenuItem1.Name = "zYToolStripMenuItem1";
            this.zYToolStripMenuItem1.Size = new System.Drawing.Size(88, 22);
            this.zYToolStripMenuItem1.Text = "ZY";
            this.zYToolStripMenuItem1.Click += new System.EventHandler(this.OnAlignLighting);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(48, 48);
            this.toolStripLabel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnChangeLightHemisphere);
            this.toolStripLabel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnChangeLightDirection);
            this.toolStripLabel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnRedrawLightDirection);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolStripDropDownButton4
            // 
            this.toolStripDropDownButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сглаживаниеToolStripMenuItem});
            this.toolStripDropDownButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton4.Image")));
            this.toolStripDropDownButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton4.Name = "toolStripDropDownButton4";
            this.toolStripDropDownButton4.Size = new System.Drawing.Size(62, 48);
            this.toolStripDropDownButton4.Text = "Прочее";
            // 
            // сглаживаниеToolStripMenuItem
            // 
            this.сглаживаниеToolStripMenuItem.Checked = true;
            this.сглаживаниеToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.сглаживаниеToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.сглаживаниеToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.сглаживаниеToolStripMenuItem.Name = "сглаживаниеToolStripMenuItem";
            this.сглаживаниеToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.сглаживаниеToolStripMenuItem.Text = "Сглаживание";
            this.сглаживаниеToolStripMenuItem.Click += new System.EventHandler(this.OnEnableAntialiasing);
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RenderForm";
            this.Text = "Визуализатор";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem перспективнаяToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ортографическаяToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem xYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zYToolStripMenuItem;
        private CoreVisualizer.RenderControl renderControl;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem затененнаяToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сеткаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem точкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripLabel2;
        private System.Windows.Forms.ToolStripMenuItem xYToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem yXToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zXToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem xZToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem yZToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zYToolStripMenuItem1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton4;
        private System.Windows.Forms.ToolStripMenuItem сглаживаниеToolStripMenuItem;
    }
}

