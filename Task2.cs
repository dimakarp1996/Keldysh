using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    class Task2 : ITaskBase
    {
        public Task2()
        {
            InitialData = new DataItemCollection();
        }


        private int m,n;
        private double x0, X;
        private SimplePlotResult m_results = null;
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

            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "число узлов", "число узлов", "", false, null);
            InitialData.AddSimpleProperty<int>("m", (int)rwInitials["m"], "число членов", "число членов", "", false, null);
            InitialData.AddSimpleProperty<double>("x0", (double)rwInitials["x0"], "x0", "начальная точка", "", false, null);
            InitialData.AddSimpleProperty<double>("X", (double)rwInitials["X"], "X", "конечная", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            m = (int)InitialData["m"];
            n = (int)InitialData["n"];
            x0 = (double)InitialData["x0"];
            X = (double)InitialData["X"];
            if(m<0)
            {
                m = 0;
            }

            LineSeries ser = new LineSeries();
            ser.Title = "Taylor";
            LineSeries ser1 = new LineSeries();
            ser1.Title =  "Exp";
            double j;
            double exp= 2.71828182845904523536;
            double[] a=new double[9];
            double one = 1;
            a[0] = one;
            a[1] = one/2;
            a[2] = one / 6;
            a[3] = one / 24;
            a[4] = one / 120;
            a[5] = one / 720;
            a[6] = one / 5040;
            a[7] = one / 40320;
            a[8] = one / 362880;

            for (int i = 0; i < n; i++)
            {
                double x = x0 + i * (X - x0) / (n - 1);
                double y=0;
                for (j = 0.5; j <= (double)m/2; j+=0.5)
                {
               
                    if (j >= 5)
                    {
                        break;
                    }
                    y += a[(int)(2*j-1)] * Math.Pow(x, j);
                    
                }
                ser.Points.Add(new OxyPlot.DataPoint(x, y));
                double z = Math.Pow(x, 0.5);
                ser1.Points.Add(new OxyPlot.DataPoint(x, (Math.Pow(exp, z) - 1)));
            }

            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser);
            ListBasePlotSeries plot_ser1 = new ListBasePlotSeries("x", "-", "y", "-", ser1);

            /* LineSeries ser1 = new LineSeries();

             ser1.Title = "Exp";

             for (int i = 0; i < 100; i++)
             {
                 double x = 0.01 * i;
                 ser1.Points.Add(new OxyPlot.DataPoint(x, Math.Pow(exp, x)));
             }

             ListBasePlotSeries plot_ser1 = new ListBasePlotSeries("x", "-", "y", "-", ser1);*/

            m_results = new SimplePlotResult();
            //m1_results = new SimplePlotResult();
            //m1_results.AddSeries(plot_ser1);
            m_results.AddSeries(plot_ser);
            m_results.AddSeries(plot_ser1);
        }

        public IPlotResult PlotResult
        {
            get { return m_results;
            }
        }
        public IPlotResult PlotResult1
        {
            get
            {
                return m_results;
            }
        }
        public string Name
        {
            get { return "Task2"; }
        }

        public string Description
        {
            get { return "Экспонента и Тейлор"; }
        }

        public ITaskBase CreateInstance()
        {
            return new Task2();
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
