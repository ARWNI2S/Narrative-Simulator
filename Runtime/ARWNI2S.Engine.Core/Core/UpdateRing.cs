namespace ARWNI2S.Engine.Core
{
    internal class UpdateRing : LinkedList<UpdateCycle>
    {
        public UpdateCycle Current { get; private set; }
        public UpdateCycle Next { get; private set; }
    }
}
