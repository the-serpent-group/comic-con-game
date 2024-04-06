using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    GridManager gridManager;
    LevelManager levelManager;
    MoneyManager moneyManager;

    [SerializeField] // This is here so that there is no need to enable debug mode via Konami Code in editor.
    private bool isDebugEnabled = false;

    private void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();
    }

    void Update()
    {
        if (isDebugEnabled == true)
        {
            // Add Future Debug Code Here   ||
            //                              \/
            if (Input.GetKeyDown(KeyCode.Keypad2)) moneyManager.AddMoney(-100);
            if (Input.GetKeyDown(KeyCode.Keypad8)) moneyManager.AddMoney(100);
            if (Input.GetKeyDown(KeyCode.Keypad9)) levelManager.ProgressDay();
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) gridManager.upgradeMap();
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) gridManager.downgradeMap();
        }
    }

    public GameObject Debug_Killer;
    public GameObject Debug_Spawner;

    private void FixedUpdate()
    {
        GameObject Debug_Killer_Ref = GameObject.Find("Debug_Killer(Clone)");
        if (isDebugEnabled == true)
        {
            if (!Debug_Killer_Ref) Instantiate(Debug_Killer).transform.position = gridManager.killerOffsetPosition;
            else Debug_Killer_Ref.transform.position = gridManager.killerOffsetPosition;
        }
        if (isDebugEnabled != true)
        {
            if (Debug_Killer_Ref != null) Destroy(GameObject.Find("Debug_Killer(Clone)"));
        }

        GameObject Debug_Spawner_Ref = GameObject.Find("Debug_Spawner(Clone)");
        if (isDebugEnabled == true)
        {
            if (!Debug_Spawner_Ref) Instantiate(Debug_Spawner).transform.position = gridManager.spawnerOffsetPosition;
            else Debug_Spawner_Ref.transform.position = gridManager.spawnerOffsetPosition;
        }
        if (isDebugEnabled != true)
        {
            if (Debug_Spawner_Ref != null) Destroy(GameObject.Find("Debug_Spawner(Clone)"));
        }
    }
}
