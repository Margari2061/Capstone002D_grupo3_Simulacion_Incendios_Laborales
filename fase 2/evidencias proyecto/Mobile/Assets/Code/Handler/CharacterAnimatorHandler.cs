using AideTool.Animation;
using UnityEngine;

public class CharacterAnimatorHandler : AnimatorHandler
{
    public readonly IntParameterHandler RunningLevel;
    public readonly BoolParameterHandler Alert;

    public CharacterAnimatorHandler(Animator anim)
    {
        RunningLevel = new(GetAnimatorArray(anim), "RunningLevel");
        Alert = new(GetAnimatorArray(anim), "Alert");
    }
}

