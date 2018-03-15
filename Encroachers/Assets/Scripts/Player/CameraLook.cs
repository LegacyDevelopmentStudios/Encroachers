using System;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public GameObject PlayerObject;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    
    private float rotX = 0.0f;
    private float rotY = 0.0f;

    bool localCam = false;

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        rotX += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation.
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);


        // Rotate the player on the Y euler axis.
        Quaternion localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        PlayerObject.transform.rotation = localRotation;
        //Rotate the camera on the x axis and set it to the player y axis.
        localRotation = Quaternion.Euler(rotX, PlayerObject.transform.rotation.eulerAngles.y, 0.0f);
        transform.rotation = localRotation;

    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
}