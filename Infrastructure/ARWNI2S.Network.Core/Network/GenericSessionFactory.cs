using ARWNI2S.Engine.Network.Session;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Engine.Network
{
    public class GenericSessionFactory<TSession> : ISessionFactory
        where TSession : IAppSession
    {
        public Type SessionType => typeof(TSession);

        public IServiceProvider ServiceProvider { get; private set; }

        public GenericSessionFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IAppSession Create()
        {
            return ActivatorUtilities.CreateInstance<TSession>(ServiceProvider);
        }
    }
}