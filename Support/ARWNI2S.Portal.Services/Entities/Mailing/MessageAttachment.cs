using ARWNI2S.Node.Data.Entities;

namespace ARWNI2S.Portal.Services.Entities.Mailing
{
    public partial class MessageAttachment : BaseDataEntity
    {
        public int SystemMessageId { get; set; }

        public int RewardId { get; set; }

        public bool Claimed { get; set; }

    }
}
