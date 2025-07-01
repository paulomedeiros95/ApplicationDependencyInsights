using ADI.ConsoleApp.Configuration;
using ADI.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🔍 Iniciando leitura de repositórios...");

        // Carrega configuração
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var repos = configuration.GetSection("Repositories").Get<List<RepositoryOptions>>();

        if (repos is null || repos.Count == 0)
        {
            Console.WriteLine("⚠️ Nenhum repositório configurado.");
            return;
        }

        var processor = new RepositoryProcessor();

        foreach (var repo in repos)
        {
            processor.Process(repo);
        }

        Console.WriteLine("\n✅ Processamento concluído.");
    }
}
