using UnityEngine;

namespace AideTool.Animation
{
    public sealed class TriggerParameterHandler : AnimatorParameterHandler<bool>
    {
        public TriggerParameterHandler(Animator[] animators, string tParameter) : base(animators, tParameter) { }

        protected override void SetAction(Animator animator, bool value)
        {
            if (value)
            {
                animator.SetTrigger(Hashes[0]);
                return;
            }

            animator.ResetTrigger(Hashes[0]);
        }

        public override void Set(bool value)
        {
            Debug.LogWarning("Trigger parameters can't be set");
        }

        public override void Trigger()
        {
            Value = true;
            foreach (Animator anim in m_animators)
                SetAction(anim, true);
        }

        public override void Reset()
        {
            Value = false;
            foreach (Animator anim in m_animators)
                SetAction(anim, false);
        }
    }
}
