using System;
using UnityEngine;
using UnityEngine.Networking;

public class CameraLook : NetworkBehaviour
{
    public GameObject PlayerObject;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    
    private float rotX = 0.0f; // rotation around the right/x axis

    bool localCam = false;

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        localCam = transform.parent.GetComponent<NetworkIdentity>().isLocalPlayer;
    }

    void Update()
    {
        if (!localCam)
            return;
        float mouseY = -Input.GetAxis("Mouse Y");
        
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation.
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        //Rotate the camera.
        Quaternion localRotation = Quaternion.Euler(rotX, PlayerObject.transform.rotation.eulerAngles.y, 0.0f);
        transform.rotation = localRotation;
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
}