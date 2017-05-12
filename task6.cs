using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    class task6 : ITaskBase
    {
        public task6()
        {
            InitialData = new DataItemCollection();
        }

        private SimplePlotResult m_results = null;

        private Double x0, h;
        private int n = 0;

        private List<Double> _x = new List<Double>();
        private List<Double> _y = new List<Double>();

        #region ITaskBase Members

        public eResultType ResultType
        {
            get { return eResultType.plotted; }
        }

        public void Form(System.Data.DataRow rwInitials)
        {
            InitialData.Clear();

            InitialData.AddSimpleProperty<double>("x0", (Double)rwInitials["x0"], "левая граница", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "число узлов", "число узлов", "", false, null);
            InitialData.AddSimpleProperty<double>("h", (Double)rwInitials["h"], "шаг", "шаг", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            x0 = (double)InitialData["x0"];
            h = (double)InitialData["h"];
            n = (int)InitialData["n"];

            LineSeries ser = new LineSeries();
            ser.Title = this.Name + " Anal";
            LineSeries ser1 = new LineSeries();
            ser1.Title = this.Name + "Rung";
            LineSeries ser2 = new LineSeries();
            ser2.Title = this.Name + "Eul";
            LineSeries ser3 = new LineSeries();
            ser3.Title = this.Name + "EulKosh";
            double exp = 2.71828182845904523536;
            double[] x=new double[n];
            double[] yanal = new double[n];
            double[] yrung = new double[n];
            double[] yeul = new double[n];
            double[] yeulkosh = new double[n];
            x[0] = 0;
            yanal[0] = 1;
            yrung[0] = 1;
            yeul[0] = 1;
            yeulkosh[0] = 1;
            ser.Points.Add(new OxyPlot.DataPoint(x[0], yanal[0]));
            ser1.Points.Add(new OxyPlot.DataPoint(x[0], yrung[0]));
            ser2.Points.Add(new OxyPlot.DataPoint(x[0], yeul[0]));
            ser3.Points.Add(new OxyPlot.DataPoint(x[0], yeulkosh[0]));
            for (int i = 1; i <n; i++)
            {
                x[i] = x0 + i *h;
                yanal[i]= 2*Math.Pow(exp,x[i]) - x[i] -1;
                yeul[i] = yeul[i - 1] + (yeul[i - 1] + x[i - 1]) * h;
                yeulkosh[i] = yeulkosh[i - 1] + (h / 2) * (x[i - 1] + yeulkosh[i - 1] + x[i] + yeul[i]);
                double yrungprom = yrung[i - 1] + (h / 2) * (yrung[i - 1] + x[i - 1]);
                yrung[i] = yrung[i - 1] + h * (x[i - 1] + h / 2 + yrungprom);
                ser.Points.Add(new OxyPlot.DataPoint(x[i], yanal[i]));
                ser1.Points.Add(new OxyPlot.DataPoint(x[i], yrung[i]));
                ser2.Points.Add(new OxyPlot.DataPoint(x[i], yeul[i]));
                ser3.Points.Add(new OxyPlot.DataPoint(x[i], yeulkosh[i]));
            }

            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser);
            ListBasePlotSeries plot_ser1 = new ListBasePlotSeries("x", "-", "y", "-", ser1);
            ListBasePlotSeries plot_ser2 = new ListBasePlotSeries("x", "-", "y", "-", ser2);
            ListBasePlotSeries plot_ser3 = new ListBasePlotSeries("x", "-", "y", "-", ser3);
            m_results = new SimplePlotResult();
            m_results.AddSeries(plot_ser);
            m_results.AddSeries(plot_ser1);
            m_results.AddSeries(plot_ser2);
            m_results.AddSeries(plot_ser3);
        }

        public IPlotResult PlotResult
        {
            get { return m_results; }
        }
        public IPlotResult PlotResult1
        {
            get { return m_results; }
        }
        public string Name
        {
            get { return "Task6"; }
        }

        public string Description
        {
            get { return "Численное интегрирование"; }
        }

        public ITaskBase CreateInstance()
        {
            return new task6();
        }

        public double ComputationDuration
        {
            get { return 0.0; }
        }

        public NetExtTools.Data.DataItemCollection InitialData
        {
            get;
            private set;
        }

        #endregion
       
    }
}
