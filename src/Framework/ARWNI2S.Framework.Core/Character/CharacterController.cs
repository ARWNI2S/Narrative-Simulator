using ARWNI2S.Engine.Core;
using ARWNI2S.Engine.Core.Object;

namespace ARWNI2S.Framework.Character
{
    public class CharacterController : NI2SObject, IController
    {
        public NI2SCharacter ControlledCharacter { get; protected set; }

        INiisActor IController.ControlledActor => ControlledCharacter;

        internal virtual void AssumeControl(NI2SCharacter character)
        {
            ControlledCharacter = character;
        }

        internal virtual void DropControl()
        {
            ControlledCharacter = null;
        }

        void IController.AssumeControl(INiisActor actor) => AssumeControl((NI2SCharacter)actor);
        void IController.DropControl() => DropControl();
    }
}
