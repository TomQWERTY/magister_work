using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace CourseWork
{
    public partial class ViewForm : Form
    {
        int vRad;
        List<Point?> places, transitions;
        int mode;
        Pen pPen, tPen, lPen;
        Graphics gr;
        Controller controller;
        bool? placeInConnSelected;
        Point? firstSelected;
        Point firstClick;//kostyl
        List<int> horizontal;
        List<string> connections;
        WeightEnterForm wef;
        AnalyzeForm analyzeForm;

        public ViewForm(Controller con)
        {
            InitializeComponent();
            pictureBox1.Width = 1800;
            pictureBox1.Height = 1000;
            Width = 2000;
            Height = 1200;
            Bitmap mapBit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = mapBit;
            vRad = 30;
            places = new List<Point?>();
            transitions = new List<Point?>();
            mode = 0;
            pPen = new Pen(Color.Black, 5);
            tPen = new Pen(Color.Black, 10);
            lPen = new Pen(Color.Black, 2);
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
            lPen.CustomEndCap = bigArrow;
            gr = Graphics.FromImage(pictureBox1.Image);
            controller = con;
            placeInConnSelected = null;
            firstSelected = null;
            horizontal = new List<int>();
            connections = new List<string>();

            //dgv block
            dgv1.Columns.Add("", "");
            dgv1.Columns[dgv1.Columns.Count - 1].Width = 50;
            /*dgv1.Columns.Add("", "");
            dgv1.Columns.Add("", "");
            dgv1.Columns.Add("", "");
            dgv1.Columns.Add("", "");
            dgv1.Rows.Add();
            dgv1.Rows.Add();
            dgv1.Rows.Add();
            dgv1.Rows.Add();
            foreach (DataGridViewTextBoxColumn d in dgv1.Columns)
            {
                d.Width = 50;
            }*/
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case 0:
                    {
                        if (ModifierKeys.HasFlag(Keys.Shift))
                        {
                            Point p = new Point(e.X - vRad / 2, e.Y - vRad / 2);
                            AddPlaceElement(p);
                        }
                        else
                        {
                            Point t = new Point(e.X, e.Y);
                            if (ModifierKeys.HasFlag(Keys.Control))
                            {
                                AddTransitionElement(t, true);
                            }
                            else
                            {
                                AddTransitionElement(t, false);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        if (firstSelected == null)
                        {
                            for (int i = 0; i < places.Count; i++)
                            {
                                if (Math.Sqrt(Math.Pow(e.X - places[i].Value.X, 2)
                                    + Math.Pow(e.Y - places[i].Value.Y, 2)) < vRad + 20)
                                {
                                    firstSelected = places[i];
                                    firstClick = new Point(e.X, e.Y);//kostyl
                                    break;
                                }
                            }
                            if (firstSelected == null)
                            {
                                for (int i = 0; i < transitions.Count; i++)
                                {
                                    if (Math.Sqrt(Math.Pow(e.X - transitions[i].Value.X, 2)
                                        + Math.Pow(e.Y - transitions[i].Value.Y, 2)) < vRad + 20)
                                    {
                                        firstSelected = transitions[i];
                                        firstClick = new Point(e.X, e.Y);//kostyl
                                        break;
                                    }
                                }
                            }
                        }
                        else if (places.Contains(firstSelected))
                        {
                            for (int i = 0; i < transitions.Count; i++)
                            {
                                if (Math.Sqrt(Math.Pow(e.X - transitions[i].Value.X, 2)
                                    + Math.Pow(e.Y - transitions[i].Value.Y, 2)) < vRad + 20)
                                {
                                    int[] v = new int[1];
                                    if (new WeightEnterForm(v, "Вага дуги").ShowDialog() == DialogResult.OK)
                                    {
                                        controller.AddConection(true, places.IndexOf(firstSelected), i, v[0]);
                                        DrawConnection(firstClick, new Point(e.X, e.Y), v[0]);
                                        connections.Add(firstClick.X + ";" + firstClick.Y + " " + e.X + ";" + e.Y + " " + v[0]);
                                    }
                                    break;
                                }
                            }
                            firstSelected = null;
                        }
                        else if (transitions.Contains(firstSelected))
                        {
                            for (int i = 0; i < places.Count; i++)
                            {
                                if (Math.Sqrt(Math.Pow(e.X - places[i].Value.X, 2)
                                    + Math.Pow(e.Y - places[i].Value.Y, 2)) < vRad + 20)
                                {
                                    int[] v = new int[1];
                                    if (new WeightEnterForm(v, "Вага дуги").ShowDialog() == DialogResult.OK)
                                    {
                                        controller.AddConection(false, transitions.IndexOf(firstSelected), i, v[0]);
                                        DrawConnection(firstClick, new Point(e.X, e.Y), v[0]);
                                        connections.Add(firstClick.X + ";" + firstClick.Y + " " + e.X + ";" + e.Y + " " + v[0]);
                                    }
                                    break;
                                }
                            }
                            firstSelected = null;
                        }
                        break;
                    }

                case 2:
                    {
                        for (int i = 0; i < places.Count; i++)
                        {
                            if (Math.Sqrt(Math.Pow(e.X - places[i].Value.X, 2)
                                + Math.Pow(e.Y - places[i].Value.Y, 2)) < 30)
                            {
                                int[] v = new int[1];
                                if (new WeightEnterForm(v, "Кількість міток").ShowDialog() == DialogResult.OK)
                                {
                                    controller.AddTokens(i, v[0]);
                                }
                                break;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        break;
                    }
            }
            pictureBox1.Refresh();
        }

        private void DrawConnection(Point p1, Point p2, int weigth)
        {
            gr.DrawLine(lPen, p1, new Point(p2.X, p2.Y));
            if (weigth > 1)
            {
                gr.DrawString(weigth + "", new Font("Arial", 12), Brushes.Black,
                new Point(Math.Min(p1.X, p2.X) + Math.Abs(p1.X - p2.X) / 2,
                Math.Min(p1.Y, p2.Y) + Math.Abs(p1.Y - p2.Y) / 2 + 10));
            }
            pictureBox1.Refresh();
        }

        private void AddPlaceElement(Point p)
        {
            places.Add(p);
            p.X -= vRad;
            p.Y -= vRad;
            gr.DrawEllipse(pPen, new Rectangle(p, new Size(vRad * 2, vRad * 2)));
            controller.AddPlaceElement();
            dgv1.Rows.Add();
            for (int i = 0; i < dgv1.Columns.Count; i++)
            {
                dgv1[i, dgv1.Rows.Count - 1].Value = 0;
            }
            WriteElementName(true, places.Count - 1);
            pictureBox1.Refresh();
        }

        private void AddTransitionElement(Point t, bool hor)
        {
            if (hor)
            {
                gr.DrawLine(tPen, new Point(t.X + vRad, t.Y), new Point(t.X - vRad, t.Y));
                horizontal.Add(transitions.Count);
            }
            else
            {
                gr.DrawLine(tPen, new Point(t.X, t.Y + vRad), new Point(t.X, t.Y - vRad));
            }
            transitions.Add(t);
            if (controller.TransitionsPresent) dgv1.Columns.Add("", "");
            dgv1.Columns[dgv1.Columns.Count - 1].Width = 50;
            for (int i = 0; i < dgv1.Rows.Count; i++)
            {
                dgv1[dgv1.Columns.Count - 1, i].Value = 0;
            }
            controller.AddTransitionElement();
            WriteElementName(false, transitions.Count - 1);
            pictureBox1.Refresh();
        }

        public void AddIndex(int rN, int cN, int val)
        {
            dgv1[cN, rN].Value = val;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                mode = 1;
            }
        }

        public void ChangeMatrixValue(int rN, int cN, int val)
        {
            dgv1[cN, rN].Value = val;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                mode = 2;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                mode = 0;
            }
        }

        public void ModifyTokens(int ind, int num)
        {
            int clearRad = vRad * 3 / 4;
            Point p = new Point(places[ind].Value.X - clearRad, places[ind].Value.Y - clearRad);
            gr.FillEllipse(Brushes.White, new Rectangle(p, new Size(clearRad * 2, clearRad * 2)));
            if (num == 1)
            {
                p = new Point(places[ind].Value.X - clearRad / 2, places[ind].Value.Y - clearRad / 2);
                gr.FillEllipse(Brushes.Black, new Rectangle(p, new Size(clearRad, clearRad)));
            }
            else if (num > 1)
            {
                gr.DrawString(num + "", new Font("Arial", 12), Brushes.Black, places[ind].Value.X - 6,
                                            places[ind].Value.Y - 12);
            }
            pictureBox1.Refresh();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                button1.Enabled = true;
                mode = 3;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (checkBox1.Enabled)
                {
                    timer1.Start();
                    checkBox1.Enabled = false;
                    numericUpDown1.Enabled = false;
                    button1.Text = "Стоп";
                    timer1.Interval = 1000 * (int)numericUpDown1.Value;
                }
                else
                {
                    timer1.Stop();
                    checkBox1.Enabled = true;
                    numericUpDown1.Enabled = true;
                    button1.Text = "Старт";
                }
            }
            else
            {
                controller.ImitationStep();
                pictureBox1.Refresh();
            }
        }

        public void MarkTransition(int ind, bool next)
        {
            Brush br = Brushes.LightBlue;
            if (next)
            {
                br = Brushes.LightGreen;
            }
            Point p = new Point(transitions[ind].Value.X - 3, transitions[ind].Value.Y - 4);
            gr.FillRectangle(br, new Rectangle(p, new Size(6, 8)));
            pictureBox1.Refresh();
        }

        public void ClearTransitionMarks()
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                Point p = new Point(transitions[i].Value.X - 3, transitions[i].Value.Y - 4);
                gr.FillRectangle(Brushes.Black, new Rectangle(p, new Size(6, 8)));
            }
            pictureBox1.Refresh();
        }

        public void WriteElementName(bool place, int ind)
        {
            if (place)
            {
                gr.DrawString("p" + (ind + 1), new Font("Arial", 12), Brushes.Black, places[ind].Value.X,
                                            places[ind].Value.Y + vRad);
            }
            else
            {
                gr.DrawString("t" + (ind + 1), new Font("Arial", 12), Brushes.Black, transitions[ind].Value.X,
                                            transitions[ind].Value.Y + vRad);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ViewSaveData viewSaveData = new ViewSaveData();
                viewSaveData.places = places;
                viewSaveData.transitions = transitions;
                viewSaveData.horizontal = horizontal;
                viewSaveData.connections = connections;
                controller.SaveData(saveFileDialog1.FileName, viewSaveData);
            }           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ViewSaveData viewSaveData;
                controller.LoadData(openFileDialog1.FileName, out viewSaveData);
                horizontal = viewSaveData.horizontal;
                connections = viewSaveData.connections;
                for (int i = 0; i < viewSaveData.places.Count; i++)
                {
                    AddPlaceElement(new Point(viewSaveData.places[i].Value.X, viewSaveData.places[i].Value.Y));
                }
                for (int i = 0, j = 0; i < viewSaveData.transitions.Count; i++)
                {
                    if (j < horizontal.Count && i == horizontal[j])
                    {
                        AddTransitionElement(new Point(viewSaveData.transitions[i].Value.X, viewSaveData.transitions[i].Value.Y), true);
                        j++;
                    }
                    else
                    {
                        AddTransitionElement(new Point(viewSaveData.transitions[i].Value.X, viewSaveData.transitions[i].Value.Y), false);
                    }
                }
                if (dgv1.ColumnCount > 1) dgv1.Columns.RemoveAt(dgv1.ColumnCount - 1);
                for (int i = 0; i < viewSaveData.connections.Count; i++)
                {
                    string[] connData = connections[i].Split(' ');
                    string[] p1Data = connData[0].Split(";");
                    string[] p2Data = connData[1].Split(";");
                    DrawConnection(new Point(Convert.ToInt32(p1Data[0]), Convert.ToInt32(p1Data[1])),
                        new Point(Convert.ToInt32(p2Data[0]), Convert.ToInt32(p2Data[1])), Convert.ToInt32(connData[2]));
                }
                controller.UpdateView();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            controller.Analyze();
        }

        public void ShowResults(bool[] results, int rank, int[,] tInv, int[,] pInv)
        {
            analyzeForm = new AnalyzeForm(results, rank, tInv, pInv);
            analyzeForm.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            controller.ImitationStep();
            pictureBox1.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                numericUpDown1.Enabled = true;
                button1.Text = "Старт";
            }
            else
            {
                numericUpDown1.Enabled = false;
                button1.Text = "Наступний крок";
            }
        }
        public AnalysisType GetAnalysisMethod()
        {
            AnalysisTypeClassWrapper at = new AnalysisTypeClassWrapper();
            at.analysisType = AnalysisType.None;
            MethodChooseForm mcf = new MethodChooseForm(at);
            mcf.ShowDialog();
            return at.analysisType;
        }
    }
}