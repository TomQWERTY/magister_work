using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class AnalyzeForm : Form
    {
        public AnalyzeForm(bool[] results, int rank, int[,] tInv, int[,] pInv)
        {
            InitializeComponent();
            Label[] labels = new Label[6] { label7, label8, label9, label10, label11, label12 };
            for (int i = 0; i < 5; i++)
            {
                if (results[i])
                {
                    labels[i].Text = "ПІДТВЕРДЖЕНО";
                    labels[i].ForeColor = Color.Green;
                }
                else
                {
                    labels[i].Text = "НЕ ПІДТВЕРДЖЕНО";
                    labels[i].ForeColor = Color.Orange;
                }
            }
            if (results[5])
            {
                labels[5].Text = "ПОВНА";
                labels[5].ForeColor = Color.Green;
            }
            else
            {
                labels[5].Text = "НЕПОВНА";
                labels[5].ForeColor = Color.Red;
            }
            labels[5].Text += "(ранг = " + rank + ")";
            for (int j = 0; j < tInv.GetLength(1); j++)
            {
                dgvt.Columns.Add("", "");
                dgvt.Columns[dgvt.ColumnCount - 1].Width = 100;
            }
            for (int i = 0; i < tInv.GetLength(0) - 1; i++)
            {
                dgvt.Rows.Add();
            }
            dgvt.Width = 100 * dgvt.ColumnCount + 30;
            if (dgvt.Width > groupBox2.Width)
            {
                groupBox2.Width = dgvt.Width + 50;
                Width = groupBox2.Width + 50;
            }
            string[] lines = new string[tInv.GetLength(0)];
            for (int j = 0; j < tInv.GetLength(0); j++)
            {
                string line = "";
                for (int i = 0; i < tInv.GetLength(1); i++)
                {
                    line += tInv[j, i].ToString() + ", ";
                }
                lines[j] = line;
            }
            File.WriteAllLines("inv12.txt", lines);


            for (int j = 0; j < tInv.GetLength(1); j++)
            {
                for (int i = 0; i < tInv.GetLength(0); i++)
                {
                    dgvt[j, i].Value = tInv[i, j];
                }
            }
            //
            for (int j = 0; j < pInv.GetLength(1); j++)
            {
                dgvp.Columns.Add("", "");
                dgvp.Columns[dgvp.ColumnCount - 1].Width = 100;
            }
            for (int i = 0; i < pInv.GetLength(0) - 1; i++)
            {
                dgvp.Rows.Add();
            }
            dgvp.Width = 100 * dgvp.ColumnCount + 30;
            for (int j = 0; j < pInv.GetLength(1); j++)
            {
                for (int i = 0; i < pInv.GetLength(0); i++)
                {
                    dgvp[j, i].Value = pInv[i, j];
                }
            }
        }
    }
}
