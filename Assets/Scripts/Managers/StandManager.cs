using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StandManager : MonoBehaviour
{
    [SerializeField]
    
    // Yes, very readeable and usable code.
    // "Good" job.
    // Like what the fuck - I wanted to yoink a colour for the editor and see this shit.
    // Tell me you have no respect for the team without telling me that you have no respect for the team.
    // - Eric
    Color c_Games = new Color(138f / 255f, 201f / 255f, 38f / 255f);
    Color c_Anime = new Color(255f / 255f, 202f / 255f, 58f / 255f);
    Color c_Comics = new Color(255f / 255f, 89f / 255f, 94f / 255f);
    Color c_Books = new Color(25f / 255f, 130f / 255f, 196f / 255f);

    List<Color> ColorList = null;
    [SerializeField]
    Sprite ErrorSprite = null;


    public List<GameObject> possibleStandList = new List<GameObject>();
    public Queue<GameObject> availableStandList = new Queue<GameObject>();
    public List<string> activeStandList = new List<string>();

    private GameObject placementPreview = null;
    private GameObject placementGrid = null;

    public Dictionary<Color, string> colorCategoryMap;


   
    void Start()
    {
        placementPreview = GameObject.Find("Placement_Preview");
        placementGrid = GameObject.Find("Placement_Grid");

        ColorList = new List<Color>() { c_Games, c_Anime, c_Comics, c_Books };

        
        colorCategoryMap = new Dictionary<Color, string>();
        AddColorToMap(c_Games, "Economy");
        AddColorToMap(c_Anime, "Entertainment");
        AddColorToMap(c_Comics, "Art");
        AddColorToMap(c_Books, "Education");


        foreach (var pair in colorCategoryMap)
        {
            Debug.Log($"Color {pair.Key} is mapped to category {pair.Value}");
        }

    }

    public string GetCategoryFromColor(Color color)
    {
        if (colorCategoryMap.TryGetValue(color, out string category))
        {
            return category;
        }

      
        return null;
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) 
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
        if (heldStand == null)
        {
            Debug.LogError("heldStand is not assigned.");
            return;
        }

        StandController standController = heldStand.GetComponent<StandController>();
        SpriteRenderer spriteRenderer = heldStand.GetComponent<SpriteRenderer>();

        if (standController == null || spriteRenderer == null)
        {
            Debug.LogError("heldStand does not have the necessary components.");
            return;
        }

        if (!standController.validateStands(ErrorSprite))
        {
            Debug.LogError("Stand validation failed.");
            return;
        }

        if (string.IsNullOrEmpty(standController.StandCategory))
        {
            Color standColor = spriteRenderer.color; 
            standController.SetCategoryFromColor(standColor);
        }

        string standCategory = standController.StandCategory;
        if (string.IsNullOrEmpty(standCategory))
        {
            Debug.LogError("Stand category is null or empty.");
            return;
        }

        Debug.Log($"Placing stand with category: {standCategory}");
        activeStandList.AddRange(standController.fetchTileStrings());

        GameObject placedStand = Instantiate(heldStand, placementGrid.transform);
        placedStand.transform.position = placementPreview.transform.position;
        placedStand.GetComponent<StandController>().updateColor(-1f, -1f, -1f, 1f);

        Debug.Log("Placed Stand Category: " + standCategory);

        NextStand();
    }

    void NextStand()
       
    {
        foreach (Transform child in placementPreview.transform)
        {
            Destroy(child.gameObject);
        }

        if (availableStandList.Count > 0)
        {
            GameObject standPrefab = availableStandList.Dequeue();
            GameObject newPreview = Instantiate(standPrefab, placementPreview.transform);
            newPreview.transform.localPosition = Vector3.zero;

            StandController newStandController = newPreview.GetComponent<StandController>();
            SpriteRenderer newSpriteRenderer = newPreview.GetComponent<SpriteRenderer>();

            if (newStandController == null || newSpriteRenderer == null)
            {
                Debug.LogError("New preview does not have the necessary components.");
                return;
            }

            heldStand = newPreview;
            UpdatePreviewStandColor(newSpriteRenderer); 
        }
        else
        {
            Debug.Log("No more stands available in the queue.");
        }
    }
    private void AddColorToMap(Color color, string category)
    {
        if (!colorCategoryMap.ContainsKey(color)) 
        {
            colorCategoryMap.Add(color, category);
            Debug.Log($"Color {color} is mapped to category {category}");
        }
        else
        {
            Debug.LogError($"Attempted to add duplicate color key to map: {color}");
        }
    }

    void UpdatePreviewStandColor(SpriteRenderer spriteRenderer)
    {
        
        Color selectedColor = ColorList[UnityEngine.Random.Range(0, ColorList.Count)];
        spriteRenderer.color = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0.25f); //alpha
    } 
}