using System;

namespace Aethra.RayTracer.Utils
{
    public class IncorrectMethodOrderException : Exception
    {
        public string? MethodName { get; }

        public IncorrectMethodOrderException()
        {
            
        }
        
        public IncorrectMethodOrderException(string? methodName, string? message = null) : base(message)
        {
            MethodName = methodName;
        }
        
    }
}