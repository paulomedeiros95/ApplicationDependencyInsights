using ApplicationDependencyInsights.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace ApplicationDependencyInsights.Core.Engine.Analyzer
{
    public class DependencyAnalyzer
    {
        public List<ClassDependency> AnalyzeSampleCode()
        {
            var dependencies = new List<ClassDependency>();

            var rootPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SampleCode");
            var fullPath = Path.GetFullPath(rootPath);

            if (!Directory.Exists(fullPath))
            {
                Console.WriteLine($"❌ Diretório não encontrado: {fullPath}");
                return dependencies;
            }

            Console.WriteLine($"📂 Analisando arquivos em: {fullPath}");

            var csFiles = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                // 🔹 Cria compilação temporária
                var compilation = CSharpCompilation.Create("Analysis")
                    .AddReferences(
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(HttpClient).Assembly.Location),
                        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
                    )
                    .AddSyntaxTrees(tree);

                var model = compilation.GetSemanticModel(tree);

                var classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var classDecl in classDecls)
                {
                    var className = classDecl.Identifier.Text;

                    // 🔸 Coleta instâncias de classes
                    var calls = classDecl.DescendantNodes()
                        .OfType<ObjectCreationExpressionSyntax>()
                        .Select(o => o.Type.ToString())
                        .Distinct()
                        .ToList();

                    // 🔍 Análise semântica de chamadas HTTP
                    bool callsHttp = false;
                    var urls = new List<string>();

                    var invocations = classDecl.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    foreach (var invocation in invocations)
                    {
                        var symbolInfo = model.GetSymbolInfo(invocation);
                        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                        {
                            var typeName = methodSymbol.ContainingType.ToString();
                            var methodName = methodSymbol.Name;

                            if (typeName == "System.Net.Http.HttpClient")
                            {
                                callsHttp = true;
                                Console.WriteLine($"🌐 Chamada de API detectada: {typeName}.{methodName}() em {Path.GetFileName(file)}");

                                // Extrai URLs hardcoded
                                foreach (var arg in invocation.ArgumentList.Arguments)
                                {
                                    if (arg.Expression is LiteralExpressionSyntax literal &&
                                        literal.IsKind(SyntaxKind.StringLiteralExpression))
                                    {
                                        var url = literal.Token.ValueText;
                                        urls.Add(url);
                                        Console.WriteLine($"   → URL detectada: {url}");
                                    }
                                }
                            }
                        }
                    }

                    dependencies.Add(new ClassDependency
                    {
                        ClassName = className,
                        Calls = calls,
                        CallsHttpApi = callsHttp,
                        CalledUrls = urls.Distinct().ToList()
                    });
                }
            }

            return dependencies;
        }
    }
}
