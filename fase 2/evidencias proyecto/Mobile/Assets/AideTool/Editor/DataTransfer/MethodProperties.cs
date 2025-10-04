using System.Reflection;

namespace AideTool.ExtendedEditor.DataTransfer
{
    internal sealed class MethodProperties
    {
        internal MethodInfo Method { get; private set; }
        internal string Name { get; private set; }

        internal MethodProperties(MethodInfo method, string name)
        {
            Method = method;
            Name = name;
        }
    }
}
