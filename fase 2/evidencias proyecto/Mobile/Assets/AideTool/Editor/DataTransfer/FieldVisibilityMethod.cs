using System.Reflection;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal sealed class FieldVisibilityMethod
    {
        internal MethodInfo Method { get; private set; }
        internal string[] FieldNames { get; private set; }

        internal FieldVisibilityMethod(MethodInfo method, string[] fields) 
        {
            Method= method;
            FieldNames= fields;
        }
    }
}