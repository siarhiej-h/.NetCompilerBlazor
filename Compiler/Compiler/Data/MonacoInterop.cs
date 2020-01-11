using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Compiler.Data
{
    public class MonacoInterop
    {
        private readonly IJSRuntime Runtime;

        private const string MonacoContainerId = Constants.MonacoContainer;

        public MonacoInterop(IJSRuntime runtime)
        {
            Runtime = runtime;
        }

        public async Task Initialize(string initialCode, string language)
        {
            await Runtime.InvokeAsync<object>("monacoInterop.initialize", MonacoContainerId, initialCode, language);
        }

        public async Task<string> GetCode()
        {
            return await Runtime.InvokeAsync<string>("monacoInterop.getCode", MonacoContainerId);
        }

        public async Task SetCode(string code)
        {
            await Runtime.InvokeAsync<object>("monacoInterop.setCode", MonacoContainerId, code);
        }

        public async Task SetMarkers(object[] markers)
        {
            await Runtime.InvokeAsync<object>("monacoInterop.setMarkers", MonacoContainerId, markers);
        }
    }
}
