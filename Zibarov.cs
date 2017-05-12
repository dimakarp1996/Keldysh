using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;
using System.IO;
using System.Globalization;

namespace Tasks.custom_task
{
    class Params
    {
        public List<Double> P, U, Ro, NewP,NewE,E, NewU,NewRo,AvgU,AvgE;
        public int N,Nt;
        public double LastSize, h,tau, Length,M,U0,deltaL,deltaE,U1,Time;
        private static readonly double A = 0;
        private static readonly double B = 0;
        private static readonly double k = 1.4;
        public static Params(int Nin, double Lengthin,double Timein, int Ntin)
        {
            int i;
            N = Nin;
            Length = Lengthin;
            Time = Timein;
            Nt = Ntin;
            tau = Timein / Nt;      
            h = Length / N;
            for (i = 0; i < N / 2; i++)
            {

                Ur.Add(0);

                P.Add(30 * 101325);
                Ro.Add(30 * 1.29);
                E.Add(Energy(P[i], Ro[i]));
            }
            for (i = N / 2; i < N; i++)
            {

                Ur.Add(0);

                P.Add(101325);
                Ro.Add(1.29);
                E.Add(Energy(P[i], Ro[i]));
            }
        }
        private static double Energy(double Press, double Dens)
        {
            return (Press + A) / (Dens * (k - 1)) + B;
        }
        public  double Avg(double x1,double x2)
        {
            return (x1 + x2) / 2;
        }
        public static void AvgStream()
        {
            for(int i=0;i<N;i++)
            {
                if (i == 0)
                {
                    AvgU[i] = U[i] - Avg(P[i], P[i + 1]) * tau / (h * Ro[i]);
                    AvgE[i] = E[i] - Avg(P[i] * U[i], P[i + 1] * U[i + 1]) * tau / (h * Ro[i]);
                }
                else 
                if(i==N-1)
                {
                    AvgU[i] = U[i] - (- Avg(P[i], P[i - 1])) * tau / (h * Ro[i]);
                    AvgE[i] = E[i] - ( - Avg(P[i] * U[i], P[i - 1] * U[i - 1])) * tau / (h * Ro[i]);
                }
                else
                {
                    AvgU[i] = U[i] - (Avg(P[i], P[i + 1]) - Avg(P[i], P[i - 1])) * tau / (h * Ro[i]);
                    AvgE[i] = E[i] - (Avg(P[i] * U[i], P[i + 1] * U[i + 1]) - Avg(P[i] * U[i], P[i - 1] * U[i - 1])) * tau / (h * Ro[i]);
                }
            }
        }
        public double Stream(int i)
        {
           if(Avg(AvgU[i],AvgU[i+1])>0)
            {
                return Ro[i] * (tau / 2) * Avg(AvgU[i], AvgU[i + 1]);
            }
           else
            {
                return Ro[i+1] * (tau / 2) * Avg(AvgU[i], AvgU[i + 1]);
            }
        }
        public static void FinalVar()
        {
            for (int i = 0; i < N; i++)
            {
                NewRo[i] = Ro[i] + (Stream(i - 1) - Stream(i)) / h;
                NewU[i] = U[i] * (Ro[i] / NewRo[i]) + (AvgU[i - 1] * Stream(i - 1) - AvgU[i + 1] * Stream[i]) / (h * Ro[i]);
                NewE[i] = E[i] * (Ro[i] / NewRo[i]) + (AvgE[i - 1] * Stream(i - 1) - AvgE[i + 1] * Stream[i]) / (h * Ro[i]);
                NewP[i] = (NewE[i] - B) * NewRo[i] * (k - 1) - A;
            }
        }



        public static  void PrintCurrentCond()
        {//вот это почему так
            System.IO.Directory.CreateDirectory("C:\\Program Files\\Zibarov\\");
            String g = "C:\\Program Files\\Zibarov\\" + "Time" + Time.ToString() + ".txt";
            FileStream fs = File.Create(g, 1024);
            StreamWriter a = new StreamWriter(g);
            a.WriteLine("Time is\n %d Length is \n%d\n", Time, Length);
            a.WriteLine("X dens Ur P \n");
            a.Close();
            for (int i = 0; i < N; i++)
            {
                a.WriteLine("%f %f %f %f %f %f\n", i * h,  Ro[i], U[i], P[i]);
            }
            a.Close();
        }
        public static void Equal()
        {
            P = NewP;
            U = NewU;
            Ro = NewRo;
            E = NewE;
        }
       
    }
    
    class Zibarov : ITaskBase
    {
        private IPlotResult m_results=null;

        public Zibarov()
        {
            InitialData = new DataItemCollection();
        }
        private List<Double> _x = new List<Double>();
        private List<Double> _y = new List<Double>();
        private double Time,Length,MaxValue,MinValue,H,U0,M;
        private int Nr, Nfi, Nt,Show,Con;
        private Boolean radial,contour;
      public static  double pi = 3.1415926;//пи
       public static double k = 1.4;//показатель адиабаты
       public static double A = 0;
       public static double B = 0;//(12.8)с.101 Годунов 1976 - два параметра для двучленной модели энергии
        #region ITaskBase Members
        public eResultType ResultType
        {
            get { return eResultType.plotted; }
        }
       
        public void Form(System.Data.DataRow rwInitials)
        {
            InitialData.Clear();
            //InitialData.AddSimpleProperty<bool>("contour", (bool)rwInitials["contour"], "контурная заливка", "true - контурная, false - сплошная", "", false, null);
            //InitialData.AddSimpleProperty<bool>("radial", (bool)rwInitials["radial"], "радиальные координаты", "true - угол и радиус в кач-ве осей, false - x и y", "", false, null);
           // InitialData.AddSimpleProperty<int>("Con", (int)rwInitials["Con"], "число контуров для контурной заливки", "число контуров для контурной заливки", "", false, null);
            InitialData.AddSimpleProperty<int>("Nr", (int)rwInitials["Nr"], "число радиальных ячеек", "число радиальных ячеек", "", false, null);
           // InitialData.AddSimpleProperty<int>("Nfi", (int)rwInitials["Nfi"], "число углов", "число угловых координат","", false, null);
            InitialData.AddSimpleProperty<int>("Nt", (int)rwInitials["Nt"], "номер слоя", "число временных слоев, которые были до момента наблюдения включительно(ВАЖНО - минимум 1)", "", false, null);
            InitialData.AddSimpleProperty<double>("Time", (double)rwInitials["Time"], "момент времени(сек)", "момент времени, в который ведется наблюдение", "", false, null);
            InitialData.AddSimpleProperty<double>("Length", (double)rwInitials["Length"], "длина(м)", "радиус показываемой области", "", false, null);
            InitialData.AddSimpleProperty<int>("Show", (int)rwInitials["Show"], "объект изображения", "при значении 1 -на графике показывается распределение Ur(радиальной скорости), при 2 - Ufi(трансверсальной скорости),при  3 - dens(плотности),при  4 - P(давления)", "", false, null);
            //InitialData.AddSimpleProperty<double>("MaxValue", (double)rwInitials["MaxValue"], "максимальное значение показываемой величины", "максимальное значение показываемой величины", "", false, null);
            //InitialData.AddSimpleProperty<double>("MinValue", (double)rwInitials["MinValue"], "минимальное значение показываемой величины", "минимальное значение показываемой величины", "", false, null);
            //InitialData.AddSimpleProperty<double>("H", (double)rwInitials["H"], "высота цилиндра(м)", "область считается цилиндрической, но моделируются только 2 координаты, а не 3. Зависимость распределения от высоты явно не моделируется, но высота задается", "", false, null);
            //InitialData.AddSimpleProperty<double>("U0", (double)rwInitials["U0"], "начальная скорость поршня(м/с)", "начальная скорость поршня(м/с)", "", false, null);
            //InitialData.AddSimpleProperty<double>("M", (double)rwInitials["M"], "масса поршня(кг) ", "масса поршня(кг)", "", false, null);
        }    
        public void Compute(bool bDataChanged)
        {
            Nr = (int)InitialData["Nr"];
            //Nfi = (int)InitialData["Nfi"];
            Nt = (int)InitialData["Nt"];
           // Con = (int)InitialData["Con"];
            Show = (int)InitialData["Show"];
            Time = (double)InitialData["Time"];
            Length = (double)InitialData["Length"];//начальная длина рассчетной области. ВАЖНО - система координат начинается на поршне
            //MaxValue = (double)InitialData["MaxValue"];
            //MinValue = (double)InitialData["MinValue"];
            //U0 = (double)InitialData["U0"];
            //M = (double)InitialData["M"];
            //H = (double)InitialData["H"];
           // contour = (bool)InitialData["contour"];
          //  radial = (bool)InitialData["radial"];
            LineSeries ser = new LineSeries();
            if (Show == 1)
            {
                ser.Title = this.Name + " Давление";
            }
            if (Show == 2)
            {
                ser.Title = this.Name + " Скорость";
            }
            if (Show == 3)
            {
                ser.Title = this.Name + "Плотность";
            }
        }
        public IPlotResult PlotResult
        {
            get { return m_results; }
        }

        public string Name
        {
            get { return "Zibarov"; }
        }

        public string Description
        {
            get { return ("Поршень одномерный \n"); }
        }

        public ITaskBase CreateInstance()
        {
            return new Zibarov();
        }

        public double ComputationDuration
        {
            get { return Time; }
        }

        public NetExtTools.Data.DataItemCollection InitialData
        {
            get;
            private set;
        }


        #endregion
    }
}
