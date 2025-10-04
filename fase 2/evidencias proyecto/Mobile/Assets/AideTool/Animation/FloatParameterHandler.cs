using UnityEngine;

namespace AideTool.Animation
{
    public sealed class FloatParameterHandler : AnimatorParameterHandler<float>
    {
        public FloatParameterHandler(Animator[] animators, string fParameter) : base(animators, fParameter) { }
        protected override void SetAction(Animator animator, float value)
        {
            if (animator != null)
                animator.SetFloat(Hashes[0], value);
        }
    }
}
