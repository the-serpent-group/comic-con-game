using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandManager : MonoBehaviour
{

    public List<GameObject> standList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        print(standList[0]);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            
        }
    }
}
