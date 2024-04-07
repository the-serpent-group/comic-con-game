using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransitions : MonoBehaviour
{
    Settings_Manager settings;

    public RectTransform canvas;

    public float logoDelay = 3f;
    public GameObject logoObject;
    public float logoDuration = 10f;
    public bool logoFinished = false;
    float logoStartTime = 0f;


    public GameObject fadeawayObject;
    RectTransform fadeawayTransform;
    float fadeawayThreshold;
    Vector3 fadeawayDirection;
    bool fadeoutFinished = false;
    public float fadeoutDuration = 5f;
    float fadeStartTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("SettingsManager").GetComponent<Settings_Manager>();
        fadeawayTransform = fadeawayObject.GetComponent<RectTransform>();
        fadeawayDirection = Vector3.left;
        fadeawayThreshold = canvas.sizeDelta.x;
        fadeawayTransform.sizeDelta = 1.5f * fadeawayThreshold * Vector2.one;
        logoStartTime = logoDelay;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (fadeoutFinished == false)
        {
            if (logoFinished == false)
            {
                float logoRatio = 1 - Mathf.Clamp((Time.time - logoStartTime) / logoDuration, 0f, 1f);

                foreach (var __child in logoObject.GetComponentsInChildren<Image>())
                {
                    __child.color = new Color(__child.color.r, __child.color.g, __child.color.b, logoRatio);
                }


                foreach (var __child in logoObject.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    __child.color = new Color(__child.color.r, __child.color.g, __child.color.b, logoRatio);
                }

                if (logoRatio <= 0)
                {
                    fadeStartTime = Time.time;
                    logoFinished = true;
                    Destroy(logoObject);
                }
            }
            else if (settings.finishedSetup)
            {
                float fadeoutRatio = Mathf.Clamp((Time.time - fadeStartTime) / fadeoutDuration, 0f, 1f);


                fadeawayTransform.transform.localPosition = new Vector3(-Mathf.Lerp(0, fadeawayThreshold * 3, fadeoutRatio), 0f, 0f);

                if (fadeoutRatio >= 1)
                {
                    fadeoutFinished = true;
                    Destroy(fadeawayObject);
                }
            }
            else
            {
                fadeStartTime = Time.time;

            }
        }

    }
}
