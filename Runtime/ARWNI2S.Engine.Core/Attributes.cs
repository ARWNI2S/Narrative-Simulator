namespace ARWNI2S.Engine
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
    public abstract class NI2SAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NI2S_ObjectAttribute : NI2SAttribute { }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class NI2S_StructAttribute : NI2SAttribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NI2S_PropertyAttribute : NI2SAttribute
    {
        public PropertyUsage Usage { get; }
        public string Description { get; }

        public NI2S_PropertyAttribute(PropertyUsage usage = PropertyUsage.ReadOnly, string description = null)
        {
            Usage = usage;

            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NI2S_MethodAttribute : NI2SAttribute
    {
        public MethodUsage Usage { get; }
        public string Description { get; }

        public NI2S_MethodAttribute(MethodUsage usage = MethodUsage.Function | MethodUsage.Callable, string description = null)
        {
            Usage = usage;

            Description = description;
        }
    }
}
