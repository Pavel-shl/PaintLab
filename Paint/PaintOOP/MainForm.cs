﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintOOP
{
    public partial class MainForm : Form
    {
        private Plugin plugin;
        private Type currentFactory;

        private FigureStorage figureStorage;
        private Serializer serializer;

        private ICreator currentCreator;
        private Figure currentFigure;

        private Color color;
        private Color fillColor;

        private float penWidth;
        private int cornersNum;

        private Boolean isDrawing;
        private Boolean isFeel;

        private List<Point> points;
        private Bitmap bitmap;

        private Point startPoint;
        private Point endPoint;
        public MainForm()
        {
            InitializeComponent();
            figureStorage = new FigureStorage();

            plugin = new Plugin();

            serializer = new Serializer();

            color = Color.Black;
            fillColor = Color.White;

            penWidth = 3;
            cornersNum = 3;

            isDrawing = false;
            isFeel = false;

            points = new List<Point>();

            bitmap = new Bitmap(PictureBox.Width, PictureBox.Height);
            PictureBox.Image = bitmap;
            CornersTrackBar.Enabled = false;
        }

        private Graphics CreateGraphics(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            PictureBox.Image = bitmap;
            return Graphics.FromImage(bitmap);
        }
        private void StateChange(ICreator creator)
        {
            if (creator.isCanFeel)
            {
                IsFeelCheck.Enabled = true;
            }
            else
            {
                IsFeelCheck.Enabled = false;
            }

            if (creator.isVariableAngles)
            {
                CornersTrackBar.Enabled = true;
            }
            else
            {
                CornersTrackBar.Enabled = false;
            }
        }

        private void Ellipsebutton_Click(object sender, EventArgs e)
        {
            currentCreator = new Figures.EllipseCreator();
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            StateChange(currentCreator);
        }

        private void Linebutton_Click(object sender, EventArgs e)
        {
            currentCreator = new Figures.LineCreator();
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            StateChange(currentCreator);
        }

        private void Rectanglebutton_Click(object sender, EventArgs e)
        {
            currentCreator = new Figures.RectangleCreator();
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            StateChange(currentCreator);
        }

        private void RegPolygonbutton_Click(object sender, EventArgs e)
        {
            currentCreator = new Figures.RegularPolygonCreator();
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            (currentFigure as Figures.RegularPolygon).numOfCorners = cornersNum;
            StateChange(currentCreator);
        }

        private void BrokenLinebutton_Click(object sender, EventArgs e)
        {
            currentCreator = new Figures.BrokenLineCreator();
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            StateChange(currentCreator);
        }

        private void PenWidthTrackBar_Scroll(object sender, EventArgs e)
        {
            penWidth = (float)PenWidthTrackBar.Value;

            if (currentFigure != null)
            {
                currentFigure.penWidth = penWidth;
                currentFigure.SetPen();
            }

            PenWidthLabel.Text = "Размер контура: " + penWidth.ToString();
        }

        private void CornerTrackBar_Scroll(object sender, EventArgs e)
        {
            cornersNum = (int)CornersTrackBar.Value;
            CornersNumLabel.Text = "Количество сторон: " + cornersNum.ToString();

            (currentFigure as Figures.RegularPolygon).numOfCorners = cornersNum;
        }

        private void IsFeelCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (IsFeelCheck.Checked)
            {
                isFeel = true;
            }
            else
            {
                isFeel = false;
            }
        }

        private void FirstColorButton_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                FirstColorButton.BackColor = ColorDialog.Color;
                color = ColorDialog.Color;

                if (currentFigure != null)
                {
                    currentFigure.color = color;
                    currentFigure.SetPen();
                }
            }
        }

        private void SecondColorButton_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                SecondColorButton.BackColor = ColorDialog.Color;
                fillColor = ColorDialog.Color;
            }

            if ((currentFigure != null) && IsFeelCheck.Enabled)
            {
                if (currentCreator.isVariableAngles)
                {
                    object[] args = new object[4] { cornersNum, color, fillColor, penWidth };
                    currentFigure = (Figure)Activator.CreateInstance(currentFigure.GetType(), args);
                }
                else
                {
                    object[] args = new object[3] { color, fillColor, penWidth };
                    currentFigure = (Figure)Activator.CreateInstance(currentFigure.GetType(), args);
                }
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentFigure != null && !currentCreator.isManyPoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    startPoint.X = e.X;
                    startPoint.Y = e.Y;

                    points.Clear();
                    points.Add(startPoint);
                    points.Add(startPoint);

                    isDrawing = !isDrawing;
                    currentFigure.isFeel = isFeel;
                }
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                endPoint.X = e.X;
                endPoint.Y = e.Y;

                points[points.Count - 1] = endPoint;

                currentFigure.points = points.ToArray();
                PictureBox.Refresh();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && !currentCreator.isManyPoint)
            {
                endPoint.X = e.X;
                endPoint.Y = e.Y;

                isDrawing = !isDrawing;

                if (startPoint != endPoint)
                {
                    points[1] = endPoint;
                    figureStorage.undoList.Add(currentFigure);

                    if (!figureStorage.redoStack.IsEmpty())
                    {
                        figureStorage.redoStack.ClearStack();
                        RedoButton.Enabled = false;
                    }

                    UndoButton.Enabled = true;
                }

                currentFigure = currentFigure.Clone();
            }
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentCreator != null && currentCreator.isManyPoint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    startPoint.X = e.X;
                    startPoint.Y = e.Y;

                    if (!isDrawing)
                    {
                        points.Clear();
                        points.Add(startPoint);
                        points.Add(startPoint);

                        isDrawing = !isDrawing;

                        currentFigure.isFeel = isFeel;
                    }
                    else
                    {
                        points.Add(startPoint);
                    }
                }
                else if (e.Button == MouseButtons.Right && isDrawing)
                {
                    isDrawing = !isDrawing;

                    UndoButton.Enabled = true;
                    figureStorage.undoList.Add(currentFigure);
                    currentFigure = currentFigure.Clone();

                    if (!figureStorage.redoStack.IsEmpty())
                    {
                        figureStorage.redoStack.ClearStack();
                       RedoButton.Enabled = false;
                    }
                }
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            figureStorage.DrawFigures(e.Graphics);
            if (isDrawing)
            {
                currentFigure.Draw(e.Graphics);
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            figureStorage.Undo();
            PictureBox.Refresh();

            RedoButton.Enabled = true;
            if (figureStorage.undoList.IsEmpty())
            {
                UndoButton.Enabled = false;
            }
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            figureStorage.Redo();
            PictureBox.Refresh();

            UndoButton.Enabled = true;
            if (figureStorage.redoStack.IsEmpty())
            {
                RedoButton.Enabled = false;
            }
        }

        private void SerializerButton_Click(object sender, EventArgs e)
        {
            serializer.Serialize(figureStorage);
        }

        private void DeserializerButton_Click(object sender, EventArgs e)
        {
            figureStorage = serializer.Deserialize(figureStorage);
            PictureBox.Refresh();

            if (!figureStorage.undoList.IsEmpty())
            {
                UndoButton.Enabled = true;
            }
            if (!figureStorage.redoStack.IsEmpty())
            {
                RedoButton.Enabled = true;
            }
        }

        private void PluginButton_Click(object sender, EventArgs e)
        {
            string pluginName = plugin.Load();

            if (pluginName != "")
            {
                PluginComboBox.Items.Add(pluginName);
                if (!PluginComboBox.Enabled)
                {
                    PluginComboBox.Enabled = true;
                }
            }
        }

        private void PluginComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pluginName = PluginComboBox.GetItemText(PluginComboBox.SelectedItem);

            currentFactory = plugin.GetPluginType(pluginName);
            currentCreator = (ICreator)Activator.CreateInstance(currentFactory);
            currentFigure = currentCreator.Create(color, fillColor, penWidth);

            StateChange(currentCreator);
        }
    }
}
