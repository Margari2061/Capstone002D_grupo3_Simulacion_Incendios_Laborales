using UnityEngine;

namespace AideTool.Patterns
{
    public interface IAideCore
    {
        public Transform Transform { get; }
        public GameObject GameObject { get; }
        public string Name { get; }
    }
}
