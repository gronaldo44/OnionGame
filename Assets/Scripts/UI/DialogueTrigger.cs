using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.NetworkInformation;

public class DialogueTrigger : MonoBehaviour
{
    private bool inTriggerRange;
    private bool isFirstTrigger = true;
    private PlayerController playerController;

    [SerializeField] public TextAsset textFile;
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private SpriteRenderer seleneSprite;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inTriggerRange = false;
        interactIcon.SetActive(false);
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<PlayerController>(out var player)) //Check if player is in range
        {
            interactIcon.SetActive(true);
            player.InDialogueTriggerRange = true;
            player.DialogueTextFile = textFile;
            Debug.Log("In dialogue trigger range");
            if (!isFirstTrigger)
            {
                FadeIn(0.5f);
            } else
            {
                isFirstTrigger = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<PlayerController>(out var player))
        {
            interactIcon.SetActive(false);
            player.InDialogueTriggerRange = false;
            player.DialogueTextFile = null;
            Debug.Log("Out of dialogue trigger range");
            FadeOut(0.5f);
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
        SpriteRenderer interactSprite = interactIcon.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        Color selene_startColor = seleneSprite.color;
        Color interact_startColor = interactSprite.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            seleneSprite.color = new Color(selene_startColor.r, selene_startColor.g, selene_startColor.b, alpha);
            interactSprite.color = new Color(interact_startColor.r, interact_startColor.g, interact_startColor.b, alpha);
            
            yield return null; // Wait for the next frame
        }

        // Ensure the final alpha value is set
        seleneSprite.color = new Color(selene_startColor.r, selene_startColor.g, selene_startColor.b, endAlpha);
        interactSprite.color = new Color(interact_startColor.r, interact_startColor.g, interact_startColor.b, endAlpha);
    }
}
