namespace ARWNI2S.Framework.Character.Player
{
    public class PlayerCharacter : NI2SCharacter
    {
        public new PlayerCharacterController Controller { get => (PlayerCharacterController)base.Controller; protected set => base.Controller = value; }

        public void SetController(PlayerCharacterController controller)
        {
            base.SetController(controller);
        }

        public override void SetController(CharacterController controller)
        {
            if (controller is not PlayerCharacterController playerCharacterController)
                throw new InvalidCastException($"A {nameof(PlayerCharacter)} can only be controlled by a {nameof(PlayerCharacterController)}");

            SetController(playerCharacterController);
        }
    }
}
