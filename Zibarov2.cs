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
        public List<Double> P = new List<double>();
        public List<Double> U = new List<double>();
        public List<Double> Ro = new List<double>();
        public List<Double> E = new List<double>();
        public List<Double> NewP = new List<double>();
        public List<Double> NewU = new List<double>();
        public List<Double> NewRo = new List<double>();
        public List<Double> NewE = new List<double>();
        public List<Double> AvgU = new List<double>();
        public List<Double> AvgE = new List<double>();
        public List<Double> x = new List<double>();
        public static int N,Nt;
        public static double h,tau, Length,Time;
        private static readonly double A = 0;
        private static readonly double B = 0;
        private static readonly double k = 1.4;
        public  Params(int Nin, double Lengthin,double Timein, int Ntin)
        {
            int i;
            N = Nin;
            Length = Lengthin;
            Time = Timein;
            Nt = Ntin;
            tau = Timein / Nt;      
            h = Length / (N-1);
            //for (i = 0; i < N / 2; i++)
            //{

            //    U.Add(-379.15213178);
            //    P.Add(42.271033803 * 101325);
            //    Ro.Add(6.8046564525);
            //    E.Add(Energy(P[i], Ro[i]));
            //    NewU.Add(0);
            //    NewP.Add(0);
            //    NewRo.Add(0);
            //    NewE.Add(0);
            //    AvgU.Add(0);
            //    AvgE.Add(0);
            //    x.Add(h * i);
            //}
            //for (i = N / 2; i < N; i++)
            //{

            //    U.Add(-2000);
            //    P.Add(101325);
            //    Ro.Add(1.29);
            //    E.Add(Energy(P[i], Ro[i]));
            //    NewU.Add(0);
            //    NewP.Add(0);
            //    NewRo.Add(0);
            //    NewE.Add(0);
            //    AvgU.Add(0);
            //    AvgE.Add(0);
            //    x.Add(h * i);

            //}
            for (i = 0; i < N/2; i++)
            {

                U.Add(0);
                P.Add(10*101325);
                Ro.Add(10*1.29);
                E.Add(Energy(P[i], Ro[i]));
                NewU.Add(0);
                NewP.Add(0);
                NewRo.Add(0);
                NewE.Add(0);
                AvgU.Add(0);
                AvgE.Add(0);
                x.Add(h * i);

            }
            for (i = N/2; i < N; i++)
 {

     U.Add(0);
     P.Add(101325);
     Ro.Add(1.29);
     E.Add(Energy(P[i], Ro[i]));
     NewU.Add(0);
     NewP.Add(0);
     NewRo.Add(0);
     NewE.Add(0);
     AvgU.Add(0);
     AvgE.Add(0);
     x.Add(h * i);

 }
           
/*>>>>>>> norma dx = 9.42905e-10
P is 1572126.506672
T is 3098.044939
po is 1.497939
D is 1995.000373
nu is 886.873034
Gamma is 1.170000*/

}
private  double Energy(double Press, double Dens)
{
return (Press + A) / (Dens * (k - 1)) + B;
}
public  double Avg(double x1,double x2)
{
return (x1 + x2) / 2;
}
public void AvgStream()//!!!
{
for(int i=0;i<N;i++)
{
   if (i == 0)
   {
       AvgU[i] = U[i] - (Avg(P[i], P[i + 1]) - Avg(P[i], P[i])) * tau / (h * Ro[i]);
       AvgE[i] = E[i] - (Avg(P[i] * U[i], P[i + 1] * U[i + 1])-Avg(P[i]*U[i],P[i]*U[i])) * tau / (h * Ro[i]);
   }
   else 
   if(i==N-1)
   {
       AvgU[i] = U[i] - (Avg(P[i], P[i] ) - Avg(P[i], P[i - 1])) * tau / (h * Ro[i]);
       AvgE[i] = E[i] - (Avg(P[i] * U[i], P[i ] * U[i ]) - Avg(P[i] * U[i], P[i - 1] * U[i - 1])) * tau / (h * Ro[i]);
   }
   else
   {
       AvgU[i] = U[i] - (Avg(P[i], P[i + 1]) - Avg(P[i], P[i - 1])) * tau / (h * Ro[i]);
       AvgE[i] = E[i] - (Avg(P[i] * U[i], P[i + 1] * U[i + 1]) - Avg(P[i] * U[i], P[i - 1] * U[i - 1])) * tau / (h* Ro[i]);
}
}
}
public double Stream(int i)
{
{
   if (Avg(AvgU[i], AvgU[i + 1]) > 0)
   {
       return Ro[i] *tau * Avg(AvgU[i], AvgU[i + 1]);
   }
   else
   {
       return Ro[i + 1] * tau * Avg(AvgU[i], AvgU[i + 1]);
   }
}
}
public double URoStream(int i)
{
{
   if (Avg(AvgU[i], AvgU[i + 1]) > 0)
   {
       return U[i]*Ro[i] *tau  * Avg(AvgU[i], AvgU[i + 1]);
   }
   else
   {
       return U[i+1]*Ro[i + 1] * tau * Avg(AvgU[i], AvgU[i + 1]);
   }
}
}
public double UEStream(int i)
{
{
   if (Avg(AvgU[i], AvgU[i + 1]) > 0)
   {
       return E[i] * Ro[i] * tau * Avg(AvgU[i], AvgU[i + 1]);
   }
   else
   {
       return E[i + 1] * Ro[i + 1] * tau * Avg(AvgU[i], AvgU[i + 1]);
   }
}
}
public  void FinalVar()
{
int i;
for ( i = 1; i < N-1; i++)
{

       NewRo[i] = Ro[i] + (Stream(i - 1) - Stream(i)) / h;
       NewU[i] = U[i] * (Ro[i] / NewRo[i]) + (URoStream(i - 1) - URoStream(i)) / (h * NewRo[i]);
       NewE[i] = E[i] * (Ro[i] / NewRo[i]) + (UEStream(i - 1) - UEStream(i)) / (h * NewRo[i]);
       NewP[i] = (NewE[i] - B) * NewRo[i] * (k - 1) - A;

}
i = 0;
   NewRo[i] = NewRo[i+1];
   NewU[i] = NewU[i+1];
   NewE[i] = NewE[i+1];
   NewP[i] = NewP[i+1];
i = N - 1 ;
   NewRo[i] = NewRo[i-1];
   NewU[i] = NewU[i-1];
   NewE[i] = NewE[i-1];
   NewP[i] = NewP[i-1];

}



public  void PrintCurrentCond()
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
public  void Equal()
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
private double Time,Length;
private int Nr, Nt, Show;
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
InitialData.AddSimpleProperty<int>("Nt", (int)rwInitials["Nt"], "номер слоя", "число временных слоев, которые были до момента наблюдения ", "", false, null);
InitialData.AddSimpleProperty<double>("Time", (double)rwInitials["Time"], "момент времени(сек)", "момент времени, в который ведется наблюдение", "", false, null);
InitialData.AddSimpleProperty<double>("Length", (double)rwInitials["Length"], "длина(м)", "радиус показываемой области", "", false, null);
InitialData.AddSimpleProperty<int>("Show", (int)rwInitials["Show"], "объект изображения", "при значении 1 -на графике показывается распределение давления, 2 - скорости, 3 - плотности", "", false, null);
//InitialData.AddSimpleProperty<double>("MaxValue", (double)rwInitials["MaxValue"], "максимальное значение показываемой величины", "максимальное значение показываемой величины", "", false, null);
//InitialData.AddSimpleProperty<double>("MinValue", (double)rwInitials["MinValue"], "минимальное значение показываемой величины", "минимальное значение показываемой величины", "", false, null);
//InitialData.AddSimpleProperty<double>("H", (double)rwInitials["H"], "высота цилиндра(м)", "область считается цилиндрической, но моделируются только 2 координаты, а не 3. Зависимость распределения от высоты явно не моделируется, но высота задается", "", false, null);
//InitialData.AddSimpleProperty<double>("U0", (double)rwInitials["U0"], "начальная скорость поршня(м/с)", "начальная скорость поршня(м/с)", "", false, null);
//InitialData.AddSimpleProperty<double>("M", (double)rwInitials["M"], "масса поршня(кг) ", "масса поршня(кг)", "", false, null);
}    
public void Compute(bool bDataChanged)
{
Nr = (int)InitialData["Nr"];
Nt = (int)InitialData["Nt"];
Show = (int)InitialData["Show"];
Time = (double)InitialData["Time"];
Length = (double)InitialData["Length"];//начальная длина рассчетной области. ВАЖНО - система координат начинается на поршне
Params A = new Params(Nr,  Length, Time, Nt);
if ((Nt > 0)&&(Time>0))
{
   for (int j = 0; j < Nt; j++)
   {
       A.AvgStream();
       A.FinalVar();
       A.Equal();
       //A.PrintCurrentCond();
   }
}
LineSeries ser = new LineSeries();
string str1="D", str2="e";
switch (Show)
{
   case 1:
       str1 = "Pressure";
       str2 = "Pa";
       ser.Title = this.Name + " Давление";
       break;
   case 2:
       str1 = "Speed";
       str2 = "m/s";
       ser.Title = this.Name + "Скорость";
       break;
   case 3:
       str1 = "Density";
       str2 = "kg/m3";
       ser.Title = this.Name + "Плотность";
       break;

}
for (int i = 0; i < Params.N; i++)
{
   if (Show == 1)
   { 
       ser.Points.Add(new OxyPlot.DataPoint(A.x[i], A.P[i]));
   }
   if (Show == 2)
   {
       ser.Points.Add(new OxyPlot.DataPoint(A.x[i], A.U[i]));
   }
   if (Show == 3)
   {
       ser.Points.Add(new OxyPlot.DataPoint(A.x[i], A.Ro[i]));
   }
}
ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "m", str1, str2, ser);
m_results = new SimplePlotResult();
m_results.AddSeries(plot_ser);
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
