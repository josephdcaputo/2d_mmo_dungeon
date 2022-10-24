using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    Container with references important to controlling
    and displaying unit frames properly
*/
public class Nameplate : MonoBehaviour
{
    
    public Text unitName;
    public Image healthFill;
    public Image resourceFill;
    public Slider healthBar;
    public Slider resourceBar;
    public Actor actor;
    public Vector2 offset;

    void Awake(){
        offset = new Vector2(0f, 1.5f);
    }
    void Start(){
       healthBar =  transform.GetChild(1).GetComponent<Slider>();
       resourceBar =  transform.GetChild(2).GetComponent<Slider>();
       unitName.text = actor.getActorName();
    }
    public static void Create(Actor _actor){
        Nameplate npRef = (Instantiate(UIManager.nameplatePrefab) as GameObject).GetComponentInChildren<Nameplate>();
        npRef.transform.position = _actor.transform.position + (Vector3)npRef.offset;
         npRef.actor = _actor;
    }

    void Update(){
        transform.position = actor.transform.position + (Vector3)offset;
        updateSliderHealth();
        updateSliderResource(resourceBar);
    }
    void updateSliderHealth(){
        healthBar.maxValue = actor.getMaxHealth();
        healthBar.value = actor.getHealth();
    }
    void updateSliderResource(Slider silder){
        if(actor.ResourceTypeCount() > 0){
            silder.maxValue = actor.getResourceMax(0);
            silder.value = actor.getResourceAmount(0);
        }
        
    }
}
