using ADI.AnalyzerEngine.Models;
using ADI.Infrastructure.Interfaces;

namespace ADI.Infrastructure.Repositories
{
    public class MemoryDataStore : IDataStore
    {
        private static readonly MemoryDataStore _instance = new();
        public static MemoryDataStore Instance => _instance;

        private readonly Dictionary<string, ApplicationScanResult> _store = new();

        public void SaveResult(string appId, ApplicationScanResult result)
        {
            _store[appId] = result;
        }

        public ApplicationScanResult GetResult(string appId)
        {
            return _store.TryGetValue(appId, out var result) ? result : null!;
        }

        public List<ApplicationScanResult> GetAll()
        {
            return _store.Values.ToList();
        }
    }
}
