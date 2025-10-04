using UnityEngine;

namespace AideTool.Animation
{
    public sealed class Vector3IntParameterHandler : AnimatorParameterHandler<Vector3Int>
    {
        public Vector3IntParameterHandler(Animator[] animators, string xParameter, string yParameter, string zParameter) : base(animators, xParameter, yParameter, zParameter) { }

        protected override void SetAction(Animator animator, Vector3Int value)
        {
            animator.SetInteger(Hashes[0], Mathf.RoundToInt(value.x));
            animator.SetInteger(Hashes[1], Mathf.RoundToInt(value.y));
            animator.SetInteger(Hashes[2], Mathf.RoundToInt(value.z));
        }
    }
}
