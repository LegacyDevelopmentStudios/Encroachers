using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
// public class PlayerMovement : MonoBehaviour {
public class PlayerMovement : NetworkBehaviour {
    
    // Added these to aid in debugging. Should be removed.
    // public Text txt, txt2, txt3;

    

    /// <summary>Scalar for scaling input.</summary>
    [Tooltip("This controls how responsive movement feels.")]
    [Range(5f,100f)]
    [SerializeField] private float responsiveness = 42.0f;

    /// <summary>Float for storing jump height in meters.</summary>
    [Tooltip("The number of meters high the player jumps.")]
    [SerializeField] private float jumpHeight = 1.6f;


    /// <summary>Scalar for scaling force applied in movement.</summary>
    [Header("Speed")]
    [Tooltip("This limits the speed of the player.")]
    [SerializeField] private float speedMax = 13f;

    /// <summary>Float to store the normal max speed. "speedMax" is set to this when shift is released.</summary>
    [Tooltip("This limits the maximum normal speed.")]
    [SerializeField] private float speedMaxNormal = 13f;

    /// <summary>Float to store the max speed for sprinting. "speedMax" is set to this when shift is pressed.</summary>
    [Tooltip("This limits the maximum speed of sprinting..")]
    [SerializeField] private float speedMaxSprint = 26f;


    /// <summary>Scalar for all dampening.</summary>
    [Header("Dampening")]
    [Range(0f,10f)]
    [Tooltip("This affects all dampening.")]
    [SerializeField] private float dampFactor = 1.8f;

    /// <summary>Scalar for dampening on x and z axes.</summary>
    [Range(0f,10f)]
    [Tooltip("This affects horizontal movement. Moving forward, backward, strafe.")]
    [SerializeField] private float dampMultHorizontal = 1.0f;

    /// <summary>Scalar for dampening on y axis.</summary>
    [Range(0f,10f)]
    [Tooltip("This affects vertical movement. Jumping, falling.")]
    [SerializeField] private float dampMultVertical = 0.01f;


    /// <summary>Whether the player is on the ground/object.</summary>
    private bool isOnGround = true;

    /// <summary>
    /// Calculate a force multiplier based on velocity magnitude and max speed.
    /// This is used to control the force applied to the rigidbody and ultimately
    /// to limit the player speed so that the velocity doesn't continuously 
    /// stack, gradually increasing the player velocity beyond usable.
    ///</summary>
    private float forceMult;
    
    /// <summary>Same as forceMult except it limits to sprint speed.</summary>
    private float forceMultSprint;


    /// <summary>The size of the collider on the y axis.</summary>
    private float distToGround;
    
    /// <summary>Velocity vector converted to an angle in degrees.</summary>
    private float rbVelAng;
    
    /// <summary>Input vector converted to an angle in degrees.</summary>
    private float inputAng;
    
    /// <summary>The resulting angle of input angle + look angle.</summary>
    private float rotInputAng;
    
    /// <summary>A scalar used to avoid applying force in the direction of current velocity.</summary>
    private float inputRotMult;
    
    /// <summary>The force in meters per second to reach a certain height in meters.</summary>
    private float jumpForce;


    /// <summary>An enum to store the different states of jumping.</summary>
    private enum JumpStates{
        ground = 0,
        jumpedOnce = 1,
        jumpedTwice = 2
    };
    /// <summary>An variable that controls the state of jumping.</summary>
    private JumpStates jumpState = JumpStates.ground;

    /// <summary>First person view camera.</summary>
    private Camera fpsCamera;

    /// <summary>The Rigidbody component of the GameObject that this script is attached to.</summary>
    private Rigidbody thisrb;

    /// <summary>The Rigidbody component of the GameObject that this script is attached to.</summary>
    private Collider thiscldr;

    /// <summary>A variable to store RaycastHit info from the IsGrounded check.</summary>
    private RaycastHit hitInfo;

    /// <summary>A vector to store input.</summary>
    private Vector3 input;



    /// <summary>Get/Set how responsive movement is.</summary>
    public float Responsiveness { get { return responsiveness; } set { responsiveness = value; } }


    /// <summary>Get/Set the height of jumping in meters.</summary>
    public float JumpHeight {
        get { return jumpHeight; }
        set {
            jumpHeight = value;
            jumpForce = EncTools.GetJumpForce(jumpHeight, Physics.gravity.y);
        }
    }

    /// <summary>Get whether the player is standing on the ground/object. Return only.</summary>
    public bool IsOnGround { get { return isOnGround; } }


    /// <summary>Get/Set the overall dampening factor.</summary>
    public float Dampening { get { return dampFactor; } set { dampFactor = Mathf.Clamp(value, 0f, 10f); } }

    /// <summary>Get/Set the dampening factor affecting the x and z axes.</summary>
    public float LateralDampening { get { return dampMultHorizontal; } set { dampMultHorizontal = Mathf.Clamp(value, 0f, 10f); } }

    /// <summary>Get/Set the dampening factor affecting the y axis.</summary>
    public float VerticalDampening { get { return dampMultVertical; } set { dampMultVertical = Mathf.Clamp(value, 0f, 10f); } }


    /// <summary>Get/Set the normal max speed.</summary>
    public float NormalMaxSpeed { get { return speedMaxNormal; } set { speedMaxNormal = value; speedMax = speedMaxNormal; } }

    /// <summary>Get/Set the max speed for sprinting.</summary>
    public float SprintMaxSpeed { get { return speedMaxSprint; } set { speedMaxSprint = value; } }



    // Use this for initialization
    void Start () {
        fpsCamera = GameObject.FindWithTag("FPS-Cam").GetComponent<Camera>();
        thisrb = GetComponent<Rigidbody>();
        thiscldr = GetComponent<Collider>();

        distToGround = thiscldr.bounds.extents.y;

        jumpForce = EncTools.GetJumpForce(jumpHeight, Physics.gravity.y);

        EncTools.ToggleCursorState();
    }

// Added this to test shooting enemies. Can remove.
    // // Use this for GUI stuff
    // private void OnGUI()
    // {
    //     int size = (int)(Screen.height * 0.0075f);
    //     GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, size, size), "");
    // }

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
            EncTools.ToggleCursorState();
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
        
        GetInput();
        HandleJumping();

        // Added this to aid in debugging. Should be removed.
        // txt.text = GetDebugText();
    }


    void FixedUpdate()
    {
        forceMult = Mathf.Clamp( ((speedMax - thisrb.velocity.magnitude) / speedMax), 0f, 1f);
        forceMultSprint = Mathf.Clamp( ((speedMaxSprint - thisrb.velocity.magnitude) / speedMaxSprint), 0f, 1f);
        rbVelAng = EncTools.AtanFULL(thisrb.velocity.x, thisrb.velocity.z);
        inputRotMult = EncTools.MinAngDiff(rbVelAng, rotInputAng) / 180f;

        // If there is no input then avoid applying a movement force.
        if (input.magnitude > 0f || input.magnitude < 0f)
        {
            if (jumpState == JumpStates.ground)
            {
                // Normal movement.
                if (forceMult == 0f)
                {
                    EncTools.ApplyMoveForce(ref thisrb, input, forceMult, 2f);
                }
                // If moving too fast then don't apply force in direction of velocity.
                else if (forceMultSprint == 0f)
                {
                    EncTools.ApplyMoveForce(ref thisrb, input, inputRotMult, 1f);
                }
                // If moving around sprint speed then continue applying force on the sprint scale to retain some control.
                else
                {
                    EncTools.ApplyMoveForce(ref thisrb, input, forceMultSprint, 2f);
                }
            }
            else
            {
                // If moving too fast then don't apply force in direction of velocity.
                if (forceMultSprint == 0f)
                {
                    EncTools.ApplyMoveForce(ref thisrb, input, inputRotMult, 1f);
                }
                // Apply a constant force based on input.
                else
                {
                    EncTools.ApplyMoveForce(ref thisrb, input, 0.25f, 1f);
                }
            }

            // Velocity dampening.
            EncTools.Dampen(ref thisrb, dampFactor, dampMultHorizontal, dampMultVertical, 0.5f);
        }
        else
        {
            // Dampen velocity only when on the ground if there is no input.
            if (jumpState == JumpStates.ground)
            {
                EncTools.Dampen(ref thisrb, dampFactor, dampMultHorizontal, dampMultVertical, 1f);
            }
        }
    }



    /// <summary>
	/// This sets the dampening properties to the given parameters.
	/// </summary>
    /// <param name="overall">Affects all dampening.</param>
    /// <param name="hori">Affects dampening on the x and z axis.</param>
    /// <param name="vert">Affects dempening on the y axis.</param>
    /// <remarks>All properties are clamped between 0f and 10f.</remarks>
    public void SetDampeningProperties(float overall, float hori, float vert)
    {
        dampFactor = Mathf.Clamp(overall, 0f, 10f);
        dampMultHorizontal = Mathf.Clamp(hori, 0f, 10f);
        dampMultVertical = Mathf.Clamp(vert, 0f, 10f);
    }


    /// <summary>
	/// This sets the max speed properties to the given parameters.
	/// </summary>
    /// <param name="normal">The normal max speed.</param>
    /// <param name="sprint">The max speed of sprinting.</param>
    /// <remarks>All properties are clamped between 0f and 100f.</remarks>
    public void SetMaxSpeedProperties(float normal, float sprint)
    {
        speedMaxNormal = Mathf.Clamp(normal, 0f, 100f);
        speedMaxSprint = Mathf.Clamp(sprint, 0f, 100f);
    }


    /// <summary>Reset responsiveness to original value.</summary>
    public void ResetResponsiveness()
    {
        if(responsiveness != 42f) { responsiveness = 42f; }
    }

    /// <summary>Reset jump height to original value.</summary>
    public void ResetJumpHeight()
    {
        if (jumpHeight != 1.5f)
        {
            jumpHeight = 1.5f;
            jumpForce = EncTools.GetJumpForce(jumpHeight, Physics.gravity.y);
        }
    }

    /// <summary>Reset all speed properties to original values.</summary>
    public void ResetSpeedProperties()
    {
        if(speedMax != 13f) { speedMax = 13f; }
        if(speedMaxNormal != 13f) { speedMaxNormal = 13f; }
        if(speedMaxSprint != 26f) { speedMaxSprint = 26f; }
    }

    /// <summary>Reset all dampening properties to original values.</summary>
    public void ResetDampeningProperties()
    {
        if(dampFactor != 1.8f) { dampFactor = 1.8f; }
        if(dampMultHorizontal != 1f) { dampMultHorizontal = 1f; }
        if(dampMultVertical != 0.01f) { dampMultVertical = 0.01f; }
    }


    /// <summary>Reset all properties to original values.</summary>
    public void ResetAllProperties()
    {
        ResetResponsiveness();
        ResetJumpHeight();
        ResetSpeedProperties();
        ResetDampeningProperties();
    }
    



    /// <summary>Private method for handling jumping.</summary>
    private void HandleJumping()
    {
        // Test whether the player is touching a collider.
        bool ig = EncTools.IsGrounded(transform.position, thiscldr, distToGround * 0.75f, out hitInfo);
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
                    EncTools.Jump(ref thisrb, transform, jumpForce, input.normalized);
                    jumpState = JumpStates.jumpedOnce;
                    isOnGround = false;
                }
                break;
            case JumpStates.jumpedOnce:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    EncTools.Jump(ref thisrb, transform, jumpForce, input.normalized);
                    jumpState = JumpStates.jumpedTwice;
                }
                break;
            case JumpStates.jumpedTwice:
                break;
        }

        // If the player is off the ground without jumping then
        // change states to jumped to avoid a second mid-air jump.
        if (ig == false)
        {
            if (jumpState == JumpStates.ground && isOnGround == true)
            {
                isOnGround = false;
                jumpState = JumpStates.jumpedOnce;
            }
        }
    }

    /// <summary>Private method for storing input.</summary>
    private void GetInput()
    {
        float inputMult = Time.deltaTime * thisrb.mass * Responsiveness;
        
        input.x = Input.GetAxis("Horizontal") * inputMult;
        input.y = 0f;
        input.z = Input.GetAxis("Vertical") * inputMult;
        
        inputAng = EncTools.AtanFULL(input.x, input.z);
        rotInputAng = transform.rotation.eulerAngles.y + inputAng;
        if(rotInputAng > 360f) { rotInputAng -= 360f; }
    }








// Added this to aid in debugging. Should be removed.
// ====================== don't return strings like this. ================
    // private string GetDebugText()
    // {
    //     return "forceMult: " + (forceMult * 100f).ToString() + "\n" +
    //             "forceMultSprint: " + (forceMultSprint * 100f).ToString() + "\n\n" +
    //             "forceMultSprint: " + (inputRotMult * 100f).ToString() + "\n\n" +

    //             "Input: " + input.ToString() + "\n" +
    //             "input Ang: " + inputAng.ToString() + "\n\n" +

    //             "RB Vel: " + thisrb.velocity.ToString() + "\n" +
    //             "RB MAG: " + thisrb.velocity.magnitude.ToString() + "\n\n" +

    //             "rbVelAng: " + rbVelAng.ToString() + "\n" +
    //             "Rot + Input: " + rotInputAng.ToString() + "\n" +
    //             "Y rot: " + transform.eulerAngles.y.ToString() + "\n\n" +

    //             "Rot Vel: " + EncTools.AngleInRange(transform.rotation.eulerAngles.y, rbVelAng, 45f).ToString() + "\n" +
    //             "Vel Input: " + EncTools.AngleInRange(rbVelAng, inputAng, 45f).ToString() + "\n" +
    //             "Rot + Input: " + EncTools.AngleInRange(rotInputAng, rbVelAng, 45f).ToString();
    // }
//======================================================================
}
