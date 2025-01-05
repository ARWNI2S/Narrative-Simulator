using ARWNI2S.Engine.Core;
using ARWNI2S.Engine.Core.Actor;

namespace ARWNI2S.Framework.Character
{
    public class NI2SCharacter : NI2SActor, IControllerTarget
    {
        public CharacterController Controller { get; protected set; }

        public virtual void SetController(CharacterController controller)
        {
            if (Controller != null)
            {
                Controller.DropControl();
                Controller = null;
            }
            controller.AssumeControl(this);
            Controller = controller;
        }

        IController IControllerTarget.Controller => Controller;
        void IControllerTarget.SetController(IController controller) => SetController((CharacterController)controller);
    }
}
