using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace VISAP商科应用
{
    public class QuickPlot
    {
        public static void QPlot(DataGridView dataGridView1, Chart chart_basic, int xCol, int yCol, TextBox ChooseColor,string type, string Legend = "")
        {
            //如果xCol = -1，则认为x轴没有数值
            //yCol必须存在
            Series series = new Series();
            series.BorderWidth = 3;
            series.MarkerSize = 6;
            series.MarkerStyle = MarkerStyle.Circle;
            if (Legend != ""){
                series.LegendText = Legend;
            }
            if (type == "散点图")
            {
                series.ChartType = SeriesChartType.Point;
            }
            else if (type == "折线图")
            {
                series.ChartType = SeriesChartType.Line;
            }
            else if (type == "箱线图")
            {
                series.ChartType = SeriesChartType.BoxPlot;
            }
            if (ChooseColor.Text != "随机颜色")
            {
                series.Color = ChooseColor.BackColor;
            }
            double TempX = 0;
            double TempY = 0;
            //TempX和TempY用于录入数据
            double MaxXValue = 0;
            double MinXValue = 0;
            //MaxXValue和MinXValue用于找出X的最大和最小值，以供调整间隔使用
            int ReadDataCounts = 0;
            //ReadDataCounts用于计数
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[yCol].Value.ToString() != "")
                {
                    if (xCol != -1){
                        if (dataGridView1.Rows[i].Cells[xCol].Value.ToString() != "")
                        {
                            TempX = Convert.ToDouble(dataGridView1.Rows[i].Cells[xCol].Value.ToString());
                            TempY = Convert.ToDouble(dataGridView1.Rows[i].Cells[yCol].Value.ToString());
                            series.Points.AddXY(TempX,TempY);
                            if (ReadDataCounts == 0)
                            {
                                MaxXValue = TempX;
                                MinXValue = TempX;
                            }
                            else
                            {
                                if (MaxXValue < TempX)
                                {
                                    MaxXValue = TempX;
                                }
                                if (MinXValue > TempX)
                                {
                                    MinXValue = TempX;
                                }
                            }
                            ReadDataCounts++;
                        }
                    }
                    else
                    {
                        TempX = ReadDataCounts;
                        TempY = Convert.ToDouble(dataGridView1.Rows[i].Cells[yCol].Value.ToString());
                        series.Points.AddY(TempY);
                        if (ReadDataCounts == 0)
                        {
                            MaxXValue = TempX;
                            MinXValue = TempX;
                        }
                        else
                        {
                            if (MaxXValue < TempX)
                            {
                                MaxXValue = TempX;
                            }
                            if (MinXValue > TempX)
                            {
                                MinXValue = TempX;
                            }
                        }
                        ReadDataCounts++;
                    }
                }
            }
            if (ReadDataCounts > 0)
            {
                char[] separator = { ',' };

                string[] MinAndMax = RegulateAll(MinXValue, MaxXValue, 6).Split(separator);
                chart_basic.Series.Add(series);
                var XAxis = chart_basic.ChartAreas[0].AxisX;
                XAxis.Maximum = Convert.ToDouble(MinAndMax[1]);
                XAxis.Minimum = Convert.ToDouble(MinAndMax[0]);
                XAxis.MajorGrid.Enabled = false;
                var YAxis = chart_basic.ChartAreas[0].AxisY;
                YAxis.MajorGrid.Enabled = false;
            }
        }
        static string RegulateAll(double dMin, double dMax, int iMaxAxisNum)
        {
            //用于调整X轴间隔
            if (iMaxAxisNum < 1 || dMax < dMin)
                return "NA";

            double dDelta = dMax - dMin;
            if (dDelta < 1.0) //Modify this by your requirement.
            {
                dMax += (1.0 - dDelta) / 2.0;
                dMin -= (1.0 - dDelta) / 2.0;
            }
            dDelta = dMax - dMin;

            int iExp = (int)(Math.Log(dDelta) / Math.Log(10.0)) - 2;
            double dMultiplier = Math.Pow(10, iExp);
            double[] dSolutions = new double[] { 1, 2, 2.5, 5, 10, 20, 25, 50, 100, 200, 250, 500 };
            int i;
            for (i = 0; i < dSolutions.Length; i++)
            {
                double dMultiCal = dMultiplier * dSolutions[i];
                if (((int)(dDelta / dMultiCal) + 1) <= iMaxAxisNum)
                {
                    break;
                }
            }

            double dInterval = dMultiplier * dSolutions[i];

            double dStartPoint = ((int)Math.Ceiling(dMin / dInterval) - 1) * dInterval;
            int iAxisIndex;
            double dEndPoint = 0;
            for (iAxisIndex = 0; ; iAxisIndex++)
            {
                if (dStartPoint + dInterval * iAxisIndex > dMax)
                {
                    dEndPoint = dStartPoint + dInterval * Convert.ToDouble(iAxisIndex);
                    break;
                }
            }
            return dStartPoint.ToString() + "," + dEndPoint.ToString();
        }
    }
}
