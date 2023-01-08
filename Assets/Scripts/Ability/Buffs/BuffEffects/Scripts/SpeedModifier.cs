using UnityEngine;

namespace BuffSystem
{
    [CreateAssetMenu(fileName = ProjectPaths.buffEffects + "NewSpeedModifierEffect", menuName = ProjectPaths.buffEffectsMenu + "SpeedModifier")]
public class SpeedModifier : BuffEffect
{
    public override void EndEffect(IBuff t, float s)
    {
        var target = t as ISpeedModifier;
        if (target != null)
        {
            target.SpeedModifier = 1 / s;
        }
    }

    public override void StartEffect(IBuff t, float s)
    {
        var target = t as ISpeedModifier;
        if (target != null)
        {
            target.SpeedModifier = s;
        }
    }
}
