using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandValidator : MonoBehaviour
{
    [SerializeField]
    private string locationString = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public string generateLocationString()
    {
        Vector3 pos = transform.position;
        pos.x = (int) pos.x;
        pos.y = (int) pos.y;
        pos.z = (int) pos.z;
        locationString = $"x[{pos.x}]&y[{pos.y}]&z[{pos.z}]";
        return locationString;
    }

    public bool validateStand()
    {
        generateLocationString();
        StandManager standManager = GameObject.Find("Stand Manager").GetComponent<StandManager>();
        if (standManager.activeStandList.Find(e => e == locationString) != null) return false;
        return true;
    }

}
