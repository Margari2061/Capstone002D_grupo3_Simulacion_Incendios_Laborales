using UnityEngine;

namespace AideTool.Animation
{
    public sealed class Vector2ParameterHandler : AnimatorParameterHandler<Vector2>
    {
        public Vector2ParameterHandler(Animator[] animators, string xParameter, string yParameter) : base(animators, xParameter, yParameter) { }

        protected override void SetAction(Animator animator, Vector2 value)
        {
            animator.SetFloat(Hashes[0], value.x);
            animator.SetFloat(Hashes[1], value.y);
        }
    }
}
