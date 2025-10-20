using AideTool.Animation;
using UnityEngine;

public class ExtinguisherAnimatorHandler : AnimatorHandler
{
    public readonly BoolParameterHandler Active;

    public ExtinguisherAnimatorHandler(Animator animator)
    {
        Active = new(GetAnimatorArray(animator), "Active");
    }
}
