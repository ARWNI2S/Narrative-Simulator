namespace ARWNI2S.Engine.Hosting
{
    public class GDESKServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _localServiceProvider;
        private readonly IServiceProvider _mainServiceProvider;

        public GDESKServiceProvider(IServiceProvider localServiceProvider, IServiceProvider mainServiceProvider)
        {
            _localServiceProvider = localServiceProvider;
            _mainServiceProvider = mainServiceProvider;
        }

        public object GetService(Type serviceType)
        {
            // Primero buscar en el contenedor local
            var localService = _localServiceProvider.GetService(serviceType);
            if (localService != null)
            {
                return localService;
            }

            // Si no existe en el contenedor local, buscar en el principal
            return _mainServiceProvider.GetService(serviceType);
        }
    }
}
