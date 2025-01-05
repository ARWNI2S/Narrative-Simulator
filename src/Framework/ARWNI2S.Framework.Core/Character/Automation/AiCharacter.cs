namespace ARWNI2S.Framework.Character.Automation
{
    public class AiCharacter : NI2SCharacter
    {
        public new AiCharacterController Controller { get => (AiCharacterController)base.Controller; protected set => base.Controller = value; }

        public void SetController(AiCharacterController controller)
        {
            base.SetController(controller);
        }

        public override void SetController(CharacterController controller)
        {
            if (controller is not AiCharacterController aiCharacterController)
                throw new InvalidCastException($"A {nameof(AiCharacter)} can only be controlled by a {nameof(AiCharacterController)}");

            SetController(aiCharacterController);
        }

    }
}
