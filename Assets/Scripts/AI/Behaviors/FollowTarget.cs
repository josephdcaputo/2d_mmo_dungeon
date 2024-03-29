﻿using TheKiwiCoder;
using UnityEngine;

[Movement]
public class FollowTarget : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        //Debug.Log("follow target called");
        if (Vector2.Distance(blackboard.target.position, context.transform.position) <= context.boxCollider.size.x)
        {
            context.agent.ResetPath();
            return State.Success;
        }

        if (context.agent.destination != blackboard.target.position)
        {
            context.agent.destination = blackboard.target.position;
        }

        if (context.agent.pathPending)
        {
            return State.Running;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}
