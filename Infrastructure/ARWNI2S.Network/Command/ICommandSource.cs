namespace ARWNI2S.Engine.Network.Command
{
    public interface ICommandSource
    {
        IEnumerable<Type> GetCommandTypes(Predicate<Type> criteria);
    }
}
