using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform mainCamera;

    void Start()
    {
        // set main camera as target
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        // Set the rotation for the text to face the camera
        // Pointing the UI texts to camera although it appears backwards
        // if set facing therefore rotation set to facing away
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
    }
	
}
