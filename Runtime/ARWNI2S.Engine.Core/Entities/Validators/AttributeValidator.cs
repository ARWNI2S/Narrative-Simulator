namespace ARWNI2S.Engine.Entities.Validators
{
    internal sealed class AttributeValidator
    {
        public static void ValidateAttributes()
        {
            var typesWithAttribute = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(NI2SAttribute), inherit: true).Any());

            foreach (var type in typesWithAttribute)
            {
                if (!typeof(EntityBase).IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"El atributo [EntityAttribute] solo puede aplicarse a clases derivadas de EntityBase. Clase no válida: {type.FullName}");
                }
            }
        }
    }
}
