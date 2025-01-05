using ARWNI2S.Engine;
using ARWNI2S.Engine.Core;
using ARWNI2S.Engine.Core.Object;
using ARWNI2S.Framework.Character;

namespace ARWNI2S.Framework.Factories
{
    internal sealed class CharacterFactory<TCharacter> : ObjectFactoryBase, IActorFactory<TCharacter> where TCharacter : NI2SCharacter
    {
        public override TObject CreateInstance<TObject>()
        {
            if(!typeof(TObject).IsAssignableFrom(typeof(TCharacter)))
            {
                var factory = EngineContext.Current.Resolve<IActorFactory<TObject>>();
            }
            return (TObject)CreateInstance(typeof(TObject));
        }

        public override INiisObject CreateInstance(Type type)
        {
        }

        public TCharacter CreateInstance()
        {
        }
    }
}
