using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Unity.VisualScripting;

using UnityEngine;

public class StandController : MonoBehaviour
{
    Sprite initialSprite = null;
    public void SetColour(Color color)
    {
        Debug.Log($"Setting color to {color}");
        updateColor(color.r, color.g, color.b, -1f);
        SetCategoryFromColor(color);
    }


    CompositeCollider2D CC2D = new CompositeCollider2D();
    void Start()
    {
        CC2D = GetComponent<CompositeCollider2D>();
        initialSprite = GetComponentsInChildren<SpriteRenderer>()[0].sprite;
        updateColor(-1f, -1f, -1f, 0.25f);
    }
    void Update()
    {

    }

    public bool validateStands(Sprite errorSprite)
    {
        bool valid = true;
        StandValidator[] validators = GetComponentsInChildren<StandValidator>();
        for (int i = 0; i < validators.Length; i++)
        {
            if (validators[i].validateStand() == false)
            {
                validators[i].gameObject.GetComponent<SpriteRenderer>().sprite = errorSprite;
                valid = false;
            }
            else
            {
                validators[i].gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;
            }
        }

        return valid;
    }

    public List<string> fetchTileStrings()
    {
        List<string> result = new List<string>();
        StandValidator[] validators = GetComponentsInChildren<StandValidator>();
        for (int i = 0; i < validators.Length; i++)
        {
            result.Add(validators[i].generateLocationString());
        }
        return result;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }


    [ContextMenu("Set to Active")]
    public void SetStandToActive()
    {
        print("Set Stand To Active");
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer SR = spriteRenderers[i];
            print(SR.name);
            Color col = SR.color;
            col.a = 1f;
            SR.color = col;
        }
    }

    [SerializeField]
    private string standCategory;
    private Renderer standRenderer;
    public string StandCategory
    {
        get { return standCategory; }
        private set
        {
            standCategory = value;
            Debug.Log("Stand category set to: " + standCategory);
        }
    }

    public void SetColorBasedOnCategory(string category)
    {
        StandCategory = category;
        UpdateColorBasedOnCategory();
    }

    public void SetCategoryFromColor(Color color)
    {
        StandManager standManager = GameObject.Find("Stand Manager")?.GetComponent<StandManager>();
        if (standManager == null)
        {
            Debug.LogError("StandManager component not found on 'Stand Manager' object or 'Stand Manager' object not found.");
            return;
        }

        bool found = false;
        foreach (var pair in standManager.colorCategoryMap)
        {
            Debug.Log($"Comparing with map color: R={pair.Key.r}, G={pair.Key.g}, B={pair.Key.b}");

            if (Mathf.Approximately(color.r, pair.Key.r) &&
                Mathf.Approximately(color.g, pair.Key.g) &&
                Mathf.Approximately(color.b, pair.Key.b))
            {
                StandCategory = pair.Value;
                found = true;
                Debug.Log($"Color {color} is set to category: {StandCategory}");
                break;
            }
        }

        if (!found)
        {
            Debug.LogError($"Color R={color.r}, G={color.g}, B={color.b} not found in category map.");
            StandCategory = "Unknown";
        }
    }



    public void updateColor(float r, float g, float b, float a)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer SR in spriteRenderers)
        {
            if (SR != null)
            {
                Color col = SR.color;
                col.r = r != -1f ? r : col.r;
                col.g = g != -1f ? g : col.g;
                col.b = b != -1f ? b : col.b;
                col.a = a != -1f ? a : col.a;
                SR.color = col;
            }
            else
            {
                Debug.LogError("SpriteRenderer is null.");
            }
        }
    }

    private void UpdateColorBasedOnCategory()
    {
        if (standRenderer == null) return;

        switch (standCategory)
        {
            case "Economy":
                standRenderer.material.color = new Color(0.541f, 0.788f, 0.149f, 1f);
                break;
            case "Entertainment":
                standRenderer.material.color = new Color(0.096f, 0.510f, 0.769f, 1f);
                break;
            case "Art":
                standRenderer.material.color = new Color(1f, 0.349f, 0.369f, 1f);
                break;
            case "Education":
                standRenderer.material.color = new Color(1f, 0.792f, 0.227f, 1f);
                break;
            default:
                Debug.LogError("Category not recognized, cannot set color", this);
                break;
        }
    }
    private void Awake()
    {
        UpdateColorBasedOnCategory(); 

        standRenderer = GetComponent<Renderer>();
        if (standRenderer == null)
        {
            Debug.LogError("Renderer component is missing from this stand GameObject", this);
        }
    }

}