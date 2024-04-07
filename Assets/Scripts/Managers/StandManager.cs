using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public enum StandCategory
{
    Education,
    Art,
    Games,
    Anime,
    Comics,
    Books
}

public class StandManager : MonoBehaviour
{

    public static event Action<GameObject> OnStandPlaced;
    [SerializeField]
    Color c_Art = new Color(128f / 255f, 0f / 255f, 128f / 255f);
    [SerializeField]
    Color c_Games = new Color(138f / 255f, 201f / 255f, 38f / 255f);
    [SerializeField]
    Color c_Anime = new Color(255f / 255f, 202f / 255f, 58f / 255f);
    [SerializeField]
    Color c_Comics = new Color(255f / 255f, 89f / 255f, 94f / 255f);
    [SerializeField]
    Color c_Books = new Color(25f / 255f, 130f / 255f, 196f / 255f);
    [SerializeField]
    Color c_Education = new Color(0f / 255f, 0f / 255f, 255f / 255f);

    List<Color> ColorList = null;
    [SerializeField]
    Sprite ErrorSprite = null;


    public List<GameObject> possibleStandList = new List<GameObject>();
    public Queue<GameObject> availableStandList = new Queue<GameObject>();
    public List<string> activeStandList = new List<string>();

    private GameObject placementPreview = null;
    private GameObject placementGrid = null;
    [SerializeField]
    private Dictionary<Color, StandCategory> colorCategoryMap = new Dictionary<Color, StandCategory>();

    public StandCategory GetCategoryByColor(Color color)
    {
        if (colorCategoryMap.ContainsKey(color))
        {
            return colorCategoryMap[color];
        }
        else
        {
            Debug.LogWarning("Color not found in category map.");
            return default;
        }
    }

    void Start()
    {
        placementPreview = GameObject.Find("Placement_Preview");
        placementGrid = GameObject.Find("Placement_Grid");
        ColorList = new List<Color>() { c_Games, c_Anime, c_Comics, c_Books };

        // Initialize the colorCategoryMap
        colorCategoryMap = new Dictionary<Color, StandCategory>();
        AddColorToMap(c_Games, StandCategory.Games);
        AddColorToMap(c_Art, StandCategory.Art);
        AddColorToMap(c_Anime, StandCategory.Anime);
        AddColorToMap(c_Comics, StandCategory.Comics);
        AddColorToMap(c_Books, StandCategory.Books);
        AddColorToMap(c_Education, StandCategory.Education);
    }

    private void AddColorToMap(Color color, StandCategory category)
    {
        if (!colorCategoryMap.ContainsKey(color))
        {
            colorCategoryMap[color] = category;
        }
    }


    // Update is called once per frame
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

        Color colorToApply = heldStand.GetComponent<SpriteRenderer>().color;

        GameObject placedStand = Instantiate(heldStand, placementGrid.transform);
        placedStand.transform.position = placementPreview.transform.position;
        OnStandPlaced?.Invoke(placedStand);

        StandController standController = placedStand.GetComponent<StandController>();
        if (standController != null)
        {
            StandCategory category = heldStand.GetComponent<StandController>().Category;
            DestroyImmediate(heldStand);
            standController.UpdateStand(colorToApply, category);
            float initialRevenue = 100.0f;
            float initialProfits = 20.0f;
            float initialSupply = 500.0f;

            standController.InitializeStats(initialRevenue, initialProfits, initialSupply);
            Debug.Log($"Placed Stand: {category} with Revenue: {initialRevenue}, Profits: {initialProfits}, Supply: {initialSupply}");
        }
        else
        {
            Debug.LogError("StandController component missing on placedStand");
        }
        OnStandPlaced?.Invoke(placedStand);
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
            heldStand = Instantiate(availableStandList.Dequeue(), placementPreview.transform);
            heldStand.transform.localPosition = Vector3.zero + Vector3.back;

            StandCategory selectedCategory = (StandCategory)UnityEngine.Random.Range(0, Enum.GetNames(typeof(StandCategory)).Length);
            Color selectedColor = GetColorByCategory(selectedCategory);
            heldStand.GetComponent<StandController>().UpdateStand(selectedColor, selectedCategory);
            heldStand.SetActive(true);
            Debug.Log($"Next stand set. Category: {selectedCategory}, Color: {selectedColor}");
        }
        else
        {
        }

    }

    private Color GetColorByCategory(StandCategory category)
    {
        foreach (var pair in colorCategoryMap)
        {
            if (pair.Value == category)
            {
                Debug.Log($"Found color {pair.Key} for category {category}");
                return pair.Key;
            }
        }

        Debug.LogWarning($"Category {category} does not have an associated color. Returning default color.");
        foreach (var pair in colorCategoryMap)
        {
            Debug.Log($"Map contains color {pair.Key} for category {pair.Value}");
        }
        return Color.white; // default color if not found.
    }
}

