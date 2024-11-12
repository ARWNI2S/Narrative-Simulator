namespace ARWNI2S.Engine.Extensions
{
    internal static class NI2SMethodAttributeExtensions
    {
        public static bool IsFunction(this NI2S_MethodAttribute attribute) => (attribute.Usage & MethodUsage.Function) == MethodUsage.Function;
        public static bool IsEvent(this NI2S_MethodAttribute attribute) => (attribute.Usage & MethodUsage.Event) == MethodUsage.Event;
        public static bool IsCallable(this NI2S_MethodAttribute attribute) => (attribute.Usage & MethodUsage.Callable) == MethodUsage.Callable;
        public static bool IsImplementable(this NI2S_MethodAttribute attribute) => (attribute.Usage & MethodUsage.Implementable) == MethodUsage.Implementable;
        public static bool IsCallableFunction(this NI2S_MethodAttribute attribute) => (attribute.Usage & (MethodUsage.Function | MethodUsage.Callable)) == (MethodUsage.Function | MethodUsage.Callable);
        public static bool IsImplementableEvent(this NI2S_MethodAttribute attribute) => (attribute.Usage & (MethodUsage.Event | MethodUsage.Implementable)) == (MethodUsage.Event | MethodUsage.Implementable);
        public static bool IsNativeEvent(this NI2S_MethodAttribute attribute) => (attribute.Usage & (MethodUsage.Event | MethodUsage.Native)) == (MethodUsage.Event | MethodUsage.Native);
        public static bool IsPureFunction(this NI2S_MethodAttribute attribute) => (attribute.Usage & (MethodUsage.Function | MethodUsage.Pure)) == (MethodUsage.Function | MethodUsage.Pure);

    }
}
