using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADI.AnalyzerEngine.Models
{
    public class ApplicationScanResult
    {
        public string ApplicationId { get; set; }
        public string RepositoryPath { get; set; }

        public List<ClassDependency> Classes { get; set; }
        public List<NugetDependency> NugetPackages { get; set; }
        public List<string> ExternalServices { get; set; }
    }
}
