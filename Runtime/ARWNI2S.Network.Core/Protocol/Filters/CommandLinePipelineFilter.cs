namespace ARWNI2S.Engine.Network.Protocol.Filters
{
    public class CommandLinePipelineFilter : TerminatorPipelineFilter<StringPackageInfo>
    {
        public CommandLinePipelineFilter()
            : base("\r\n"u8.ToArray())
        {

        }
    }
}
