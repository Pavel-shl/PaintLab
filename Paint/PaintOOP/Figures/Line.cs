﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PaintOOP.Figures
{
    [DataContract]
    public class Line : Figure
    {
    
        public Line() { }

        public Line(Color color, float penWidth) : base(color, penWidth) { }

        public Line(Point startPoint, Point endPoint, Color color, float penWidth) : base(color, penWidth)
        {
            points[0] = startPoint;
            points[1] = endPoint;
        }

        public override Figure Clone()
        {
            return (Line)MemberwiseClone();
        }

        public override void Draw(Graphics graphics)
        {
            if (pen == null)
            {
                SetPen();
            }

            graphics.DrawLine(pen, points[0], points[1]);
        }
    }
}
