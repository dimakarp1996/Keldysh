

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Security;
using OxyPlot.Series;
using NetExtTools.Data;
using OxyPlot;


namespace Tasks.custom_task
{
    unsafe class Massive
    {
        int n;
        public double[] a;
        public Massive(double a0, double a1, double a2,int m)
        {
            n = m;
            int i;

            this.a = new double[m];
            for(i=0;i<n-3;i++)
            {
                this.a[i] = 0;
            }
            this.a[n - 3] = a0;
            a[n - 2] = a1;
            a[n - 1] = a2;

        }


        public static Massive operator *(Massive m, double[] n)
        {
            int i;
            for (i = 0; i < m.a.Length; i++)
            {
                m.a[i] *= n[i];
            }
            return m;
        }
        public static Massive operator +(Massive m, Massive n)
        {
            int i;
            for (i = 0; i < m.a.Length; i++)
            {
                m.a[i] *= n.a[i];
            }
            return m;
        }
    }
    class task7 : ITaskBase
    {
        

        public task7()
        {
            InitialData = new DataItemCollection();
        }

        private SimplePlotResult m_results = null;

        private Double U,G,po,tau;
        private int n,m = 0;

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

            InitialData.AddSimpleProperty<double>("U", (Double)rwInitials["U"], "скорость", "скорость", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "число узлов по времени", "число узлов по времени", "", false, null);
            InitialData.AddSimpleProperty<double>("tau", (double)rwInitials["tau"], "наблюдаемый момент времени", "наблюдаемый момент времени", "", false, null);
            InitialData.AddSimpleProperty<int>("m", (int)rwInitials["m"], "число узлов по пространству", "число узлов по пространству", "", false, null);
            InitialData.AddSimpleProperty<double>("G", (Double)rwInitials["G"], "Гамма", "Гамма", "", false, null);
            InitialData.AddSimpleProperty<double>("po", (Double)rwInitials["po"], "Плотность", "Плотность", "", false, null);
        }

        
    unsafe public double[] Phi1Coef(int n, double[] alpha, double beta,int m)
        {
            int i;
            /*double[] betamassive = new double[3];
            betamassive[0] = 0;
            betamassive[1] = 0;
            betamassive[2] = beta;*/
            Massive alphamassive=new Massive(alpha[0], alpha[1],alpha[2],m);//альфа и бета должны быть полноценными векторами такой же длины, как и phi1coef
            Massive betamassive=new Massive(0, 0, beta, m);
            double[] gamma = new double[m];
            if (n == 1)
            {
                for (i = 0; i <m; i++)
                {
                    gamma[i] = 1;
                
                }
                return gamma;
            }
            if (n < 1)
            {
                throw new Exception();
            }
            if (n == 2)
            {
                return alpha;//фи2= альфа*фи1
            }
            if (n > 2)
            {
                Massive gammamassive =  alphamassive * Phi1Coef(n - 1, alpha, beta,m) + betamassive * Phi1Coef(n - 2, alpha, beta,m);
                for (i = 0; i < m; i++)
                {
                    gamma[i] = gammamassive.a[i];
                }
                return gamma;
            }
            throw new Exception();

        }

        public void Compute(bool bDataChanged)
        {
            U = (double)InitialData["U"];
            po = (double)InitialData["po"];
            G = (double)InitialData["G"];
            tau = (double)InitialData["tau"];
            n = (int)InitialData["n"];
            m = (int)InitialData["m"];
            double StepX = 1.0 / (m - 1);
            double StepT = tau / (n - 1);
            int i, j;
            LineSeries ser = new LineSeries();
            ser.Title = this.Name + " Явный метод Эйлера";
            LineSeries ser1 = new LineSeries();
            ser1.Title = this.Name + "Неявный метод Эйлера";
            LineSeries ser2 = new LineSeries();
            ser2.Title = this.Name + "Метод Кранка-Николсона";
            LineSeries ser3 = new LineSeries();
            ser3.Title = this.Name + "Трехточечный неявный метод";
            double[] x = new double[m];
            double[] t = new double[n];
            double[,] yser = new double[m, n];
            double[,] yser1 = new double[m, n];
            double[,] yser2 = new double[m, n];
            double[,] yser3 = new double[m, n];
            double d = G * StepT / (po * StepX * StepX);
            double c = U * StepT / StepX;
            for (i = 0; i < m; i++)
            {
                x[i] = i * StepX;

                yser[i, 0] = 0;
                yser1[i, 0] = 0;
                yser2[i, 0] = 0;
                yser3[i, 0] = 0;
            }
            for (i = 0; i < n; i++)
            {
                t[i] = i * StepT;
                yser[0, i] = 0;
                yser[m - 1, i] = 1;
                yser1[0, i] = 0;
                yser1[m - 1, i] = 1;
                yser2[0, i] = 0;
                yser2[m - 1, i] = 1;
                yser3[0, i] = 0;
                yser3[m - 1, i] = 1;
            }
            for (j = 0; j + 1 < n; j++)//явный метод Эйлера
            {
                for (i = 1; i < m - 1; i++)
                {
                    if (i == m - 2)
                    {
                        yser[i + 1, j + 1] = (1 - 2 * d) * yser[i + 1, j] + (d + (c / 2)) * yser[i, j];//??
                    }
                    else
                    {
                        yser[i + 1, j + 1] = (1 - 2 * d) * yser[i + 1, j] + (d - (c / 2)) * yser[i + 2, j] + (d + (c / 2)) * yser[i, j];
                    }
                }
            }

            for (j = n - 2; j > 0; j--)//неявный метод Эйлера
            {
                for (i = 1; i + 1 < m; i++)
                {
                    yser1[i, j] = (-d - (c / 2)) * yser1[i - 1, j + 1] + (d + 1) * yser1[i, j + 1] + (-d + (c / 2)) * yser1[i + 1, j + 1];
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
                A[m - 1] = 0;
                C[m - 1] = 0;
                B[m - 1] = 1;
                D[m - 1] = 0;
                for (i = 1; i < m - 1; i++)
                {
                    D[i] = yser2[i, j] * (1.5 - d) + yser2[i - 1, j] * (d / 2 + c / 4) + yser2[i + 1, j] * (d / 2 - c / 4);
                    A[i] = -d / 2 + c / 4;
                    B[i] = 1 + d / 2 + c / 4;
                    C[i] = -1 - d / 2;
                }//y[i-1]=alpha[i]*y[i]+beta[i]
                alpha[1] = beta[1] = 0;
                for (i = 2; i < m; i++)
                {
                    alpha[i] = -C[i - 1] / (A[i - 1] * alpha[i - 1] + B[i - 1]);
                    beta[i] = (D[i - 1] - A[i - 1] * beta[i - 1]) / (A[i - 1] * alpha[i - 1] + B[i - 1]);
                }
                for (i = m - 2; i > 0; i--)
                {
                    yser2[i, j + 1] = yser2[i + 1, j + 1] * alpha[i + 1] + beta[i + 1];
                }
                //память можно не удалять!!!

            }
        
            /*
             double[] alpha2 = new double[3];
                double beta2;
                alpha2[0] = 1 / (2 * StepT * (c / 2 - d));
                alpha2[1] = 2 / (StepT * (c / 2 - d));
                alpha2[2] = ((-3) / (2 * StepT) - 2 * d) / (c / 2 - d);
                beta2 = (d + c / 2) / (c / 2 - d);
                double[] Phi = new double [n-1];//n-1 член массива: от координаты по временной оси,равной 1, до равной n-1
                Phi =  Phi1Coef(m - 1, alpha2, beta2,n);//фи[m-1] должно быть равно вектору из единиц. Функция Phi1Coef(m-1,alpha2,beta2) - это то, во сколько раз каждый элемент m-1 - го вектора больше, чем соотв.элемент 1-го вектора
                for(i=0;i<n-1;i++)
                {
                    Phi[i] = 1 / Phi[i]; //так как фи[m-1]=1, то ищем фи[1], теперь у нас есть фи[1]
                }
                for(j=1;j<n-1;j++)
                {
                    yser3[1,j] = Phi[j - 1];
                }
                for(i=2;i<m-1;i++)
                {
                    Phi = Phi1Coef(i, alpha2, beta2,n);
                    yser3[i,j + 1] = yser1[i,j + 1] * Phi[i];
                }*/
            //алгоритм для y[3] следующий - представляем пространственные координаты как вектор(состоящий из n-1 компонент) и вектор этих координат для i+1 ячейки равен альфа*вектор координат для iтой ячейки + бета*вектор координат для i-1той ячейки, далее можно рекурсивно задать функцию
            for (i=0;i<m;i++)
               {
                   ser.Points.Add(new OxyPlot.DataPoint(x[i], yser[i,n-1]));
                  ser1.Points.Add(new OxyPlot.DataPoint(x[i], yser1[i,n - 1]));
                  ser2.Points.Add(new OxyPlot.DataPoint(x[i], yser2[i,n - 1]));
                  //ser3.Points.Add(new OxyPlot.DataPoint(x[i], yser3[i,n - 1]));
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
            get { return "Task7"; }
        }

        public string Description
        {
            get { return "Численное интегрирование"; }
        }

        public ITaskBase CreateInstance()
        {
            return new task7();
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

