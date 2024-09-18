using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider tells the player it can be swinged off of
/// </summary>
public class SwingCollider : MonoBehaviour
{
    [SerializeField] private bool isRopeSwing;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject hairLasso;

    private HairLassoController hairLassoController;
    private PlayerController playerController;

    // Called when the collider is created
    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        hairLassoController = hairLasso.GetComponent<HairLassoController>();

        Debug.Log("HERE Distance joint is: " + hairLassoController.distanceJoint);
        Debug.Log("HERE Line Renderer is: " + hairLassoController.lineRenderer);
    }

    /// <summary>
    /// Tells the player it can swing when it collides with this
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player) && !isRopeSwing)
        {
            Debug.Log("Can Swing");
            playerController.CanSwing = true;
        }
        else if (collision.TryGetComponent<HairLassoController>(out var lasso) && isRopeSwing)
        {
            Debug.Log("Can LassoSwing");
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
        if (collision.TryGetComponent<PlayerController>(out var player) && !isRopeSwing)
        {
            Debug.Log("Can't Swing");
            playerController.CanSwing = false;
        }
        else if (collision.TryGetComponent<HairLassoController>(out var lasso) && isRopeSwing)
        {
            // Ensure the player is no longer swinging before clearing the swingable object
            if (!playerController.IsRopeSwinging)
            {
                Debug.Log("Can't LassoSwing");
                hairLassoController.ClearSwingableObject();
            }
        }
    }
}
