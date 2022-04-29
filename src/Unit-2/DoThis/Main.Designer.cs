namespace ChartApp
{
    partial class Main
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
            this.AddSeries = new System.Windows.Forms.Button();
            this.sysChart = new DevExpress.XtraCharts.ChartControl();
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // AddSeries
            // 
            this.AddSeries.Location = new System.Drawing.Point(679, 357);
            this.AddSeries.Name = "AddSeries";
            this.AddSeries.Size = new System.Drawing.Size(75, 23);
            this.AddSeries.TabIndex = 0;
            this.AddSeries.Text = "AddSeries";
            this.AddSeries.UseVisualStyleBackColor = true;
            this.AddSeries.Click += new System.EventHandler(this.addSeries_Click);
            // 
            // sysChart
            // 
            this.sysChart.Location = new System.Drawing.Point(12, 12);
            this.sysChart.Name = "sysChart";
            this.sysChart.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.sysChart.Size = new System.Drawing.Size(661, 491);
            this.sysChart.TabIndex = 1;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 515);
            this.Controls.Add(this.sysChart);
            this.Controls.Add(this.AddSeries);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button AddSeries;
        private DevExpress.XtraCharts.ChartControl sysChart;
    }
}

