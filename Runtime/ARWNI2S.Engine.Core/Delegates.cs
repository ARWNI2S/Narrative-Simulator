using ARWNI2S.Infrastructure.Entities;

namespace ARWNI2S.Engine
{
    public delegate void EventDelegate<TSender, TArgs>(TSender sender, TArgs args) where TSender : IEntity;
}
