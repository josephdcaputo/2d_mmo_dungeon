﻿using TheKiwiCoder;
using UnityEngine;

[TargetFinding]
public class TargetRandom : ActionNode
{
    public LayerMask targetMask;
    public float range;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Transform res = context.controller.FindRandomTarget(targetMask, range);
        if (res != null)
        {   
            blackboard.target = res;
            return State.Success;
        }
        return State.Failure;
    }
}
