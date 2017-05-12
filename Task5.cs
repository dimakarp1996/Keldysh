using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
   
    class Task5 : ITaskBase
    {
        public Task5()
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
                for (i = 0; i < 3; i++)
                {
                    A[i] = a[i];
                    B[i] = b[i];
                    C[i] = c[i];
                    F[i] = f[i];
                }
            }
        }
        double[] Solve(Coef a, int n, double y0, double yn)//a,b,c,f - резмеры n+1 от 0 до n
        {
            //метод прогонки, a - коэффициенты перед i-1 членом, b перед i, c перед i+1, d - неоднородность
            int i;
            double[] alpha = new double[n + 1];
            double[] beta = new double[n + 1];
            double[] y = new double[n + 1];
            alpha[1] = 0;
            beta[1] = y0;
            for (i = 1; i < n; i++)
            {
                alpha[i + 1] = -a.C[1] / (a.A[1] * alpha[i] + a.B[1]);
                beta[i + 1] = (a.F[1] - a.A[1] * beta[i]) / (a.A[1] * alpha[i] + a.B[1]);
            }
            y[0] = y0;
            y[n] = yn;
            for (i = n - 1; i > 0; i--)
            {
                y[i] = alpha[i + 1] * y[i + 1] + beta[i + 1];
            }
            return y;
        }
        private SimplePlotResult m_results = null;

        private Double D1, D2,Dz,L,po,Gamma,U,Y0,Yn;
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

            InitialData.AddSimpleProperty<double>("D1", (Double)rwInitials["D1"], "левая сторона", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("D2", (Double)rwInitials["D2"], "правая сторона", "значение с плавающей точкой", "", false, null);
			InitialData.AddSimpleProperty<double>("Dz", (Double)rwInitials["Dz"], "поперечный размер", "значение с плавающей точкой", "", false, null);
			InitialData.AddSimpleProperty<double>("L", (double)rwInitials["L"], "длина счётной области", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<int>("n", (int)rwInitials["n"], "количество узлов", "целое число", "", false, null);
            InitialData.AddSimpleProperty<double>("po", (double)rwInitials["po"], "плотность", "значение с плавающей точкой", "", false, null);
			InitialData.AddSimpleProperty<double>("Gamma", (double)rwInitials["Gamma"], "гамма", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("U", (double)rwInitials["U"], "скорость", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("y0", (double)rwInitials["y0"], "начальное условие", "значение с плавающей точкой", "", false, null);
            InitialData.AddSimpleProperty<double>("yn", (double)rwInitials["yn"], "конечное условие", "значение с плавающей точкой", "", false, null);
        }

        public void Compute(bool bDataChanged)
        {
            D1 = (double)InitialData["D1"];//R1
            D2 = (double)InitialData["D2"];//R2
            Dz = (double)InitialData["Dz"];//число узлов
            n = (int)InitialData["n"];
            L =(double)InitialData["L"];
            po=(double)InitialData["po"];
            Gamma=(double)InitialData["Gamma"];
            U = (double)InitialData["U"];
            Y0= (double)InitialData["y0"];
            Yn = (double)InitialData["yn"];
            double a0;
            a0=Gamma/(U * po);
            int i;
            LineSeries ser0 = new LineSeries();
            ser0.Title = this.Name + "Anal";
            LineSeries ser1 = new LineSeries();
            ser1.Title = this.Name + "CDS";//здесь и в следующих 2 - аппроксимация диффузного члена
            LineSeries ser2 = new LineSeries();
            ser2.Title = this.Name + "UWS";
            LineSeries ser3 = new LineSeries();
            ser3.Title = this.Name + "QUICK";
            double[] x = new double [n+1];
            double[] y0 = new double[n+1];
            double[] y1 = new double[n+1];
            double[] y2 = new double[n+1];
            double[] y3 = new double[n+1];
            double[] Gran = new double[n+1];
            double[] A = new double[n+1];
            double[] B = new double[n+1];
            double[] C = new double[n+1];
            double[] D = new double[n + 1];
            Gran[0] = Gran[n] = B[0] = B[n] = 1;
            A[0] = A[n] = C[0] = C[n] = 0;
            double[] alpha = new double[n + 1];
            double[] beta = new double[n + 1];
            double[] S = new double[n + 1];
            
            double h = L / (n);
            double pi = 3.1915926;
            for ( i = 0;i<n+1;i++)
            {
             x[i] = i *h;
            // y0[i] = (a0 - Math.Exp(x[i]/a0))/ (a0 - Math.Exp(L/ a0));неверно!!!
                S[i] = pi * Dz * (D1 + x[i] * (D2 - D1) / L);  
            }
            double k = Gamma / (po * U * h);
            //A*коэффициент перед yi-1+*коэффициент перед yi+C*коэффициент перед yi+1 = F; Первое значение - это нулевой коэффт(i=0), второе - это при i от 1 до n-2 и третье при i=n-1
            y1[n] = y2[n] =y3[n]= Yn;
            for (i = 1; i < n; i++)
            {
                Gran[i] = 0;
                A[i] = S[i + 1] / (2 * h) - S[i] * Gamma / (U * po * h * h) - Gamma * (D2 - D1) * pi * Dz / (U * po *L* 2 * h);
                B[i] = 2 * S[i] * Gamma / (U * po * h * h);
                C[i] = -S[i - 1] / (2 * h) - S[i] * Gamma / (U * po * h * h) + Gamma * (D2 - D1) * pi * Dz / (U * po*L * 2 * h);//условия на концах массива- подробнее
            }
            alpha[1] = 0;//y[i]=alpha[i+1]*y[i+1]+beta[i+1], то есть i+1 от 0 до _n, определяем альфу начиная с 1го элемента, нулевой элемент массива не используется и не инициализируется, хотя так писать плохо 
            beta[1] = Y0;
            for (i = 1; i < n; i++)
            {//условия на концах массива- подробнее
                alpha[i + 1] = -C[i] / (A[i] * alpha[i] + B[i]);
                beta[i + 1] = (Gran[i] - A[i] * beta[i]) / (A[i] * alpha[i] + B[i]);
            }


            for (i = n - 1; i > 0; i--)
            {
                y1[i] = alpha[i + 1] * y1[i + 1] + beta[i + 1];
            }
            //ТЕПЕРЬ ДЛЯ ВТОРОГО
            for (i = 1; i < n; i++)
            {
                Gran[i] = 0;
                A[i] =  - S[i] * Gamma / (U * po * h * h) - Gamma * (D2 - D1) * pi * Dz / (U * po *L* 2 * h);
                B[i] = 2 * S[i] * Gamma / (U * po * h * h)+(S[i]/h);
                C[i] = -S[i - 1] / ( h) - S[i] * Gamma / (U * po * h * h) + Gamma * (D2 - D1) * pi * Dz / (U * po *L* 2 * h);//условия на концах массива- подробнее
            }
            alpha[1] = 0;//y[i]=alpha[i+1]*y[i+1]+beta[i+1], то есть i+1 от 0 до _n, определяем альфу начиная с 1го элемента, нулевой элемент массива не используется и не инициализируется, хотя так писать плохо 
            beta[1] = Y0;
            for (i = 1; i < n; i++)
            {//условия на концах массива- подробнее
                alpha[i + 1] = -C[i] / (A[i] * alpha[i] + B[i]);
                beta[i + 1] = (Gran[i] - A[i] * beta[i]) / (A[i] * alpha[i] + B[i]);
            }

            for (i = n - 1; i > 0; i--)
            {
                y2[i] = alpha[i + 1] * y1[i + 1] + beta[i + 1];
            }
            //теперь для третьего - производная выражается через u,u-1 и u-2
            for (i = 1; i < n; i++)
            {
                Gran[i] = 0;
                A[i] = 3*S[i + 1] / (8 * h)- S[i] * Gamma / (U * po * h * h) - Gamma * (D2 - D1) * pi * Dz / (U * po * L * 2 * h);
                B[i] = 2 * S[i] * Gamma / (U * po * h * h);
                C[i] = 6 * S[i - 1] / (8 * h) - S[i + 1] / (2 * h) - S[i] * Gamma / (U * po * h * h) + Gamma * (D2 - D1) * pi * Dz / (U * po * L * 2 * h);
                D[i] = -S[i - 2] / (8 * h);//условия на концах массива- подробнее
            }
            alpha[1] = 0;//y[i]=alpha[i+1]*y[i+1]+beta[i+1], то есть i+1 от 0 до _n, определяем альфу начиная с 1го элемента, нулевой элемент массива не используется и не инициализируется, хотя так писать плохо 
            beta[1] = Y0;
            for (i = 1; i < n; i++)
            {//условия на концах массива- подробнее
                alpha[i + 1] = -C[i] / (A[i] * alpha[i] + B[i]);
                beta[i + 1] = (Gran[i] - A[i] * beta[i]) / (A[i] * alpha[i] + B[i]);
            }
            for (i = 0; i < n+1; i++)
            {
               //ser0.Points.Add(new OxyPlot.DataPoint(x[i], S[i]));
                ser1.Points.Add(new OxyPlot.DataPoint(x[i], y1[i]));
               ser2.Points.Add(new OxyPlot.DataPoint(x[i], y2[i]));
               ///ser3.Points.Add(new OxyPlot.DataPoint(x[i], y3[i]));
            }


            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser0);
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
            get { return "Task5"; }
        }

        public string Description
        {
            get { return "Пример реализации вычислительной задачи"; }
        }

        public ITaskBase CreateInstance()
        {
            return new Task5();
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
