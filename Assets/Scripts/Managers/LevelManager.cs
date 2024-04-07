using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Aoiti.Pathfinding;


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

    // - Ernest its okay my ai is abit shit but its what it is for now. 

    private enum DayPhase
    {
        Morning = 0,    // Preparation Phase
        Noon = 1,       // Customers Enter
        Evening = 2,    // Customers Leave
        Night = 3       // Day Summary
    }

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
    DayPhase currentPhase = DayPhase.Morning;
    int currentLevel = 0;
    private float time = 0f;

    //NPC SPAWN MIN - MAX
    private int currentSpawnCount = 4;
    private const int MaxSpawnCount = 40;


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

    void Start()
    {
        UICounterText = GameObject.Find("LevelCounter_Text").GetComponent<TextMeshProUGUI>();
        UIDayProgressSlider = GameObject.Find("LevelCounter_Progress").GetComponent<Slider>();
        UIDayProgressField = GameObject.Find("ProgressValueField").GetComponent<TextMeshProUGUI>();
        UIDayProgressSliderBackground = GameObject.Find("LevelCounter_Background").GetComponent<Image>();
        UIDayProgressSliderFill = GameObject.Find("LevelCounter_Fill").GetComponent<Image>();
        globalLight = GameObject.Find("WorldLight").GetComponent<Light2D>();
        startDayButton = GameObject.Find("ButtonStartDay");
        endDayButton = GameObject.Find("ButtonEndDay");
        endDayButton.SetActive(false);

        UICounterText.text = $"Day {currentLevel} - {(currentPhase)}";

        float sumOfTime = 0f;
        for (int i = 0; i < DayPhaseDurations.Length; i++) if (DayPhaseDurations[i] >= 0) sumOfTime += DayPhaseDurations[i];
        DayPhaseRatios = new float[DayPhaseDurations.Length];
        for (int i = 0; i < DayPhaseDurations.Length; i++) DayPhaseRatios[i] = DayPhaseDurations[i] / sumOfTime;



        SetUIText();
        SetUISlider();
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
        Vector2 resLightSettings = Vector2.Lerp(previousGlobalLightSettings, globalLightSettings[(int)currentPhase], ratio);
        globalLight.falloffIntensity = resLightSettings.x;
        globalLight.shapeLightFalloffSize = resLightSettings.y;

        if (progressToNext == true) ProgressDay();

        // next day
        if (progressToNext)
        {
            ProgressDay();
            if (currentPhase == DayPhase.Noon)
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
        int nextPhase = ((int)currentPhase) + 1;
        if (Enum.IsDefined(typeof(DayPhase), nextPhase))
        {
            currentPhase = (DayPhase)nextPhase;
        }
        else
        {
            currentPhase = DayPhase.Morning;
            currentLevel++;
            currentSpawnCount = 4;
        }
        time = 0f;
        if (currentPhase != DayPhase.Morning) startDayButton.SetActive(false);
        else startDayButton.SetActive(true);

        if (currentPhase != DayPhase.Night) endDayButton.SetActive(false);
        else endDayButton.SetActive(true);

        previousGlobalLightSettings = new Vector2(globalLight.falloffIntensity, globalLight.shapeLightFalloffSize);

        SetUIText();
        SetUISlider();
        if (currentPhase == DayPhase.Morning && npcSpawnText != null)
        {
            npcSpawnText.text = "NPCs to Spawn Today: " + currentSpawnCount.ToString();
        }
    }

    // Button functions
    GameObject startDayButton;
    public void StartDay()
    {
        if (currentPhase != DayPhase.Morning) return;

        ProgressDay();

        if (currentPhase == DayPhase.Noon)
        {
            int npcsToSpawnToday = Mathf.Min(currentSpawnCount, MaxSpawnCount);
            int npcsSpawnedToday = 0; 

            for (int i = 0; i < npcsToSpawnToday; i++)
            {
                Transform spawnPoint = (i % 2 == 0) ? leftRoadSpawnPoint : rightRoadSpawnPoint;
                GameObject npcObject = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
                npcsSpawnedToday++;

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
            }
            npcSpawnText.text = $"NPCs to Spawn Today: {npcsSpawnedToday}";
            Debug.Log($"NPCs spawned today: {npcsSpawnedToday}");
        }
    }

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
        if (currentPhase != DayPhase.Night) return;
        ProgressDay();
    }

    /// <summary>
    /// UI MANAGEMENT 
    /// </summary>
    private void SetUIText()
    {
        UICounterText.text = $"Day {currentLevel.ToString().PadLeft(3, '_')} - {(currentPhase)}";
    }

    private void SetUISlider()
    {
        UIDayProgressSliderBackground.sprite = DayPhaseSliderBackground[(int)currentPhase];
        UIDayProgressSliderFill.color = DayPhaseFillerColors[(int)currentPhase];
    }

    private float UpdateUISlider()
    {
        float ratio = time / DayPhaseDurations[(int)currentPhase];

        switch (currentPhase)
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

    // Light Management
    private void SetLight()
    {

    }
}

