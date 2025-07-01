using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationDependencyInsights.Core.Models
{
    public class ClassDependency
    {
        public string ClassName { get; set; }
        public List<string> Calls { get; set; } = new();

        public bool CallsHttpApi { get; set; } = false;
        public List<string> CalledUrls { get; set; } = new();
    }
}
