using ARWNI2S.Node.Services.Plugins;

namespace ARWNI2S.Portal.Services.Common
{
    /// <summary>
    /// Misc plugin interface. 
    /// It's used by the plugins that have a configuration page but don't fit any other category (such as payment or tax plugins)
    /// </summary>
    public partial interface IMiscPlugin : IPlugin
    {
    }
}