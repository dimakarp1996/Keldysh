using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    
    class Task3 : ITaskBase
    {
        public Task3()
        {
            InitialData = new DataItemCollection();
        }
        public struct Coef
        {
            public double[] A;
            public double[] B;
            public double[] C;
            public double[] F;
            public void init(double[] a, double[] b, double[] c, double[] f)
            {
                A = new double[3];
                B = new double[3];
                C = new double[3];
                F = new double[3];
                int i;
                for(i=0;i<3;i++)
                {
                    A[i] = a[i];
                    B[i] = b[i];
                    C[i] = c[i];
                    F[i] = f[i];
                }
            }
        }
         double[] Solve ( Coef a,int n,double y0,double yn)//a,b,c,f - резмеры n+1 от 0 до n
        {
            //метод прогонки, a - коэффициенты перед i-1 членом, b перед i, c перед i+1, d - неоднородность
            int i;
            double[] alpha = new double[n+1];
            double[] beta = new double[n+1];
            double[] y = new double[n+1];
            alpha[1] = 0;
            beta[1] = y0;
            for (i = 1; i < n; i++)
            {
                alpha[i + 1] = -a.C[1] / (a.A[1] * alpha[i] + a.B[1]);
                beta[i + 1] = (a.F[1] - a.A[1] * beta[i]) / (a.A[1] * alpha[i] + a.B[1]);
            }
            y[0] = y0;
            y[n] = yn;
            for (i = n-1; i > 0; i--)
            {
               y[i] = alpha[i+1] * y[i + 1] + beta[i+1];
            }
            return y;
        }
        private SimplePlotResult m_results = null;

        private Double Phi0,Phi1,L,po,Gamma,U;
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

            InitialData.AddSimpleProperty<double>("phi0", (Double)rwInitials["Phi0"], "начальные условия", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("phi1", (Double)rwInitials["Phi1"], "конечные условия", "значение с плавающей точкой", "", false, null);
			InitialData.AddSimpleProperty<double>("L", (double)rwInitials["L"], "длина счётной области", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "количество узлов", "целое число", "", false, null);
            InitialData.AddSimpleProperty<double>("po", (double)rwInitials["po"], "плотность", "значение с плавающей точкой", "", false, null);
			InitialData.AddSimpleProperty<double>("Gamma", (double)rwInitials["Gamma"], "гамма", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("U", (double)rwInitials["U"], "скорость", "значение с плавающей точкой", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            Phi0 = (double)InitialData["Phi0"];
            Phi1 = (double)InitialData["Phi1"];
            n = (int)InitialData["n"];
            L =(double)InitialData["L"];
            po=(double)InitialData["po"];
            Gamma=(double)InitialData["Gamma"];
            U = (double)InitialData["U"];
            double Pe = po * U * L / Gamma;
            int i;
            LineSeries ser0 = new LineSeries();
            ser0.Title = this.Name + "Anal";
            LineSeries ser1 = new LineSeries();
            ser1.Title = this.Name + "CDS";//здесь и в следующих 2 - аппроксимация диффузного члена
            LineSeries ser2 = new LineSeries();
            ser2.Title = this.Name + "UDS";
            LineSeries ser3 = new LineSeries();
            ser3.Title = this.Name + "CDSalt";
            LineSeries ser4 = new LineSeries();
            ser4.Title = this.Name + "UDSalt";
            double[] x = new double [n+1];
            double[] y0 = new double[n+1];
            double[] y1 = new double[n+1];
            double[] y2 = new double[n+1];
            double[] y3 = new double[n+1];
            double h = L / (n );
            for ( i = 0;i<n+1;i++)
            {
             x[i] = i * L / (n);
                y0[i] = Phi0 + ((Math.Exp(x[i] * Pe / L) - 1) / (Math.Exp(Pe) - 1)) * (Phi1 - Phi0);
            }

            //A*коэффициент перед yi-1+*коэффициент перед yi+C*коэффициент перед yi+1 = F; Первое значение - это нулевой коэффт(i=0), второе - это при i от 1 до n-2 и третье при i=n-1
            Coef Y1 = new Coef();
            double adif = -2 * Gamma / (2 * h * h);
            double cdif = -2 * Gamma / (2 * h * h);
            double bdif = (adif + cdif) * (-1);
            double aUDS = (Math.Max(po * U, 0))/h;//2h
            double cUDS= (Math.Min(po * U, 0)) / h;
            double bUDS = (aUDS + cUDS) * (-1);
            double aCDS = -po * U / (2 * h);
            double cCDS = po * U / (2 * h);
            double bCDS = 0;
            double a = aCDS+adif;
            double b = bCDS+bdif;
            double c = cCDS+cdif;

            double[] A0 = new double[] { 0, a, 0 };
            double[] B0 = new double[] { 1, b, 1 };
            double[] C0 = new double[] { 0, c, 0 };
            double[] F0 = new double[] { Phi0, 0, Phi1 };
            Y1.init(A0,B0,C0,F0);
            y1 = Solve(Y1, n, Phi0, Phi1);
            Coef Y2 = new Coef();
            a = adif - aUDS;
            b = bdif - bUDS;
            c = cdif - cUDS;
            double[] A = new double[] { 0, a, 0 };
            double[] B = new double[] { 1, b, 1 };
            double[] C = new double[] { 0, c, 0 };
            double[] F = new double[] { 0, 0, 1 };
            Y2.init(A, B, C, F);
            y2 = Solve(Y2, n, Phi0,Phi1);
            
            for (i = 0; i <= n; i++)
            {
               ser0.Points.Add(new OxyPlot.DataPoint(x[i], y0[i]));
                ser1.Points.Add(new OxyPlot.DataPoint(x[i], y1[i]));
               ser2.Points.Add(new OxyPlot.DataPoint(x[i], y2[i]));

            }

            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser0);
            ListBasePlotSeries plot_ser1 = new ListBasePlotSeries("x", "-", "y", "-", ser1);
            ListBasePlotSeries plot_ser2 = new ListBasePlotSeries("x", "-", "y", "-", ser2);

            
            m_results = new SimplePlotResult();
            m_results.AddSeries(plot_ser);
          m_results.AddSeries(plot_ser1);
            m_results.AddSeries(plot_ser2);

        }

        public IPlotResult PlotResult
        {
            get { return m_results; }
        }

        public string Name
        {
            get { return "Task3"; }
        }

        public string Description
        {
            get { return "Пример реализации вычислительной задачи"; }
        }

        public ITaskBase CreateInstance()
        {
            return new Task3();
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
