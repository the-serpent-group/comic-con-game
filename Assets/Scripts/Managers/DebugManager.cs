using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    [SerializeField] // This is here so that there is no need to enable debug mode via Konami Code in editor.
    private bool isDebugEnabled = false;

    void Update()
    {
        if (isDebugEnabled == true)
        {
            // Add Future Debug Code Here   ||
            //                              \/
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                GameObject.Find("LevelManager").GetComponent<LevelManager>().ProgressDay();
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                GameObject.Find("Grid").GetComponent<GridManager>().upgradeMap();
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                GameObject.Find("Grid").GetComponent<GridManager>().downgradeMap();
            }
        }
    }
}
