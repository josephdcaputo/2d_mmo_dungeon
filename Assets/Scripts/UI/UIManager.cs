﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/*
    Handler for all UI HUD elements
*/
public class UIManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject castBarPrefab;
    public GameObject healingGaugePrefab;
    public GameObject hotbuttonPrefab;
    public GameObject buffBarPrefab;
    public UnitFrame targetFrame; // should be TargetFrame but I didn't want to break other scenes by changing ti right now
    public TargetFrame targetOfTargetFrame;
    public GameObject classGauge;
    public List<UnitFrame> frames;
    public static Actor playerActor;
    public static Controller playerController;
    public GameObject cameraPrefab;
    public static GameObject nameplatePrefab;
    public static GameObject damageTextPrefab;
    public Color defaultColor;
    public static UnityEvent<AbilityCooldown> removeCooldownEvent = new UnityEvent<AbilityCooldown>();
    public List<Ability_V2> glowList = new List<Ability_V2>();
    public UnityEvent<Ability_V2> StartAbiltyGlow = new UnityEvent<Ability_V2>();
    public UnityEvent<Ability_V2> EndAbilityGlow = new UnityEvent<Ability_V2>();
    public UnityEvent glowChecks;
    public List<Hotbar> hotbars = new List<Hotbar>();
    public bool useMouseOver = true;
    public GameObject castBar;
    public float clickWindow = 0.66f;
    public float clickTravelWindow = 66.0f;
    public ClickData clickData0 = new ClickData();
    public ClickData clickData1 = new ClickData();
    public GameObject inGameMenu;
    public GameObject respawnButton;
    public bool draggingObject = false;
    public ClickManager clickManager;
    public GameObject buffBar;
    public bool blockCameraControls = false;

    public void SpawnBuffBar()
    {
        if(buffBar)
        {
            Destroy(buffBar);
        }
        buffBar = Instantiate(buffBarPrefab, canvas.transform);
    }

    public static UIManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("UIManager instance set");
            // DontDestroyOnLoad(gameObject);
        }
        nameplatePrefab = Resources.Load("Nameplate") as GameObject;
        damageTextPrefab = Resources.Load("DamageText") as GameObject;
        removeCooldownEvent = new UnityEvent<AbilityCooldown>();
        //hotbuttonPrefab = Resources.Load("Hotbutton 1") as GameObject;
        

    }
    void OnDestroy()
    {
        Instance = null;
        playerActor = null;
        removeCooldownEvent = null;
        nameplatePrefab = null;
        damageTextPrefab = null;
        CustomNetworkManager.singleton.GamePlayers.CollectionChanged -= AddPlayerFrame;
        respawnButton.GetComponent<Button>().onClick.RemoveListener(RespawnLocalPlayer);

    }
    // Start is called before the first frame update
    void Start()
    {
        if (cameraPrefab == null)
        {
            Debug.LogError("Please add a camera prefab to UIManager.cameraPrefab");
        }
        if(!clickManager)
        {
            clickManager = gameObject.AddComponent<ClickManager>();
        }
        /* Not sure if unit frames should have refences to actors
           like this. Later I might change this so the UIManager
         v   has refs to unitframes and correponding actors      v*/
        /*updateUnitFrame(partyFrame, partyFrame.actor);
        updateUnitFrame(partyFrame1, partyFrame1.actor);
        updateUnitFrame(partyFrame2, partyFrame2.actor);
        updateUnitFrame(partyFrame3, partyFrame3.actor);*/
        //setUpUnitFrame(partyFrame, partyFrame.actor);
        targetOfTargetFrame.castBar.gameObject.active = false;
        if (CustomNetworkManager.singleton != null)
        {
            CustomNetworkManager.singleton.GamePlayers.CollectionChanged += AddPlayerFrame;
            respawnButton.GetComponent<Button>().onClick.AddListener(RespawnLocalPlayer);
        }

    }
    void RespawnLocalPlayer()
    {
        //I really don't like this line being here
        // 1) I feel like it should be in a different method
        // 2) I wish I could do this from the host, but I can't
        //    figure out a good way to move a client with 
        //    client authoritive movement
        // 
        // It is what it is
        playerActor.transform.position = CustomNetworkManager.singleton.GetStartPosition().position;
        
        playerActor.CmdRespawnPlayer();
    }

    void AddPlayerFrame(object sender, NotifyCollectionChangedEventArgs e)
    {
        int i = 0;
        foreach (var player in CustomNetworkManager.singleton.GamePlayers)
        {
            if (frames[i] == null)
            {
                Debug.Log("frame[" + i + "] was null. Frames.Count: " + frames.Count);
            }
            updateUnitFrame(frames[i], player.GetComponent<Actor>());
            player.GetComponent<BuffHandler>().Buffs.Callback += frames[i].OnBuffsChanged;
            i++;
        }
    }
    // public void UpdateFrameBuffs(UnitFrame _frame)
    // {
    //     BuffHandler _bh = _frame.actor.GetComponent<BuffHandler>();
    //     for(int i = 0; i < _frame.buffs.Length; i++)
    //     {
    //         if(i < _bh.Buffs.Count)
    //         {
    //             _frame.buffs[i].gameObject.active = true;
    //             _frame.buffs[i].buff = _bh.Buffs[i];
    //         }
    //         else
    //         {
    //             _frame.buffs[i].gameObject.active = false;
    //         }
    //     }
    // }
    // Update the max health and current health of all ally frames
    public void UpdateAllyFrames()
    {
        foreach (UnitFrame frame in frames)
        {
            if (frame.actor != null)
            {
                frame.healthBar.maxValue = frame.actor.MaxHealth;
                frame.healthBar.value = frame.actor.Health;
                UpdateUnitFrameResource(frame);
            }
        }
    }
    void UpdateUnitFrameResource(UnitFrame unitFrame)
    {
        if (unitFrame.actor.ResourceTypeCount() > 0)
        {
            unitFrame.resourceBar.maxValue = unitFrame.actor.getResourceMax(0);
            unitFrame.resourceBar.value = unitFrame.actor.getResourceAmount(0);
            unitFrame.resourceBar.fillRect.GetComponent<Image>().color = unitFrame.actor.getResourceType(0).color;

        }
        // else{
        //     Debug.LogError(GetType() + ", skipping resources");
        // }
    }
    public void updateUnitFrame(UnitFrame unitFrame, Actor actor)
    {

        if (unitFrame.actor != actor)
        {
            if (unitFrame.GetType() == typeof(TargetFrame))
            {
                (unitFrame as TargetFrame).portrait.sprite = actor.GetComponent<SpriteRenderer>().sprite;

                if (unitFrame == targetFrame)
                {
                    if (unitFrame.actor != null)
                    {
                        unitFrame.actor.abilityHandler.OnCastStarted.RemoveListener(OnTargetCast);
                    }
                    (unitFrame as TargetFrame).castBar.caster = actor.abilityHandler;
                    actor.abilityHandler.OnCastStarted.AddListener(OnTargetCast);
                }

            }
            unitFrame.actor = actor;

        }

        if (unitFrame.actor != null)
        {

            //  Getting name
            unitFrame.unitName.text = unitFrame.actor.ActorName;
            //  Getting health current and max
            unitFrame.healthBar.maxValue = actor.MaxHealth;
            unitFrame.healthBar.value = actor.Health;

            //  Getting apropriate healthbar color from actor
            unitFrame.healthFill.color = unitFrame.actor.unitColor;
            UpdateUnitFrameResource(unitFrame);
        }
        else
        {
            unitFrame.unitName.text = "No actor";
            unitFrame.healthBar.maxValue = 1.0f;
            unitFrame.healthBar.value = 1.0f;
            unitFrame.healthFill.color = defaultColor;
        }

    }
    void OnTargetCast()
    {
        if ((targetFrame as TargetFrame).castBar.gameObject.active == false)
        {
            (targetFrame as TargetFrame).castBar.gameObject.active = true;
        }
        (targetFrame as TargetFrame).castBar.OnCastStarted();
    }
    // public void setUpUnitFrame(PointerEventData data){
    //     Debug.Log("Test");
    // }
    // public void setUpUnitFrame(UnitFrame unitFrame, Actor actor){   
    //     EventTrigger trigger = unitFrame.GetComponent<EventTrigger>();
    //     EventTrigger.Entry entry = trigger.triggers.Find(ent => ent.eventID == EventTriggerType.PointerClick);
    //     if(entry != null){
    //         Debug.LogError("UnitFrame has no PointerClick entry trigger.triggers.Count: " + trigger.triggers.Count.ToString());
    //     }
    //     entry.callback.AddListener((methodIWant) => { setTarget(); });

    // }
    void UpdateTargetFrame()
    {
        if (playerActor == null)
        {
            return;
        }

        if (playerActor.target == null)
        {
            if (targetFrame.gameObject.active)
            {
                targetFrame.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!targetFrame.gameObject.active)
            {
                Debug.Log("Setting target to active");
                targetFrame.gameObject.SetActive(true);

            }

            //Debug.Log("Updating Targetframe");
            updateUnitFrame(targetFrame, playerActor.target);
        }

    }

    public void SpawnHealingGauge()
    {
        classGauge = Instantiate(healingGaugePrefab, canvas.transform);
    }
    void UpdateTargetOfTarget()
    {
        if (targetOfTargetFrame == null)
        {
            return;
        }
        if (playerActor == null)
        {
            return;
        }

        if (playerActor.target == null || playerActor.target.target == null)
        {
            if (targetOfTargetFrame.gameObject.active)
            {
                targetOfTargetFrame.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!targetOfTargetFrame.gameObject.active)
            {
                // Debug.Log("Setting target of target to active");
                targetOfTargetFrame.gameObject.SetActive(true);

            }

            //Debug.Log("Updating Targetframe");
            updateUnitFrame(targetOfTargetFrame, playerActor.target.target);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            inGameMenu.active = !inGameMenu.active;
        }
        UpdateAllyFrames();
        UpdateTargetFrame();
        UpdateTargetOfTarget();
        CheckClassGlows();
        if (Input.GetKeyDown("page up"))
        {
            useMouseOver = !useMouseOver;
            Debug.Log("Mouseover toggled to.. " + useMouseOver);
        }
        if((playerActor && playerActor.state == ActorState.Dead) || !playerActor)
        {
            if(!respawnButton.active)
            {
                respawnButton.active = true;
            }
        }
        else 
        {
            if(respawnButton.active)
            {
                respawnButton.active = false;
            }
        }

    }
    /// <summary>
    ///	If the mouse button is active and mouse posistion moved atleast the clickTravelWindow distance
    /// </summary>
    public bool MouseButtonDrag(int _buttonId) => clickManager.MouseButtonDrag(_buttonId);
    
    /// <summary>
    ///	If the mouse button was held for less than or equal to the clickWindow
    /// </summary>
    public bool MouseButtonShort(int _buttonId) => clickManager.MouseButtonShort(_buttonId);
    public bool MouseButtonLong(int _buttonId) => !clickManager.MouseButtonShort(_buttonId);
    public bool MouseButtonClick(int _buttonId) => clickManager.MouseButtonClick(_buttonId);
    
    public Vector2 MouseButtonDragVector(int _buttonId) => clickManager.MouseButtonDragVector(_buttonId);
  
    public bool MouseButtonHold(int _buttonId) => clickManager.MouseButtonHold(_buttonId);
    
    void UpdateGlows()
    {
        int current = 0;
        foreach (Ability_V2 _a in glowList)
        {
            bool alreadyChecked = glowList.IndexOf(_a) != current;
            if (!alreadyChecked)
            {

            }
        }
    }
    void CheckClassGlows()
    {
        if (playerActor == null || playerActor.combatClass == null)
        {
            return;
        }
        int current = 0;
        foreach (GlowCheck _gc in playerActor.combatClass.classGlowChecks)
        {
            // Debug.Log("invoking class GlowCheck");
            _gc.glowChecks.Invoke();
        }
    }

    public void setTarget()
    {
        Debug.Log("Test");
    }
    public void setTargetGmObj(GameObject input)
    {
        UnitFrame uF = input.GetComponent<UnitFrame>();
        //Debug.Log(uF != null ? "uF found" : "uF NULL");
        //Debug.Log(temp != null ? "temp actor found" : "temp actor NULL");
        //Debug.Log(playerActor != null ? "playerActor assigned" : "playerActor NULL");
        //playerActor.target = (temp != null ? temp : null);
        playerActor.target = (uF != null ? uF.actor : null);
    }
    public void SpawnHotbuttons(CombatClass _combatClass)
    {

        if (hotbuttonPrefab == null)
        {
            Debug.LogError("No Hotbutton Prefab in UIManager. Can't spawn ability hotbuttons");
            return;
        }
        // if(_combatClass.defaultBinds != null){
        //     SetUpHotbars();
        //     return;
        // }

        foreach (Ability_V2 a in _combatClass.abilityList)
        {
            SpawnHotbutton(a);
        }
    }
    public void SpawnButtonsFromPrefs(TextAsset _hotbarPrefs)
    {

    }
    public Hotbutton SpawnHotbutton(Ability_V2 _ability)
    {
        if (_ability == null)
        {
            return null;
        }
        Hotbutton hotbuttonInst = null;
        hotbuttonInst = Instantiate(hotbuttonPrefab, new Vector2(0.0f, 0.0f) + (Vector2)canvas.transform.position, Quaternion.identity, canvas.transform).GetComponent<Hotbutton>();
        hotbuttonInst.ability = _ability;
        hotbuttonInst.canvas = canvas.GetComponent<Canvas>();
        hotbuttonInst.SetUp();

        return hotbuttonInst;
    }
    public void MakeGlow(Ability_V2 _ability)
    {
        if (glowList.Contains(_ability) == false)
        {
            glowList.Add(_ability);
            Debug.Log("UIManager: Making " + _ability.name + " glow");
        }
    }
    [System.Serializable]
    public class HotbarItem
    {
        public int abilityID;
        public int barNumber;
        public int slotNumber;
    }

    public class PrefsData
    {
        public HotbarItem[] HotbarItems;
    }
    public void SetUpHotbars()
    {
        /*
            In here I will grab the data from the preferences file then use it to auto set up the hotbars

            Just don't forget to call this somewhere later
        */
        Debug.Log("SetUpHotbars called");
        if (playerActor.combatClass == null)
        {
            return;
        }

        string filePath = Application.dataPath + "/" + playerActor.combatClass.name + "HotbarPrefs.json";

        if (File.Exists(filePath) == false)
        {
            // if There is a combatClass but no prefs file
            // Just spawn all it's abilities in the center of the screen
            Debug.LogError("No hotbar prefs found spawning hotbttons in middle");
            SpawnHotbuttons(playerActor.combatClass);
            return;
        }

        string jsonString = File.ReadAllText(filePath);

        PrefsData prefsData = null;
        prefsData = JsonUtility.FromJson<PrefsData>(jsonString);
        // prefsData = JsonUtility.FromJson<PrefsData>(_hotbarPrefs.text);

        if (prefsData == null)
        {
            Debug.LogError("No prefs found in: " + filePath);
        }
        else
        {
            Debug.Log("Prefs found in: " + filePath);
        }
        if (prefsData.HotbarItems == null)
        {
            Debug.Log("prefsData.HotBarItems is null");
        }


        for (int i = 0; i < prefsData.HotbarItems.Length; i++)
        {
            // Debug.Log("Player Pref read, ID: " + prefsData.HotbarItems[i].abilityID);

            Ability_V2 _ability = AbilityData.instance.find(prefsData.HotbarItems[i].abilityID);

            if (_ability == null)
            {
                continue;
            }

            Hotbutton _hotButton = SpawnHotbutton(_ability);
            if (_hotButton == null)
            {
                continue;
            }


            AddToHotbars(_hotButton, prefsData.HotbarItems[i].barNumber, prefsData.HotbarItems[i].slotNumber);



        }
    }
    public bool AddToHotbars(Hotbutton _hotbutton, int _hotbarNumber, int _slotNumber)
    {
        if (_hotbutton == null)
        {
            Debug.Log("trying to add null hotbutton to hotbar");
            return false;
        }
        if (_hotbarNumber < 0 || hotbars.Count <= _hotbarNumber)
        {
            Debug.Log("Trying to add hotbar item to a hotbar that doesn't exist, _hotbarNumber: " + _hotbarNumber);
            return false;
        }
        if (_slotNumber < 0 || hotbars[_hotbarNumber].slots.Count <= _slotNumber)
        {
            Debug.Log("Hotbar slotNumber is out of range for this hotbar, _slotNumber: " + _slotNumber + " _hotbarNumber" + _hotbarNumber);
            return false;
        }

        return hotbars[_hotbarNumber].AddHotbutton(_hotbutton.gameObject, _slotNumber);
    }

    public void WriteHotbarPrefs()
    {

        if (playerActor.combatClass == null)
        {
            return;
        }

        Debug.Log("Trying to written new keybinds..");
        List<HotbarItem> newUserPrefs = new List<HotbarItem>();


        for (int barCount = 0; barCount < hotbars.Count; barCount++)
        {
            Hotbar _hb = hotbars[barCount];

            for (int slotCount = 0; slotCount < _hb.slots.Count; slotCount++)
            {
                HotbarSlot _slot = _hb.slots[slotCount];

                if (_slot.HasHotButton)
                {
                    HotbarItem hotbarItem = new HotbarItem();

                    hotbarItem.abilityID = _slot.GetButton().ability.id;
                    hotbarItem.barNumber = barCount;
                    hotbarItem.slotNumber = slotCount;

                    newUserPrefs.Add(hotbarItem);
                }


            }
        }

        if (newUserPrefs.Count <= 0)
        {
            return;
        }

        PrefsData newPrefsData = new PrefsData();

        newPrefsData.HotbarItems = new HotbarItem[newUserPrefs.Count];
        for (int i = 0; i < newUserPrefs.Count; i++)
        {
            newPrefsData.HotbarItems[i] = newUserPrefs[i];
        }

        string newJsonString = JsonUtility.ToJson(newPrefsData);
        File.WriteAllText(Application.dataPath + "/" + playerActor.combatClass.name + "HotbarPrefs.json", newJsonString);

        Debug.Log("New Keybinds written");
        Debug.Log("newPrefs Length: " + newPrefsData.HotbarItems.Length);
    }



}
