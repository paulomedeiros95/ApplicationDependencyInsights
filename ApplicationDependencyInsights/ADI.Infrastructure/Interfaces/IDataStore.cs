using ADI.AnalyzerEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADI.Infrastructure.Interfaces
{
    public interface IDataStore
    {
        void SaveResult(string appId, ApplicationScanResult result);
        ApplicationScanResult GetResult(string appId);
        List<ApplicationScanResult> GetAll();
    }
}
