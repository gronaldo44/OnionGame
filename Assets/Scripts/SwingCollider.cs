using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider tells the player it can be swinged off of
/// </summary>
public class SwingCollider : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] GameObject player;

    // Called when the collider is created
    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    /// <summary>
    /// Tells the player it can swing when it collides with this
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            //Debug.Log("Can Swing");
            playerController.CanSwing = true;
        }
    }

    /// <summary>
    /// Tells the player it can no longer swing when it stops colliding
    /// with this
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            //Debug.Log("Can't Swing");
            playerController.CanSwing = false;
        }
    }
}
