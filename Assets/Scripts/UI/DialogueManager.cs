using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    private TextAsset textFile;
    private string[] linesOfDialogue;
    private int dialogueIndex;

    public bool dialogueIsPlaying { get; private set; }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one DialogueManager");
        }

        instance = this;
        linesOfDialogue = new string[0];
        dialogueIndex = 0;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueIsPlaying) { return; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ContinueDialogue();
        }
    }

    public void EnterDialogueMode(TextAsset fullDialogueText)
    {
        linesOfDialogue = fullDialogueText.text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueDialogue();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueDialogue()
    {
        if (dialogueIndex <= linesOfDialogue.Length - 1)
        {
            dialogueText.text = linesOfDialogue[dialogueIndex];
            dialogueIndex += 2; //skip the whitespace in between
        }
        else
        {
            ExitDialogueMode();
        }
    }
}
