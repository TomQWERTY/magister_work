using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public enum AnalysisType
    {
        Parametrical, TSS, AlOpt, None
    }

    public class AnalysisTypeClassWrapper
    {
        public AnalysisType analysisType { get; set; }
    }
}
