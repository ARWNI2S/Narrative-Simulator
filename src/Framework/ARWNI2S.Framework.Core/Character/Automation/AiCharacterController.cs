namespace ARWNI2S.Framework.Character.Automation
{
    public class AiCharacterController : CharacterController
    {
        public AiCharacter AiCharacter { get => (AiCharacter)ControlledCharacter; protected set => ControlledCharacter = value; }

        internal virtual void AssumeControl(AiCharacter character)
        {
            base.AssumeControl(character);
        }
    }
}
