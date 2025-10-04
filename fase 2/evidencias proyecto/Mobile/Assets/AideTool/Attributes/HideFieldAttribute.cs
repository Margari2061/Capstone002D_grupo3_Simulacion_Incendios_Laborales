using System;

namespace AideTool
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = false)]
    public sealed class HideFieldAttribute : Attribute
    {
        public string[] FieldNames { get; private set; }

        public HideFieldAttribute(params string[] fieldNames)
        {
            FieldNames= fieldNames;
        }
    }
}