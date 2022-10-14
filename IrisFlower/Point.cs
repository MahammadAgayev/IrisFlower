using System;

namespace IrisFlower
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y, int color)
        {
            X = x;
            Y = y;
            ClusterId = color;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int ClusterId { get; set; }
    }
}
