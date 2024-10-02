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
        if (inTriggerRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Initiating Dialogue");
                interactIcon.SetActive(false);
                DialogueManager.GetInstance().EnterDialogueMode(textFile);
            }
        }
        else if (DialogueManager.GetInstance().dialogueIsPlaying && inTriggerRange)
        {
            // Only allow continuation of dialogue if it's already playing
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.GetInstance().ContinueDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") //Check if player is in range
        {
            interactIcon.SetActive(true);
            inTriggerRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        interactIcon.SetActive(false);
        inTriggerRange = false;
        Debug.Log("Out of trigger range");
    }

    public bool GetInTriggerRange()
    {
        return inTriggerRange;
    }
}
