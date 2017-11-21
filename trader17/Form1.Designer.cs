namespace trader17
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.button1 = new System.Windows.Forms.Button();
            this.txtZoom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.txtBuyThresh = new System.Windows.Forms.TextBox();
            this.txtBuyTally = new System.Windows.Forms.TextBox();
            this.txtSellThresh = new System.Windows.Forms.TextBox();
            this.txtSellTally = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblGain = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.txtBuyThreshLong = new System.Windows.Forms.TextBox();
            this.txtSellThreshLong = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSellLongTally = new System.Windows.Forms.TextBox();
            this.txtBuyLongTally = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbLoadFile = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtChartStart = new System.Windows.Forms.TextBox();
            this.txtChartEnd = new System.Windows.Forms.TextBox();
            this.txtShort = new System.Windows.Forms.TextBox();
            this.txtLong = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.txtSpan = new System.Windows.Forms.TextBox();
            this.txtFit = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtZoom
            // 
            this.txtZoom.Location = new System.Drawing.Point(162, 14);
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(100, 20);
            this.txtZoom.TabIndex = 1;
            this.txtZoom.TextChanged += new System.EventHandler(this.txtZoom_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "pctIncrease";
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 40);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(974, 300);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            // 
            // txtBuyThresh
            // 
            this.txtBuyThresh.Location = new System.Drawing.Point(464, 5);
            this.txtBuyThresh.Name = "txtBuyThresh";
            this.txtBuyThresh.Size = new System.Drawing.Size(100, 20);
            this.txtBuyThresh.TabIndex = 4;
            this.txtBuyThresh.TextChanged += new System.EventHandler(this.txtBuyThresh_TextChanged);
            // 
            // txtBuyTally
            // 
            this.txtBuyTally.Location = new System.Drawing.Point(570, 5);
            this.txtBuyTally.Name = "txtBuyTally";
            this.txtBuyTally.Size = new System.Drawing.Size(100, 20);
            this.txtBuyTally.TabIndex = 5;
            this.txtBuyTally.TextChanged += new System.EventHandler(this.txtBuyTally_TextChanged);
            // 
            // txtSellThresh
            // 
            this.txtSellThresh.Location = new System.Drawing.Point(947, 5);
            this.txtSellThresh.Name = "txtSellThresh";
            this.txtSellThresh.Size = new System.Drawing.Size(100, 20);
            this.txtSellThresh.TabIndex = 6;
            this.txtSellThresh.TextChanged += new System.EventHandler(this.txtSellThresh_TextChanged);
            // 
            // txtSellTally
            // 
            this.txtSellTally.Location = new System.Drawing.Point(1053, 5);
            this.txtSellTally.Name = "txtSellTally";
            this.txtSellTally.Size = new System.Drawing.Size(100, 20);
            this.txtSellTally.TabIndex = 7;
            this.txtSellTally.TextChanged += new System.EventHandler(this.txtSellTally_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(461, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "buyThresh";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(570, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "buyTally";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(944, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "sellThresh";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1050, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "sellTally";
            // 
            // lblGain
            // 
            this.lblGain.AutoSize = true;
            this.lblGain.Location = new System.Drawing.Point(268, 17);
            this.lblGain.Name = "lblGain";
            this.lblGain.Size = new System.Drawing.Size(13, 13);
            this.lblGain.TabIndex = 12;
            this.lblGain.Text = "0";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 347);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(290, 121);
            this.txtStatus.TabIndex = 13;
            this.txtStatus.Text = "";
            // 
            // txtBuyThreshLong
            // 
            this.txtBuyThreshLong.Location = new System.Drawing.Point(358, 5);
            this.txtBuyThreshLong.Name = "txtBuyThreshLong";
            this.txtBuyThreshLong.Size = new System.Drawing.Size(100, 20);
            this.txtBuyThreshLong.TabIndex = 14;
            this.txtBuyThreshLong.TextChanged += new System.EventHandler(this.txtBuyThreshLong_TextChanged);
            // 
            // txtSellThreshLong
            // 
            this.txtSellThreshLong.Location = new System.Drawing.Point(841, 5);
            this.txtSellThreshLong.Name = "txtSellThreshLong";
            this.txtSellThreshLong.Size = new System.Drawing.Size(100, 20);
            this.txtSellThreshLong.TabIndex = 15;
            this.txtSellThreshLong.TextChanged += new System.EventHandler(this.txtSellThreshLong_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(355, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "buyThreshLong";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(838, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "sellThreshLong";
            // 
            // txtSellLongTally
            // 
            this.txtSellLongTally.Location = new System.Drawing.Point(1159, 5);
            this.txtSellLongTally.Name = "txtSellLongTally";
            this.txtSellLongTally.Size = new System.Drawing.Size(100, 20);
            this.txtSellLongTally.TabIndex = 18;
            this.txtSellLongTally.TextChanged += new System.EventHandler(this.txtSellLongTally_TextChanged);
            // 
            // txtBuyLongTally
            // 
            this.txtBuyLongTally.Location = new System.Drawing.Point(676, 5);
            this.txtBuyLongTally.Name = "txtBuyLongTally";
            this.txtBuyLongTally.Size = new System.Drawing.Size(100, 20);
            this.txtBuyLongTally.TabIndex = 19;
            this.txtBuyLongTally.TextChanged += new System.EventHandler(this.txtBuyLongTally_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(673, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "buyLongTally";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1156, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "sellLongTally";
            // 
            // lbLoadFile
            // 
            this.lbLoadFile.FormattingEnabled = true;
            this.lbLoadFile.Location = new System.Drawing.Point(841, 347);
            this.lbLoadFile.Name = "lbLoadFile";
            this.lbLoadFile.Size = new System.Drawing.Size(284, 121);
            this.lbLoadFile.TabIndex = 22;
            this.lbLoadFile.SelectedIndexChanged += new System.EventHandler(this.lbLoadFile_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1145, 353);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "start";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1145, 381);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "end";
            // 
            // txtChartStart
            // 
            this.txtChartStart.Location = new System.Drawing.Point(1178, 350);
            this.txtChartStart.Name = "txtChartStart";
            this.txtChartStart.Size = new System.Drawing.Size(60, 20);
            this.txtChartStart.TabIndex = 25;
            this.txtChartStart.TextChanged += new System.EventHandler(this.txtChartStart_TextChanged);
            // 
            // txtChartEnd
            // 
            this.txtChartEnd.Location = new System.Drawing.Point(1178, 378);
            this.txtChartEnd.Name = "txtChartEnd";
            this.txtChartEnd.Size = new System.Drawing.Size(60, 20);
            this.txtChartEnd.TabIndex = 26;
            this.txtChartEnd.TextChanged += new System.EventHandler(this.txtChartEnd_TextChanged);
            // 
            // txtShort
            // 
            this.txtShort.Location = new System.Drawing.Point(1178, 422);
            this.txtShort.Name = "txtShort";
            this.txtShort.Size = new System.Drawing.Size(60, 20);
            this.txtShort.TabIndex = 27;
            this.txtShort.TextChanged += new System.EventHandler(this.txtShort_TextChanged);
            // 
            // txtLong
            // 
            this.txtLong.Location = new System.Drawing.Point(1178, 448);
            this.txtLong.Name = "txtLong";
            this.txtLong.Size = new System.Drawing.Size(60, 20);
            this.txtLong.TabIndex = 28;
            this.txtLong.TextChanged += new System.EventHandler(this.txtLong_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1140, 425);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "short";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1140, 451);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "long";
            // 
            // chart2
            // 
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(316, 353);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(300, 115);
            this.chart2.TabIndex = 31;
            this.chart2.Text = "chart2";
            // 
            // txtSpan
            // 
            this.txtSpan.Location = new System.Drawing.Point(735, 448);
            this.txtSpan.Name = "txtSpan";
            this.txtSpan.Size = new System.Drawing.Size(100, 20);
            this.txtSpan.TabIndex = 32;
            // 
            // txtFit
            // 
            this.txtFit.Location = new System.Drawing.Point(735, 418);
            this.txtFit.Name = "txtFit";
            this.txtFit.Size = new System.Drawing.Size(100, 20);
            this.txtFit.TabIndex = 33;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(673, 451);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(30, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "span";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(673, 422);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(15, 13);
            this.label15.TabIndex = 35;
            this.label15.Text = "fit";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 480);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtFit);
            this.Controls.Add(this.txtSpan);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtLong);
            this.Controls.Add(this.txtShort);
            this.Controls.Add(this.txtChartEnd);
            this.Controls.Add(this.txtChartStart);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lbLoadFile);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtBuyLongTally);
            this.Controls.Add(this.txtSellLongTally);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSellThreshLong);
            this.Controls.Add(this.txtBuyThreshLong);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lblGain);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSellTally);
            this.Controls.Add(this.txtSellThresh);
            this.Controls.Add(this.txtBuyTally);
            this.Controls.Add(this.txtBuyThresh);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZoom);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TextBox txtBuyThresh;
        private System.Windows.Forms.TextBox txtBuyTally;
        private System.Windows.Forms.TextBox txtSellThresh;
        private System.Windows.Forms.TextBox txtSellTally;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblGain;
        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.TextBox txtBuyThreshLong;
        private System.Windows.Forms.TextBox txtSellThreshLong;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSellLongTally;
        private System.Windows.Forms.TextBox txtBuyLongTally;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lbLoadFile;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtChartStart;
        private System.Windows.Forms.TextBox txtChartEnd;
        private System.Windows.Forms.TextBox txtShort;
        private System.Windows.Forms.TextBox txtLong;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.TextBox txtSpan;
        private System.Windows.Forms.TextBox txtFit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
    }
}

