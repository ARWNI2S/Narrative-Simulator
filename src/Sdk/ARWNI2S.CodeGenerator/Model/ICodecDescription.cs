using Microsoft.CodeAnalysis;

namespace ARWNI2S.CodeGenerator.Model
{
    internal interface ICopierDescription
    {
        ITypeSymbol UnderlyingType { get; }
    }
}