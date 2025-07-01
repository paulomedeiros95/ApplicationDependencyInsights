using ADI.AnalyzerEngine;
using ADI.ConsoleApp.Configuration;
using ADI.Infrastructure.Repositories;
using System.Diagnostics;
using System.Text.Json;
namespace ADI.ConsoleApp.Services
{
    public class RepositoryProcessor
    {
        private readonly DependencyAnalyzer _analyzer = new();
        private readonly MemoryDataStore _store = MemoryDataStore.Instance;

        public void Process(RepositoryOptions repo)
        {
            Console.WriteLine($"\n📥 Iniciando análise para repositório: {repo.Name}");

            // Define path absoluto baseado na raiz do projeto
            var rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var localPath = Path.Combine(rootPath, "cloned", repo.Name);

            // Clona se não existir
            if (!Directory.Exists(localPath))
            {
                Console.WriteLine($"🔄 Clonando repositório para: {localPath}");

                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone \"{repo.Url}\" \"{localPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = System.Diagnostics.Process.Start(psi);

                process?.WaitForExit();

                if (!Directory.Exists(localPath))
                {
                    Console.WriteLine($"❌ Falha ao clonar o repositório '{repo.Name}'. Verifique a URL ou permissões.");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"✔️ Repositório já clonado: {localPath}");
            }

            // Faz análise
            var appId = Guid.NewGuid().ToString();
            var analysisResult = _analyzer.Analyze(localPath, appId);
            _store.SaveResult(appId, analysisResult);

            Console.WriteLine($"✅ Análise concluída para {repo.Name} → ID atribuído: {appId}");


            var jsonPath = Path.Combine(rootPath, "results", $"{repo.Name}_{appId}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);

            var json = JsonSerializer.Serialize(analysisResult, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(jsonPath, json);

            Console.WriteLine($"📄 Resultado exportado: {jsonPath}");
        }
    }
}
