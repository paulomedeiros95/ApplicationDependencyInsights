using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationDependencyInsights.SampleCode
{
    public class ServiceA
    {
        public async Task RunAsync()
        {
            // Chamada à ServiceB
            var b = new ServiceB();
            b.Execute();

            // Chamada HTTP para uma API pública
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 Conteúdo da API: {content.Substring(0, Math.Min(100, content.Length))}...");
            }
            else
            {
                Console.WriteLine($"❌ Falha na chamada de API: {response.StatusCode}");
            }
        }
    }
}
