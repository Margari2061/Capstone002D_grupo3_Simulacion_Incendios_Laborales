using UnityEngine;

namespace AideTool.Animation
{
    public abstract class AnimatorHandler
    {
        protected AnimatorHandler() { }

        protected Animator[] GetAnimatorArray(params Animator[] animators) => animators;
    }
}
