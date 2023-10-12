using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer.Models
{
    public abstract class VariatyCommon
    {
        public int Id { get; set; }

        public int Frequency { get; set; }

        public double RelativeFrequency { get; set; }

        public double ECDFValue { get; set; }
    }
}
