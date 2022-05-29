using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpellEffect
{
    /*
        These are private bc they shoud be thought about as "starting points".

        If you want an effect but, for ex., want it to last a little longer 

        you call an Actor's applySpellEffect() then manipulate the ActiveSpellEffect
        that it generates 
        
    */
    private string effectName;
    private int effectType; // 0=damage, 1=heal, 2=DoT, 3=Hot, 4= something else... tbd
    private float power;
    private float duration;
    private float tickRate; // for now will be rounded

    
public SpellEffect(){
}
public SpellEffect(string inEffectName, int inEffectType, float inPower, float inDuration, float inTickRate){
    effectName = inEffectName;
    effectType = inEffectType;
    power = inPower;
    if((inEffectType != 0) && (inEffectType != 1))
        duration = inDuration;
    else{
        duration = 0.0f;
        Debug.Log("Tried to make a damage or heal type with a durtion that isn't 0.0f. Setting duration to 0.0f");
    }
    tickRate = MathF.Round(inTickRate);

}

    public string getEffectName(){
        return effectName;
    }
    public int getEffectType(){
        return effectType;
    }
    public float getPower(){
        return power;
    }
    public float getDuration(){
        return duration;
    }
    public float getTickRate(){
        return tickRate;
    }


}
