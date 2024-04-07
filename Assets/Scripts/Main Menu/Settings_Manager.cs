using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings_Manager : MonoBehaviour
{
    public bool finishedSetup = false;
    [SerializeField]
    GameObject dyslexiaSettings = null;
    MainMenuTransitions mainMenuTransitions = null;

    public TMP_FontAsset fancyFont = null;
    public TMP_FontAsset dyslexicFont = null;


    // Start is called before the first frame update
    void Start()
    {
        mainMenuTransitions = GameObject.Find("FillerCanvas").GetComponent<MainMenuTransitions>();
        int settingSet = PlayerPrefs.GetInt("SettingSet", -1);
        if (settingSet != -1)
        {
            Destroy(dyslexiaSettings);
            finishedSetup = true;
        }
    }

    float clearPrefsLimit = 2f;
    float clearPrefsPressed = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            clearPrefsPressed = clearPrefsLimit;
        }
        if (Input.GetKey(KeyCode.Delete))
        {
            clearPrefsPressed -= Time.deltaTime;
            print(clearPrefsPressed);
            if (clearPrefsPressed <= 0)
            {
                PlayerPrefs.DeleteAll();
                print("Cleared Player Prefs");
            }
        }
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            updateSettings();
        }
    }

    void FixedUpdate()
    {
        if (!finishedSetup)
        {
            dyslexiaSettings.SetActive(mainMenuTransitions.logoFinished);
        }
    }

    public void setDyslexiaToTrue()
    {
        PlayerPrefs.SetInt("DyslexicFont", 1);
        PlayerPrefs.SetInt("SettingSet", 1);
        Destroy(dyslexiaSettings);
        finishedSetup = true;
    }

    public void setDyslexiaToFalse()
    {
        PlayerPrefs.SetInt("DyslexicFont", 0);
        PlayerPrefs.SetInt("SettingSet", 1);
        Destroy(dyslexiaSettings);
        finishedSetup = true;

    }

    void updateSettings()
    {
        if (PlayerPrefs.GetInt("DyslexicFont") == 1) foreach (var text in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()) text.font = dyslexicFont;
        else foreach (var text in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()) text.font = fancyFont;
    }
}
