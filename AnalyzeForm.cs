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
        public AnalyzeForm(bool[] results, int rank, int[,] tInv, int[,] pInv, List<int> notCoveredIndsT, List<int> notCoveredIndsP)
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
            dgvt.Columns.Add("", "");//number
            dgvt.Columns[dgvt.ColumnCount - 1].Width = 50;
            for (int j = 0; j < tInv.GetLength(1); j++)
            {
                dgvt.Columns.Add("", "");
                dgvt.Columns[dgvt.ColumnCount - 1].Width = 30;
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
            for (int j = 0; j < tInv.GetLength(1); j++)
            {
                for (int i = 0; i < tInv.GetLength(0); i++)
                {
                    dgvt[j + 1, i].Value = tInv[i, j];
                    dgvt[0, i].Value = "t" + (i + 1);
                    dgvt[0, i].Style.BackColor = Color.Gray;
                }
            }
            for (int ni = 0; ni < notCoveredIndsT.Count; ni++)
            {
                for (int r = 0; r < dgvt.Rows.Count; r++)
                {
                    dgvt[notCoveredIndsT[ni] + 1, r].Style.BackColor = Color.PaleVioletRed;
                }
            }
            //
            dgvp.Columns.Add("", "");//number
            dgvp.Columns[dgvp.ColumnCount - 1].Width = 50;
            for (int j = 0; j < pInv.GetLength(1); j++)
            {
                dgvp.Columns.Add("", "");
                dgvp.Columns[dgvp.ColumnCount - 1].Width = 30;
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
                    dgvp[j + 1, i].Value = pInv[i, j];
                    dgvp[0, i].Value = "p" + (i + 1);
                    dgvp[0, i].Style.BackColor = Color.Gray;
                }
            }
            for (int ni = 0; ni < notCoveredIndsP.Count; ni++)
            {
                for (int r = 0; r < dgvp.Rows.Count; r++)
                {
                    dgvp[notCoveredIndsP[ni] + 1, r].Style.BackColor = Color.PaleVioletRed;
                }
            }
        }
    }
}
