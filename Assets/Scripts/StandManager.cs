using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StandManager : MonoBehaviour
{
    [SerializeField]
    Color c_Games = new Color(138, 201, 38);
    [SerializeField]
    Color c_Anime = new Color(255, 202, 58);
    [SerializeField]
    Color c_Comics = new Color(255, 89, 94);
    [SerializeField]
    Color c_Books = new Color(25, 130, 196);

    List<Color> ColorList = null;
    [SerializeField]
    Sprite ErrorSprite = null;


    public List<GameObject> possibleStandList = new List<GameObject>();
    public Queue<GameObject> availableStandList = new Queue<GameObject>();
    public List<string> activeStandList = new List<string>();

    private GameObject placementPreview = null;
    private GameObject placementGrid = null;

    // Start is called before the first frame update
    void Start()
    {
        placementPreview = GameObject.Find("Placement_Preview");
        placementGrid = GameObject.Find("Placement_Grid");
        ColorList = new List<Color>() { c_Games, c_Anime, c_Comics, c_Books };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            resetStandList();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotateStand(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotateStand(-1);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && heldStand != null)
        {
            placeStand();
        }
    }

    private void FixedUpdate()
    {
        if (heldStand != null)
        {
            heldStand.GetComponent<StandController>().validateStands(ErrorSprite);
        }
    }

    private void rotateStand(int direction)
    {
        heldStand.transform.Rotate(Vector3.forward * direction * 90);
    }

    [SerializeField]
    private GameObject heldStand = null;
    void resetStandList()
    {
        availableStandList.Clear();
        // Calculate count here from any difficulty multiplier n crap
        int standCount = 10;
        for (int i = 0; i < standCount; i++)
        {
            GameObject stand = possibleStandList[UnityEngine.Random.Range(0, possibleStandList.Count)];
            stand.name = "Stand " + i;
            availableStandList.Enqueue(stand);
        }
        NextStand();
    }

    void placeStand()
    {
        if (heldStand.GetComponent<StandController>().validateStands(ErrorSprite) == false)
        {
            return;
        }

        activeStandList.AddRange(heldStand.GetComponent<StandController>().fetchTileStrings());

        GameObject placedStand = Instantiate(heldStand, placementGrid.transform);
        placedStand.transform.position = placementPreview.transform.position;
        placedStand.GetComponent<StandController>().updateColor(-1f, -1f, -1f, 1f);


        GameObject previousStand = placementPreview.GetComponentInChildren<StandController>().gameObject;
        Destroy(previousStand);

        NextStand();
    }

    void NextStand()
    {
        heldStand = Instantiate(availableStandList.Dequeue(), placementPreview.transform);
        heldStand.transform.localPosition = Vector3.zero + Vector3.back;
        Color selectedColor = ColorList[UnityEngine.Random.Range(0, ColorList.Count)];

        heldStand.GetComponent<StandController>().updateColor(selectedColor.r, selectedColor.g, selectedColor.b, -1f);
    }
}
