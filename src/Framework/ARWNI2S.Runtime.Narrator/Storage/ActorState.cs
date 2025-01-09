using ARWNI2S.Engine.Data;

namespace ARWNI2S.Runtime.Narrator.Storage
{
    public sealed class ActorState : DataEntity
    {
        /// <summary>
        /// Gets or sets the state binary
        /// </summary>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int ActorId { get; set; }
    }
}