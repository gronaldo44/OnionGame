using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Collider tells the player it can be swinged off of
/// </summary>
public class SwingController : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private SpriteRenderer flower;
    private Coroutine activeFlowerTransition;
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

        if (IsRopeSwing)
        {
            ring = transform.Find("RING_0").gameObject;
            SpriteRenderer ringSprite = ring.GetComponent<SpriteRenderer>();
            ringSprite.color = new Color(ringSprite.color.r, ringSprite.color.g, ringSprite.color.b, 0);
        } else
        {
            flower = GetComponent<SpriteRenderer>();
            originalScale = flower.transform.localScale;
            originalPosition = flower.transform.localPosition;
        }
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
            playerController.Event_OnFlowerLaunch += LaunchFlower;

            // Slow player fall
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null && playerRigidbody.velocity.y < 0) // Only slow if falling
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y * 0.5f);
            }

            WiltFlower();
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
            playerController.Event_OnFlowerLaunch -= LaunchFlower;

            UnwiltFlower();
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

    private void WiltFlower()
    {
        Vector3 wiltedScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 0.6f, originalScale.z); // Compressed
        Vector3 wiltedPosition = originalPosition - transform.right * 0.6f; // Pulled back
        activeFlowerTransition = StartCoroutine(SmoothTransform(flower, wiltedScale, wiltedPosition, 0.3f));
    }

    private void UnwiltFlower()
    {
        activeFlowerTransition = StartCoroutine(SmoothTransform(flower, originalScale, originalPosition, 0.3f));
    }

    private void LaunchFlower()
    {
        Debug.Log("Launch Flower Sprite");
        if (activeFlowerTransition != null)
        {
            StopCoroutine(activeFlowerTransition);
            activeFlowerTransition = null;
        }

        Vector3 targetPosition = originalPosition + (transform.right * 2f);
        activeFlowerTransition = StartCoroutine(SmoothTransform(flower, originalScale, targetPosition, 0.15f));
    }

    private IEnumerator SmoothTransform(SpriteRenderer sprite, Vector3 targetScale, Vector3 targetPosition, float duration)
    {
        Vector3 startScale = sprite.transform.localScale;
        Vector3 startPosition = sprite.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smoothly interpolate scale and position
            sprite.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            sprite.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null; // Wait for the next frame
        }

        // Ensure final values are precisely set
        sprite.transform.localScale = targetScale;
        sprite.transform.localPosition = targetPosition;

        activeFlowerTransition = null;
    }


    private void FadeIn(float duration)
    {
        StartCoroutine(Fade(0, 1, duration)); // From transparent (0) to opaque (1)
    }

    private void FadeOut(float duration)
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
