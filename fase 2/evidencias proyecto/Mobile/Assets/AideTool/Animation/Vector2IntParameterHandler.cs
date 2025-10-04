using UnityEngine;

namespace AideTool.Animation
{
    public sealed class Vector2IntParameterHandler : AnimatorParameterHandler<Vector2Int>
    {
        public Vector2IntParameterHandler(Animator[] animators, string xParameter, string yParameter) : base(animators, xParameter, yParameter) { }

        protected override void SetAction(Animator animator, Vector2Int value)
        {
            animator.SetInteger(Hashes[0], Mathf.RoundToInt(value.x));
            animator.SetInteger(Hashes[1], Mathf.RoundToInt(value.y));
        }
    }
}
