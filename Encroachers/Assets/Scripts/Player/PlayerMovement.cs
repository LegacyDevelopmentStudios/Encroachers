using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Networking;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
public class PlayerMovement : Photon.MonoBehaviour {

    // ===== Public Vars
    public float speed      = 10.0f;
    public float speedMax   = 15.0f;
    public float jumpForce  = 5.0f;
    public float dampFactor = 2.0f;

    public Camera   fpsCamera;
    public Text     debugText;

    // ===== Private Vars
    private bool    isOnGround = true;

    private int     jumpState = 0;

    private float distToGround, rotY, forceMult, speedMaxNormal, speedMaxSprint;
    

    private Rigidbody   thisrb;
    private Collider    thiscldr;
    private CameraLook  camlook;
    private RaycastHit  hitInfo;
    private Vector3     velDamp, input;

	// Use this for initialization
	void Start () {
        //enabled = photonView.isMine;

        thisrb = GetComponent<Rigidbody>();
        thiscldr = GetComponent<Collider>();
        camlook = fpsCamera.GetComponent<CameraLook>();

        distToGround = thiscldr.bounds.extents.y;
        rotY = transform.rotation.eulerAngles.y;

        speedMaxNormal = 15f;
        speedMaxSprint = 30f;

        ToggleCursorState();

        debugText.text = "";
    }

    // Use this for GUI stuff
    private void OnGUI()
    {
        int size = (int)(Screen.height * 0.0075f);
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, size, size), "");
    }

    // Update is called once per frame
    void Update ()
    {
        //if (photonView.isMine == true)
        //{

        // Toggle cursor state. This should be moved to a manager script or something.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorState();
        }

        // Handle shift key, sprinting.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isOnGround == true)
            {
                speedMax = speedMaxSprint;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedMax = speedMaxNormal;
        }

        // Get input.
        GetInput();

        /*======================================================================
            Calculate a force multiplier based on velocity magnitude and
            max speed.
            This is used to control the force applied to the rigidbody
            and ultimately to limit the player speed so that the velocity
            doesn't continuously stack, gradually increasing the player
            velocity beyond usable. */
        forceMult = ((speedMax - thisrb.velocity.magnitude) / speedMax);
        //======================================================================

        if (input.magnitude > 0f || input.magnitude < 0f)
        {
            // Move the player.
            if (isOnGround == true)
                thisrb.AddRelativeForce(input * 2f * forceMult, ForceMode.VelocityChange);
            else
                thisrb.AddRelativeForce(input * forceMult, ForceMode.VelocityChange);

            // Velocity dampening.
            thisrb.AddForce(velDamp * thisrb.mass * dampFactor * 0.5f);
        }
        else
        {
            // Velocity dampening.
            thisrb.AddForce(velDamp * thisrb.mass * dampFactor);
        }



        // Rotate the player on the Y euler axis.
        rotY += Input.GetAxis("Mouse X") * camlook.mouseSensitivity * Time.deltaTime;
        Quaternion localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        transform.rotation = localRotation;

        HandleJump();

        //debugText.text = GetDebugText();


        //}
    }


    /* ===================================================== *
     *  -Temporary functions for debugging.-
     * ----------------------------------------------------- */
    private string GetDebugText()
    {
        return "isOnGround: " + isOnGround.ToString() + NL() +
                "jumpState: " + jumpState.ToString() + NL() +
                "IsGrounded: " + IsGrounded().ToString() + NL() +
                "RB Vel Mag: " + thisrb.velocity.magnitude.ToString() + NL() +
                "input Mag: " + input.magnitude.ToString() + NL() +
                "input: " + input.ToString() + NL() +
                "velDamp: " + velDamp.ToString() + NL() +
                "percent: " + (forceMult * 100f).ToString() + "%";
    }

    private string NL() { return "\n"; }
    private string NL(int count)
    {
        string r = "";
        for (int i = 0; i < count; i++)
            r += "\n";

        return r;
    }
    // -----------------------------------------------------


    /* ===================================================== *
     *  -This function handles jumping.-
     * ----------------------------------------------------- */
    private void HandleJump()
    {
        // Test whether the player is touching a collider.
        bool ig = IsGrounded();
        if (ig == true)
        {
            isOnGround = true;
            jumpState = 0;
        }

        switch (jumpState)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpForce, input.normalized);
                    jumpState = 1;
                    isOnGround = false;
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpForce, input.normalized);
                    jumpState = 2;
                }
                break;
            case 2:
                break;
        }
        // -----------------------------------------------------

        // If the player is off the ground without jumping then
        // change states to jumped to avoid a second mid-air jump.
        if (ig == false)
        {
            if (jumpState == 0 && isOnGround == true)
            {
                isOnGround = false;
                jumpState = 1;
            }
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
        thisrb.AddForce(transform.TransformDirection(new Vector3(movement.x * 0.1f, 1f, movement.z * 0.1f)) * jumpHeight, ForceMode.Impulse);
    }

    // Cast a ray in the negative up direction to test if the player is on the ground.
    private bool IsGrounded()
    {
        //return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        return Physics.SphereCast(transform.position, 
                                    thiscldr.bounds.extents.x, 
                                    -Vector3.up, 
                                    out hitInfo, 
                                    distToGround * 0.75f);
    }

    // Store input.
    private void GetInput()
    {
        float inputMult = Time.deltaTime * speed * thisrb.mass * 5f;
        input = new Vector3(Input.GetAxis("Horizontal") * inputMult,
                            0f,
                            Input.GetAxis("Vertical") * inputMult);

        velDamp = new Vector3(-thisrb.velocity.x,
                            -(thisrb.velocity.y * 0.2f),
                            -thisrb.velocity.z);
    }

     // Toggle the cursor visibility.
     private void ToggleCursorState()
    {
        Cursor.lockState = (Cursor.visible) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
}
