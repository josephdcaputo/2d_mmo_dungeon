﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ReadyAllPartners : ActionNode
{
    protected override void OnStart()
    {
 
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
       
        if(context.gameObject.GetComponent<Multiboss>() == null){
            Debug.LogError("ReadyAllPartners on GameObject, "+ context.gameObject.name + ",  with no Multiboss component");
            return State.Failure;
        }
        
        context.gameObject.GetComponent<Multiboss>().ReadyAllPartners();
        return State.Success;
        
    }
}
