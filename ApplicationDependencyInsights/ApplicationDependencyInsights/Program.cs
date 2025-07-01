using ApplicationDependencyInsights.Core.Engine.Analyzer;
using ApplicationDependencyInsights.Core.Models;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🔍 Iniciando análise de dependências...\n");

        try
        {
            var analyzer = new DependencyAnalyzer();
            List<ClassDependency> dependencies = analyzer.AnalyzeSampleCode();

            if (dependencies.Count == 0)
            {
                Console.WriteLine("⚠️ Nenhuma dependência encontrada.");
            }
            else
            {
                ExibirResultadoConsole(dependencies);
                SalvarJson(dependencies, "dependencies.json");
                Console.WriteLine("\n✅ Análise concluída com sucesso. Arquivo 'dependencies.json' gerado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro durante análise: {ex.Message}");
        }
    }

    private static void ExibirResultadoConsole(List<ClassDependency> dependencies)
    {
        foreach (var dep in dependencies)
        {
            Console.WriteLine($"📦 Classe: {dep.ClassName}");
            foreach (var call in dep.Calls)
            {
                Console.WriteLine($"   → Usa: {call}");
            }
        }
    }

    private static void SalvarJson(List<ClassDependency> dependencies, string fileName)
    {
        string json = JsonSerializer.Serialize(dependencies, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(fileName, json);
    }
}
