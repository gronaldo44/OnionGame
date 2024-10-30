using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider tells the player it can be swinged off of
/// </summary>
public class SwingController : MonoBehaviour
{
    [SerializeField] private bool _isRopeSwing;
    public bool IsRopeSwing
    {
        get
        { return _isRopeSwing; }
        set
        {
            _isRopeSwing = value;
            if (!_isRopeSwing)
            {
                hairLassoController.ClearSwingableObject();
            }
        }
    }
    [SerializeField] private GameObject player;
    private HairLassoController hairLassoController;
    private PlayerController playerController;

    // Called when the collider is created
    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        hairLassoController = playerController.hairLassoController;
    }

    /// <summary>
    /// Tells the player it can swing when it collides with this
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player) && !_isRopeSwing)
        {
            Debug.Log("Can FlowerLaunch");
            playerController.CanLaunch = true;
            playerController.launchDir = transform.right;
        }
        else if (collision.TryGetComponent<HairLassoController>(out var lasso) && _isRopeSwing)
        {
            Debug.Log("Can RopeSwing");
            hairLassoController.SetSwingableObject(gameObject);
        }
    }

    /// <summary>
    /// Tells the player it can no longer swing when it stops colliding
    /// with this
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player) && !_isRopeSwing)
        {
            Debug.Log("Can't FlowerLaunch");
            playerController.CanLaunch = false;
            playerController.launchDir = Vector2.zero;
        }
        else if (collision.TryGetComponent<HairLassoController>(out var lasso) && _isRopeSwing)
        {
            // Ensure the player is no longer swinging before clearing the swingable object
            if (!playerController.IsRopeSwinging)
            {
                Debug.Log("Can't LassoSwing");
                hairLassoController.ClearSwingableObject();
            }
        }
    }

    public void SetPlayerReference(GameObject player)
    {
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            hairLassoController = playerController.hairLassoController;
        }
    }
}
