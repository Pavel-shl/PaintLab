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
    public class Ellipse : Figure
    {
        [DataMember]
        private float width;
        [DataMember]
        private float height;
        [DataMember]
        private Brush brush;

        public Ellipse() { }

        public Ellipse(Color color, Color fillColor, float penWidth) : base(color, penWidth)
        {
            brush = new SolidBrush(fillColor);
        }

        public Ellipse(Point startPoint, Point endPoint, Color color, Color fillColor, float penWidth) : base(color, penWidth)
        {
            points[0] = new Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y));
            points[1] = new Point(Math.Max(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y));

            brush = new SolidBrush(fillColor);
        }


        public override Figure Clone()
        {
            return (Ellipse)MemberwiseClone();
        }

        public override void Draw(Graphics graphics)
        {
            if (pen == null)
            {
                SetPen();
            }

            width = points[1].X - points[0].X;
            height = points[1].Y - points[0].Y;

            if (isFeel)
            {
                graphics.FillEllipse(brush, points[0].X, points[0].Y, width, height);
            }

            graphics.DrawEllipse(pen, points[0].X, points[0].Y, width, height);
        }
    }
}