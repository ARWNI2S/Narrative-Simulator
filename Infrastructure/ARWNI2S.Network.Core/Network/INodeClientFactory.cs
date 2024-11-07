namespace ARWNI2S.Engine.Network
{
    public interface INodeClientFactory
    {
        NodeClient GetOrCreateClient<TScope>();
    }
}