namespace ARWNI2S.Framework.Character.Player
{
    public class PlayerCharacterController : CharacterController
    {
        public PlayerCharacter PlayerCharacter { get => (PlayerCharacter)ControlledCharacter; protected set => ControlledCharacter = value; }

        internal void AssumeControl(PlayerCharacter character)
        {
            AssumeControl(character);
        }
    }
}
