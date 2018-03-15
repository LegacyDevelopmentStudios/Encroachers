using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
// public class PlayerMovement : MonoBehaviour {
public class PlayerMovement : NetworkBehaviour {
    
    public Text txt;

    [Tooltip("This controls how responsive movement feels.")]
    [Range(5f,100f)]
    [SerializeField] private float responsiveness     = 42.0f;

    [Tooltip("The number of meters high the player jumps.")]
    [SerializeField] private float jumpHeight         = 1.6f;


    [Header("Speed")]

    [Tooltip("This limits the speed of the player.")]
    [SerializeField] private float speedMax           = 13f;

    [Tooltip("This limits the maximum normal speed.")]
    [SerializeField] private float speedMaxNormal     = 13f;

    [Tooltip("This limits the maximum speed of sprinting..")]
    [SerializeField] private float speedMaxSprint     = 26f;


    [Header("Dampening")]

    [Range(0f,10f)]
    [Tooltip("This affects all dampening.")]
    [SerializeField] private float dampFactor         = 1.8f;

    [Range(0f,10f)]
    [Tooltip("This affects horizontal movement. Moving forward, backward, strafe.")]
    [SerializeField] private float dampMultHorizontal = 1.0f;

    [Range(0f,10f)]
    [Tooltip("This affects vertical movement. Jumping, falling.")]
    [SerializeField] private float dampMultVertical   = 0.01f;

    private enum JumpStates{
        ground = 0,
        jumpedOnce = 1,
        jumpedTwice = 2
    };
    private JumpStates jumpState = JumpStates.ground;
    private float jumpForce;

    private bool isOnGround = true, stopUpdate = false;

    private float distToGround, forceMult;
    

    private Camera fpsCamera;
    private Rigidbody thisrb;
    private Collider thiscldr;
    private RaycastHit hitInfo;
    private Vector3 velDamp, input;

	// Use this for initialization
	void Start () {
        fpsCamera = GameObject.FindWithTag("FPS-Cam").GetComponent<Camera>();
        thisrb = GetComponent<Rigidbody>();
        thiscldr = GetComponent<Collider>();

        distToGround = thiscldr.bounds.extents.y;

        jumpForce = GetJumpForce(jumpHeight, Physics.gravity.y);

        ToggleCursorState();
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
        
        // ========
        GetInput();
        // ========

        //======================================================================
        //    Calculate a force multiplier based on velocity magnitude and max speed.
        //    This is used to control the force applied to the rigidbody and ultimately
        //    to limit the player speed so that the velocity doesn't continuously 
        //    stack, gradually increasing the player velocity beyond usable.
        forceMult = Mathf.Clamp( ((speedMax - thisrb.velocity.magnitude) / speedMax), 0f, 1f);
        //======================================================================

        // ==========
        HandleJump();
        // ==========

        // txt.text = GetDebugText();
    }


    void FixedUpdate()
    {
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
            if (thisrb.velocity.magnitude > 0.05f)
            {
                // Velocity dampening.
                thisrb.AddForce(velDamp * thisrb.mass * dampFactor);
            }
        }
    }


    // private string GetDebugText()
    // {
    //     return "Input: " + input.ToString() + "\n" +
    //             "RB Vel: " + thisrb.velocity.ToString() + "\n" +
    //             "RB MAG: " + thisrb.velocity.magnitude.ToString();
                
    // }

    /// <summary>
	/// This sets the dampening properties to the given parameters. All properties are clamped between 0f and 10f.
    /// <para/>"overall" affects all dampening.
    /// <para/>"hori" affects dampening on the x and z axis.
    /// <para/>"vert" affects dempening on the y axis.
	/// </summary>
    public void SetDampeningProperties(float overall, float hori, float vert)
    {
        dampFactor = Mathf.Clamp(overall, 0f, 10f);
        dampMultHorizontal = Mathf.Clamp(hori, 0f, 10f);
        dampMultVertical = Mathf.Clamp(vert, 0f, 10f);
    }


    /// <summary>
	/// This sets the dampening properties to the given parameters. All properties are clamped between 0f and 100f.
    /// <para/>"max" affects all dampening.
    /// <para/>"maxNormal" affects dampening on the x and z axis.
    /// <para/>"maxSprint" affects dempening on the y axis.
	/// </summary>
    public void SetMaxSpeedProperties(float normal, float sprint)
    {
        speedMaxNormal = Mathf.Clamp(normal, 0f, 100f);
        speedMaxSprint = Mathf.Clamp(sprint, 0f, 100f);
    }







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
                    Jump(jumpForce, input.normalized);
                    jumpState = JumpStates.jumpedOnce;
                    isOnGround = false;
                }
                break;
            case JumpStates.jumpedOnce:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump(jumpForce, input.normalized);
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
    private void Jump(float force, Vector3 movement)
    {
        thisrb.AddForce(transform.TransformDirection(new Vector3(movement.x, force, movement.z)), ForceMode.Impulse);
    }



    // This returns a force in m/s to use in RB.AddForce(retVal, ForceMode.Impulse) to jump to a certain height in meters.
    private float GetJumpForce(float _meters_, float _gravity_)
    {
        return Mathf.Sqrt(-(2f * _gravity_ * _meters_));
    }



    // Cast a ray in the negative up direction to test if the player is on the ground.
    private bool IsGrounded()
    {
        //return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        return Physics.SphereCast(transform.position, thiscldr.bounds.extents.x,  -Vector3.up,  out hitInfo, distToGround * 0.75f);
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
