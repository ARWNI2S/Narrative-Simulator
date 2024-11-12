namespace ARWNI2S.Engine.Extensions
{
    internal static class NI2SPropertyAttributeExtensions
    {
        public static bool IsReadOnly(this NI2S_PropertyAttribute attribute) => (attribute.Usage & PropertyUsage.ReadOnly) == PropertyUsage.ReadOnly;
        public static bool IsReadWrite(this NI2S_PropertyAttribute attribute) => (attribute.Usage & PropertyUsage.ReadWrite) == PropertyUsage.ReadWrite;
    }
}
