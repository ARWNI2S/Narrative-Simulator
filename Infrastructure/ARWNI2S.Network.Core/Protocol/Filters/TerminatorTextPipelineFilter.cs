using System.Buffers;
using System.Text;

namespace ARWNI2S.Engine.Network.Protocol.Filters
{
    public class TerminatorTextPipelineFilter : TerminatorPipelineFilter<TextPackageInfo>
    {

        public TerminatorTextPipelineFilter(ReadOnlyMemory<byte> terminator)
            : base(terminator)
        {

        }

        protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            return new TextPackageInfo { Text = buffer.GetString(Encoding.UTF8) };
        }
    }
}
