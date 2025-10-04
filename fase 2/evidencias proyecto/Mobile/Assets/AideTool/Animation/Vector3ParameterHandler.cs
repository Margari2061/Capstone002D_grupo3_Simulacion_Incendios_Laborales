using UnityEngine;

namespace AideTool.Animation
{
    public sealed class Vector3ParameterHandler : AnimatorParameterHandler<Vector3>
    {
        public Vector3ParameterHandler(Animator[] animators, string xParameter, string yParameter, string zParameter) : base(animators, xParameter, yParameter, zParameter) { }

        protected override void SetAction(Animator animator, Vector3 value)
        {
            animator.SetFloat(Hashes[0], value.x);
            animator.SetFloat(Hashes[1], value.y);
            animator.SetFloat(Hashes[2], value.z);
        }
    }
}
