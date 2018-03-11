using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Networking;

public class PlayerMovement : Photon.MonoBehaviour {

    // ===== Public Vars
    public float speed = 10.0f;
    public float jumpForce = 5.0f;

    public Camera fpsCamera;

    public Text debugText;


    // ===== Private Vars
    private bool isOnGround = true;

    private int jumpState = 0;

    private float distToGround;
    private float rotY;
    private float maxSpeed;
    

    private Rigidbody thisrb;
    private Collider thiscldr;
    private CameraLook camlook;

    private Vector3 vel;

	// Use this for initialization
	void Start () {
        //enabled = photonView.isMine;

        thisrb = GetComponent<Rigidbody>();
        thiscldr = GetComponent<Collider>();
        camlook = fpsCamera.GetComponent<CameraLook>();

        distToGround = thiscldr.bounds.extents.y;
        rotY = transform.rotation.eulerAngles.y;

        ToggleCursorState();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (photonView.isMine == true)
        //{
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorState();
            }
            // Get input.
            vel.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed * 25.0f;
            vel.y = 0f;
            vel.z = Input.GetAxis("Vertical") * Time.deltaTime * speed * 25.0f;

            if (isOnGround == true)
            {
                // Move the player.
                transform.Translate(vel);
            }
            else
            {
                // Move the player.
                if (thisrb.velocity.magnitude < 12.0f)
                {
                    thisrb.AddRelativeForce(vel);
                }
            }

            // Rotate the player on the Y euler axis.
            rotY += Input.GetAxis("Mouse X") * camlook.mouseSensitivity * Time.deltaTime;
            Quaternion localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
            transform.rotation = localRotation;

            HandleJump();

            debugText.text = GetDebugText();
        //}
    }


    private string GetDebugText()
    {
        return "isOnGround: " + isOnGround.ToString() + NL() +
                "jumpState: " + jumpState.ToString() + NL() +
                "IsGrounded: " + IsGrounded().ToString() + NL() +
                "Vel Mag: " + thisrb.velocity.magnitude.ToString();
    }

    private string NL()
    {
        return "\n";
    }


    /* ===================================================== *
     *  -This function handles jumping.-
     *====================================================== */
    private void HandleJump()
    {

        if (IsGrounded() == true)
        {
            isOnGround = true;
            jumpState = 0;
        }

        switch (jumpState)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpForce, vel.normalized);
                    jumpState = 1;
                    isOnGround = false;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpForce, vel.normalized);
                    jumpState = 2;
                }
                break;
            case 2:
                break;
        }
    }

    // Apply a force in the upward direction. aka Jump.
    //private void Jump()
    //{
    //    thisrb.AddForce(transform.up * jumpForce * 10.0f, ForceMode.Impulse);
    //}
    void Jump(float jumpHeight, Vector3 movement)
    {
        // Add an Impulse force to the Rigidbody´s transform (direction up) and multiply it by the jumpHeight float.
        thisrb.AddForce(transform.TransformDirection(new Vector3(movement.x * 0.25f, 1f, movement.z * 0.25f)) * jumpHeight, ForceMode.Impulse);
    }

    // Cast a ray in the negative up direction to test if the player is on the ground.
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

     // Toggle the cursor visibility.
     private void ToggleCursorState()
    {
        Cursor.lockState = (Cursor.visible) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
}
