using ARWNI2S.Infrastructure.Engine;

namespace ARWNI2S.Engine
{
    internal class DefaultConnection //: IConnection
    {
        private IFeatureCollection features;

        public DefaultConnection(IFeatureCollection features)
        {
            this.features = features;
        }
    }

    //public class RelayPipeConnection : PipeConnection
    //{
    //    static ConnectionOptions RebuildOptionsWithPipes(ConnectionOptions options, Pipe pipeIn, Pipe pipeOut)
    //    {
    //        options.Input = pipeIn;
    //        options.Output = pipeOut;
    //        return options;
    //    }

    //    public RelayPipeConnection(ConnectionOptions options, Pipe pipeIn, Pipe pipeOut)
    //        : base(RebuildOptionsWithPipes(options, pipeIn, pipeOut))
    //    {

    //    }

    //    protected override void Close()
    //    {
    //        Input.Writer.Complete();
    //        Output.Writer.Complete();
    //    }

    //    protected override async ValueTask<int> SendOverIOAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    //    {
    //        var writer = OutputWriter;
    //        var total = 0;

    //        foreach (var data in buffer)
    //        {
    //            var result = await writer.WriteAsync(data, cancellationToken);

    //            if (result.IsCompleted)
    //                total += data.Length;
    //            else if (result.IsCanceled)
    //                break;
    //        }

    //        return total;
    //    }

    //    protected override ValueTask<int> FillPipeWithDataAsync(Memory<byte> memory, CancellationToken cancellationToken)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}

    //internal sealed class DefaultConnectionInfo : ConnectionInfo
    //{
    //    // Lambdas hoisted to static readonly fields to improve inlining https://github.com/dotnet/roslyn/issues/13624
    //    private static readonly Func<IFeatureCollection, IHttpConnectionFeature> _newHttpConnectionFeature = f => new HttpConnectionFeature();
    //    private static readonly Func<IFeatureCollection, ITlsConnectionFeature> _newTlsConnectionFeature = f => new TlsConnectionFeature();
    //    private static readonly Func<IFeatureCollection, IConnectionLifetimeNotificationFeature> _newConnectionLifetime = f => new DefaultConnectionLifetimeNotificationFeature(f.Get<IHttpResponseFeature>());

    //    private FeatureReferences<FeatureInterfaces> _features;

    //    public DefaultConnectionInfo(IFeatureCollection features)
    //    {
    //        Initialize(features);
    //    }

    //    public void Initialize(IFeatureCollection features)
    //    {
    //        _features.Initalize(features);
    //    }

    //    public void Initialize(IFeatureCollection features, int revision)
    //    {
    //        _features.Initalize(features, revision);
    //    }

    //    public void Uninitialize()
    //    {
    //        _features = default;
    //    }

    //    private IHttpConnectionFeature HttpConnectionFeature =>
    //        _features.Fetch(ref _features.Cache.Connection, _newHttpConnectionFeature)!;

    //    private ITlsConnectionFeature TlsConnectionFeature =>
    //        _features.Fetch(ref _features.Cache.TlsConnection, _newTlsConnectionFeature)!;

    //    private IConnectionLifetimeNotificationFeature ConnectionLifetime =>
    //        _features.Fetch(ref _features.Cache.ConnectionLifetime, _newConnectionLifetime)!;

    //    /// <inheritdoc />
    //    public override string Id
    //    {
    //        get { return HttpConnectionFeature.ConnectionId; }
    //        set { HttpConnectionFeature.ConnectionId = value; }
    //    }

    //    public override IPAddress RemoteIpAddress
    //    {
    //        get { return HttpConnectionFeature.RemoteIpAddress; }
    //        set { HttpConnectionFeature.RemoteIpAddress = value; }
    //    }

    //    public override int RemotePort
    //    {
    //        get { return HttpConnectionFeature.RemotePort; }
    //        set { HttpConnectionFeature.RemotePort = value; }
    //    }

    //    public override IPAddress LocalIpAddress
    //    {
    //        get { return HttpConnectionFeature.LocalIpAddress; }
    //        set { HttpConnectionFeature.LocalIpAddress = value; }
    //    }

    //    public override int LocalPort
    //    {
    //        get { return HttpConnectionFeature.LocalPort; }
    //        set { HttpConnectionFeature.LocalPort = value; }
    //    }

    //    public override X509Certificate2 ClientCertificate
    //    {
    //        get { return TlsConnectionFeature.ClientCertificate; }
    //        set { TlsConnectionFeature.ClientCertificate = value; }
    //    }

    //    public override Task<X509Certificate2> GetClientCertificateAsync(CancellationToken cancellationToken = default)
    //    {
    //        return TlsConnectionFeature.GetClientCertificateAsync(cancellationToken);
    //    }

    //    public override void RequestClose()
    //    {
    //        ConnectionLifetime.RequestClose();
    //    }

    //    struct FeatureInterfaces
    //    {
    //        public IHttpConnectionFeature? Connection;
    //        public ITlsConnectionFeature? TlsConnection;
    //        public IConnectionLifetimeNotificationFeature? ConnectionLifetime;
    //    }
    //}
}