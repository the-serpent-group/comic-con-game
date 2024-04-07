using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Manager : MonoBehaviour
{
    GameObject menuCanvas;
    [SerializeField]
    List<RectTransform> menuItems; // First always banner, next - buttons;

    [SerializeField]
    Vector2 screenSize;

    public float screenBorder = 50;
    //SizeRatios
    public Vector3 bannerWidth = new Vector3(900f, 0.5f, 1800f); // Min, Ratio, Max
    public Vector3 bannerHeigh = new Vector3(200f, 0.2f, 300f);  // Min, Ratio, Max
    public Vector3 buttonWidth = new Vector3(450f, 0.3f, 900f); // Min, Ratio, Max

    public float transitionTime;
    float transitionNow;
    float transitionStart = 0f;
    public GameObject menuObject = null;
    public GameObject settingsObject = null;
    Vector3 menuPositionSource = Vector3.zero;
    Vector3 menuPositionTarget = Vector3.zero;
    Vector3 settingsPositionTarget = Vector3.right * 1920f;
    Vector3 settingsPositionSource = Vector3.right * 1920f;

    void Start()
    {
        menuCanvas = GameObject.Find("MainMenu");

        menuItems.Add(menuCanvas.transform.Find("MenuBanner").GetComponent<RectTransform>());

        screenSize = menuCanvas.GetComponent<RectTransform>().sizeDelta;

        Transform menuButtons = menuCanvas.transform.Find("MenuButtons");
        for (int i = 0; i < menuButtons.childCount; i++) menuItems.Add(menuButtons.GetChild(i).GetComponent<RectTransform>());

    }

    private void Update()
    {
        if (menuObject.transform.localPosition != menuPositionTarget)
        {
            float ratio = (Time.time - transitionStart) / transitionTime;
            print($"{Time.time} - {transitionStart} / {transitionTime}");
            print(ratio);
            menuObject.transform.localPosition = new Vector3(Mathf.Lerp(menuPositionSource.x, menuPositionTarget.x, ratio), 0, 0);
            settingsObject.transform.localPosition = new Vector3(Mathf.Lerp(settingsPositionSource.x, settingsPositionTarget.x, ratio), 0, 0);
        }
    }


    public static void CloseGame()
    {
        Application.Quit();
    }


    public static void StartGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void openSettings()
    {
        menuPositionTarget = Vector3.left * screenSize.x;
        menuPositionSource = menuObject.transform.localPosition;
        settingsPositionTarget = Vector3.zero * screenSize.x;
        settingsPositionSource = settingsObject.transform.localPosition;
        transitionStart = Time.time;
    }

    public void closeSettings()
    {
        menuPositionTarget = Vector3.zero * screenSize.x;
        menuPositionSource = menuObject.transform.localPosition;
        settingsPositionTarget = Vector3.right * screenSize.x;
        settingsPositionSource = settingsObject.transform.localPosition;
        transitionStart = Time.time;
    }

}
