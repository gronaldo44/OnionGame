using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player partially by interpreting user inputs
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Class params
    public Rigidbody2D rb;     // Player body
    TouchingDirections touchingDirections;  // What the player's body is touching
    Animator animator;
    [SerializeField] public Vector3 spawnLocation = Vector3.zero;

    public TextAsset DialogueTextFile = null;
    public bool InDialogueTriggerRange = false;
    private bool inDialogue = false;

    public float jumpImpulse = 16f; // Increased for more snappy ascent
    public float jumpCutMultiplier = 0.5f;

    #region Jump Enhancements
    [Header("Jump Enhancements")]
    public float normalGravityScale = 2f;     // Increased gravity scale for snappy ascent
    public float fallMultiplier = 3f;         // Gravity multiplier when falling
    public float lowJumpMultiplier = 2.5f;    // Gravity multiplier for short jumps
    public float additionalJumpForce = 1f;    // Optional upward force during ascent

    private bool _isJumpPressed = false;        // Tracks if the jump button is held
    private bool isJumpPressed
    {
        get
        {
            return _isJumpPressed;
        }
        set
        {
            _isJumpPressed = value;
            if (_isJumpPressed)
            {
                //Debug.Log("Jump pressed");
            } else
            {
                //Debug.Log("Jump released");
            }
        }
    }
    #endregion

    #region Movement params
    public float moveSpeed = 10f;
    public float acceleration = 50f; // Increased for snappier movement
    public float deceleration = 50f; // Increased for snappier movement
    private float currentSpeed = 0f;
    Vector2 moveInput;
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1); // Flip along x-axis
            }
            _isFacingRight = value;
        }
    }
    #endregion

    #region Dash params
    private bool canDash = true;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCD = 0.5f;
    [SerializeField]
    private bool _isDashing = false;
    public bool IsDashing
    {
        get => _isDashing;
        set
        {
            _isDashing = value;
            animator.SetBool(AnimationStrings.isDashing, value);
        }
    }
    #endregion

    #region Swingables params
    private float launchPower = 18f;
    private float launchTime = 0.4f;
    public Vector2 launchDir = Vector2.zero;
    [SerializeField]
    private bool _canLaunch = false;
    public bool CanLaunch
    {
        get => _canLaunch;
        set
        {
            _canLaunch = value;
            if (_canLaunch)
            {
                Debug.Log("Can Launch");
            }
        }
    }
    [SerializeField]
    private bool _isLaunching;
    public bool IsLaunching
    {
        get => _isLaunching;
        set
        {
            _isLaunching = value;
            animator.SetBool(AnimationStrings.isFlowerLaunching, value);
        }
    }

    public HairLassoController hairLassoController;
    [SerializeField]
    private bool _canRopeSwing;
    public bool CanRopeSwing
    {
        get => _canRopeSwing;
        set => _canRopeSwing = value;
    }
    [SerializeField]
    private bool _isRopeSwinging;
    public bool IsRopeSwinging
    {
        get => _isRopeSwinging;
        set
        {
            _isRopeSwinging = value;
            animator.SetBool(AnimationStrings.isRopeSwinging, value);
        }
    }
    #endregion

    #region Coyote Time and Jump Buffer
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    #endregion
    #endregion

    // Called when the controller is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravityScale; // Use normal gravity scale
        rb.drag = 0f; // Ensure no unintended linear drag affects movement
        rb.angularDrag = 0f; // Ensure no unintended angular drag affects rotation
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>(); // **Initialized here**
    }


    // Start is called before the first frame update
    void Start()
    {
        CanLaunch = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle coyote time
        if (touchingDirections.IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Handle jump buffering
        if (isJumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (IsDashing) return;

        // Additional non-physics updates can go here
        if (DialogueManager.GetInstance().dialogueIsPlaying) //if dialogue is active return
        {
            return;
        }
    }

    // Called on the Fixed Timestep in Unity making it ideal for physics calculations
    private void FixedUpdate()
    {
        if (IsDashing || IsLaunching)    // a coroutine is setting physics 
        {
            return;
        }

        // Handle swinging
        if (IsRopeSwinging)
        {
            rb.gravityScale = normalGravityScale * 1.5f; // Use normal gravity during swinging
            float torqueAmount = 50f;

            if (Mathf.Abs(moveInput.x) > Mathf.Epsilon)
            {
                rb.AddTorque((-moveInput.x * moveInput.y) * torqueAmount);
            }
            return;
        }

        // Handle buffered jumps
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            jumpBufferCounter = 0f;
            PerformJump();
        }

        HandleMovement();

        // Adjust gravity for better jump feel
        AdjustGravity();

        // Apply additional upward force if ascending and jump button is held
        if (rb.velocity.y > 0 && isJumpPressed)
        {
            rb.AddForce(Vector2.up * additionalJumpForce, ForceMode2D.Force); // Continuous upward force
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    private void HandleMovement()
    {
        // Determine target speed based on input
        float targetSpeed = moveInput.x * moveSpeed;

        // Debug input and target speed
        //Debug.Log($"moveInput.x: {moveInput.x}, targetSpeed: {targetSpeed}, currentSpeed: {currentSpeed}");

        // Calculate acceleration or deceleration
        if (Mathf.Abs(targetSpeed) > Mathf.Epsilon)
        {
            // Accelerate towards target speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerate to zero
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        bool onWall = GetComponent<TouchingDirections>().IsOnWall;

        if (onWall)
        {
            currentSpeed = 0f;
            return;
        }

        // Apply the calculated speed (only horizontal velocity)
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // Debug applied velocity
        //Debug.Log($"Applied velocity: {rb.velocity}");
    }

    private void AdjustGravity()
    {
        if (IsLaunching || IsRopeSwinging)
        {
            rb.gravityScale = normalGravityScale; // Use normal gravity during swinging
            return;
        }

        if (IsDashing)
        {
            rb.gravityScale = normalGravityScale;
            return;
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = normalGravityScale * fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !isJumpPressed)
        {
            rb.gravityScale = normalGravityScale * lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
        }
    }



    private void PerformJump()
    {
        Debug.Log("Jumping");
        animator.SetTrigger(AnimationStrings.jump);
        rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse); // Apply upward impulse
        coyoteTimeCounter = 0f;
    }

    #region Player inputs and actions
    public void OnLasso(InputAction.CallbackContext context)
    {
        if (context.started && !IsLaunching && !IsDashing && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Debug.Log("Attempting to lasso");
            if (hairLassoController.TryAttachLasso())
            { // Notify HairLassoController to attach the lasso
                animator.SetBool(AnimationStrings.isRopeSwinging, true);
            }
        }
        if (context.canceled && IsRopeSwinging)
        {
            Debug.Log("Releasing lasso");
            hairLassoController.ReleaseLasso(); // Notify HairLassoController to release the lasso
            animator.SetBool(AnimationStrings.isRopeSwinging, false);
        }
    }

    /// <summary>
    /// called when a player issues an interact command
    /// </summary>
    /// <param name="context"></param>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {   
            if (InDialogueTriggerRange && !inDialogue)  // Starting dialogue
            {
                Debug.Log("Initiating Dialogue");
                DialogueManager.GetInstance().EnterDialogueMode(DialogueTextFile);
                inDialogue = true; 
            } else if (InDialogueTriggerRange && inDialogue)
            {
                Debug.Log("Continue dialogue");
                DialogueManager.GetInstance().ContinueDialogue();
                if (!DialogueManager.GetInstance().dialogueIsPlaying)
                {
                    inDialogue = false;
                }
            }
        }
    }

    /// <summary>
    /// Called when a player issues a movement command
    /// </summary>
    /// <param name="context">Move Command</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (inDialogue) { return; } //Don't allow move while dialogue is playing

        // Directly set moveInput without smoothing
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput.magnitude > 0.1f;
        SetScaleDirection(moveInput);
    }

    /// <summary>
    /// Sets what direction the player scale should face
    /// </summary>
    /// <param name="moveInput"></param>
    private void SetScaleDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            // Debug.Log("Facing Right");
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // Debug.Log("Facing Left");
            IsFacingRight = false;
        }
    }

    /// <summary>
    /// Called when a player issues a launch command
    /// </summary>
    /// <param name="context">Swing Command</param>
    public void OnFlowerLaunch(InputAction.CallbackContext context)
    {
        if (context.started && CanLaunch && !touchingDirections.IsGrounded && !IsDashing && !IsLaunching && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Debug.Log("Flower Launch");
            animator.SetTrigger(AnimationStrings.flowerLaunch);   
            StartCoroutine(FlowerLaunch());
        }
    }

    /// <summary>
    /// Luanch the player in the direction of the Flower
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlowerLaunch()
    {
        rb.gravityScale = normalGravityScale;
        // Apply velocity in the direction of movement input
        rb.velocity = launchDir * launchPower;
        IsLaunching = true;
        yield return new WaitForSeconds(launchTime);
        currentSpeed = rb.velocity.x;
        IsLaunching = false;
        canDash = true;
    }

    /// <summary>
    /// Called when a player issues a dash command
    /// </summary>
    /// <param name="context">Dash command</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && canDash && !IsLaunching && !IsLaunching && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Debug.Log("Dash");
            IsDashing = true;
            StartCoroutine(Dash());
        }
    }

    /// <summary>
    /// Dashes the player forward in the direction they're facing
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dash()
    {
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Determine dash direction
        Vector2 dashDirection = IsFacingRight ? Vector2.right : Vector2.left;
        rb.velocity = dashDirection * dashingPower;

        // Optionally, add a dash effect or sound here
        // e.g., Instantiate(dashEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        IsDashing = false;
        //Debug.Log("finished dash");

        yield return new WaitForSeconds(dashingCD);
        canDash = true;
    }

    /// <summary>
    /// Called when a player issues a jump command
    /// 
    /// Performs a jump if the player is touching the ground or within coyote time
    /// </summary>
    /// <param name="context">Jump command</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying) { return; } //Don't allow jump if dialogue is playing

        if (context.started && coyoteTimeCounter > 0)
        {
            Debug.Log("Jump");
            animator.SetTrigger(AnimationStrings.jump);
            rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse); // Apply upward impulse
            coyoteTimeCounter = 0;
            isJumpPressed = true; // Jump button pressed
        }

        if (context.canceled)
        {
            isJumpPressed = false; // Jump button released
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }
        }
    }
    #endregion
}
