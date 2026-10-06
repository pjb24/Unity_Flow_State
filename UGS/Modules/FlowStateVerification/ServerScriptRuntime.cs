using System;
using System.IO;
using System.Text.RegularExpressions;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Services.CloudCode.Core;

namespace FlowState.Server
{
    internal sealed class ServerScriptRuntime
    {
        internal object Run(string endpoint, IExecutionContext context, object parameters, Func<string, int, string> getSecret)
        {
            if (context.ProjectId != ServerServices.ProjectId || context.EnvironmentId != ServerServices.EnvironmentId ||
                string.IsNullOrEmpty(context.PlayerId) || string.IsNullOrEmpty(context.ServiceToken))
                throw new InvalidOperationException("ContextMismatch");
            using ServerServices services = new ServerServices(context);
            // Never enable CLR access. Each invocation owns its interpreter and module cache.
            using Engine engine = new Engine(options => options
                .LimitMemory(64_000_000).MaxStatements(4_000_000)
                .TimeoutInterval(TimeSpan.FromMilliseconds(ServerServices.RequestBudgetMilliseconds))
                .CancellationToken(services.DeadlineToken).LimitRecursion(256));
            engine.SetValue("__source", new Func<string, string>(ReadSource));
            engine.SetValue("__random", new Func<int, string>(ServerCryptography.RandomHex));
            engine.SetValue("__hmac", new Func<string, string, string>(ServerCryptography.HmacHex));
            engine.SetValue("__equals", new Func<string, string, bool>(ServerCryptography.FixedEquals));
            engine.SetValue("__encoding", new Func<string, string, string, string>(ServerCryptography.ConvertEncoding));
            engine.SetValue("__service", new Func<string, string, string>(services.Call));
            engine.SetValue("__secret", new Func<string, string>(name => ReadSecret(name, getSecret, services)));
            string contextJson = JsonConvert.SerializeObject(new {
                projectId = context.ProjectId, environmentId = context.EnvironmentId,
                playerId = context.PlayerId, serviceToken = context.ServiceToken
            });
            try
            {
                services.ThrowIfExpired();
                engine.Execute(ReadResource("runtime.js"));
                string result = engine.Invoke("__invoke", endpoint, contextJson,
                    JsonConvert.SerializeObject(parameters)).UnwrapIfPromise().AsString();
                services.ThrowIfExpired();
                object response = JsonConvert.DeserializeObject<object>(result);
                // Safe timing only; never expose context, payload or credentials.
                // Excludes platform scheduling/startup before Run was entered.
                if (endpoint == "get-account-transfer-status" && response is JObject accountResponse)
                {
                    accountResponse["serverElapsedMilliseconds"] = services.ElapsedMilliseconds;
                    accountResponse["serverServiceCalls"] = services.ServiceCalls;
                    accountResponse["serverTimingPresent"] = true;
                }
                services.ThrowIfExpired();
                return response;
            }
            catch (Exception)
            {
                // Interpreter errors can include source arguments; never return their details.
                throw new InvalidOperationException("ServiceUnavailable");
            }
        }

        private static string ReadSecret(string name, Func<string, int, string> getSecret, ServerServices services)
        {
            if (name != "FS_TRANSFER_HMAC_VERIFICATION_V1") return "{\"ok\":false,\"status\":403}";
            try
            {
                services.ThrowIfExpired();
                string value = getSecret(name, services.RemainingMilliseconds);
                services.ThrowIfExpired();
                return JsonConvert.SerializeObject(new { ok = true, value = new { value } });
            }
            catch (Exception) { return "{\"ok\":false,\"status\":503}"; }
        }

        private static string ReadSource(string name)
        {
            if (!Regex.IsMatch(name, "^[a-z0-9-]+\\.js$")) throw new InvalidOperationException("Unsupported module");
            return ReadResource("server/" + name);
        }

        private static string ReadResource(string name)
        {
            using Stream stream = typeof(ServerScriptRuntime).Assembly.GetManifestResourceStream(name);
            if (stream == null) throw new InvalidOperationException("Embedded module missing.");
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
