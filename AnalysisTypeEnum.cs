using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public enum AnalysisType
    {
        Farkas, TSS, Alaivan, TSSOpt, AlaivanOpt, TSSMod, None
    }

    public class AnalysisTypeClassWrapper
    {
        public AnalysisType analysisType { get; set; }
    }
}
