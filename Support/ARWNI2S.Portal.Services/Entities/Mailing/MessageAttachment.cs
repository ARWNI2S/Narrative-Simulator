using ARWNI2S.Node.Core.Entities;

namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    public partial class MessageAttachment : DataEntity
    {
        public int SystemMessageId { get; set; }

        public int RewardId { get; set; }

        public bool Claimed { get; set; }

    }
}
