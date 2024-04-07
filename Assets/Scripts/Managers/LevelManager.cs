using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Aoiti.Pathfinding;

public enum DayPhase
{
    Morning, // Preparation Phase
    Noon,    // Customers Enter
    Evening, // Customers Leave
    Night    // Day Summary
}

public class LevelManager : MonoBehaviour
{
    public StandManager standManager;

    [SerializeField]
    private TextMeshProUGUI npcSpawnText;

    // Do. Not. Touch. My. Code. Until. The. Refactoring. Stage.
    // Adapt to the fucking code - use your time wisely.
    // I know my code is mid at best.
    // But the shit Ernie did to my code is stupid and unnecessary without a working product.
    // And I would know, a dude got fired specifically for that in my job.
    // - Eric

    // - Ernest its okay my ai is finished but thank you for the constructive criticism i guess  0 s 0. 

    // Presets

    // // UI Sprites
    public int[] DayPhaseDurations = { -1 /* Morning - Infinite */, 40 /* Noon */, 40 /* Evening */, -1 /* Night - Infinite */ };
    private float[] DayPhaseRatios;

    public Color[] DayPhaseFillerColors =
    {
        new Color(0f,   0f,     0f,     0f), // Morning - NONE
        new Color(6f,   214f,   160f,   1f), // Noon
        new Color(255f, 209f,   102f,   1f), // Evening
        new Color(0f,   0f,     0f,     0f)  // Night - NONE
    };

    [Tooltip("Only the first 4 sprites will be used.")]
    public Sprite[] DayPhaseSliderBackground;

    // // Lights
    Light2D globalLight;
    public Vector2[] globalLightSettings =
    {
        // X - Falloff Strength
        // Y - Falloff
        new Vector2(0.7f, 10f),
        new Vector2(0.2f, 15f),
        new Vector2(0.7f, 10f),
        new Vector2(0.9f, 5f)
    };
    Vector2 previousGlobalLightSettings;

    // Vars
    private DayPhase _currentPhase = DayPhase.Morning;
    public DayPhase CurrentPhase
    {
        get { return _currentPhase; }
        private set { _currentPhase = value; }
    }
    int currentLevel = 0;
    private float time = 0f;

    //NPC SPAWN MIN - MAX
    private int currentSpawnCount = 4;
    private const int MaxSpawnCount = 40;
    public TextMeshProUGUI happyCustomersCounterText;
    private int happyCustomersCount = 0;

    // UI Elements
    [SerializeField]
    TextMeshProUGUI UICounterText;
    Slider UIDayProgressSlider;
    TextMeshProUGUI UIDayProgressField;
    Image UIDayProgressSliderBackground;
    Image UIDayProgressSliderFill;

    //centre initally for making sure AI moved towards centre -ernest 
    public Transform centerPoint;

    // reference spawn points
    public Transform leftRoadSpawnPoint;
    public Transform rightRoadSpawnPoint;

    //npc being spawned 
    public GameObject npcPrefab;

    public TextMeshProUGUI playerHealthText;
    private int playerHealth = 100; 
    private int unhappyCustomerThreshold = 10; 

    void Start()
    {
        //ternary debug check for counter check (dedudant now) - ernest. 
        Debug.Log(happyCustomersCounterText != null ? "Happy Customers Counter Text is assigned" : "Happy Customers Counter Text is NOT assigned");
        UpdateHappyCustomersUI();


        ///Find components and GUI and Image etc
        UICounterText = GameObject.Find("LevelCounter_Text").GetComponent<TextMeshProUGUI>();
        UIDayProgressSlider = GameObject.Find("LevelCounter_Progress").GetComponent<Slider>();
        UIDayProgressField = GameObject.Find("ProgressValueField").GetComponent<TextMeshProUGUI>();
        UIDayProgressSliderBackground = GameObject.Find("LevelCounter_Background").GetComponent<Image>();
        UIDayProgressSliderFill = GameObject.Find("LevelCounter_Fill").GetComponent<Image>();
        globalLight = GameObject.Find("WorldLight").GetComponent<Light2D>();
        startDayButton = GameObject.Find("ButtonStartDay");
        endDayButton = GameObject.Find("ButtonEndDay"); 
        endDayButton.SetActive(false); //set end of day button to false.

        UICounterText.text = $"Day {currentLevel} - {(CurrentPhase)}";

        float sumOfTime = 0f;
        for (int i = 0; i < DayPhaseDurations.Length; i++) if (DayPhaseDurations[i] >= 0) sumOfTime += DayPhaseDurations[i];
        DayPhaseRatios = new float[DayPhaseDurations.Length];
        for (int i = 0; i < DayPhaseDurations.Length; i++) DayPhaseRatios[i] = DayPhaseDurations[i] / sumOfTime;



        SetUIText();
        SetUISlider();
        //find the happyCustomer Textmeshpro gui which is attached to the game object LevelManager from the given field- ernest.
        happyCustomersCounterText = GameObject.Find("happyCustomersCounterText").GetComponent<TextMeshProUGUI>();
        if (happyCustomersCounterText != null)
        {
            UpdateHappyCustomersUI(); //call update ui customer function. 
        }
        else
        {
            Debug.LogError("happyCustomersCounterText is not assigned.");
        }

        playerHealthText = GameObject.Find("PlayerHealth").GetComponent<TextMeshProUGUI>();
        if (playerHealthText != null)
        {
            playerHealthText.text = $"{playerHealth}"; // just prints out numerical value for player health. after getting the ui component from the game object.
        }
        else
        {
            Debug.LogError("PlayerHealth Text is not assigned.");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        float ratio = UpdateUISlider();
        bool progressToNext = false;
        if (ratio >= 1) progressToNext = true;

        // For Lighting Calculations
        if (ratio < 0f) ratio = time / 1f;
        ratio = Mathf.Clamp(ratio, 0f, 1f);
        Vector2 resLightSettings = Vector2.Lerp(previousGlobalLightSettings, globalLightSettings[(int)CurrentPhase], ratio); //lerping
        globalLight.falloffIntensity = resLightSettings.x;
        globalLight.shapeLightFalloffSize = resLightSettings.y;

        if (progressToNext == true) ProgressDay();

        // next day
        if (progressToNext)
        {
            ProgressDay();
            if (CurrentPhase == DayPhase.Noon)
            {
                StartDay();
            }
        }
    }
    /// <summary>
    /// NPC SPAWN STUFF
    /// </summary>

    // Day Progress Functions
    public void ProgressDay()
    {
        int nextPhase = ((int)CurrentPhase) + 1;
        if (Enum.IsDefined(typeof(DayPhase), nextPhase))
        {
            CurrentPhase = (DayPhase)nextPhase;
        }
        else
        {
            CurrentPhase = DayPhase.Morning;
            currentLevel++;
            currentSpawnCount = Mathf.Min(currentSpawnCount + 4, MaxSpawnCount);
        }
        time = 0f;
        if (CurrentPhase != DayPhase.Morning) startDayButton.SetActive(false);
        else startDayButton.SetActive(true);

        if (CurrentPhase != DayPhase.Night) endDayButton.SetActive(false);
        else endDayButton.SetActive(true);

        previousGlobalLightSettings = new Vector2(globalLight.falloffIntensity, globalLight.shapeLightFalloffSize);

        SetUIText();
        SetUISlider();

        if (CurrentPhase == DayPhase.Morning && npcSpawnText != null) //npc spawn update
        {
            npcSpawnText.text = "NPCs to Spawn Today: " + currentSpawnCount;
        }
    }

    // Button functions
    GameObject startDayButton;
    public void StartDay()
    {
        if (CurrentPhase != DayPhase.Morning) return;

        ProgressDay();

        if (CurrentPhase == DayPhase.Noon)
        {
            int npcsToSpawnToday = Mathf.Min(currentSpawnCount, MaxSpawnCount);

            for (int i = 0; i < npcsToSpawnToday; i++)
            {
                Transform spawnPoint = (i % 2 == 0) ? leftRoadSpawnPoint : rightRoadSpawnPoint;
                GameObject npcObject = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);

                NPCBehavior npcBehavior = npcObject.GetComponent<NPCBehavior>();
                if (npcBehavior != null)
                {
                    StandCategory wantedCategory = DetermineNPCWantedCategory();
                    npcBehavior.SetWantedCategory(wantedCategory);

                    StandController nearestStand = FindNearestStandOfferingCategory(wantedCategory, npcObject.transform.position);
                    if (nearestStand != null)
                    {
                        npcBehavior.SetTargetStand(nearestStand);
                        npcObject.GetComponent<MovementController2D>().SetTarget(nearestStand.transform.position);

                        // npc behavior sub to event. 
                        npcBehavior.OnReachedTarget.AddListener(() =>
                        {
                            npcBehavior.SetHappinessFull(); //setter for npc happiness on event condition. 
                            Debug.Log($"{npcObject.name} is happy!");
                        });
                    }
                    else
                    {
                        Debug.LogWarning("No stand found for category: " + wantedCategory.ToString());
                    }
                    npcBehavior.DebugWantedCategoryAndTargetStand();
                }
                else
                {
                    Debug.LogError("NPCBehavior component not found on the spawned NPC.");
                }
                npcBehavior.OnReachedTarget.AddListener(HandleHappyCustomer);
            }

            npcSpawnText.text = "NPCs to Spawn Today: " + npcsToSpawnToday;
        }
    }

    private void HandleHappyCustomer()
    {
        happyCustomersCount++;
        UpdateHappyCustomersUI();
    }

    private void UpdateHappyCustomersUI()
    {
        happyCustomersCounterText.text = $"Happy Customers: {happyCustomersCount}";
    }



    //setting npc target using the movement controller ref for having npcs track towards centre. 
    private void SetNpcTarget(GameObject npc)
    {
        MovementController2D movementController = npc.GetComponent<MovementController2D>();
        if (movementController != null)
        {
            if (centerPoint != null)
            {
                movementController.SetTarget(centerPoint.position);
            }
            else
            {
                Debug.LogError("Center point not assigned in the LevelManager.");
            }
        }
        else
        {
            Debug.LogError("MovementController2D component missing on the spawned NPC.");
        }
    }

    StandCategory DetermineNPCWantedCategory()
    {
        var values = System.Enum.GetValues(typeof(StandCategory));
        return (StandCategory)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    StandController FindNearestStandOfferingCategory(StandCategory category, Vector2 npcPosition)
    {
        float closestDistance = float.MaxValue;
        StandController closestStand = null;

        foreach (GameObject standObj in standManager.possibleStandList)
        {
            StandController standController = standObj.GetComponent<StandController>();
            if (standController != null && standController.Category == category)
            {
                float distance = Vector2.Distance(npcPosition, standObj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestStand = standController;
                }
            }
        }

        return closestStand;
    }


    GameObject endDayButton;
    /// <summary>
    /// END OF THE DAY SECTION
    /// </summary>
    public void EndDay()
    {
        if (CurrentPhase != DayPhase.Night) return;
        ProgressDay();
        
        if (MaxSpawnCount - happyCustomersCount > unhappyCustomerThreshold)
        {
            playerHealth -= 10; // health decrements by 10 
            playerHealthText.text = $"{playerHealth}"; 

            if (playerHealth <= 0)
            {
                Debug.Log("End of scene");
            }
        } else
        {
            GameObject.Find("GridManager").GetComponent<GridManager>().upgradeMap();
        }
    }

    /// <summary>
    /// UI MANAGEMENT 
    /// </summary>
    private void SetUIText()
    {
        UICounterText.text = $"Day {currentLevel.ToString().PadLeft(3, '_')} - {(CurrentPhase)}";
    }

    private void SetUISlider()
    {
        UIDayProgressSliderBackground.sprite = DayPhaseSliderBackground[(int)CurrentPhase];
        UIDayProgressSliderFill.color = DayPhaseFillerColors[(int)CurrentPhase];
    }

    private float UpdateUISlider()
    {
        float ratio = time / DayPhaseDurations[(int)CurrentPhase];

        switch (CurrentPhase)
        {
            case DayPhase.Night:
            case DayPhase.Morning:
                UIDayProgressField.text = "N/A";
                break;
            case DayPhase.Noon:
                UIDayProgressField.text = $"{(int)(ratio * 100 * DayPhaseRatios[(int)DayPhase.Noon])}%";
                break;
            case DayPhase.Evening:
                UIDayProgressField.text = $"{(100 * DayPhaseRatios[(int)DayPhase.Noon]) + (int)(ratio * 100 * DayPhaseRatios[(int)DayPhase.Evening])}%";
                break;
        }


        UIDayProgressSlider.value = ratio;
        return ratio;
    }

    // Light Management.  not done. 
    private void SetLight()
    {

    }
}

