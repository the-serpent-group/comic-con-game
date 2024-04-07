using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        menuCanvas = GameObject.Find("MenuCanvas");

        menuItems.Add(menuCanvas.transform.Find("MenuBanner").GetComponent<RectTransform>());

        Transform menuButtons = menuCanvas.transform.Find("MenuButtons");
        for (int i = 0; i < menuButtons.childCount; i++) menuItems.Add(menuButtons.GetChild(i).GetComponent<RectTransform>());

        ScaleUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ScaleUI()
    {
        float currentYPos = 0;
        screenSize = menuCanvas.GetComponent<RectTransform>().sizeDelta;

        menuItems[0].sizeDelta = new Vector2(
            Mathf.Clamp(screenSize.x * bannerWidth.y, bannerWidth.x, bannerWidth.z),
            Mathf.Clamp(screenSize.y * bannerHeigh.y, bannerHeigh.x, bannerHeigh.z)
        );
        currentYPos = screenBorder + currentYPos;
        menuItems[0].anchoredPosition = new Vector3(0, -1 * (currentYPos + menuItems[0].sizeDelta.y * 0.5f), 0);

        for (int i = 1; i < menuItems.Count; i++)
        {
            currentYPos = screenBorder + menuItems[i - 1].sizeDelta.y * 0.5f + currentYPos;
            menuItems[i].sizeDelta = new Vector2(
                Mathf.Clamp(screenSize.x * buttonWidth.y, buttonWidth.x, buttonWidth.z),
                (screenSize.y - screenBorder * (3 + menuItems.Count) - menuItems[0].sizeDelta.y) / menuItems.Count
            );
            currentYPos = screenBorder + menuItems[i].sizeDelta.y * 0.5f + currentYPos;
            menuItems[i].anchoredPosition = new Vector3(0, -1 * currentYPos, 0);
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
}
