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
                at.analysisType = AnalysisType.Farkas;
            }
            else if (radioButton2.Checked)
            {
                at.analysisType = AnalysisType.TSS;
            }
            else if (radioButton3.Checked)
            {
                at.analysisType = AnalysisType.Alaivan;
            }
            else if (radioButton4.Checked)
            {
                at.analysisType = AnalysisType.TSSMod;
            }
            else if (radioButton5.Checked)
            {
                at.analysisType = AnalysisType.TSSOpt;
            }
            else
            {
                at.analysisType = AnalysisType.AlaivanOpt;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
