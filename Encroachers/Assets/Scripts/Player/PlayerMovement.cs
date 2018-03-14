using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// using UnityEngine.UI;


[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
// public class PlayerMovement : MonoBehaviour {
public class PlayerMovement : NetworkBehaviour {


    // ===== Public Vars
    [Tooltip("This controls how responsive movement feels.")]
    [Range(5f,100f)]
    public float responsiveness     = 40.0f;

    [Tooltip("The number of meters high the player jumps.")]
    public float jumpHeight         = 1.5f;

    [Space(10)]
    [Header("Speed")]

    [Tooltip("This limits the speed of the player.")]
    public float speedMax           = 12f;

    [Tooltip("This limits the maximum normal speed.")]
    public float speedMaxNormal     = 12f;

    [Tooltip("This limits the maximum speed of sprinting..")]
    public float speedMaxSprint     = 24f;

    [Space(10)]
    [Header("Dampening")]

    [Range(0f,10f)]
    [Tooltip("This affects all dampening.")]
    public float dampFactor         = 2.0f;

    [Range(0f,5f)]
    [Tooltip("This affects horizontal movement. Moving forward, backward, strafe.")]
    public float dampMultHorizontal = 1.0f;

    [Range(0f,10f)]
    [Tooltip("This affects vertical movement. Jumping, falling.")]
    public float dampMultVertical   = 0.01f;

    [Space(10)]

    [Tooltip("This is the first person camera.")]
    public Camera   fpsCamera;



    // public Text     debugText;

    // ===== Private Vars
    private enum JumpStates{
        ground = 0,
        jumpedOnce = 1,
        jumpedTwice = 2
    };
    private JumpStates jumpState = JumpStates.ground;
    private float jumpForce;

    private bool isOnGround = true;

    private float distToGround, rotY, forceMult;
    

    private Rigidbody thisrb;
    private Collider thiscldr;
    private CameraLook camlook;
    private RaycastHit hitInfo;
    private Vector3 velDamp, input;

	// Use this for initialization
	void Start () {
        thisrb = GetComponent<Rigidbody>();
        thiscldr = GetComponent<Collider>();
        camlook = fpsCamera.GetComponent<CameraLook>();

        distToGround = thiscldr.bounds.extents.y;
        rotY = transform.rotation.eulerAngles.y;

        jumpForce = GetJumpForce(jumpHeight, Physics.gravity.y);

        ToggleCursorState();

        // debugText.text = "";
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
        if(!isLocalPlayer)
        {
            fpsCamera.enabled = false;
            return;
        }
        
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

        //======================================================================
        //    Calculate a force multiplier based on velocity magnitude and
        //    max speed.
        //    This is used to control the force applied to the rigidbody
        //    and ultimately to limit the player speed so that the velocity
        //    doesn't continuously stack, gradually increasing the player
        //    velocity beyond usable.
        forceMult = Mathf.Clamp( ((speedMax - thisrb.velocity.magnitude) / speedMax), 0f, 1f);
        //======================================================================

        if (input.magnitude > 0f || input.magnitude < 0f)
        {
                // Move the player.
                if (isOnGround == true)
                {
                    thisrb.AddRelativeForce(input * 2f * forceMult, ForceMode.VelocityChange);
                }
                else
                {
                    thisrb.AddRelativeForce(input * forceMult, ForceMode.VelocityChange);
                }

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

        // debugText.text = GetDebugText();
    }


    /* ===================================================== *
     *  -Temporary functions for debugging.-
     * ----------------------------------------------------- */
    // private string GetDebugText()
    // {
    //     return "isOnGround: " + isOnGround.ToString() + "\n" +
    //             "jumpState: " + jumpState.ToString() + "\n" +
    //             "IsGrounded: " + IsGrounded().ToString() + "\n" +
    //             "RB Vel Mag: " + thisrb.velocity.magnitude.ToString() + "\n" +
    //             "RB Vel: " + thisrb.velocity.ToString() + "\n" +
    //             "RB mass: " + thisrb.mass.ToString() + "\n" +
    //             "input Mag: " + input.magnitude.ToString() + "\n" +
    //             "input: " + input.ToString() + "\n" +
    //             "velDamp: " + velDamp.ToString() + "\n" +
    //             "percent: " + (forceMult * 100f).ToString() + "%" + "\n" +
    //             "position: " + transform.position.ToString() + "\n" +
    //             "gravity: " + Physics.gravity.ToString() + "\n" +
    //             "jump force: " + jumpForce.ToString() + "\n" +
    //             "jump distance: " + (transform.position.y - 1.2f).ToString();
    // }
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
            jumpState = JumpStates.ground;
        }

        switch (jumpState)
        {
            case JumpStates.ground:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpHeight, input.normalized);
                    jumpState = JumpStates.jumpedOnce;
                    isOnGround = false;
                }
                break;
            case JumpStates.jumpedOnce:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpHeight, input.normalized);
                    jumpState = JumpStates.jumpedTwice;
                }
                break;
            case JumpStates.jumpedTwice:
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
                jumpState = JumpStates.jumpedOnce;
            }
        }
    }

    // Apply a force in the upward direction. aka Jump.
    private void Jump(float jumpHeight, Vector3 movement)
    {
        thisrb.AddForce(transform.TransformDirection(new Vector3(movement.x, jumpForce, movement.z)), ForceMode.Impulse);
    }

    private float GetJumpForce(float _meters_, float _gravity_)
    {
        float two = 2f * _gravity_;
        float b = -(two * _meters_);
        float ret = Mathf.Sqrt(b);
        return ret;
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
        float inputMult = Time.deltaTime * thisrb.mass * responsiveness;
        
        input.x = Input.GetAxis("Horizontal") * inputMult;
        input.y = 0f;
        input.z = Input.GetAxis("Vertical") * inputMult;
                            
        velDamp.x = -(thisrb.velocity.x * dampMultHorizontal);
        velDamp.y = -(thisrb.velocity.y * dampMultVertical);
        velDamp.z = -(thisrb.velocity.z * dampMultHorizontal);
    }

     // Toggle the cursor visibility.
     private void ToggleCursorState()
    {
        Cursor.lockState = (Cursor.visible) ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !Cursor.visible;
    }
}
