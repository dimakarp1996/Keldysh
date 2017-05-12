using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Series;
using NetExtTools.Data;

namespace Tasks.custom_task
{
    class Task_2_2 : ITaskBase
    {
        public Task_2_2()
        {
            InitialData = new DataItemCollection();
        }

        private SimplePlotResult m_results = null;

        private Double _x0, _x1;
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
        }

        public void Compute(bool bDataChanged)
        {
            _x0 = (double)InitialData["x0"];
            _x1 = (double)InitialData["x1"];
            _n = (int)InitialData["n"];

            LineSeries ser = new LineSeries();
            ser.Title = this.Name + " y(x)";

            for (int i = 0; i < _n; i++)
            {
                double x_i = _x0 + i * (_x1 - _x0) / (_n - 1);
                double y_i = Math.Sqrt(x_i) + 0.5 * x_i;

                ser.Points.Add(new OxyPlot.DataPoint(x_i, y_i));
            }

            ListBasePlotSeries plot_ser = new ListBasePlotSeries("x", "-", "y", "-", ser);

            m_results = new SimplePlotResult();
            m_results.AddSeries(plot_ser);
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
            get { return "Task_2_2"; }
        }

        public string Description
        {
            get { return "Пример реализации вычислительной задачи"; }
        }

        public ITaskBase CreateInstance()
        {
            return new Task_2_2();
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
