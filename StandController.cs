using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class StandStats
{
    public float revenue;
    public float profits;
    public float supply;
    public int inventoryCapacity;

    public StandStats(float initialRevenue, float initialProfits, float initialSupply, int inventoryCapacity)
    {
        this.revenue = initialRevenue;
        this.profits = initialProfits;
        this.supply = initialSupply;
        this.inventoryCapacity = inventoryCapacity;
    }


}


public class StandController : MonoBehaviour
{
    public StandStats Stats { get; private set; }
    Sprite initialSprite = null;
    public void SetColour(Color color)
    {
        updateColor(color);
    }


    CompositeCollider2D CC2D = new CompositeCollider2D();

    public StandCategory Category { get; private set; }

    void Start()
    {
        CC2D = GetComponent<CompositeCollider2D>();
        initialSprite = GetComponentsInChildren<SpriteRenderer>()[0].sprite;
        SetStandToActive(); 
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void InitializeStats(float initialRevenue, float initialProfits, float initialSupply)
    {
        int maxCapacity = 4; 
        int inventoryCapacity = UnityEngine.Random.Range(1, maxCapacity + 1);
        Stats = new StandStats(initialRevenue, initialProfits, initialSupply, inventoryCapacity);
        Debug.Log($"Stand initialized with Revenue: {Stats.revenue}, Profits: {Stats.profits}, Supply: {Stats.supply}, Inventory Capacity: {Stats.inventoryCapacity}");
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
            } else
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
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer SR in spriteRenderers)
        {
            Color col = SR.color;
            col.a = 1f;
            SR.color = col;
        }
    }

    public void UpdateStand(Color color, StandCategory category)
    {
        Debug.Log($"Updating stand to color: {color} for category: {category}");
        updateColor(color);
        Category = category;
    }

    public void updateColor(Color color)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
}
