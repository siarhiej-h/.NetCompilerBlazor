using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Compiler.Data
{
    public class CompilerService
    {
        private IEnumerable<PortableExecutableReference> References { get; }

        public CompilerService()
        {
            References = Directory
                .EnumerateFiles(Path.GetDirectoryName(typeof(Object).Assembly.Location), "System*.dll")
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToList();
        }

        public (bool success, long compilationTime, Assembly asm, ICollection<ErrorMarker> markers) LoadSource(string source)
        {
            var sw = Stopwatch.StartNew();

            var compilation = CSharpCompilation.Create("DynamicCode")
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
                .AddReferences(References)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp8)));

            sw.Stop();

            var diagnostics = compilation.GetDiagnostics();
            var markers = new List<ErrorMarker>(diagnostics.Length);
            bool error = false;
            foreach (Diagnostic diag in diagnostics)
            {
                var position = diag.Location.GetLineSpan();
                var marker = new ErrorMarker(
                    position.StartLinePosition.Line + 1,
                    position.StartLinePosition.Character + 1,
                    position.EndLinePosition.Line + 1,
                    position.EndLinePosition.Character + 1,
                    diag.GetMessage(CultureInfo.InvariantCulture),
                    diag.Severity);

                markers.Add(marker);

                error = diag.Severity == DiagnosticSeverity.Error || error;
            }

            long compilationTime = sw.ElapsedMilliseconds;
            if (error)
            {
                return (false, compilationTime, null, markers);
            }

            using (var outputAssembly = new MemoryStream())
            {
                compilation.Emit(outputAssembly);

                return (true, compilationTime, Assembly.Load(outputAssembly.ToArray()), markers);
            }
        }
    }
}
