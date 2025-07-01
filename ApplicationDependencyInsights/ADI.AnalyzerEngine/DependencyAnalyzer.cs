using ADI.AnalyzerEngine.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADI.AnalyzerEngine
{
    public class DependencyAnalyzer
    {
        public ApplicationScanResult Analyze(string repoPath, string applicationId)
        {
            var classes = new List<ClassDependency>();
            var nugets = new List<NugetDependency>();
            var allExternalUrls = new List<string>();

            var csFiles = Directory.GetFiles(repoPath, "*.cs", SearchOption.AllDirectories);
            var csprojFiles = Directory.GetFiles(repoPath, "*.csproj", SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                // Cria compilação para análise semântica
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
                    var calls = classDecl.DescendantNodes()
                        .OfType<ObjectCreationExpressionSyntax>()
                        .Select(o => o.Type.ToString())
                        .Distinct()
                        .ToList();

                    bool callsHttp = false;
                    var urls = new List<string>();

                    var invocations = classDecl.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    foreach (var invocation in invocations)
                    {
                        var symbolInfo = model.GetSymbolInfo(invocation);
                        if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                        {
                            var typeName = methodSymbol.ContainingType.ToString();
                            if (typeName == "System.Net.Http.HttpClient")
                            {
                                callsHttp = true;
                                foreach (var arg in invocation.ArgumentList.Arguments)
                                {
                                    if (arg.Expression is LiteralExpressionSyntax literal &&
                                        literal.IsKind(SyntaxKind.StringLiteralExpression))
                                    {
                                        var url = literal.Token.ValueText;
                                        urls.Add(url);
                                        allExternalUrls.Add(url);
                                    }
                                }
                            }
                        }
                    }

                    classes.Add(new ClassDependency
                    {
                        ClassName = className,
                        Calls = calls,
                        CallsHttpApi = callsHttp,
                        CalledUrls = urls.Distinct().ToList()
                    });
                }
            }

            // 📦 Lê pacotes NuGet dos arquivos .csproj
            foreach (var proj in csprojFiles)
            {
                var lines = File.ReadAllLines(proj);
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("<PackageReference"))
                    {
                        var parts = line.Split("\"");
                        if (parts.Length >= 4)
                        {
                            nugets.Add(new NugetDependency
                            {
                                PackageName = parts[1],
                                Version = parts[3]
                            });
                        }
                    }
                }
            }

            return new ApplicationScanResult
            {
                ApplicationId = applicationId,
                RepositoryPath = repoPath,
                Classes = classes,
                NugetPackages = nugets.DistinctBy(p => p.PackageName).ToList(),
                ExternalServices = allExternalUrls.Distinct().ToList()
            };
        }
    }
}
