using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName="CasterKnockback", menuName = "HBCsystem/CasterKnockback")]
public class CasterKnockback : AbilityEff
{   
    public int school = -1;
    public override void startEffect(Actor _target = null, NullibleVector3 _targetWP = null, Actor _caster = null, Actor _secondaryTarget = null){
       
       Vector2 force = new Vector2();
       force = _target.transform.position - _caster.transform.position;
       force.Normalize();
       force *= power;

        _target.GetComponent<Rigidbody2D>().AddForce(force);
       
    }
    public CasterKnockback(string _effectName, int _id = -1, float _power = 0, int _school = -1){
        effectName = _effectName;
        id = _id;
        power = _power;
        school = _school;
    }
    public CasterKnockback(){}
    public override AbilityEff clone()
    {
        CasterKnockback temp_ref = ScriptableObject.CreateInstance(typeof (CasterKnockback)) as CasterKnockback;
        temp_ref.effectName = effectName;
        temp_ref.id = id;
        temp_ref.power = power;
        temp_ref.school = school;
        temp_ref.targetIsSecondary = targetIsSecondary;

        return temp_ref;
    }
}
