using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using Jojatekok.PoloniexAPI;

namespace trader17
{
    public partial class Form1 : Form
    {
        List<float> points = new List<float>();
        List<float> hodlPoints = new List<float>();
        List<float> divs = new List<float>();
        List<DateTime> dates = new List<DateTime>();
        float zoom = 1;
        private Timer timer1;
        public int interval = 2000;
        public List<float> centers = new List<float>();
        public List<float> dips = new List<float>();
        public List<float> dipPoints = new List<float>();
        public List<float> lrPoints = new List<float>();
        public List<float> BBTop = new List<float>();
        public List<float> BBBot = new List<float>();
        public List<float> cutoffPoints = new List<float>();
        float dipCooldown = 0;

        List<float> pctTally = new List<float>();
        List<float> pctTallyHodlMode = new List<float>();
        List<float> emaShort = new List<float>();
        List<float> emaLong = new List<float>();
        List<float> smasShort = new List<float>();
        List<float> smasLong = new List<float>();
        int emaShortLength = 130;
        int emaLongLength = 340;
        float divergence = 0;
        float divergencePrev = 0;
        float divergenceMax = 0;
        float divergenceMin = 0;

        float asks = 0;
        float bids = 0;
        float boughtAt = 0;
        float pctSum = 0;
        string strLog = "";
        string logPath = "";
        string strTradeAction = "";

        float buyThreshLong = 0.3f;
        float buyThresh = 0.5f;
        int buyTally = 50;
        int buyLongTally = 3000;
        float sellThreshLong = -0.3f;
        float sellThresh = -0.5f;
        int sellTally = 20;
        int sellLongTally = 6000;

        int chartStartIdx = 0;
        int chartEndIdx = 0;

        float tradeAmount = 0.00034f;

        int ticks = 0;

        bool testMode = false;
        bool tradeEnabled = false;

        int splitSpan = 2500;
        float fit = 10f;

        bool bulkFiles = false;
        float fee = 0.005f;
        float buyCutoffPct = 1.02f;
        float sellCutoffPct = 0.97f;
        int bolingerLength = 2000;

        Random r = new Random();

        private static readonly HttpClient client = new HttpClient();

        public enum tradeState
        {
            buy,
            sell,
            hodl
        }

        public List<tradeState> states = new List<tradeState>();

        public enum emaState
        {
            shortAbove,
            shortBelow,
            shortCrossedUp,
            shortCrossedDown
        }

        public List<emaState> emaStates = new List<emaState>();

        public enum TrackState
        {
            belowBB,
            aboveBB,
            trackingDip
        }

        public List<TrackState> trackStates = new List<TrackState>();

        public struct testResult
        {
            public float buyThresh;
            public float sellThresh;
            public int buyTally;
            public int sellTally;
            public float gain;
            public float gainPct;
        }

        public struct testResultMA
        {
            public int emaShort;
            public int emaLong;
            public float gain;
            public float gainPct;
        }

        public List<testResult> results = new List<testResult>();
        public int testNum = 0;
        public float highestGain = -10;

        public string buyMsg = "";
        //public string buyException = "";
        public string buyMsgPrev = "";
        public string sellMsg = "";
        //public string buyExceptionPrev = "";
        public string sellMsgPrev = "";
        //public string sellExceptionPrev = "";

        private PoloniexClient PoloniexClient { get; set; }
        Task<ulong> bID;
        Task<ulong> sID;

        public Form1()
        {
            InitializeComponent();
            txtZoom.Text = zoom.ToString();

            logPath = DateTime.Now.ToString().Replace("/", "-").Replace(":", ".") + ".csv";
            strLog += "last" + ",," + "pct" + ",," + "tally" + ",," + "TimeStamp" + ",," + "Trade Action" + "\n";


            PoloniexClient = new PoloniexClient("", "");
            // Jojatekok.PoloniexAPI.WalletTools.Balance w = PoloniexClient.Wallet.GetBalancesAsync();

            //TopMost = true;
            
            //getBalance();

            //ChartAreas();
            //ChartSeries();

            string loadFile = "";

            //loadFile = "7-23-2017 4.38.48 PM.csv";
            //loadFile = "7-23-2017 7.37.50 PM.csv";
            //loadFile = "7-23-2017 8.07.37 PM.csv";
            //loadFile = "7-24-2017 10.44.05 AM.csv";
            //loadFile = "7-24-2017 9.25.29 PM.csv";
            //loadFile = "7-24-2017 8.26.57 PM.csv";
            //loadFile = "7-24-2017 12.40.44 PM.csv";
            //loadFile = "7-25-2017 12.06.37 AM.csv";
            //loadFile = "7-25-2017 9.21.33 AM.csv";
            //loadFile = "7-25-2017 7.37.52 PM.csv";
            //loadFile = "8-5-2017 8.54.09 AM.csv";

            loadFiles();

            txtBuyThresh.Text = buyThresh.ToString();
            txtBuyThreshLong.Text = buyThreshLong.ToString();
            txtSellThresh.Text = sellThresh.ToString();
            txtSellThreshLong.Text = sellThreshLong.ToString();
            txtBuyTally.Text = buyTally.ToString();
            txtBuyLongTally.Text = buyLongTally.ToString();
            txtSellTally.Text = sellTally.ToString();
            txtSellLongTally.Text = sellLongTally.ToString();

            txtShort.Text = emaShortLength.ToString();
            txtLong.Text = emaLongLength.ToString();

            txtSpan.Text = splitSpan.ToString();
            txtFit.Text = fit.ToString();

            //testMode = true;

            if (loadFile == "")
            {
                loadBlankChart();
                //tradeEnabled = true;
            }
            else
            {
                loadChart(loadFile);
            }

        }

        private async void getBalance()
        {
            //var returnData = new Dictionary<string, IBalance>();
            //returnData = await PoloniexClient.Wallet.GetBalancesAsync();

            //balance = balance;
        }


        private void loadBlankChart()
        {
            for (int i = 0; i < bolingerLength + 100; i++)
            {
                points.Add(0);
                hodlPoints.Add(0);
                dipPoints.Add(0);
                lrPoints.Add(0);
                cutoffPoints.Add(0); 
                BBTop.Add(0);
                BBBot.Add(0);
            }
        }

        private void loadFiles()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string localPath = new Uri(path).LocalPath;
            DirectoryInfo di = new DirectoryInfo(localPath);
            FileInfo[] rgFiles = di.GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            foreach (FileInfo fi in rgFiles)
            {
                lbLoadFile.Items.Add(fi.Name);
            }
        }

        private void loadChart(string loadFile)
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string localPath = new Uri(path).LocalPath;
            DirectoryInfo di = new DirectoryInfo(localPath);
            FileInfo[] rgFiles = di.GetFiles(loadFile, SearchOption.AllDirectories);

            List<string> lines;
            string line;

            points.Clear();
            dipPoints.Clear();
            lrPoints.Clear();
            cutoffPoints.Clear();
            BBTop.Clear();
            BBBot.Clear();
            pctTallyHodlMode.Clear();

            foreach (FileInfo fi in rgFiles)
            {
                lines = new List<string>();

                using (StreamReader reader = new StreamReader(fi.Directory + "/" + fi.Name))
                {
                    line = reader.ReadLine();

                    while (line != null)
                    {
                        if (!line.Contains("TimeStamp") &&
                            !line.Contains("bought") &&
                            !line.Contains("sold") &&
                            !line.Contains("gain"))
                        {
                            float point = float.Parse(line.Substring(0, line.IndexOf(",")));
                            points.Add(point);
                            dipPoints.Add(0);
                            lrPoints.Add(0);
                            cutoffPoints.Add(0);
                            BBTop.Add(0);
                            BBBot.Add(0);




                            int idx = line.IndexOf(",,") + 2;
                            line = line.Substring(idx, line.Length - idx);
                            float pct = float.Parse(line.Substring(0, line.IndexOf(",,")));
                            pctTallyHodlMode.Add(pct);

                            idx = line.IndexOf(",,") + 2;
                            line = line.Substring(idx, line.Length - idx);
                            idx = line.IndexOf(",,") + 2;
                            line = line.Substring(idx, line.Length - idx);
                            DateTime date = DateTime.Parse(line.Substring(0, line.IndexOf(",,")));
                            dates.Add(date);

                            idx = line.LastIndexOf(",") + 1;
                            string endStr = line.Substring(idx, line.Length - idx);

                            if (endStr.Contains("hodl"))
                                hodlPoints.Add(point);
                            else
                                hodlPoints.Add(0);
                        }

                        line = reader.ReadLine();
                    }
                }

            }

            updateGraph();
        }

        private void ChartAreas()
        {
            var axisX = new System.Windows.Forms.DataVisualization.Charting.Axis
            {
                Interval = 1,
            };

            float min = float.MaxValue;
            float max = 0;

            if (points.Count > 0)
            {
                //min = (int)points.Min();
                //max = (int)points.Max();

                min = points[points.Count - 1] - 5f;
                max = points[points.Count - 1] + 5f;

            }

            foreach (float p in points)
            {
                if (p > max)
                    max = p;

                if (p < min && p > 0)
                    min = p;
            }

            //min = -1;
            //max = 1;

            var axisY = new System.Windows.Forms.DataVisualization.Charting.Axis
            {
                Minimum = (int)min,
                Maximum = (int)max,
                Title = "BTC",
            };

            var chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea
            {
                AxisX = axisX,
                AxisY = axisY,
            };

            chartArea1.AxisX.LabelStyle.Format = "dd/MMM\nhh:mm";
            //chartArea1.AxisX.LabelStyle.Format = "hh:mm";


            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisX.LabelStyle.Enabled = false;

            this.chart1.ChartAreas.Clear();
            this.chart1.ChartAreas.Add(chartArea1);
        }

        private void ChartAreas2()
        {
            float min = float.MaxValue;
            float max = 0;

            if (divs.Count > 0)
            {
                //min = (int)points.Min();
                //max = (int)points.Max();

                min = divs[divs.Count - 1];
                max = divs[divs.Count - 1];

            }

            foreach (float p in divs)
            {
                if (p > max)
                    max = p;

                if (p < min && p > 0)
                    min = p;
            }

            min = -10;
            max = 10;

            var axisX = new System.Windows.Forms.DataVisualization.Charting.Axis
            {
                Minimum = min,
                Maximum = max,
                Title = "div",
            };

            var axisY = new System.Windows.Forms.DataVisualization.Charting.Axis
            {
                //Interval = 1,
            };

            var chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea
            {
                AxisX = axisY,
                AxisY = axisX,
            };

            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisX.LabelStyle.Enabled = false;

            this.chart2.ChartAreas.Clear();
            this.chart2.ChartAreas.Add(chartArea2);
        }

        private void ChartSeries()
        {
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "red",
                Color = Color.Red,
                BorderWidth = 2,
                //IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            var series2 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "blue",
                Color = Color.Blue,
                BorderWidth = 1,
                //IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            chart1.Series[0]["PieLabelStyle"] = "Disabled";

            //series1.XValueType = ChartValueType.DateTime;


            var series3 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "green",
                Color = Color.Green,
                BorderWidth = 1,
                //IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            var series4 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "purple",
                Color = Color.Purple,
                BorderWidth = 1,
                //IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };



            for (int i = 0; i < points.Count; i++)
            {
                //DateTime d = DateTime.Now;
                //if (dates.Count > 0)
                //    d = dates[i];

                series1.Points.AddXY(i, points[i]);
                series2.Points.AddXY(i, hodlPoints[i]);
            }


            chart1.Series.Clear();
            chart1.Series.Add(series1);
            chart1.Series.Add(series3);
            chart1.Series.Add(series4);

            //chart1.DataManipulator.CopySeriesValues("red line", "green line");

            //chart1.Series["red line"].Points.DataBindXY(xvals, yvals);

            //chart1.DataManipulator.IsStartFromFirst = false;
            

            var series5 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "dips",
                Color = Color.Orange,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < points.Count; i++)
            {
                if (dipPoints[i] > 0)
                    i = i;

                series5.Points.AddXY(i, dipPoints[i]);
            }

            chart1.Series.Add(series5);


            var series6 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "smasShort",
                Color = Color.Orange,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < smasShort.Count; i++)
            {
                series6.Points.AddXY(i, smasShort[i]);
            }

            chart1.Series.Add(series6);

            var series7 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "smasLong",
                Color = Color.Yellow,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < smasLong.Count; i++)
            {
                series7.Points.AddXY(i, smasLong[i]);
            }

            chart1.Series.Add(series7);


            var series8 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "lr",
                Color = Color.Pink,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < points.Count; i++)
            {
                series8.Points.AddXY(i, lrPoints[i]);
            }

            chart1.Series.Add(series8);



            var series9 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "bbTop",
                Color = Color.Blue,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < points.Count; i++)
            {
                series9.Points.AddXY(i, BBTop[i]);
            }

            chart1.Series.Add(series9);


            var series10 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "bbBot",
                Color = Color.Blue,
                BorderWidth = 1,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < points.Count; i++)
            {
                series10.Points.AddXY(i, BBBot[i]);
            }

            chart1.Series.Add(series10);


            var series11 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "cutoff",
                Color = Color.HotPink,
                BorderWidth = 2,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            for (int i = 0; i < cutoffPoints.Count; i++)
            {
                series11.Points.AddXY(i, cutoffPoints[i]);
            }

            chart1.Series.Add(series11);


            chart1.Series.Add(series2);


            if (series1.Points.Count > emaShortLength)
                chart1.DataManipulator.FinancialFormula(
                    FinancialFormula.ExponentialMovingAverage, emaShortLength.ToString(), "red:Y", "green:Y");
            if (series1.Points.Count > emaLongLength)
                chart1.DataManipulator.FinancialFormula(
                    FinancialFormula.ExponentialMovingAverage, emaLongLength.ToString(), "red:Y", "purple:Y");


            /*
            if (series1.Points.Count > 240)
                chart1.DataManipulator.FinancialFormula(
                    FinancialFormula.MovingAverageConvergenceDivergence, "12,26", "red:Y", "green:Y");

            List<float> newPoints = new List<float>();

            foreach (DataPoint p in series3.Points)
                newPoints.Add((float)p.YValues[0]);

            series3.Points.Clear();
            for (int i = 0; i < newPoints.Count; i++)
            {
                series3.Points.AddXY(i, newPoints[i] * ((chart1.ChartAreas[0].AxisY.Maximum + chart1.ChartAreas[0].AxisY.Minimum)/2f));
            }
            */


            /*
            emaShort.Clear();

            //for (int i = 0; i < emaShortLength; i++)
            //    emaShort.Add(0);

            foreach (DataPoint p in series3.Points)
                emaShort.Add((float)p.YValues[0]);


            emaLong.Clear();

            //for (int i = 0; i < emaLongLength; i++)
            //    emaLong.Add(0);

            foreach (DataPoint p in series4.Points)
                emaLong.Add((float)p.YValues[0]);
            */
            divs.Clear();
            for (int i = 0; i < Math.Min(smasShort.Count, smasLong.Count); i++)
                divs.Add(smasShort[i] - smasLong[Math.Min(i, smasLong.Count - 1)]);
            
            //series1.ChartType = SeriesChartType.FastLine;


            /*
            if(series1.Points.Count > 0)
            {
                chart1.Series.Add("TrendLine");
                chart1.Series["TrendLine"].ChartType = SeriesChartType.Line;
                chart1.Series["TrendLine"].BorderWidth = 3;
                chart1.Series["TrendLine"].Color = Color.Orange;
                // Line of best fit is linear
                string typeRegression = "Linear";//"Exponential";//
                                                 // The number of days for Forecasting
                string forecasting = "1";
                // Show Error as a range chart.
                string error = "false";
                // Show Forecasting Error as a range chart.
                string forecastingError = "false";
                // Formula parameters
                string parameters = typeRegression + ',' + forecasting + ',' + error + ',' + forecastingError;
                chart1.Series[0].Sort(PointSortOrder.Ascending, "X");
                // Create Forecasting Series.
                chart1.DataManipulator.FinancialFormula(FinancialFormula.Forecasting, parameters, chart1.Series[0], chart1.Series["TrendLine"]);
            }
            */


        }

        private void ChartSeries2()
        {
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "diverge",
                Color = Color.Yellow,
                BorderWidth = 2,
                IsXValueIndexed = false,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
            };

            //chart2.Series[0]["PieLabelStyle"] = "Disabled";


            for (int i = 0; i < divs.Count; i++)
            {
                series1.Points.AddXY(i, divs[i]);
            }


            chart2.Series.Clear();
            chart2.Series.Add(series1);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1 == null)
            {
                InitTimer();
                button1.Text = "Stop";
            }
            else
            {
                timer1.Stop();
                timer1 = null;
                button1.Text = "Start";
            }
        }

        public void InitTimer()
        {
            //bulkFileTest();

            
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = interval; // in miliseconds
            timer1.Start();
            
        }

        public void bulkFileTest()
        {
            bulkFiles = true;

            for(int i=0; i<lbLoadFile.Items.Count; i++)
            {
                txtStatus.Text += "Loading file " + (i + 1) + " out of " + lbLoadFile.Items.Count + ".\n";
                string file = lbLoadFile.Items[i].ToString() + "\n";
                Debug.WriteLine(file);
                loadChart(file);
                updateHodlSeriesMADip();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //run_cmd(@"C:\Users\Peter\Desktop\tr\test.py", "");

            ticks++;

            if (testMode)
                TestMA();
            else
                getHttpPolo();

        }

        public async void getHttpPolo()
        {
            var responseString = "";

            try
            {
                responseString = await client.GetStringAsync("https://poloniex.com/public?command=returnOrderBook&currencyPair=USDT_BTC&depth=1");
            }
            catch (TaskCanceledException e1)
            {
                txtStatus.Text = e1.Message + "\n" + txtStatus.Text;
                return;
            }
            catch (HttpRequestException e1)
            {
                txtStatus.Text = e1.Message + "\n" + txtStatus.Text;
                return;
            }


            responseString = responseString.Replace("{\"asks\":[[\"", "");

            asks = float.Parse(responseString.Substring(0, responseString.IndexOf("\",")));

            string phrase = "\"bids\":[[\"";
            responseString = responseString.Substring(responseString.IndexOf(phrase) + phrase.Length, 30);

            bids = float.Parse(responseString.Substring(0, responseString.IndexOf("\"")));

            //{ "asks":[["2671.30000000",0.33697612]],"bids":[["2670.33904965",9.32269263]],"isFrozen":"0","seq":114438621}

            float lastPrice = (asks + bids) / 2f;

            centers.Add(lastPrice);

            if (centers.Count > 100)
                centers.RemoveAt(0);

            httpStrat();
            //centerLiteStrat();
        }

        public async void getHttpBittrex()
        {
            var responseString = "";


            try
            {
                responseString = await client.GetStringAsync("https://bittrex.com/api/v1.1/public/getorderbook?market=USDT-BTC&type=both&depth=1");
            }
            catch (TaskCanceledException e1)
            {
                txtStatus.Text = e1.Message + "\n" + txtStatus.Text;
                return;
            }
            catch (HttpRequestException e1)
            {
                txtStatus.Text = e1.Message + "\n" + txtStatus.Text;
                return;
            }


            string phrase = "\"Rate\":";
            int startIdx = responseString.IndexOf(phrase) + phrase.Length;
            responseString = responseString.Substring(startIdx, responseString.Length - startIdx);
            float asks = float.Parse(responseString.Substring(0, responseString.IndexOf("},{")));

            phrase = "sell\":[{\"Quantity\":";
            startIdx = responseString.IndexOf(phrase) + phrase.Length;
            responseString = responseString.Substring(startIdx, responseString.Length - startIdx);

            phrase = "\"Rate\":";
            startIdx = responseString.IndexOf(phrase) + phrase.Length;
            responseString = responseString.Substring(startIdx, responseString.Length - startIdx);

            float bids = float.Parse(responseString.Substring(0, responseString.IndexOf("},{")));


            float lastPrice = (asks + bids) / 2f;

            centers.Add(lastPrice);

            if (centers.Count > 100)
                centers.RemoveAt(0);

            httpStrat();
        }

        public void httpStrat()
        {
            float pctIncrease = 0;

            if (centers.Count > 1)
            {
                pctIncrease = centers[centers.Count - 1] - centers[centers.Count - 2];
                pctIncrease /= centers[centers.Count - 1];
                pctIncrease *= 100;
            }

            //pctIncrease /= interval;
            //pctIncrease *= 1000 * 60;

            pctTally.Add(pctIncrease);

            if (pctTally.Count > 10000)
                pctTally.Remove(0);

            pctSum = 0;

            int tallyLength = 100;// 300 * 1000 / interval;

            for (int i = 0; i < Math.Min(pctTally.Count, tallyLength); i++)
                pctSum += pctTally[pctTally.Count - 1 - i];

            points.Add(centers[centers.Count - 1]);

            if (states.Contains(tradeState.hodl))
                hodlPoints.Add(centers[centers.Count - 1]);
            else
                hodlPoints.Add(0);

            dipPoints.Add(0);
            lrPoints.Add(0);
            cutoffPoints.Add(0);
            BBTop.Add(0);
            BBBot.Add(0);

            points.RemoveAt(0);
            hodlPoints.RemoveAt(0);
            dipPoints.RemoveAt(0);
            lrPoints.RemoveAt(0);
            cutoffPoints.RemoveAt(0);
            BBTop.RemoveAt(0);
            BBBot.RemoveAt(0);

            txtZoom.Text = pctSum.ToString("N10");

            TradeLogicBB();

            updateGraph();

            SetTradeMessage();

            strTradeAction = "";

            foreach (tradeState s in states)
                strTradeAction += s.ToString();


            float curDip = 0;
            float prevDip = 0;
            float logDipPct = 0;

            if(dips.Count > 0)
            {
                curDip = dips[dips.Count - 1];
                prevDip = dips[Math.Max(0, dips.Count - 2)];
                logDipPct = (curDip - prevDip) / curDip;
            }

            strLog += centers[centers.Count - 1].ToString("N2").Replace(",", "") + ",," + pctIncrease.ToString("N20") + ",," + pctSum.ToString("N20").Replace(",", "") + ",," + DateTime.Now + ",," + dipPoints[dipPoints.Count - 1].ToString("N2").Replace(",", "") + ",," + logDipPct.ToString("N10").Replace(",", "") + ",," + strTradeAction + "\n";

            File.AppendAllText(logPath, strLog);
            strLog = "";

            /*
            label1.Text = lastPrice.ToString() + "\n" + label1.Text;
            label2.Text = pctIncrease.ToString() + "\n" + label2.Text;
            label3.Text = pctSum.ToString() + "\n" + label3.Text;
            */
        }


        public void SetTradeMessage()
        {
            buyMsg = "";

            if (bID != null)
            {
                buyMsg = "buy:" + bID.Status.ToString() + " " + "\n";

                if (bID.Exception != null)
                {
                    buyMsg = "exception:" + bID.Exception.Message + "\n";

                    if (bID.Exception.InnerExceptions.Count > 0)
                        buyMsg += ".  " + bID.Exception.InnerExceptions[0].Message + "\n";
                }

            }

            sellMsg = "";

            if (sID != null)
            {
                sellMsg = "sell:" + sID.Status.ToString() + " " + "\n";
                
                if (sID.Exception != null)
                {
                    sellMsg = "exception:" + sID.Exception.Message + "\n";

                    if (sID.Exception.InnerExceptions.Count > 0)
                        sellMsg += ".  " + sID.Exception.InnerExceptions[0].Message + "\n";
                }
            }

            if (buyMsg != buyMsgPrev)
                txtStatus.Text = buyMsg + txtStatus.Text;

            if (sellMsg != sellMsgPrev)
                txtStatus.Text = sellMsg + txtStatus.Text;

            buyMsgPrev = buyMsg;
            sellMsgPrev = sellMsg;
        }

        public void Buy(float price)
        {
            CurrencyPair p = new CurrencyPair("USDT", "BTC");
            bID = PoloniexClient.Trading.PostOrderAsync(p, OrderType.Buy, price + 200, tradeAmount);
        }

        public void Sell()
        {
            CurrencyPair p = new CurrencyPair("USDT", "BTC");
            sID = PoloniexClient.Trading.PostOrderAsync(p, OrderType.Sell, 11, tradeAmount);
        }

        #region run_cmd
        /*
        private void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\Peter\AppData\Local\Programs\Python\Python36-32\python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    //Console.Write(result);
                    //MessageBox.Show(result);

                    string phrase = "'bid': Decimal('";
                    int strStart = result.IndexOf(phrase);
                    string strPrice = result.Substring(strStart + phrase.Length);
                    strPrice = strPrice.Substring(0, strPrice.IndexOf("')"));

                    float price = float.Parse(strPrice);

                    points.Add(price);
                    points.RemoveAt(0);

                    updateGraph();
                }
            }
        }
        */
        #endregion

        private void updateGraph()
        {
            ChartAreas();
            ChartSeries();

            ChartAreas2();
            ChartSeries2();

            chart1.Invalidate();
            chart2.Invalidate();

        }

        private void txtZoom_TextChanged(object sender, EventArgs e)
        {
            //zoom = float.Parse(txtZoom.Text);
        }


        public void TradeLogic()
        {
            float buyPctSum = 0;

            for (int i = 0; i < Math.Min(pctTally.Count, buyTally); i++)
                buyPctSum += pctTally[pctTally.Count - 1 - i];


            float buyLongPctSum = 0;

            for (int i = 0; i < Math.Min(pctTally.Count, buyLongTally); i++)
                buyLongPctSum += pctTally[pctTally.Count - 1 - i];

            if (buyPctSum > buyThresh && buyLongPctSum > buyThreshLong)
            {
                if (!states.Contains(tradeState.buy) &&
                   !states.Contains(tradeState.sell) &&
                   !states.Contains(tradeState.hodl))
                {
                    states.Add(tradeState.buy);

                    if (tradeEnabled)
                        if (ticks > Math.Max(buyLongTally, sellLongTally))
                            Buy(boughtAt);
                }
            }

            float sellPctSum = 0;

            for (int i = 0; i < Math.Min(pctTally.Count, sellTally); i++)
                sellPctSum += pctTally[pctTally.Count - 1 - i];

            float sellLongPctSum = 0;

            for (int i = 0; i < Math.Min(pctTally.Count, sellLongTally); i++)
                sellLongPctSum += pctTally[pctTally.Count - 1 - i];

            if (sellPctSum < sellThresh && sellLongPctSum > sellThreshLong)
            {
                if (!states.Contains(tradeState.sell) &&
                    states.Contains(tradeState.hodl))
                {
                    states.Add(tradeState.sell);
                    states.Remove(tradeState.hodl);

                    if (tradeEnabled)
                        Sell();
                }
            }

            if (centers[centers.Count - 1] < boughtAt * 0.99f)
            {
                if (!states.Contains(tradeState.sell) &&
                    states.Contains(tradeState.hodl))
                {
                    states.Add(tradeState.sell);
                    states.Remove(tradeState.hodl);

                    if (tradeEnabled)
                        Sell();
                }
            }


            if (states.Contains(tradeState.buy))
            {
                //do buy
                strLog += "bought at " + asks + "\n";
                boughtAt = asks;

                states.Remove(tradeState.buy);
                states.Add(tradeState.hodl);
            }

            if (states.Contains(tradeState.sell))
            {
                //do sell
                strLog += "sold at " + bids + "\n";

                float fee = 0.0025f;
                float gain = (bids - boughtAt) - (bids * fee);
                float gainPct = gain / bids;
                strLog += "gainPct: " + gainPct + "  gain: " + gain + "\n";

                lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();

                states.Remove(tradeState.sell);
            }

        }

        public void TradeLogicDip()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";


            float priceCeiling = 0;
            float highestPriceCeiling = 0;
            float prevPrice = 0;


            float shorti = 0;
            float longi = 0;

            if (emaShort.Count > 0)
                shorti = emaShort[Math.Min(points.Count - 1, emaShort.Count - 1)];

            if (emaLong.Count > 0)
                longi = emaLong[Math.Min(points.Count - 1, emaLong.Count - 1)];


            List<float> splitTicks = new List<float>();
            int splitLength = splitSpan;

            for (int j = 0; j < splitLength; j++)
                if (emaShort.Count > 0 && emaLong.Count > 0)
                    splitTicks.Add(emaShort[Math.Min(emaShort.Count - 1, Math.Max(points.Count - 1 - j, 0))] - emaLong[Math.Min(emaLong.Count - 1, Math.Max(points.Count - 1 - j, 0))]);
                else
                    splitTicks.Add(0);

            List<float> leftHalf = new List<float>();
            for (int j = splitLength * 2 / 3; j < splitLength; j++)
                leftHalf.Add(splitTicks[j]);

            List<float> rightHalf = new List<float>();
            for (int j = 0; j < splitLength / 3; j++)
                rightHalf.Add(splitTicks[j]);


            float leftAvg = leftHalf.Average();
            float rightAvg = rightHalf.Average();

            dipCooldown++;
            if (dipCooldown > splitSpan)
                dipCooldown = splitSpan;

            float buyPctSum = 0;

            for (int j = 0; j < 500; j++)
                if (emaShort.Count > 0 && emaLong.Count > 0)
                    buyPctSum += emaShort[Math.Min(emaShort.Count - 1, Math.Max(points.Count - 1 - j, 0))] - emaLong[Math.Min(emaLong.Count - 1, Math.Max(points.Count - 1 - j, 0))];


            if (rightAvg - leftAvg < fit)
            {
                if (leftAvg < 0 && rightAvg > 0)
                {
                    if (dipCooldown == splitSpan)
                    {
                        dips.Add((shorti + longi) / 2f);
                        dipCooldown = 0;

                        dipPoints[dipPoints.Count - 1] = points[points.Count - 1];

                        for (int j = 0; j < splitLength; j++)
                            if(j < splitLength / 3 || j > splitLength * 2 / 3)
                            lrPoints[lrPoints.Count - 1 - j] = points[points.Count - 1 - j];
                        
                        float curDip = dips[dips.Count - 1];
                        float prevDip = dips[Math.Max(0, dips.Count - 2)];
                        float dipPct = (curDip - prevDip) / curDip;

                        if (curDip > prevDip && dipPct > 0.005f && !hodlMode)
                        {
                            if (buyPctSum > -100000)
                            {
                                emaStates.Add(emaState.shortCrossedUp);
                                priceCeiling = 0;
                                highestPriceCeiling = 0;
                                prevPrice = points[points.Count - 1];
                            }

                        }
                    }

                }
            }

            if (shorti > 0 && longi > 0)
                divergence = shorti - longi;
            


            float pt = (shorti + longi) / 2f;

            if (hodlMode)
            {
                float ceil = (pt - prevPrice);
                prevPrice = pt;

                priceCeiling += ceil;

                if (priceCeiling > highestPriceCeiling)
                    highestPriceCeiling = priceCeiling;

                if (pt < boughtPrice + (highestPriceCeiling / 2f) && highestPriceCeiling > boughtPrice * 0.005f)
                    emaStates.Add(emaState.shortCrossedDown);
                //if (true)  cutoffPoints
            }

            if (emaStates.Contains(emaState.shortCrossedUp))
            {
                emaStates.Remove(emaState.shortCrossedUp);

                if (!hodlMode)
                {

                    if (!states.Contains(tradeState.buy) &&
                       !states.Contains(tradeState.sell) &&
                       !states.Contains(tradeState.hodl))
                    {

                        boughtPrice = points[points.Count - 1];
                        hodlMode = true;
                        tradeCounter++;

                        states.Add(tradeState.buy);

                        if (tradeEnabled)
                            Buy(boughtPrice);
                        //if (ticks > Math.Max(buyLongTally, sellLongTally))
                    }
                }
            }

            if (emaStates.Contains(emaState.shortCrossedDown))
            {
                emaStates.Remove(emaState.shortCrossedDown);

                if (hodlMode)
                {
                    if (!states.Contains(tradeState.sell) &&
                        states.Contains(tradeState.hodl))
                    {
                        states.Add(tradeState.sell);
                        states.Remove(tradeState.hodl);

                        hodlMode = false;
                        if (tradeEnabled)
                            Sell();
                    }
                }

            }


            if (pt < boughtPrice * sellCutoffPct && hodlMode)
            {
                if (!states.Contains(tradeState.sell) &&
                    states.Contains(tradeState.hodl))
                {
                    states.Add(tradeState.sell);
                    states.Remove(tradeState.hodl);

                    hodlMode = false;
                    if (tradeEnabled)
                        Sell();
                }
            }
            



            if (states.Contains(tradeState.buy))
            {
                //do buy
                strLog += "bought at " + asks + "\n";
                boughtAt = asks;

                states.Remove(tradeState.buy);
                states.Add(tradeState.hodl);
            }

            if (states.Contains(tradeState.sell))
            {
                //do sell
                strLog += "sold at " + bids + "\n";

                float fee = 0.0025f;
                gain = (bids - boughtAt) - (bids * fee);
                gainPct = gain / bids;
                strLog += "gainPct: " + gainPct + "  gain: " + gain + "\n";

                lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();

                states.Remove(tradeState.sell);
            }

        }

        public void TradeLogicBB()
        {
            if (ticks < bolingerLength)
                return;

            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";



            smasShort.Clear();
            float smasShortLength = 130;

            float sma = 0;
            for (int j = 0; j < smasShortLength; j++)
                sma += points[Math.Max(points.Count - 1 - j, 0)];
            
            sma /= smasShortLength;

            float mult = 2 / (float)(smasShortLength + 1);

            float ema = points[points.Count - 1];

            if (smasShort.Count > 0)
                ema = (points[points.Count - 1] * mult) + (smasShort[Math.Max(points.Count - 1 - 1, 0)] * (1 - mult));

            smasShort.Add(ema);
            
            smasLong.Clear();
            float smasLongLength = 340;

            sma = 0;
            for (int j = 0; j < smasLongLength; j++)
                sma += points[Math.Max(points.Count - 1 - j, 0)];

            sma /= smasLongLength;

             mult = 2 / (float)(smasLongLength + 1);
            //mult =0.5f;

            ema = points[points.Count - 1];

            if (smasLong.Count > 0)
                ema = (points[points.Count - 1] * mult) + (smasLong[Math.Max(points.Count - 1 - 1, 0)] * (1 - mult));

            smasLong.Add(ema);

            float shorti = 0;
            float longi = 0;

            if (smasShort.Count > 0)
                shorti = smasShort[Math.Min(points.Count - 1, smasShort.Count - 1)];

            if (smasLong.Count > 0)
                longi = smasLong[Math.Min(points.Count - 1, smasLong.Count - 1)];




            List<double> stdevVals = new List<double>();
            sma = 0;
            for (int j = 0; j < bolingerLength; j++)
            {
                sma += points[Math.Max(points.Count - 1 - j, 0)];
                stdevVals.Add((double)points[Math.Max(points.Count - 1 - j, 0)]);
            }

            sma /= bolingerLength;
            BBTop[BBTop.Count - 1] = sma + (float)SD.StandardDeviation(stdevVals) * 2;
            BBBot[BBBot.Count - 1] = sma - (float)SD.StandardDeviation(stdevVals) * 2;


            if (hodlMode)
                if (points[points.Count - 1] > boughtPrice * buyCutoffPct)
                    emaStates.Add(emaState.shortCrossedDown);

            if (points[points.Count - 1] < BBBot[points.Count - 1])
            {
                if (!hodlMode)
                {
                    if (!trackStates.Contains(TrackState.belowBB))
                        trackStates.Add(TrackState.belowBB);
                }
            }
            else
                if (trackStates.Contains(TrackState.belowBB))
                {
                    trackStates.Remove(TrackState.belowBB);
                    emaStates.Add(emaState.shortCrossedUp);
                }


            if (emaStates.Contains(emaState.shortCrossedUp))
            {
                emaStates.Remove(emaState.shortCrossedUp);

                if (!hodlMode)
                {

                    if (!states.Contains(tradeState.buy) &&
                       !states.Contains(tradeState.sell) &&
                       !states.Contains(tradeState.hodl))
                    {

                        boughtPrice = points[points.Count - 1];
                        hodlMode = true;
                        tradeCounter++;

                        states.Add(tradeState.buy);

                        if (tradeEnabled)
                            Buy(boughtPrice);
                    }
                }
            }

            if (emaStates.Contains(emaState.shortCrossedDown))
            {
                emaStates.Remove(emaState.shortCrossedDown);

                if (hodlMode)
                {
                    if (!states.Contains(tradeState.sell) &&
                        states.Contains(tradeState.hodl))
                    {
                        states.Add(tradeState.sell);
                        states.Remove(tradeState.hodl);

                        hodlMode = false;
                        if (tradeEnabled)
                            Sell();
                    }
                }

            }



            float pt = (shorti + longi) / 2f;
            if (pt < boughtPrice * sellCutoffPct && hodlMode)
            {
                if (!states.Contains(tradeState.sell) &&
                    states.Contains(tradeState.hodl))
                {
                    states.Add(tradeState.sell);
                    states.Remove(tradeState.hodl);

                    hodlMode = false;
                    if (tradeEnabled)
                        Sell();
                }
            }




            if (states.Contains(tradeState.buy))
            {
                //do buy
                strLog += "bought at " + asks + "\n";
                boughtAt = asks;

                states.Remove(tradeState.buy);
                states.Add(tradeState.hodl);
            }

            if (states.Contains(tradeState.sell))
            {
                //do sell
                strLog += "sold at " + bids + "\n";
                
                gain = (bids - boughtAt) - (bids * fee);
                gainPct = gain / bids;
                strLog += "gainPct: " + gainPct + "  gain: " + gain + "\n";

                lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();

                states.Remove(tradeState.sell);
            }

        }

        private void txtBuyThresh_TextChanged(object sender, EventArgs e)
        {
            float t = buyThresh;
            float.TryParse(txtBuyThresh.Text, out t);
            buyThresh = t;
            updateHodlSeries();
        }

        private void txtBuyThreshLong_TextChanged(object sender, EventArgs e)
        {
            float t = buyThreshLong;
            float.TryParse(txtBuyThreshLong.Text, out t);
            buyThreshLong = t;
            updateHodlSeries();
        }

        private void txtBuyTally_TextChanged(object sender, EventArgs e)
        {
            int t = buyTally;
            int.TryParse(txtBuyTally.Text, out t);
            buyTally = t;
            updateHodlSeries();
        }

        private void txtBuyLongTally_TextChanged(object sender, EventArgs e)
        {
            int t = buyLongTally;
            int.TryParse(txtBuyLongTally.Text, out t);
            buyLongTally = t;
            updateHodlSeries();
        }

        private void txtSellThresh_TextChanged(object sender, EventArgs e)
        {
            float t = sellThresh;
            float.TryParse(txtSellThresh.Text, out t);
            sellThresh = t;

            if (sellThresh > 0)
                sellThresh *= -1;

            updateHodlSeries();
        }

        private void txtSellThreshLong_TextChanged(object sender, EventArgs e)
        {
            float t = sellThreshLong;
            float.TryParse(txtSellThreshLong.Text, out t);
            sellThreshLong = t;

            if (sellThreshLong > 0)
                sellThreshLong *= -1;

            updateHodlSeries();
        }

        private void txtSellTally_TextChanged(object sender, EventArgs e)
        {
            int t = sellTally;
            int.TryParse(txtSellTally.Text, out t);
            sellTally = t;
            updateHodlSeries();
        }

        private void txtSellLongTally_TextChanged(object sender, EventArgs e)
        {
            int t = sellLongTally;
            int.TryParse(txtSellLongTally.Text, out t);
            sellLongTally = t;
            updateHodlSeries();
        }

        private void updateHodlSeries()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";
            hodlPoints.Clear();

            for (int i = 0; i < points.Count; i++)
            {
                float buyPctSum = 0;

                for (int j = 0; j < buyTally; j++)
                    buyPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                float buyLongPctSum = 0;

                for (int j = 0; j < buyLongTally; j++)
                    buyLongPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];


                if (buyPctSum > buyThresh && !hodlMode
                    && buyLongPctSum > buyThreshLong)
                {
                    boughtPrice = points[i];
                    hodlMode = true;
                    tradeCounter++;
                }

                float sellPctSum = 0;

                for (int j = 0; j < sellTally; j++)
                    sellPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                float sellLongPctSum = 0;

                for (int j = 0; j < sellLongTally; j++)
                    sellLongPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                if (sellPctSum < sellThresh && hodlMode
                    && sellLongPctSum > sellThreshLong)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (bids * 0.0025f);
                    gainPct += gain / boughtPrice;
                    Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (points[i] < boughtPrice * 0.99f && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (bids * 0.0025f);
                    gainPct += gain / boughtPrice;
                    Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            updateGraph();
        }

        private void updateHodlSeriesMA()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";
            hodlPoints.Clear();
            emaStates.Clear();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                if (emaShort[i] > 0 && emaLong[i] > 0)
                    divergence = emaShort[i] - emaLong[i];

                if (emaShort[i] > emaLong[i])
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }


                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax && divergence > 2)
                        emaStates.Add(emaState.shortCrossedUp);
                }

                if (emaShort[i] < emaLong[i])
                {
                    if (!emaStates.Contains(emaState.shortBelow))
                        emaStates.Add(emaState.shortBelow);

                    if (emaStates.Contains(emaState.shortAbove))
                    {
                        //emaStates.Add(emaState.shortCrossedDown);
                        emaStates.Remove(emaState.shortAbove);
                    }


                    if (divergence < divergenceMin)
                        divergenceMin = divergence;

                    if (divergence > divergenceMin)// && divergence < -4)
                        emaStates.Add(emaState.shortCrossedDown);
                }


                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                        tradeCounter++;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                        gainPct = gain / boughtPrice;
                        Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    }

                }

                if (points[i] < boughtPrice * 0.99f && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                    gainPct = gain / boughtPrice;
                    Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            updateGraph();
        }

        private void updateHodlSeriesSA()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";
            hodlPoints.Clear();
            emaStates.Clear();
            float priceCeiling = 0;
            float highestPriceCeiling = 0;
            float prevPrice = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                List<float> splitTicks = new List<float>();
                int splitLength = splitSpan;

                for (int j = 0; j < splitLength; j++)
                    if (emaShort.Count > 0 && emaLong.Count > 0)
                        splitTicks.Add(emaShort[Math.Min(emaShort.Count - 1, Math.Max(i - j, 0))] - emaLong[Math.Min(emaLong.Count - 1, Math.Max(i - j, 0))]);
                    else
                        splitTicks.Add(0);
                
                List<float> leftHalf = new List<float>();
                for (int j = splitLength * 3 / 4; j < splitLength; j++)
                    leftHalf.Add(splitTicks[j]);

                List<float> rightHalf = new List<float>();
                for (int j = 0; j < splitLength / 4; j++)
                    rightHalf.Add(splitTicks[j]);


                float leftAvg = leftHalf.Average();
                float rightAvg = rightHalf.Average();

                if (Math.Abs(leftAvg - rightAvg) < fit)
                {
                    if (leftAvg < 0 && rightAvg > 0 && !hodlMode)
                    {
                        emaStates.Add(emaState.shortCrossedUp);
                        priceCeiling = 0;
                        highestPriceCeiling = 0;
                        prevPrice = points[i];
                    }
                }

                if (emaShort[i] > 0 && emaLong[i] > 0)
                    divergence = emaShort[i] - emaLong[i];

                if (emaShort[i] > emaLong[i])
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }


                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax && divergence > 2)
                    {
                    }
                }

                if (hodlMode)
                {
                    float ceil = (points[i] - prevPrice);
                    prevPrice = points[i];

                    priceCeiling += ceil;

                    if (priceCeiling > highestPriceCeiling)
                        highestPriceCeiling = priceCeiling;

                    if (points[i] < boughtPrice + (highestPriceCeiling) && highestPriceCeiling > 10)
                        emaStates.Add(emaState.shortCrossedDown);
                }

                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                        tradeCounter++;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                        gainPct = gain / boughtPrice;
                        Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    }

                }

                if (points[i] < boughtPrice * 0.99f && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                    gainPct = gain / boughtPrice;
                    Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            updateGraph();
        }

        private void updateHodlSeriesDA()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            lblGain.Text = "0";
            hodlPoints.Clear();
            dips.Clear();
            dipPoints.Clear();
            lrPoints.Clear();
            BBTop.Clear();
            BBBot.Clear();
            emaStates.Clear();
            float priceCeiling = 0;
            float highestPriceCeiling = 0;
            float prevPrice = 0;

            smasShort.Clear();
            float smasShortLength = 130;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasShortLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasShortLength;

                float mult = 2 / (float)(smasShortLength + 1);

                float ema = points[i];

                if (smasShort.Count > 0)
                    ema = (points[i] * mult) + (smasShort[Math.Max(i - 1, 0)] * (1 - mult));
                    
                smasShort.Add(ema);
            }


            smasLong.Clear();
            float smasLongLength = 340;
            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasLongLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasLongLength;

                float mult = 2 / (float)(smasLongLength + 1);
                //mult =0.5f;

                float ema = points[i];

                if (smasLong.Count > 0)
                    ema = (points[i] * mult) + (smasLong[Math.Max(i - 1, 0)] * (1-mult));

                smasLong.Add(ema);
            }

            for (int i = 0; i < points.Count; i++)
            {
                int bolingerLength = 2000;
                List<double> stdevVals = new List<double>();
                float sma = 0;
                for (int j = 0; j < bolingerLength; j++)
                {
                    sma += points[Math.Max(i - j, 0)];
                    stdevVals.Add((double)points[Math.Max(i - j, 0)]);
                }

                sma /= bolingerLength;
                BBTop.Add(sma + (float)SD.StandardDeviation(stdevVals) * 2);
                BBBot.Add(sma - (float)SD.StandardDeviation(stdevVals) * 2);



                if (i == points.Count - 1)
                    i = i;

                float shorti = 0;
                float longi = 0;

                if (smasShort.Count > 0)
                    shorti = smasShort[Math.Min(i, smasShort.Count - 1)];

                if (smasLong.Count > 0)
                    longi = smasLong[Math.Min(i, smasLong.Count - 1)];
                
                List<float> splitTicks = new List<float>();
                int splitLength = splitSpan;

                for (int j = 0; j < splitLength; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        splitTicks.Add(smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] - smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))]);
                    else
                        splitTicks.Add(0);

                //foreach (float f in emaShort)
                //    Debug.Print(f.ToString("N10"));

                List<float> leftHalf = new List<float>();
                for (int j = splitLength * 2 / 3; j < splitLength; j++)
                    leftHalf.Add(splitTicks[j]);

                List<float> rightHalf = new List<float>();
                for (int j = 0; j < splitLength / 3; j++)
                    rightHalf.Add(splitTicks[j]);


                float leftAvg = leftHalf.Average();
                float rightAvg = rightHalf.Average();

                dipCooldown++;
                if (dipCooldown > splitSpan)
                    dipCooldown = splitSpan;

                float pt = (shorti + longi) / 2f;

                float buyPctSum = 0;

                for (int j = 0; j < 500; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        buyPctSum += smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] - smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))];
                

                dipPoints.Add(0);
                lrPoints.Add(0);

                if (rightAvg - leftAvg < fit)
                {
                    if (leftAvg < 0 && rightAvg > 0)
                    {
                        if (dipCooldown == splitSpan)
                        {
                            dips.Add((shorti + longi) / 2f);
                            dipCooldown = 0;

                            dipPoints[dipPoints.Count - 1] = points[i];

                            for (int j = 0; j < splitLength; j++)
                                if (j < splitLength / 3 || j > splitLength * 2 / 3)
                                    lrPoints[Math.Max(i - j, 0)] = (smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] + smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))])/2f;

                            float curDip = dips[dips.Count - 1];
                            float prevDip = dips[Math.Max(0, dips.Count - 2)];
                            float dipPct = (curDip - prevDip) / curDip;
                            


                            if (curDip > prevDip  && dipPct > 0.005f && !hodlMode)
                            {
                                if (buyPctSum > -100000)
                                {
                                    /*
                                    emaStates.Add(emaState.shortCrossedUp);
                                    priceCeiling = 0;
                                    highestPriceCeiling = 0;
                                    prevPrice = points[i];
                                    */
                                }

                            }
                        }

                    }
                }

                if(points[i] < BBBot[i])
                {
                    emaStates.Add(emaState.shortCrossedUp);
                    priceCeiling = 0;
                    highestPriceCeiling = 0;
                    prevPrice = points[i];
                }

                //if (points[i] > BBTop[i])
                if (hodlMode)
                {
                    emaStates.Add(emaState.shortCrossedDown);
                }

                if (shorti > 0 && longi > 0)
                    divergence = shorti - longi;

                if (shorti > longi)
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }


                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax && divergence > 2)
                    {
                    }
                }

                if (hodlMode)
                {
                    float ceil = (pt - prevPrice);
                    prevPrice = pt;

                    priceCeiling += ceil;

                    if (priceCeiling > highestPriceCeiling)
                        highestPriceCeiling = priceCeiling;

                    //if (pt < boughtPrice + (highestPriceCeiling / 2f) && highestPriceCeiling > boughtPrice * 0.005f)
                    //    emaStates.Add(emaState.shortCrossedDown);
                    //if (true)
                }

                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                        tradeCounter++;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                        gainPct = gain / boughtPrice;
                        //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    }

                }

                if (pt < boughtPrice * 0.98f && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                    gainPct = gain / boughtPrice;
                    //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            updateGraph();
        }

        private void updateHodlSeriesBBDip()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;
            int ticksUnderBB = 0;
            int underBBLimit = 0;

            if (!bulkFiles)
                lblGain.Text = "0";

            hodlPoints.Clear();
            dips.Clear();
            dipPoints.Clear();
            lrPoints.Clear();
            cutoffPoints.Clear();
            BBTop.Clear();
            BBBot.Clear();
            emaStates.Clear();
            trackStates.Clear();
            float priceCeiling = 0;
            float highestPriceCeiling = 0;
            float prevPrice = 0;

            smasShort.Clear();
            float smasShortLength = 130;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasShortLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasShortLength;

                float mult = 2 / (float)(smasShortLength + 1);

                float ema = points[i];

                if (smasShort.Count > 0)
                    ema = (points[i] * mult) + (smasShort[Math.Max(i - 1, 0)] * (1 - mult));

                smasShort.Add(ema);
            }


            smasLong.Clear();
            float smasLongLength = 340;
            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasLongLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasLongLength;

                float mult = 2 / (float)(smasLongLength + 1);
                //mult =0.5f;

                float ema = points[i];

                if (smasLong.Count > 0)
                    ema = (points[i] * mult) + (smasLong[Math.Max(i - 1, 0)] * (1 - mult));

                smasLong.Add(ema);
            }

            for (int i = 0; i < points.Count; i++)
            {
                List<double> stdevVals = new List<double>();
                float sma = 0;
                for (int j = 0; j < bolingerLength; j++)
                {
                    sma += points[Math.Max(i - j, 0)];
                    stdevVals.Add((double)points[Math.Max(i - j, 0)]);
                }

                sma /= bolingerLength;
                BBTop.Add(sma + (float)SD.StandardDeviation(stdevVals) * 2);
                BBBot.Add(sma - (float)SD.StandardDeviation(stdevVals) * 2);



                if (i == points.Count - 1)
                    i = i;

                float shorti = 0;
                float longi = 0;

                if (smasShort.Count > 0)
                    shorti = smasShort[Math.Min(i, smasShort.Count - 1)];

                if (smasLong.Count > 0)
                    longi = smasLong[Math.Min(i, smasLong.Count - 1)];

                List<float> splitTicks = new List<float>();
                int splitLength = splitSpan;

                for (int j = 0; j < splitLength; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        splitTicks.Add(smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] - smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))]);
                    else
                        splitTicks.Add(0);

                //foreach (float f in emaShort)
                //    Debug.Print(f.ToString("N10"));

                List<float> leftHalf = new List<float>();
                for (int j = splitLength * 2 / 3; j < splitLength; j++)
                    leftHalf.Add(splitTicks[j]);

                List<float> rightHalf = new List<float>();
                for (int j = 0; j < splitLength / 3; j++)
                    rightHalf.Add(splitTicks[j]);


                float leftAvg = leftHalf.Average();
                float rightAvg = rightHalf.Average();

                dipCooldown++;
                if (dipCooldown > splitSpan)
                    dipCooldown = splitSpan;

                float pt = (shorti + longi) / 2f;

                float buyPctSum = 0;

                for (int j = 0; j < 10; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        buyPctSum += (pt - (smasShort[Math.Min(Math.Max(i - j, 0), smasShort.Count - 1)] + smasLong[Math.Min(Math.Max(i - j, 0), smasLong.Count - 1)])/2f)/pt;
                


                
                dipPoints.Add(0);
                lrPoints.Add(0);
                cutoffPoints.Add(0);

                if (rightAvg - leftAvg < fit)
                {
                    if (leftAvg < 0 && rightAvg > 0)
                    {
                        if (dipCooldown == splitSpan)
                        {
                            dips.Add((shorti + longi) / 2f);
                            dipCooldown = 0;

                            dipPoints[dipPoints.Count - 1] = points[i];

                            for (int j = 0; j < splitLength; j++)
                                if (j < splitLength / 3 || j > splitLength * 2 / 3)
                                    lrPoints[Math.Max(i - j, 0)] = (smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] + smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))]) / 2f;

                            float curDip = dips[dips.Count - 1];
                            float prevDip = dips[Math.Max(0, dips.Count - 2)];
                            float dipPct = (curDip - prevDip) / curDip;



                            //if (curDip > prevDip && dipPct > 0.005f && !hodlMode)
                            if (!hodlMode)
                            {
                                if (buyPctSum > -100000)
                                {
                                    if (trackStates.Contains(TrackState.trackingDip))
                                    {
                                        trackStates.Remove(TrackState.trackingDip);
                                        trackStates.Remove(TrackState.aboveBB);
                                        emaStates.Add(emaState.shortCrossedUp);
                                        priceCeiling = points[i] * 0.96f;
                                        highestPriceCeiling = 0;
                                        prevPrice = points[i];
                                    }
                                }

                            }
                        }

                    }
                }

                if (points[i] > BBTop[i])
                {
                    /*
                    if (!hodlMode)
                        if (!trackStates.Contains(TrackState.aboveBB))
                            trackStates.Add(TrackState.aboveBB);
                            */
                }
                else if (points[i] < BBBot[i])
                {

                    if (!hodlMode)
                    {
                        ticksUnderBB++;
                        if (!trackStates.Contains(TrackState.belowBB))
                            trackStates.Add(TrackState.belowBB);
                    }
                            
                }
                else
                {
                    if (trackStates.Contains(TrackState.aboveBB))
                        if (!trackStates.Contains(TrackState.trackingDip))
                        {
                            trackStates.Add(TrackState.trackingDip);
                        }



                    if (trackStates.Contains(TrackState.belowBB) && ticksUnderBB > underBBLimit)
                    {
                        trackStates.Remove(TrackState.belowBB);
                        emaStates.Add(emaState.shortCrossedUp);
                        priceCeiling = points[i] * 0.96f;
                        highestPriceCeiling = 0;
                        prevPrice = points[i];
                        Debug.WriteLine("time under BB: " + ticksUnderBB);
                        Debug.WriteLine("buyPctSum: " + buyPctSum);
                    }

                    ticksUnderBB = 0;

                }

                if (hodlMode)
                    if (buyPctSum < highestPriceCeiling)
                        highestPriceCeiling = buyPctSum;

                //if (points[i] > BBTop[i])
                //if (hodlMode)
                //    emaStates.Add(emaState.shortCrossedDown);

                if (shorti > 0 && longi > 0)
                    divergence = shorti - longi;

                if (shorti > longi)
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }


                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax && divergence > 2)
                    {
                    }
                }

                if (hodlMode)
                {
                    /*
                    float ceil = (pt - prevPrice);
                    prevPrice = pt;

                    priceCeiling += ceil;

                    if (priceCeiling > highestPriceCeiling)
                        highestPriceCeiling = priceCeiling;

                    float cutPrice = boughtPrice + (highestPriceCeiling / 2f);

                    cutoffPoints[i] = cutPrice;
                    */

                    float drop = points[i] - points[i - 1];
                    
                    if(drop < 0)
                        priceCeiling -= drop/2f;
                    //else
                    //    priceCeiling += drop/2f;

                    float cutPrice =  priceCeiling;

                    //cutoffPoints[i] = cutPrice;

                    //if (pt < cutPrice)
                    //    emaStates.Add(emaState.shortCrossedDown);

                    if (points[i] > boughtPrice * buyCutoffPct)
                        emaStates.Add(emaState.shortCrossedDown);

                    //if (true)  cutoffPoints
                }

                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                        tradeCounter++;
                        highestPriceCeiling = 0;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * fee);
                        gainPct = gain / boughtPrice;
                        //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                        Debug.WriteLine("gain: " + gainPct);
                        Debug.WriteLine("drop speed: " + highestPriceCeiling);
                    }

                }

                if (pt < boughtPrice * sellCutoffPct && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * fee);
                    gainPct = gain / boughtPrice;
                    //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    Debug.WriteLine("cutoff loss: " + gainPct);
                    Debug.WriteLine("drop speed: " + highestPriceCeiling);
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            Debug.WriteLine("total: " + lblGain.Text);

            updateGraph();
        }

        private void updateHodlSeriesMADip()
        {
            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;
            int tradeCounter = 0;

            if (!bulkFiles)
                lblGain.Text = "0";

            hodlPoints.Clear();
            dips.Clear();
            dipPoints.Clear();
            lrPoints.Clear();
            cutoffPoints.Clear();

            emaStates.Clear();
            trackStates.Clear();
            float priceCeiling = 0;
            float highestPriceCeiling = 0;
            float prevPrice = 0;

            smasShort.Clear();
            float smasShortLength = 130;

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasShortLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasShortLength;

                float mult = 2 / (float)(smasShortLength + 1);

                float ema = points[i];

                if (smasShort.Count > 0)
                    ema = (points[i] * mult) + (smasShort[Math.Max(i - 1, 0)] * (1 - mult));

                smasShort.Add(ema);
            }


            smasLong.Clear();
            float smasLongLength = 340;
            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float divisor = 1;
                float sma = 0;
                for (int j = 0; j < smasLongLength; j++)
                    sma += points[Math.Max(i - j, 0)];

                sma /= smasLongLength;

                float mult = 2 / (float)(smasLongLength + 1);
                //mult =0.5f;

                float ema = points[i];

                if (smasLong.Count > 0)
                    ema = (points[i] * mult) + (smasLong[Math.Max(i - 1, 0)] * (1 - mult));

                smasLong.Add(ema);
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1)
                    i = i;

                float shorti = 0;
                float longi = 0;

                if (smasShort.Count > 0)
                    shorti = smasShort[Math.Min(i, smasShort.Count - 1)];

                if (smasLong.Count > 0)
                    longi = smasLong[Math.Min(i, smasLong.Count - 1)];

                List<float> splitTicks = new List<float>();
                int splitLength = splitSpan;

                for (int j = 0; j < splitLength; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        splitTicks.Add(smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] - smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))]);
                    else
                        splitTicks.Add(0);

                //foreach (float f in emaShort)
                //    Debug.Print(f.ToString("N10"));

                List<float> leftHalf = new List<float>();
                for (int j = splitLength * 2 / 3; j < splitLength; j++)
                    leftHalf.Add(splitTicks[j]);

                List<float> rightHalf = new List<float>();
                for (int j = 0; j < splitLength / 3; j++)
                    rightHalf.Add(splitTicks[j]);


                float leftAvg = leftHalf.Average();
                float rightAvg = rightHalf.Average();

                dipCooldown++;
                if (dipCooldown > splitSpan)
                    dipCooldown = splitSpan;

                float pt = (shorti + longi) / 2f;

                float buyPctSum = 0;

                for (int j = 0; j < 10; j++)
                    if (smasShort.Count > 0 && smasLong.Count > 0)
                        buyPctSum += (pt - (smasShort[Math.Min(Math.Max(i - j, 0), smasShort.Count - 1)] + smasLong[Math.Min(Math.Max(i - j, 0), smasLong.Count - 1)]) / 2f) / pt;




                dipPoints.Add(0);
                lrPoints.Add(0);
                cutoffPoints.Add(0);

                if (rightAvg - leftAvg < fit)
                {
                    if (leftAvg < 0 && rightAvg > 0)
                    {
                        if (dipCooldown == splitSpan)
                        {
                            dips.Add((shorti + longi) / 2f);
                            dipCooldown = 0;

                            dipPoints[dipPoints.Count - 1] = points[i];

                            for (int j = 0; j < splitLength; j++)
                                if (j < splitLength / 3 || j > splitLength * 2 / 3)
                                    lrPoints[Math.Max(i - j, 0)] = (smasShort[Math.Min(smasShort.Count - 1, Math.Max(i - j, 0))] + smasLong[Math.Min(smasLong.Count - 1, Math.Max(i - j, 0))]) / 2f;

                            float curDip = dips[dips.Count - 1];
                            float prevDip = dips[Math.Max(0, dips.Count - 2)];
                            float dipPct = (curDip - prevDip) / curDip;



                            //if (curDip > prevDip && dipPct > 0.005f && !hodlMode)
                            if (!hodlMode)
                            {
                                if (buyPctSum > -100000)
                                {
                                    if (trackStates.Contains(TrackState.trackingDip))
                                    {
                                        trackStates.Remove(TrackState.trackingDip);
                                        trackStates.Remove(TrackState.aboveBB);
                                        emaStates.Add(emaState.shortCrossedUp);
                                        priceCeiling = points[i] * 0.96f;
                                        highestPriceCeiling = 0;
                                        prevPrice = points[i];
                                    }
                                }

                            }
                        }

                    }
                }



                if (hodlMode)
                    if (buyPctSum < highestPriceCeiling)
                        highestPriceCeiling = buyPctSum;

                //if (points[i] > BBTop[i])
                //if (hodlMode)
                //    emaStates.Add(emaState.shortCrossedDown);

                if (shorti > 0 && longi > 0)
                    divergence = shorti - longi;

                if (shorti > longi)
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }


                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax && divergence > 2)
                    {
                    }
                }

                if (hodlMode)
                {
                    /*
                    float ceil = (pt - prevPrice);
                    prevPrice = pt;

                    priceCeiling += ceil;

                    if (priceCeiling > highestPriceCeiling)
                        highestPriceCeiling = priceCeiling;

                    float cutPrice = boughtPrice + (highestPriceCeiling / 2f);

                    cutoffPoints[i] = cutPrice;
                    */

                    float drop = points[i] - points[i - 1];

                    if (drop < 0)
                        priceCeiling -= drop / 2f;
                    //else
                    //    priceCeiling += drop/2f;

                    float cutPrice = priceCeiling;

                    //cutoffPoints[i] = cutPrice;

                    //if (pt < cutPrice)
                    //    emaStates.Add(emaState.shortCrossedDown);

                    if (points[i] > boughtPrice * buyCutoffPct)
                        emaStates.Add(emaState.shortCrossedDown);

                    //if (true)  cutoffPoints
                }

                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                        tradeCounter++;
                        highestPriceCeiling = 0;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * fee);
                        gainPct = gain / boughtPrice;
                        //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                        Debug.WriteLine("gain: " + gainPct);
                        Debug.WriteLine("drop speed: " + highestPriceCeiling);
                    }

                }

                if (pt < boughtPrice * sellCutoffPct && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * fee);
                    gainPct = gain / boughtPrice;
                    //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    Debug.WriteLine("cutoff loss: " + gainPct);
                    Debug.WriteLine("drop speed: " + highestPriceCeiling);
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            txtZoom.Text = tradeCounter.ToString();

            Debug.WriteLine("total: " + lblGain.Text);

            updateGraph();
        }

        private void Test()
        {
            testResult test;

            test.buyTally = 1 + r.Next(100);
            test.sellTally = 1 + r.Next(100);
            test.buyThresh = 0.01f * r.Next(100);
            test.sellThresh = 0.01f * r.Next(100) * -1;

            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;

            hodlPoints.Clear();

            for (int i = 0; i < points.Count; i++)
            {
                float buyPctSum = 0;

                for (int j = 0; j < test.buyTally; j++)
                    buyPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                float buyLongPctSum = 0;

                for (int j = 0; j < buyLongTally; j++)
                    buyLongPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];


                if (buyPctSum > test.buyThresh && !hodlMode
                    && buyLongPctSum > test.buyThresh)
                {
                    boughtPrice = points[i];
                    hodlMode = true;
                }

                float sellPctSum = 0;

                for (int j = 0; j < test.sellTally; j++)
                    sellPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                float sellLongPctSum = 0;

                for (int j = 0; j < sellLongTally; j++)
                    sellLongPctSum += pctTallyHodlMode[Math.Max(i - j, 0)];

                if (sellPctSum < test.sellThresh && hodlMode
                    && sellLongPctSum > test.sellThresh)
                {
                    hodlMode = false;
                    gain += (points[i] - boughtPrice) - (bids * 0.0025f);
                    gainPct += gain / boughtPrice;

                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (points[i] < boughtPrice * 0.99f)
                {
                    hodlMode = false;
                    gain += (points[i] - boughtPrice) - (bids * 0.0025f);
                    gainPct += gain / boughtPrice;

                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }

            test.gain = gain;
            test.gainPct = gainPct;

            if (test.gain > highestGain)
            {
                highestGain = test.gain;

                txtBuyTally.Text = test.buyTally.ToString();
                txtSellTally.Text = test.sellTally.ToString();
                txtBuyThresh.Text = test.buyThresh.ToString();
                txtSellThresh.Text = test.sellThresh.ToString();
            }

            //results.Add(test);

            strLog = "";
            strLog += test.buyThresh.ToString("N10") + ",," + test.buyTally.ToString("N10") + ",," + test.sellThresh.ToString("N10") + ",," + test.sellTally.ToString("N10") + ",," + test.gain.ToString("N10") + ",," + test.gainPct.ToString("N10") + "\n";

            File.AppendAllText(logPath.Replace(".csv", "_Test.csv"), strLog);

            testNum++;
            txtZoom.Text = testNum.ToString();
        }

        private void TestMA()
        {
            testResultMA test;

            test.emaShort = 1 + r.Next(100) * 10;
            test.emaLong = 1 + r.Next(100) * 10;

            bool hodlMode = false;
            float boughtPrice = 0;
            float gain = 0;
            float gainPct = 0;

            //hodlPoints.Clear();
            emaStates.Clear();

            lblGain.Text = "0";
            emaShortLength = test.emaShort;
            emaLongLength = test.emaLong;

            updateGraph();

            for (int i = 0; i < points.Count; i++)
            {
                divergence = emaShort[i] - emaLong[i];

                if (emaShort[i] > emaLong[i])
                {
                    if (!emaStates.Contains(emaState.shortAbove))
                        emaStates.Add(emaState.shortAbove);

                    if (emaStates.Contains(emaState.shortBelow))
                    {
                        //emaStates.Add(emaState.shortCrossedUp);
                        emaStates.Remove(emaState.shortBelow);
                    }

                    if (divergence > divergenceMax)
                        divergenceMax = divergence;

                    if (divergence < divergenceMax)
                        emaStates.Add(emaState.shortCrossedUp);
                }

                if (emaShort[i] < emaLong[i])
                {
                    if (!emaStates.Contains(emaState.shortBelow))
                        emaStates.Add(emaState.shortBelow);

                    if (emaStates.Contains(emaState.shortAbove))
                    {
                        //emaStates.Add(emaState.shortCrossedDown);
                        emaStates.Remove(emaState.shortAbove);
                    }

                    if (divergence < divergenceMin)
                        divergenceMin = divergence;

                    if (divergence > divergenceMax)
                        emaStates.Add(emaState.shortCrossedDown);
                }



                divergencePrev = divergence;


                if (emaStates.Contains(emaState.shortCrossedUp))
                {
                    emaStates.Remove(emaState.shortCrossedUp);

                    if (!hodlMode)
                    {
                        boughtPrice = points[i];
                        hodlMode = true;
                    }
                }

                if (emaStates.Contains(emaState.shortCrossedDown))
                {
                    emaStates.Remove(emaState.shortCrossedDown);

                    if (hodlMode)
                    {
                        hodlMode = false;
                        gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                        gainPct = gain / boughtPrice;
                        //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                        lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                    }

                }

                if (points[i] < boughtPrice * 0.99f && hodlMode)
                {
                    hodlMode = false;
                    gain = (points[i] - boughtPrice) - (points[i] * 0.0025f);
                    gainPct = gain / boughtPrice;
                    //Debug.WriteLine(boughtPrice + "/" + points[i] + " = " + gain);
                    lblGain.Text = (float.Parse(lblGain.Text) + gainPct).ToString();
                }

                if (hodlMode)
                    hodlPoints.Add(points[i]);
                else
                    hodlPoints.Add(0);

            }


            //test.gain = gain;
            //test.gainPct = gainPct;

            if (float.Parse(lblGain.Text) > highestGain)
            {
                highestGain = float.Parse(lblGain.Text);

                txtShort.Text = test.emaShort.ToString();
                txtLong.Text = test.emaLong.ToString();
            }

            //results.Add(test);

            strLog = "";
            //strLog += test.emaShort.ToString() + ",," + test.emaLong.ToString() + ",," + test.gain.ToString("N10") + ",," + test.gainPct.ToString("N10") + "\n";

            //File.AppendAllText(logPath.Replace(".csv", "_Test.csv"), strLog);

            testNum++;
            txtZoom.Text = testNum.ToString();
        }

        private void lbLoadFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbLoadFile.SelectedItems.Count > 0)
            {
                loadChart(lbLoadFile.SelectedItems[0].ToString());

                chartStartIdx = 0;
                chartEndIdx = points.Count;

                txtChartStart.Text = chartStartIdx.ToString();
                txtChartEnd.Text = chartEndIdx.ToString();

                updateHodlSeriesMADip();
            }
        }

        private void txtChartStart_TextChanged(object sender, EventArgs e)
        {
            if (lbLoadFile.SelectedItems.Count > 0)
            {
                chartStartIdx = int.Parse(txtChartStart.Text);

                List<float> tempPoints = new List<float>();

                foreach (float f in points)
                    tempPoints.Add(f);

                points.Clear();

                for (int i = chartStartIdx; i < chartEndIdx - chartStartIdx; i++)
                    points.Add(tempPoints[i]);

                updateHodlSeriesMADip();
            }
        }

        private void txtChartEnd_TextChanged(object sender, EventArgs e)
        {
            if (lbLoadFile.SelectedItems.Count > 0)
            {
                loadChart(lbLoadFile.SelectedItems[0].ToString());

                chartEndIdx = int.Parse(txtChartEnd.Text);

                List<float> tempPoints = new List<float>();

                foreach (float f in points)
                    tempPoints.Add(f);

                points.Clear();

                for (int i = chartStartIdx; i < chartEndIdx; i++)
                    points.Add(tempPoints[i]);

                updateHodlSeriesMADip();
            }
        }

        private void txtShort_TextChanged(object sender, EventArgs e)
        {
            emaShortLength = int.Parse(txtShort.Text);

            updateHodlSeriesMADip();
        }

        private void txtLong_TextChanged(object sender, EventArgs e)
        {
            emaLongLength = int.Parse(txtLong.Text);

            updateHodlSeriesMADip();
        }

        private void txtSpan_TextChanged(object sender, EventArgs e)
        {
            splitSpan = int.Parse(txtSpan.Text);

            updateHodlSeriesMADip();
        }

        private void txtFit_TextChanged(object sender, EventArgs e)
        {
            fit = float.Parse(txtFit.Text);

            updateHodlSeriesMADip();
        }

    }

    public static class SD
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
