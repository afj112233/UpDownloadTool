using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace OxyPlot.Axes
{
    public class ContinueSeries : LineSeries
    {
        private int size = 5;

        public void AddPoint(DataPoint dataPoint)
        {
            this.Points.Add(dataPoint);
            if (Points.Count > size)
            {
                var data = Points[0];
                this.front.Push(data);
                Points.RemoveAt(0);
            }

        }

        public bool CanFront()
        {
            if (this.front.Count > 0) return true;
            return false;
        }

        public bool CanBack()
        {
            if (this.back.Count > 0) return true;
            return false;
        }

        public void Front(int step)
        {
            for (int i = 0; i < step; i++)
            {
                DataPoint f;
                if (this.front.Count == 0)
                {
                    var data = Points[0];
                    f = new DataPoint(data.X - 1, Double.NaN);
                    Points.Insert(0, f);

                }
                else
                {
                    var data = this.front.Pop();
                    Points.Insert(0, data);

                }
                this.back.Push(Points[size]);
                Points.RemoveAt(size);
            }
        }

        public int GetSamplingsCount()
        {
            var count = 0;
            count = front.Count + back.Count + Points.Count;
            return count;
        }

        public void Back(int step)
        {
            for (int i = 0; i < step; i++)
            {
                DataPoint f;
                if (this.back.Count == 0)
                {
                    var data = Points[0];
                    f = new DataPoint(data.X + 1, Double.NaN);
                    Points.Add(f);

                }
                else
                {
                    var data = this.back.Pop();
                    Points.Add(data);

                }
                this.front.Push(Points[0]);
                Points.RemoveAt(0);
            }
        }

        public void Recover()
        {
            while (this.back.Count > 0)
            {
                Points.Add(this.back.Pop());
            }

            while (Points.Count > size)
            {
                this.front.Push(Points[0]);
                Points.RemoveAt(0);
            }
        }

        Stack<DataPoint> front = new Stack<DataPoint>();
        Stack<DataPoint> back = new Stack<DataPoint>();
    }
}
