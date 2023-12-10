using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class WeightEnterForm : Form
    {
        int[] val;

        public WeightEnterForm(int[] val_, string header)
        {
            InitializeComponent();
            val = val_;
            Text = header;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                val[0] = Convert.ToInt32(textBox1.Text);
                DialogResult = DialogResult.OK;
            }
        }
    }
}
