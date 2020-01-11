using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Compiler.Data
{
    public class ExecutorService
    {
        private CompilerService CompilerService { get; }

        public ExecutorService(CompilerService compilerService)
        {
            CompilerService = compilerService;
        }

        public async Task<ExecutionResult> CompileAndRun(string source)
        {
            var currentOut = Console.Out;

            try
            {
                var (success, compileTime, asm, markers) = CompilerService.LoadSource(source);
                if (success)
                {
                    var entry = asm.EntryPoint;
                    if (entry.Name == "<Main>")
                    {
                        entry = entry.DeclaringType.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    }

                    var writer = new StringWriter();
                    Console.SetOut(writer);
                    var sw = Stopwatch.StartNew();
                    await InvokeAsync(entry);
                    sw.Stop();

                    var output = writer.ToString();
                    var executionTime = sw.ElapsedMilliseconds;

                    return new ExecutionResult(compileTime, executionTime, output, markers);
                }

                return new ExecutionResult(compileTime, 0, string.Empty, markers);
            }
            catch (Exception ex)
            {
                return new ExecutionResult(0, 0, ex.Message, null);
            }
            finally
            {
                Console.SetOut(currentOut);
            }
        }

        private static async Task InvokeAsync(MethodInfo entry)
        {
            var parameters = entry.GetParameters().Length > 0 ? new object[] { new string[0] } : null;
            var result = entry.Invoke(null, parameters);
            if (result is Task t)
            {
                await t;
            }
        }
    }
}
