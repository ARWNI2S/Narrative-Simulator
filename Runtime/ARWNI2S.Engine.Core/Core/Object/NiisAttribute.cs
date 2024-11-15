namespace ARWNI2S.Engine.Core.Object
{
    public enum PropertyUsage
    {
        ReadOnly,
        ReadWrite,
    }

    [Flags]
    public enum MethodUsage
    {
        Function = 1,
        Event = 2,
        Callable = 4,
        Implementable = 8,
        Native = 16,
        Pure = 32,
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public abstract class NiisAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NiisObjectAttribute : NiisAttribute { }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class NiisStructAttribute : NiisAttribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NiisPropertyAttribute : NiisAttribute
    {
        public PropertyUsage Usage { get; }
        public string Description { get; }

        public NiisPropertyAttribute(PropertyUsage usage = PropertyUsage.ReadOnly, string description = null)
        {
            Usage = usage;

            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NiisFunctionAttribute : NiisAttribute
    {
        public MethodUsage Usage { get; }
        public string Description { get; }

        public NiisFunctionAttribute(MethodUsage usage = MethodUsage.Function | MethodUsage.Callable, string description = null)
        {
            Usage = usage;

            Description = description;
        }
    }
}
