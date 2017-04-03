using System;

namespace NDeepSubrogate.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DeepSubrogateAttribute : Attribute
    {
        public bool Enabled { get; set; } = true;
    }
}
