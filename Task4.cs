using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    class Task4 : ITaskBase
    {
        public Task4()
        {
            InitialData = new DataItemCollection();
        }

        private SimplePlotResult m_results = null;

        private Double R1, R2,T1,T2;
        private int _n = 0;

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
            InitialData.AddSimpleProperty<double>("x1", (Double)rwInitials["x1"], "правая граница", "целочисленное значение", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "количество узлов", "количество узлов", "", false, null);
            InitialData.AddSimpleProperty<double>("T1", (double)rwInitials["T1"], "внутренняя температура", "внутренняя температура", "", false, null);

            InitialData.AddSimpleProperty<double>("T2", (double)rwInitials["T2"], "конечная температура", "конечная температура", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            R1 = (double)InitialData["x0"];//R1
            R2 = (double)InitialData["x1"];//R2
            _n = (int)InitialData["n"];//число узлов
            T1=(double)InitialData["T1"];
            T2=(double)InitialData["T2"];
            int i;
            LineSeries ser = new LineSeries();
            ser.Title = this.Name + "Chisl";
            LineSeries ser1 = new LineSeries();
            ser1.Title = this.Name + "Anal";
            double[] x = new double [_n+1];

            for( i = 0;i<_n+1;i++)
            {
             x[i] = R1 + i * (R2 - R1) / (_n);
            }
        
            double[] A = new double[_n+1];//from 0 to N
            double[] B = new double [_n+1];
            double[] C = new double [_n+1];
            double[] alpha = new double[_n+1];
            double[] beta = new double[_n+1];
            double[] Gran = new double[_n+1];
            double[] T = new double[_n+1];
            A[0] = 0;
            C[0] = 0;
            B[0] = 1;
            A[_n] = 0;
            C[_n] = 0;
            B[_n] = 1;
            Gran[0] = T1;
            Gran[_n] = T2;
            T[0] = T1;
           T[_n] = T2;
            for (i = 1; i < _n; i++)
            {
                Gran[i] = 0;
                A[i] = ((_n-1) / (R2 - R1)) - 1 / (x[i]);
                B[i] = -2 * (_n-1) / (R2 - R1);
                C[i] = ((_n-1) / (R2 - R1) )+ 1 / (x[i]);//условия на концах массива- подробнее
            }
            alpha[1] = 0;//T[i]=alpha[i+1]*T[i+1]+beta[i+1], то есть i+1 от 0 до _n, определяем альфу начиная с 1го элемента, нулевой элемент массива не используется и не инициализируется, хотя так писать плохо 
            beta[1] = T1;
            for (i = 1; i < _n; i++)
            {//условия на концах массива- подробнее
                alpha[i + 1] = -C[i] / (A[i] * alpha[i] + B[i]);
                beta[i + 1] = (Gran[i]-A[i] * beta[i] )/ (A[i] * alpha[i] + B[i]);
            }

            /* for (i =1;i<_n;i++)
             {
                 T[i] = (Gran[i] - A[i] * beta[i]) / (C[i] + A[i] * alpha[i]);
             }*/
            for (i = _n-1; i >0; i--)
            {
                T[i] = alpha[i + 1] * T[i + 1] + beta[i + 1];
            }
            T[0] = T1;
            T[_n] = T2;
            for (i = 0; i < _n+1; i++)
            {
                ser.Points.Add(new OxyPlot.DataPoint(x[i], T[i]));
               
               
            }
            for(i=0;i<500;i++)
            {
                double x1 = R1 + i * (R2 - R1) / 499;
                double y1 = (1 / (R2 - R1)) * (T2 * R2 * (1 - (R1 / x1)) - T1 * R1 * (1 - (R2 / x1)));
                ser1.Points.Add(new OxyPlot.DataPoint(x1, y1));
            }

            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser);
            ListBasePlotSeries plot_ser1 = new ListBasePlotSeries("x", "-", "y", "-", ser1);
            m_results = new SimplePlotResult();
            m_results.AddSeries(plot_ser);
            m_results.AddSeries(plot_ser1);
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
            get { return "Task4"; }
        }

        public string Description
        {
            get { return "Пример реализации вычислительной задачи"; }
        }

        public ITaskBase CreateInstance()
        {
            return new Task4();
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
