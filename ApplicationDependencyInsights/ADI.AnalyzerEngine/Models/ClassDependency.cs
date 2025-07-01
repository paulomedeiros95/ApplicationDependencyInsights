using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADI.AnalyzerEngine.Models
{
    public class ClassDependency
    {
        public string ClassName { get; set; }
        public List<string> Calls { get; set; } = new();
        public bool CallsHttpApi { get; set; }
        public List<string> CalledUrls { get; set; } = new();
    }
}
