using System.Reflection;
using System.Reflection.Emit;
using ARWNI2S.Infrastructure;

namespace ARWNI2S.Engine.Reflection
{
    internal static class InterfaceGenerator
    {
        private static readonly ModuleBuilder ModuleBuilder;

        static InterfaceGenerator()
        {
            var assemblyName = new AssemblyName(Constants.NI2S_DYNAMIC_CODE_ASSEMBLY);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        }

        public static Type GenerateInterface(Type simObjectType)
        {
            var interfaceName = $"{simObjectType.Name}_ProxyInterface";
            var typeBuilder = ModuleBuilder.DefineType(interfaceName, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var methods = simObjectType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<NI2S_MethodAttribute>() != null);

            foreach (var method in methods)
            {
                var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, method.ReturnType, parameterTypes);
            }

            return typeBuilder.CreateTypeInfo()!;
        }
    }
}
