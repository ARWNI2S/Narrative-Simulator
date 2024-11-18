namespace ARWNI2S.Engine.Modules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EngineModuleAttribute(int stage) : Attribute
    {
        public int Stage => stage;
    }
}
