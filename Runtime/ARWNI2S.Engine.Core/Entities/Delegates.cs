using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Engine.Entities
{
    public delegate void EventDelegate<TSender, TArgs>(TSender sender, TArgs args) where TSender : IEntity;
}
