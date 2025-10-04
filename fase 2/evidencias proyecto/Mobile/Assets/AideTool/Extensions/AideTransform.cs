using UnityEngine;

namespace AideTool
{
    public static class AideTransform
    {
        public static void DestroyChildren(this Transform transform)
        {
            int len = transform.childCount - 1;
            for(int i = len; i >= 0; i--)
                Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
