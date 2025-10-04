using UnityEngine;

namespace AideTool.Animation
{
    public sealed class BoolParameterHandler : AnimatorParameterHandler<bool>
    {
        public BoolParameterHandler(Animator[] animators, string bParameter) : base(animators, bParameter) { }
        protected override void SetAction(Animator animator, bool value)
        {
            if (animator != null)
                animator.SetBool(Hashes[0], value);
        }
    }
}
