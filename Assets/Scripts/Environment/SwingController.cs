using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Collider tells the player it can be swinged off of
/// </summary>
public class SwingController : MonoBehaviour
{
    GameObject ring;
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

        ring = transform.Find("RING_0").gameObject;
        SpriteRenderer ringSprite = ring.GetComponent<SpriteRenderer>();
        ringSprite.color = new Color(ringSprite.color.r, ringSprite.color.g, ringSprite.color.b, 0);
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

            FadeIn(0.5f);
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

                FadeOut(0.5f);
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

    public void FadeIn(float duration)
    {
        StartCoroutine(Fade(0, 1, duration)); // From transparent (0) to opaque (1)
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(1, 0, duration)); // From opaque (1) to transparent (0)
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        SpriteRenderer ringSpriteRenderer = ring.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        Color startColor = ringSpriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            ringSpriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null; // Wait for the next frame
        }

        // Ensure the final alpha value is set
        ringSpriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
    }
}
