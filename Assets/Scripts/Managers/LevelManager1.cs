using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private float time = 0f;
    private enum DayPhase
    {
        Morning = 0,    // Preparation Phase
        Noon = 1,       // Customers Enter
        Evening = 2,    // Customers Leave
        Night = 3       // Day Summary
    }
    
    public int[] DayPhaseDurations =
    {
        -1,             // Morning
        40,             // Noon
        40,             // Evening
        -1              // Night
    };

    [Tooltip("Only the first 4 sprites will be used.")]
    public Sprite[] DayPhaseSliderBackground;

    DayPhase currentPhase = DayPhase.Morning;
    int currentLevel = 0;

    [SerializeField]
    TextMeshProUGUI UICounterText;
    Slider UIDayProgressSlider;
    Image UIDayProgressSliderBackground;
    Image UIDayProgressSliderFill;



    void Start()
    {
        UICounterText = GameObject.Find("LevelCounter_Text").GetComponent<TextMeshProUGUI>();

        UIDayProgressSlider = GameObject.Find("LevelCounter_Progress").GetComponent<Slider>();
        UIDayProgressSliderBackground = GameObject.Find("LevelCounter_Background").GetComponent<Image>();
        UIDayProgressSliderFill = GameObject.Find("LevelCounter_Fill").GetComponent<Image>();

        UICounterText.text = $"Day {currentLevel} - {(currentPhase)}";

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
        UpdateUISlider();
    }

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
        }
        time = 0f;
        SetUIText();
        SetUISlider();
    }

    // UI Management
    private void SetUIText()
    {
        UICounterText.text = $"Day {currentLevel} - {(currentPhase)}";
    }

    private void SetUISlider()
    {
        UIDayProgressSliderBackground.sprite = DayPhaseSliderBackground[(int)currentPhase];
    }

    private void UpdateUISlider()
    {
        UIDayProgressSlider.value = time / DayPhaseDurations[(int)currentPhase];
    }
}
