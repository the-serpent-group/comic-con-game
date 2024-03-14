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
        updateColor(color.r, color.g, color.b, -1f);
    }

    CompositeCollider2D CC2D = new CompositeCollider2D();
    // Start is called before the first frame update
    void Start()
    {
        CC2D = GetComponent<CompositeCollider2D>();
        initialSprite = GetComponentsInChildren<SpriteRenderer>()[0].sprite;
        updateColor(-1f, -1f, -1f, 0.25f);
    }

    // Update is called once per frame
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

    public void updateColor(float r = -1f, float g = -1f, float b = -1f, float a = -1f)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer SR = spriteRenderers[i];
            Color col = SR.color;
            if (r != -1f) col.r = r;
            if (g != -1f) col.g = g;
            if (b != -1f) col.b = b;
            if (a != -1f) col.a = a;
            SR.color = col;
        }
    }

}
