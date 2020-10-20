using System;

namespace Aethra.RayTracer.Utils
{
    public class IncorrectInitializationException : Exception
    {
        public string? VariableName { get; }
        public string? MethodName { get; }

        public IncorrectInitializationException()
        {
            
        }
        
        public IncorrectInitializationException(string? variableName, string? methodName, string? message = null) : base(message)
        {
            VariableName = variableName;
            MethodName = methodName;
        }
        
        public IncorrectInitializationException(string? variableName, string? message = null) : base(message)
        {
            VariableName = variableName;
        }
    }
}