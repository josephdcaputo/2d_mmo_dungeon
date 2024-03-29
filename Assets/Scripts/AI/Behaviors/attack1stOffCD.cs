﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[Attack]
public class attack1stOffCD : ActionNode
{
    private Ability_V2 toCast = null;
    bool castStarted;
    bool castFinished;
    protected override void OnStart()
    {
        Debug.Log("attack1stOffCD START");
        toCast = null;
        castStarted = false;
        castFinished = false;
        
        if(context.controller.abilities.Count > 0)
        {
            foreach (Ability_V2 a in context.controller.abilities)
            {
                //Debug.Log("checking " + a.getName());
                if(context.actor.checkOnCooldown(a) == false)
                {
                    toCast = a;
                    break;
                }
                else
                {
                    //Debug.Log(a.getName() + " was on cd");
                }
            }
        }
        else{
            Debug.LogError(context.actor.ActorName + " has no abilities!");
        }
        
        if(toCast == null){
            Debug.LogError(context.actor.ActorName + "Couldn't find an ability to cast");
            castFinished = true;
        }
        else{
            //Debug.Log("attack1stOffCD => " + toCast.getName());
            context.actor.onAbilityCastHooks.AddListener(checkCastedAbility);
        }
        
    }

    protected override void OnStop()
    {
        context.actor.onAbilityCastHooks.RemoveListener(checkCastedAbility);
    }

    protected override State OnUpdate()
    {
        if(toCast == null){
            
            return State.Success;
        }
        
        if(!castStarted){
            castStarted = context.actor.castAbility3(toCast, context.actor.target.transform);
        }
        if(castFinished == false){
            return State.Running;
        }
        else{

            return State.Success;
        }
    }
    void checkCastedAbility(int _id){
        if(_id == toCast.id){
            castFinished = true;
        }
    }
        
}
