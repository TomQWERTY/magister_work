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
    public partial class MethodChooseForm : Form
    {
        AnalysisTypeClassWrapper at;

        public MethodChooseForm(AnalysisTypeClassWrapper at)
        {
            this.at = at;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                at.analysisType = AnalysisType.Parametrical;
            }
            else if (radioButton2.Checked)
            {
                at.analysisType = AnalysisType.TSS;
            }
            else
            {
                at.analysisType = AnalysisType.AlOpt;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
