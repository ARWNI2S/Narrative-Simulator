using ARWNI2S.Engine.Network.Session;

namespace ARWNI2S.Engine.Network.Command
{
    public interface ICommand
    {
        // empty interface
    }

    public interface ICommand<TPackageInfo> : ICommand<INodeSession, TPackageInfo>
    {

    }

    public interface ICommand<TAppSession, TPackageInfo> : ICommand
        where TAppSession : INodeSession
    {
        void Execute(TAppSession session, TPackageInfo package);
    }

    public interface IAsyncCommand<TPackageInfo> : IAsyncCommand<INodeSession, TPackageInfo>
    {

    }

    public interface IAsyncCommand<TAppSession, TPackageInfo> : ICommand
        where TAppSession : INodeSession
    {
        ValueTask ExecuteAsync(TAppSession session, TPackageInfo package, CancellationToken cancellationToken);
    }
}
