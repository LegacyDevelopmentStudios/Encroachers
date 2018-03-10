using System;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public GameObject PlayerObject;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    
    private float rotX = 0.0f; // rotation around the right/x axis

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
    }

    void Update()
    {
        float mouseY = -Input.GetAxis("Mouse Y");
        
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation.
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        // Rotate the camera.
        Quaternion localRotation = Quaternion.Euler(rotX, PlayerObject.transform.rotation.eulerAngles.y, 0.0f);
        transform.rotation = localRotation;
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }
}