using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    private bool inTriggerRange;
    private PlayerController playerController;

    [SerializeField] private TextAsset textFile;
    [SerializeField] private GameObject interactIcon;

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
        }
    }
}
