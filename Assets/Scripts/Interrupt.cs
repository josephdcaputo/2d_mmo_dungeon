using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName="Interrupt", menuName = "HBCsystem/Interrupt")]
public class Interrupt : AbilityEff
{
    public int school;
	
    public override void startEffect(Actor _target = null, NullibleVector3 _targetWP = null, Actor _caster = null, Actor _secondaryTarget = null){
       //Debug.Log("Interrupt start effect");
        target.interruptCast();
        
        
    }
    public override void clientEffect()
    {
        
    }
    public override void buffStartEffect()
    {
        
    }
    public override void buffEndEffect()
    {
       
    }
    
    public Interrupt(){
        
    }
    public override AbilityEff clone()
    {
        Interrupt temp_ref = ScriptableObject.CreateInstance(typeof (Interrupt)) as Interrupt;
        temp_ref.effectName = effectName;
        temp_ref.id = id;
        temp_ref.power = power;
        temp_ref.powerScale = powerScale;
        temp_ref.school = school;
        temp_ref.targetIsSecondary = targetIsSecondary;
        
        return temp_ref;
    }
    
}