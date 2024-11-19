using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Engine.Modules
{
    internal sealed class EngineModuleManager
    {
        private List<IEngineModule> _loadedModules = [];
        private List<IEngineModule> _allModules = [];

        public EngineModuleManager() 
        {

        }

        public void AddModule(IEngineModule module)
        {
            _loadedModules.Add(module);
        }
    }
}
