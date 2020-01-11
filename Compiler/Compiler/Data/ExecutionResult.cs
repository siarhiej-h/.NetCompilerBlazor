using System.Collections.Generic;

namespace Compiler.Data
{
    public class ExecutionResult
    {
        public long CompileTimeMs { get; }

        public long ExecutionTimeMs { get; }

        public string Output { get; }

        public ICollection<ErrorMarker> Markers { get; }

        public ExecutionResult(long compilerTimeMs, long executionTimeMs, string output, ICollection<ErrorMarker> markers)
        {
            CompileTimeMs = compilerTimeMs;

            ExecutionTimeMs = executionTimeMs;

            Output = output;

            Markers = markers;
        }
    }
}
