using ARWNI2S.Engine.Network.Session;
using ARWNI2S.Infrastructure.Network.Protocol;
using System.Text.Json;

namespace ARWNI2S.Engine.Network.Command
{
    public abstract class JsonCommand<TJsonObject> : JsonCommand<INodeSession, TJsonObject>
    {

    }

    public abstract class JsonCommand<TAppSession, TJsonObject> : ICommand<TAppSession, IStringPackage>
        where TAppSession : INodeSession
    {
        public JsonSerializerOptions JsonSerializerOptions { get; }

        public JsonCommand()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public virtual void Execute(TAppSession session, IStringPackage package)
        {
            var content = package.Body;
            ExecuteJson(session, string.IsNullOrEmpty(content) ? default : Deserialize(content));
        }

        protected abstract void ExecuteJson(TAppSession session, TJsonObject jsonObject);

        protected virtual TJsonObject Deserialize(string content)
        {
            return JsonSerializer.Deserialize<TJsonObject>(content, JsonSerializerOptions);
        }
    }

    public abstract class JsonAsyncCommand<TJsonObject> : JsonAsyncCommand<INodeSession, TJsonObject>
    {

    }

    public abstract class JsonAsyncCommand<TAppSession, TJsonObject> : IAsyncCommand<TAppSession, IStringPackage>
        where TAppSession : INodeSession
    {
        public JsonSerializerOptions JsonSerializerOptions { get; }

        public JsonAsyncCommand()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public virtual async ValueTask ExecuteAsync(TAppSession session, IStringPackage package, CancellationToken cancellationToken)
        {
            var content = package.Body;
            await ExecuteJsonAsync(session, string.IsNullOrEmpty(content) ? default : Deserialize(content), cancellationToken);
        }

        protected virtual TJsonObject Deserialize(string content)
        {
            return JsonSerializer.Deserialize<TJsonObject>(content, JsonSerializerOptions);
        }

        protected abstract ValueTask ExecuteJsonAsync(TAppSession session, TJsonObject jsonObject, CancellationToken cancellationToken);
    }
}
