namespace CustomChartControlTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.customChartControl1 = new CustomChartControlTest.CustomChartControl();
            this.SuspendLayout();
            // 
            // customChartControl1
            // 
            this.customChartControl1.Appearance.BackColor = System.Drawing.Color.White;
            this.customChartControl1.Appearance.Options.UseBackColor = true;
            this.customChartControl1.AxisMargin = new System.Windows.Forms.Padding(3);
            this.customChartControl1.AxisX.Font = new System.Drawing.Font("Tahoma", 8.830189F);
            this.customChartControl1.AxisX.MaxValue = 1300D;
            this.customChartControl1.AxisX.MinValue = 0D;
            this.customChartControl1.AxisX.SideMargin = 0.5D;
            this.customChartControl1.AxisX.ThickLength = 10;
            this.customChartControl1.AxisX.ThickMargin = 3;
            this.customChartControl1.AxisY.Font = new System.Drawing.Font("Tahoma", 8.830189F);
            this.customChartControl1.AxisY.MaxValue = 1300D;
            this.customChartControl1.AxisY.MinValue = 0D;
            this.customChartControl1.AxisY.SideMargin = 0.5D;
            this.customChartControl1.AxisY.ThickLength = 10;
            this.customChartControl1.AxisY.ThickMargin = 3;
            this.customChartControl1.DrawMode = CustomChartControlTest.DrawModeType.CustomMode;
            this.customChartControl1.Location = new System.Drawing.Point(92, 63);
            this.customChartControl1.Name = "customChartControl1";
            this.customChartControl1.Size = new System.Drawing.Size(718, 421);
            this.customChartControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 541);
            this.Controls.Add(this.customChartControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomChartControl customChartControl1;









    }
}

