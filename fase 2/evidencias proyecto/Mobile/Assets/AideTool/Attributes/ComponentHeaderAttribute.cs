using System;

namespace AideTool
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ComponentHeaderAttribute : Attribute
    {
        public bool UsesDefaultHeader { get; private set; }

        public ComponentHeaderAttribute(bool usesDefaultHeader = true)
        {
            UsesDefaultHeader = usesDefaultHeader;
        }
    }
}
