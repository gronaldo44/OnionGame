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

    #region swinging params
    private float swingPower = 18f;
    private float swingTime = 0.5f;
    [SerializeField]
    private bool _canSwing;
    public bool CanSwing
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
    public bool IsSwingLunging
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

    [SerializeField] private HairLassoController hairLassoController;
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
        if (IsDashing) return;
        if (IsSwinging) return;
        if (IsSwingLunging) return;
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
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    #region Player inputs and actions
    public void OnLasso(InputAction.CallbackContext context)
    {
        if (context.started && !IsSwinging && !IsDashing)
        {
            Debug.Log("Attempting to swing");
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
    /// Called when a player issues a swing command
    /// </summary>
    /// <param name="context">Swing Command</param>
    public void OnSwing(InputAction.CallbackContext context)
    {
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
        }
    }
    /// <summary>
    /// Performs a swing lunging the player in their movement direction
    /// </summary>
    /// <returns></returns>
    private IEnumerator Swing()
    {
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
        canDash = true;
    }

    /// <summary>
    /// Called when a player issues a dash command
    /// </summary>
    /// <param name="context">Dash command</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && canDash && !IsSwingLunging && !IsSwinging)
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
