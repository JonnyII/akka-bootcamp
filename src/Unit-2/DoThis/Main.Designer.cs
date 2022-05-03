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
            this.sysChart = new DevExpress.XtraCharts.ChartControl();
            this.Cpu = new System.Windows.Forms.Button();
            this.Memory = new System.Windows.Forms.Button();
            this.Disk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // sysChart
            // 
            this.sysChart.Location = new System.Drawing.Point(12, 12);
            this.sysChart.Name = "sysChart";
            this.sysChart.SeriesSerializable = new DevExpress.XtraCharts.Series[0];
            this.sysChart.Size = new System.Drawing.Size(661, 491);
            this.sysChart.TabIndex = 1;
            // 
            // Cpu
            // 
            this.Cpu.Location = new System.Drawing.Point(679, 368);
            this.Cpu.Name = "Cpu";
            this.Cpu.Size = new System.Drawing.Size(107, 41);
            this.Cpu.TabIndex = 2;
            this.Cpu.Text = "CPU (ON)";
            this.Cpu.UseVisualStyleBackColor = true;
            this.Cpu.Click += new System.EventHandler(this.Cpu_Click);
            // 
            // Memory
            // 
            this.Memory.Location = new System.Drawing.Point(679, 415);
            this.Memory.Name = "Memory";
            this.Memory.Size = new System.Drawing.Size(107, 41);
            this.Memory.TabIndex = 3;
            this.Memory.Text = "MEMORY (OFF)";
            this.Memory.UseVisualStyleBackColor = true;
            this.Memory.Click += new System.EventHandler(this.Memory_Click);
            // 
            // Disk
            // 
            this.Disk.Location = new System.Drawing.Point(679, 462);
            this.Disk.Name = "Disk";
            this.Disk.Size = new System.Drawing.Size(107, 41);
            this.Disk.TabIndex = 4;
            this.Disk.Text = "DISK (OFF)";
            this.Disk.UseVisualStyleBackColor = true;
            this.Disk.Click += new System.EventHandler(this.Disk_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 515);
            this.Controls.Add(this.Disk);
            this.Controls.Add(this.Memory);
            this.Controls.Add(this.Cpu);
            this.Controls.Add(this.sysChart);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sysChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraCharts.ChartControl sysChart;
        private Button Cpu;
        private Button Memory;
        private Button Disk;
    }
}

