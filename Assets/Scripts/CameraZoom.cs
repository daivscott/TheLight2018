using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters;

public class CameraZoom : MonoBehaviour
{
    // initialize variables
    public int zoom = 20;
    public int normal = 60;
    public float smooth = 5;

    private bool isZoomed = false;

    void Update ()
    {
        // Check that left shift s not pressed
        if (!Input.GetKey(KeyCode.LeftShift))       
        {
            // Check for right mouse button being pressed
            if (Input.GetMouseButtonDown(1))
            {
                // change zomed state depending on current state
                isZoomed = !isZoomed;
            }
            if (isZoomed)
            {
                // zoom in 
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, zoom, Time.deltaTime * smooth);
            }
            else
            {
                // zoom out
                GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, normal, Time.deltaTime * smooth);
            }
        }
    }
}
