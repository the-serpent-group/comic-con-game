using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Controller : MonoBehaviour
{
    public Camera playerCamera;
    // Start is called before the first frame update
    void Start()
    {


    }
    

    private void FixedUpdate()
    {
        Vector3 cursorPos = Input.mousePosition;
        Vector3 cursorCamPos = playerCamera.ScreenToWorldPoint(cursorPos);
        this.transform.position = new Vector3(Mathf.RoundToInt(cursorCamPos.x), Mathf.RoundToInt(cursorCamPos.y), -1);
    }
}
