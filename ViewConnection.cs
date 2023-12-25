using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    internal class ViewConnection
    {
        public Point start { get; set; }
        public Point end { get; set; }
        public int weight { get; set; }
        public ViewConnection(Point start, Point end, int weight)
        {
            this.start = start;
            this.end = end;
            this.weight = weight;
        }
    }
}
