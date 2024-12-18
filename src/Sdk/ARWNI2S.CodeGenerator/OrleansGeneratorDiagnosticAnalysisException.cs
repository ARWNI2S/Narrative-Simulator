using Microsoft.CodeAnalysis;

namespace ARWNI2S.CodeGenerator
{
    public class OrleansGeneratorDiagnosticAnalysisException : Exception
    {
        public OrleansGeneratorDiagnosticAnalysisException(Diagnostic diagnostic) : base(diagnostic.GetMessage())
        {
            Diagnostic = diagnostic;
        }

        public Diagnostic Diagnostic { get; }
    }
}
