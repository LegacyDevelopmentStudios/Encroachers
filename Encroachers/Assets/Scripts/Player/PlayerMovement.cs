using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour {

    // ===== Public Vars
    public float speed = 10.0f;
    public float jumpForce = 20.0f;

    public Camera fpsCamera;


    // ===== Private Vars
    private bool isOnGround = true;
    private bool isFirstJump = false;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorState();
        }

        if (isOnGround == true)
        {
            // Get input.
            vel.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            vel.z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

            // Move the player.
            transform.Translate(vel);
        }
        else
        {
            // Move the player.
            //transform.Translate(vel);
            transform.position += vel;
        }


        //vel.x = Input.GetAxis("Horizontal") * Time.deltaTime * speed * 200.0f;
        //vel.z = Input.GetAxis("Vertical") * Time.deltaTime * speed * 200.0f;

        //thisrb.AddRelativeForce(vel);

        //if(thisrb.velocity.magnitude > )

        // Rotate the player on the Y euler axis.
        rotY += Input.GetAxis("Mouse X") * camlook.mouseSensitivity * Time.deltaTime;
        Quaternion localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        transform.rotation = localRotation;

        HandleJump();
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
                    Jump();
                    jumpState = 1;
                    isOnGround = false;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                    jumpState = 2;
                }
                break;
            case 2:
                break;
        }
        print("Func: " + IsGrounded());
        print("Var: " + isOnGround);
        print(jumpState);
        
    }

    // Apply a force in the upward direction. aka Jump.
    private void Jump()
    {
        thisrb.AddForce(transform.up * jumpForce * Time.deltaTime * 980.0f);
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
