using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player partially by interpreting user inputs
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Class params
    public Rigidbody2D rb;     // player body
    TouchingDirections touchingDirections;  // what the player's body is touching
    Animator animator;
    [SerializeField] public Vector3 spawnLocation = Vector3.zero;

    public float jumpImpulse = 10f;

    #region movement params
    public float moveSpeed;
    Vector2 moveInput;
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1); // flip along x-axis
            }
            _isFacingRight = value;
        }
    }
    #endregion

    #region dash params
    private bool canDash = true;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCD = 0.5f;
    [SerializeField]
    private bool _isDashing = false;
    public bool IsDashing
    {
        get
        {
            return _isDashing;
        }
        set
        {
            _isDashing = value;
            animator.SetBool(AnimationStrings.isDashing, value);
        }
    }
    #endregion

<<<<<<< Updated upstream
    #region swinging params
    private float swingPower = 18f;
    private float swingTime = 0.5f;
    [SerializeField]
    private bool _canSwing;
    public bool CanSwing
=======
    #region Swinging params
    private float launchPower = 18f;
    private float launchTime = 0.4f;
    private Vector2 launchDirection = Vector2.zero; // tmp value
    [SerializeField]
    private bool _canSwing = true;
    public bool CanLaunch
>>>>>>> Stashed changes
    {
        get
        {
            return _canSwing;
        }
        set
        {
            _canSwing = value;
        }
    }
    [SerializeField]
    private bool _isSwinging;
    public bool IsSwinging
    {
        get
        {
            return _isSwinging;
        }
        set
        {
            _isSwinging = value;
            animator.SetBool(AnimationStrings.isSwinging, value);
        }
    }
    [SerializeField]
    private bool _isSwingLunging;
    public bool IsFlowerLaunching
    {
        get
        {
            return _isSwingLunging;
        }
        set
        {
            _isSwingLunging = value;
            animator.SetBool(AnimationStrings.isSwingLunging, value);
        }
    }

    public HairLassoController hairLassoController;
    [SerializeField]
    private bool _canRopeSwing;
    public bool CanRopeSwing
    {
        get
        {
            return _canRopeSwing;
        }
        set
        {
            _canRopeSwing = value;
        }
    }
    [SerializeField]
    private bool _isRopeSwinging;
    public bool IsRopeSwinging
    {
        get
        {
            return _isRopeSwinging;
        }
        set
        {
            _isRopeSwinging = value;
            animator.SetBool(AnimationStrings.isRopeSwinging, value);
        }
    }
    #endregion
    #endregion

    // Called when the controller is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsDashing) return;
    }

    // called on the Fixed Timestep in Unity making it ideal for physics calculations
    private void FixedUpdate()
    {
<<<<<<< Updated upstream
        if (IsDashing) return;
        if (IsSwinging) return;
        if (IsSwingLunging) return;
=======
        if (IsDashing || IsFlowerLaunching)    // a coroutine is setting physics 
        {
            return;
        }
        if (IsSwinging) // the player is choosing where to swing
        {
            return;
        }

        // Handle swinging
>>>>>>> Stashed changes
        if (IsRopeSwinging)
        {
            // Apply force based on player input to swing harder
            float swingForce = 10f; // Adjust this value to control how much force is applied
            Vector2 forceDirection = new Vector2(moveInput.x, 0).normalized;
            rb.AddForce(forceDirection * swingForce);
            return;
        }

        if (!touchingDirections.IsOnWall)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
<<<<<<< Updated upstream
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

=======

        if (!IsSwinging && !IsFlowerLaunching)
        {
            HandleMovement();
        }

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
        Debug.Log($"moveInput.x: {moveInput.x}, targetSpeed: {targetSpeed}, currentSpeed: {currentSpeed}");

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
        Debug.Log($"Applied velocity: {rb.velocity}");
    }

    private void AdjustGravity()
    {
        if (IsFlowerLaunching || IsRopeSwinging)
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
        animator.SetTrigger(AnimationStrings.jump);
        rb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse); // Apply upward impulse
        coyoteTimeCounter = 0f;
        isJumpPressed = true;
    }

>>>>>>> Stashed changes
    #region Player inputs and actions
    public void OnLasso(InputAction.CallbackContext context)
    {
        if (context.started && !IsSwinging && !IsDashing)
        {
            Debug.Log("Attempting to lasso");
            hairLassoController.TryAttachLasso(); // Notify HairLassoController to attach the lasso
        }
        if (context.canceled && IsRopeSwinging)
        {
            Debug.Log("Releasing lasso");
            hairLassoController.ReleaseLasso(); // Notify HairLassoController to release the lasso
            StartCoroutine(Swing());
        }
    }

    /// <summary>
    /// Called when a player issues a movement command
    /// </summary>
    /// <param name="context">Move Command</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // x,y movement input
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

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
    /// Called when a player issues a launchpad command
    /// </summary>
    /// <param name="context">Swing Command</param>
    public void OnFlowerLaunch(InputAction.CallbackContext context)
    {
<<<<<<< Updated upstream
        if (context.started && CanSwing && !touchingDirections.IsGrounded && !IsDashing && !IsSwinging)
        {
            Debug.Log("Swinging");
            animator.SetTrigger(AnimationStrings.swing);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;    // must be called before Swing() resets it
            IsSwinging = true;
        }
        if (context.canceled && IsSwinging && !touchingDirections.IsGrounded)
        {
            Debug.Log("Swing lunged");
            StartCoroutine(Swing());
=======
        if (context.started && CanLaunch && !IsDashing && !IsSwinging && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Debug.Log("Flower Launch");
            animator.SetTrigger(AnimationStrings.swing);        // TODO: Change to flower launch
            StartCoroutine(FlowerLaunch());
>>>>>>> Stashed changes
        }
    }
    /// <summary>
    /// Performs a swing lunging the player in their movement direction
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlowerLaunch()
    {
<<<<<<< Updated upstream
        IsSwinging = false;
        // Use the moveInput to determine the lunge direction
        if (moveInput != Vector2.zero)
        {
            // Apply velocity in the direction of movement input
            rb.velocity = moveInput * swingPower;
            IsSwingLunging = true;
        }
        rb.gravityScale = 1;
        yield return new WaitForSeconds(swingTime);
        IsSwingLunging = false;
=======
        // Use the Flower's orientation to determine the lunge direction
        rb.velocity = launchDirection * launchPower;
        IsFlowerLaunching = true;
        yield return new WaitForSeconds(launchTime);
        IsFlowerLaunching = false;
>>>>>>> Stashed changes
        canDash = true;
    }

    /// <summary>
    /// Called when a player issues a dash command
    /// </summary>
    /// <param name="context">Dash command</param>
    public void OnDash(InputAction.CallbackContext context)
    {
<<<<<<< Updated upstream
        if (context.started && canDash && !IsSwingLunging && !IsSwinging)
=======
        if (context.started && canDash && !IsFlowerLaunching && !IsSwinging && !DialogueManager.GetInstance().dialogueIsPlaying)
>>>>>>> Stashed changes
        {
            Debug.Log("Dash");
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
        IsDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        // perform dash
        yield return new WaitForSeconds(dashingTime);
        // resume original player state
        rb.gravityScale = originalGravity;
        IsDashing = false;
        // wait for dash cooldown
        yield return new WaitForSeconds(dashingCD);
        canDash = true;
    }

    /// <summary>
    /// Called when a player issues a jump command
    /// 
    /// Performs a jump if the player is touching the ground
    /// </summary>
    /// <param name="context">Jump command</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }
    #endregion
}
