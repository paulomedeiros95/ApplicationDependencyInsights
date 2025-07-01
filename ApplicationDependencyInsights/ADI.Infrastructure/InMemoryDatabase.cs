using ADI.AnalyzerEngine.Models;

namespace ADI.Infrastructure
{
    public class InMemoryDatabase
    {
        public Dictionary<string, ApplicationScanResult> Data { get; set; } = new();
    }
}
