using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UIManager uiManager;
    public GameObject player;
    //Assume that actor get you a COPY of the abililty that they want to cast
    // or.. I could copy it for them... and replace the ref with ref keyword
    public UnityEvent<int> OnMobDeath;
    void Awake()
	{
		if(instance == null){
            instance = this;
            //DontDestroyOnLoad(gameObject);
            
        }
		else{
            Destroy(gameObject);
        }
        
			
			
	}

    void Start()
    {
        OnMobDeath.AddListener(RespawnDionysus);
        if(NetworkServer.active){
            NetworkServer.Spawn(gameObject);
            /* This doesn;t work bc this object exists before the server is started*/
        }
        else{
            
        }
        
        
    }
    
    
    void Update()
    {
    }

    public void logMobDeath(int _mobId){
        Debug.Log(MobData._inst.find(_mobId).getActorName()+ " has died! spawning new one");
    }
    public void RespawnDionysus(int _mobId){
        if(NetworkServer.active == false){
            return;
        }
        if(_mobId == 1){
            Debug.Log("Respawning Dionysus after death in 7 secs");
            StartCoroutine(spawnInXSecs(7.0f, MobData._inst.find(1).gameObject));
        }
        else{
            Debug.Log("Mob that wasn't Dio died");
        }
    }

    IEnumerator spawnInXSecs(float time, GameObject prefab){
        while(time > 0){
            Debug.Log("Respawning in.." + time.ToString());
            if(time > 1.0f){
                yield return new WaitForSeconds(1.0f);
                time = time - 1.0f;
            }
            else{
                yield return new WaitForSeconds(time);
                break;
            }
            
            
        }
        NetworkServer.Spawn(Instantiate(prefab, Vector2.zero, Quaternion.identity ));
    }
    
}
