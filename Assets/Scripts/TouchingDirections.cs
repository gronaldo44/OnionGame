using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;

    Animator animator;
    Collider2D col;

    [SerializeField]
    private bool _isGrounded;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    public float groundDist = 0.05f;
    public bool IsGrounded { get
        {
            return _isGrounded;
        } private set {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        } }

    [SerializeField]
    private bool _isOnWall;
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    public float wallDist = 0.2f;
    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    [SerializeField]
    private bool _isOnCeiling;
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    public float ceilingDist = 0.05f;
    public bool IsOnCeiling
    {
        get
        {
            return _isOnCeiling;
        }
        private set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        IsGrounded = col.Cast(Vector2.down, castFilter, groundHits, groundDist) > 0;
        IsOnWall = col.Cast(wallCheckDirection, castFilter, wallHits, wallDist) > 0;
        IsOnCeiling = col.Cast(Vector2.up, castFilter, ceilingHits, ceilingDist) > 0;
    }
}
