using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool inTriggerRange;

    private PlayerController playerController;

    [SerializeField] private TextAsset textFile;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inTriggerRange = false;
    }

    private void Update()
    {
        if (inTriggerRange && !DialogueManager.GetInstance().dialogueIsPlaying) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.GetInstance().EnterDialogueMode(textFile);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") //Check if player is in range
        {
            inTriggerRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        inTriggerRange = false;
        Debug.Log("Out of trigger range");
    }

    public bool GetInTriggerRange()
    {
        return inTriggerRange;
    }
}
