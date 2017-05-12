using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    class task8 : ITaskBase
    {
        public task8()
        {
            InitialData = new DataItemCollection();
        }

        private SimplePlotResult m_results = null;

        private Double alpha,t;
        private int n,m,k = 0;

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
            InitialData.AddSimpleProperty<double>("t", (Double)rwInitials["U"], "временной интервал", "временной интервал", "", false, null);
            InitialData.AddSimpleProperty<double>("alpha", (Double)rwInitials["U"], "скорость", "скорость", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "число узлов по времени", "число узлов по времени", "", false, null);
            InitialData.AddSimpleProperty<int>("m", (int)rwInitials["m"], "число узлов по оси y", "число узлов по пространству", "", false, null);
            InitialData.AddSimpleProperty<int>("k", (int)rwInitials["k"], "число узлов по оси x", "число узлов по пространству", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            t = (double)InitialData["t"];
            alpha = (double)InitialData["alpha"];
            k = (int)InitialData["k"];
            n = (int)InitialData["n"];
            m = (int)InitialData["m"];
            double StepX = 1.0 / (k - 1);
            double StepY = 1.0 / (m - 1);
            double StepT = 1.0 / (n - 1);
            int i, j;
            LineSeries ser = new LineSeries();

            double exp = 2.71828182845904523536;
            double[] x = new double[m];
            double[] t = new double[n];
            double[] y = new double[n];
            for (i = 0; i < m; i++)
            {
                x[i] = i * StepX;
                t[i] = i * StepT;
                y[i] = i * StepY;
            }
            for (i = 0; i < m; i++)
            {
                yser[0][i] = 0;
                yser[m - 1][i] = 1;
                yser1[0][i] = 0;
                yser1[m - 1][i] = 1;
                yser2[0][i] = 0;
                yser2[m - 1][i] = 1;
                yser3[0][i] = 0;
                yser3[m - 1][i] = 1;
            }
            for (j = 0; j + 1 < n; j++)//явный метод Эйлера
            {
                for (i = 1; i + 1 < m; i++)
                {
                    yser[i][j + 1] = (1 - 2 * d) * yser[i][j] + (d - (c / 2)) * yser[i + 1][j] + (d + (c / 2)) * yser[i - 1][j];
                }
            }
            for (j = n - 2; j > 0; j--)//неявный метод Эйлера
            {
                for (i = 1; i + 1 < m; i++)
                {
                    yser1[i][j] = (-d - (c / 2)) * yser1[i - 1][j + 1] + (d + 1) * yser1[i][j + 1] + (-d + (c / 2)) * yser1[i + 1][j + 1];
                }
            }
            for (j = 0; j + 1 < n; j++)//метод Кранка-Николсона. В презентации ОШИБКА.
            {
                double[] A = new double[m];//Ai*y[i-1][n]+Bi*y[i][n]+Ci*y[i+1][n]=Di; i from 0 to m-1
                double[] B = new double[m];
                double[] C = new double[m];
                double[] D = new double[m];
                double[] alpha = new double[m];
                double[] beta = new double[m];
                A[0] = 0;
                C[0] = 0;
                D[0] = 0;
                B[0] = 1;
                A[m-1] = 0;
                C[m-1] = 0;
                B[m-1] = 1;
                D[m - 1] = 0;
                for(i=1;i<m-1;i++)
                {
                    D[i] = yser2[i][j] * (1.5-d) + yser2[i - 1][j] * (d/2+c/4) + yser2[i + 1][j] * (d/2-c/4);
                    A[i] = -d / 2 + c / 4;
                    B[i] = 1 + d / 2 + c / 4;
                    C[i] = -1 - d / 2;
                }//y[i-1]=alpha[i]*y[i]+beta[i]
                alpha[1] = beta[1] = 0;
                for(i=2;i<m;i++)
                {
                    alpha[i] = -C[i - 1] / (A[i - 1] * alpha[i - 1] + B[i - 1]);
                    beta[i] = (D[i-1]-A[i-1]*beta[i-1]) / (A[i - 1] * alpha[i - 1] + B[i - 1]);
                }
                for(i=m-2;i>0;i--)
                {
                    yser2[i][j + 1] = yser2[i + 1][j + 1] * alpha[i + 1] + beta[i + 1];
                }
            }
            /*
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

            for (i = _n - 1; i > 0; i--)
            {
                T[i] = alpha[i + 1] * T[i + 1] + beta[i + 1];
            }*/
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
        public string Name
        {
            get { return "Task8"; }
        }

        public string Description
        {
            get { return "Численное интегрирование"; }
        }

        public ITaskBase CreateInstance()
        {
            return new task8();
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
