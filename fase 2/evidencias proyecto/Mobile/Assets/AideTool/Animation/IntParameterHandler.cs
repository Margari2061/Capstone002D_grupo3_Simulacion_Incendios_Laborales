using UnityEngine;

namespace AideTool.Animation
{
    public sealed class IntParameterHandler : AnimatorParameterHandler<int>
    {
        public IntParameterHandler(Animator[] animators, string iParameter) : base(animators, iParameter) { }

        protected override void SetAction(Animator animator, int value)
        {
            animator.SetInteger(Hashes[0], value);
        }
    }
}
